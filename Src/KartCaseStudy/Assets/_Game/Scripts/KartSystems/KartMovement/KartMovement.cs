using System.Collections.Generic;
using KartSystem.Track;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace KartSystem.KartSystems
{
    /// <summary>
    ///     This class is responsible for all aspects of the kart's movement.  It uses a kinematic rigidbody and a capsule
    ///     collider
    ///     to simulate the presence of the kart but does not use the internal physics solver and instead uses its own solver.
    ///     The  movement of the kart depends on the KartStats.  These have a default value but can be adjusted by anything
    ///     implementing
    ///     the IKartModifier interface.
    /// </summary>
    [RequireComponent(typeof(IInput))]
    [RequireComponent(typeof(Rigidbody))]
    public class KartMovement : MonoBehaviour, IKartCollider, IMovable, IKartInfo
    {
        private enum DriftState
        {
            NotDrifting,
            FacingLeft,
            FacingRight
        }


        [RequireInterface(typeof(IKartModifier))]
        [Tooltip(
            "A reference to the stats modification due to the type of kart being moved.  This can be either a Component or ScriptableObject.")]
        public Object kart;

        [RequireInterface(typeof(IKartModifier))]
        [Tooltip(
            "A reference to the stats modification due to the driver of kart being moved.  This can be either a Component or ScriptableObject.")]
        public Object driver;

        [RequireInterface(typeof(IKartModifier))]
        [Tooltip(
            "A reference to the stats modification due to the kart being airborne.  This can be either a Component or ScriptableObject.")]
        public Object airborneModifier;

        [RequireInterface(typeof(IInput))]
        [Tooltip("A reference to the an object implementing the IInput class to be used to control the kart.")]
        public Object input;

        [Tooltip(
            "A reference to a transform representing the origin of a ray to help determine if the kart is grounded.  This is the front of a diamond formation.")]
        public Transform frontGroundRaycast;

        [Tooltip(
            "A reference to a transform representing the origin of a ray to help determine if the kart is grounded.  This is the right of a diamond formation.")]
        public Transform rightGroundRaycast;

        [Tooltip(
            "A reference to a transform representing the origin of a ray to help determine if the kart is grounded.  This is the left of a diamond formation.")]
        public Transform leftGroundRaycast;

        [Tooltip(
            "A reference to a transform representing the origin of a ray to help determine if the kart is grounded.  This is the rear of a diamond formation.")]
        public Transform rearGroundRaycast;

        [Tooltip(
            "A reference to the default stats for all karts.  Modifications to these are typically made using the kart and driver fields or by using the AddKartModifier function.")]
        public KartStats defaultStats;

        [Tooltip("The layers which represent any ground the kart can drive on.")]
        public LayerMask groundLayers;

        [Tooltip(
            "The layers which represent anything the kart can collide with.  This should include the ground layers.")]
        public LayerMask allCollidingLayers;

        [Tooltip("How fast the kart levels out when airborne.")]
        public float airborneOrientationSpeed = 60f;

        [Tooltip("The minimum value for the input's steering when the kart is drifting.")]
        public float minDriftingSteering = 0.2f;

        [Tooltip(
            "How fast the kart's rotation gets corrected.  This is used for smoothing the karts rotation and returning to normal driving after a drift")]
        public float rotationCorrectionSpeed = 180f;

        [Tooltip(
            "The smallest allowed angle for a kart to be turned from the velocity direction in order to a drift to start.")]
        public float minDriftStartAngle = 15f;

        [Tooltip(
            "The largest allowed angle for a kart to be turned from the velocity direction in order to a drift to start.")]
        public float maxDriftStartAngle = 90f;

        [Tooltip(
            "When karts collide the movement is based on their weight difference and this additional velocity change.")]
        public float kartToKartBump = 10f;

        [Tooltip(
            "Rainbow particle indicating the movement speed")]
        [SerializeField] private ParticleSystem _leftRainbowParticle;

        [Tooltip(
            "Rainbow particle indicating the movement speed")]
        [SerializeField] private ParticleSystem _rightRainbowParticle;

        [Tooltip(
            "Audio source for the drift")]
        [SerializeField] private AudioSource _driftAudio;

        public UnityEvent OnBecomeAirborne;
        public UnityEvent OnBecomeGrounded;
        public UnityEvent OnHop;
        public delegate void DriftDelegate();
        // public UnityEvent OnDriftStarted;
        // public UnityEvent OnDriftStopped;
        public UnityEvent OnKartCollision;

        private IInput _input;
        private Vector3 _velocity;
        private GroundInfo _currentGroundInfo;
        private Rigidbody _rigidbody;
        private CapsuleCollider _capsule;
        private IRacer _racer;

        private readonly List<IKartModifier>
            _currentModifiers =
                new(16); // The karts stats are based on a list of modifiers.  Each affects the results of the previous until the modified stats are calculated.

        private readonly List<IKartModifier> _tempModifiers = new(8);
        private KartStats _modifiedStats; // The stats that are used to calculate the kart's velocity.
        private readonly RaycastHit[] _raycastHitBuffer = new RaycastHit[8];
        private readonly Collider[] _colliderBuffer = new Collider[8];
        private Quaternion _driftOffset = Quaternion.identity;
        private DriftState _driftState;
        private bool _hasControl;
        private float _steeringInput;
        private float _accelerationInput;
        private bool _hopPressed;
        private bool _hopHeld;
        private Vector3 _repositionPositionDelta;
        private Quaternion _repositionRotationDelta = Quaternion.identity;

        private const int MaxPenetrationSolves = 3;
        private const float GroundToCapsuleOffsetDistance = 0.025f;
        private const float Deadzone = 0.01f;
        private const float VelocityNormalAirborneDot = 0.5f;

        // These properties are part of the IKartInfo interface.
        public Vector3 Position { get; private set; }

        public Quaternion Rotation => _rigidbody.rotation;
        public KartStats CurrentStats => _modifiedStats;
        public Vector3 Velocity => _velocity;
        public Vector3 Movement { get; private set; }

        public float LocalSpeed =>
            (Quaternion.Inverse(_rigidbody.rotation) * Quaternion.Inverse(_driftOffset) * _velocity).z;

        public bool IsGrounded { get; private set; }

        public GroundInfo CurrentGroundInfo => _currentGroundInfo;

        private void Reset()
        {
            groundLayers = LayerMask.GetMask("Default");
            allCollidingLayers = LayerMask.GetMask("Default");
        }

        private void Start()
        {
            _input = input as IInput;
            _rigidbody = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            _racer = GetComponent<IRacer>();

            if (kart != null)
                _currentModifiers.Add((IKartModifier)kart);

            if (driver != null)
                _currentModifiers.Add((IKartModifier)driver);

            OnDriftEnd();
        }

        private void FixedUpdate()
        {
            if (Mathf.Approximately(Time.timeScale, 0f))
                return;

            if (_repositionPositionDelta.sqrMagnitude > float.Epsilon ||
                _repositionRotationDelta != Quaternion.identity)
            {
                _rigidbody.MovePosition(_rigidbody.position + _repositionPositionDelta);
                _rigidbody.MoveRotation(_repositionRotationDelta * _rigidbody.rotation);
                _repositionPositionDelta = Vector3.zero;
                _repositionRotationDelta = Quaternion.identity;
                return;
            }

            Position = _rigidbody.position;

            KartStats.GetModifiedStats(_currentModifiers, defaultStats, ref _modifiedStats);
            ClearTempModifiers();

            var rotationStream = _rigidbody.rotation;

            var deltaTime = Time.deltaTime;

            _currentGroundInfo = CheckForGround(deltaTime, rotationStream, Vector3.zero);

            Hop(rotationStream, _currentGroundInfo);

            if (_currentGroundInfo.isGrounded && !IsGrounded)
                OnBecomeGrounded.Invoke();

            if (!_currentGroundInfo.isGrounded && IsGrounded)
                OnBecomeAirborne.Invoke();

            IsGrounded = _currentGroundInfo.isGrounded;

            ApplyAirborneModifier(_currentGroundInfo);

            var nextGroundInfo = CheckForGround(deltaTime, rotationStream, _velocity * deltaTime);

            GroundNormal(deltaTime, ref rotationStream, _currentGroundInfo, nextGroundInfo);
            TurnKart(deltaTime, ref rotationStream);

            StartDrift(_currentGroundInfo, nextGroundInfo, rotationStream);
            StopDrift(deltaTime);

            CalculateDrivingVelocity(deltaTime, _currentGroundInfo, rotationStream);

            var penetrationOffset = SolvePenetration(rotationStream);
            penetrationOffset = ProcessVelocityCollisions(deltaTime, rotationStream, penetrationOffset);

            rotationStream = Quaternion.RotateTowards(_rigidbody.rotation, rotationStream,
                rotationCorrectionSpeed * deltaTime);

            AdjustVelocityByPenetrationOffset(deltaTime, ref penetrationOffset);

            _rigidbody.MoveRotation(rotationStream);
            _rigidbody.MovePosition(Position + Movement);
        }

        /// <summary>
        ///     Removes all the temporary modifiers added through velocity collisions.  They will be re-added in
        ///     ProcessVelocityCollisions if they still apply.
        /// </summary>
        private void ClearTempModifiers()
        {
            for (var i = 0; i < _tempModifiers.Count; i++) _currentModifiers.Remove(_tempModifiers[i]);

            _tempModifiers.Clear();
        }

        /// <summary>
        ///     Determines how much the kart should be moved due to its collider overlapping with others.
        /// </summary>
        private Vector3 SolvePenetration(Quaternion rotationStream)
        {
            var summedOffset = Vector3.zero;
            for (var solveIterations = 0; solveIterations < MaxPenetrationSolves; solveIterations++)
                summedOffset = ComputePenetrationOffset(rotationStream, summedOffset);

            return summedOffset;
        }

        /// <summary>
        ///     Computes the penetration offset for a single iteration.
        /// </summary>
        /// <param name="rotationStream">The current rotation of the kart.</param>
        /// <param name="summedOffset">How much the kart's capsule is offset so far.</param>
        /// <returns>How much the kart's capsule should be offset after this solve.</returns>
        private Vector3 ComputePenetrationOffset(Quaternion rotationStream, Vector3 summedOffset)
        {
            var capsuleAxis = rotationStream * Vector3.forward * _capsule.height * 0.5f;
            var point0 = Position + capsuleAxis + summedOffset;
            var point1 = Position - capsuleAxis + summedOffset;
            var kartCapsuleHitCount = Physics.OverlapCapsuleNonAlloc(point0, point1, _capsule.radius, _colliderBuffer,
                allCollidingLayers, QueryTriggerInteraction.Ignore);

            for (var i = 0; i < kartCapsuleHitCount; i++)
            {
                var hitCollider = _colliderBuffer[i];
                if (hitCollider == _capsule)
                    continue;

                var hitColliderTransform = hitCollider.transform;
                if (Physics.ComputePenetration(_capsule, Position + summedOffset, rotationStream, hitCollider,
                        hitColliderTransform.position, hitColliderTransform.rotation, out var separationDirection,
                        out var separationDistance))
                {
                    var offset = separationDirection * (separationDistance + Physics.defaultContactOffset);
                    if (Mathf.Abs(offset.x) > Mathf.Abs(summedOffset.x))
                        summedOffset.x = offset.x;
                    if (Mathf.Abs(offset.y) > Mathf.Abs(summedOffset.y))
                        summedOffset.y = offset.y;
                    if (Mathf.Abs(offset.z) > Mathf.Abs(summedOffset.z))
                        summedOffset.z = offset.z;
                }
            }

            return summedOffset;
        }

        /// <summary>
        ///     Checks whether or not the kart is grounded given a rotation and offset.
        /// </summary>
        /// <param name="deltaTime">The time between frames.</param>
        /// <param name="rotationStream">The rotation the kart will have.</param>
        /// <param name="offset">The offset from the kart's current position.</param>
        /// <returns>Information about the ground from the offset position.</returns>
        private GroundInfo CheckForGround(float deltaTime, Quaternion rotationStream, Vector3 offset)
        {
            var groundInfo = new GroundInfo();
            var defaultPosition = offset + _velocity * deltaTime;
            var direction = rotationStream * Vector3.down;

            var capsuleRadius = _capsule.radius;
            var capsuleTouchingDistance = capsuleRadius + Physics.defaultContactOffset;
            var groundedDistance = capsuleTouchingDistance + GroundToCapsuleOffsetDistance;
            var closeToGroundDistance = Mathf.Max(groundedDistance + capsuleRadius, _velocity.y);

            var hitCount = 0;

            var ray = new Ray(defaultPosition + frontGroundRaycast.position, direction);

            var didHitFront = GetNearestFromRaycast(ray, closeToGroundDistance, groundLayers,
                QueryTriggerInteraction.Ignore, out var frontHit);
            if (didHitFront)
                hitCount++;

            ray.origin = defaultPosition + rightGroundRaycast.position;

            var didHitRight = GetNearestFromRaycast(ray, closeToGroundDistance, groundLayers,
                QueryTriggerInteraction.Ignore, out var rightHit);
            if (didHitRight)
                hitCount++;

            ray.origin = defaultPosition + leftGroundRaycast.position;

            var didHitLeft = GetNearestFromRaycast(ray, closeToGroundDistance, groundLayers,
                QueryTriggerInteraction.Ignore, out var leftHit);
            if (didHitLeft)
                hitCount++;

            ray.origin = defaultPosition + rearGroundRaycast.position;

            var didHitRear = GetNearestFromRaycast(ray, closeToGroundDistance, groundLayers,
                QueryTriggerInteraction.Ignore, out var rearHit);
            if (didHitRear)
                hitCount++;

            groundInfo.isCapsuleTouching = frontHit.distance <= capsuleTouchingDistance ||
                                           rightHit.distance <= capsuleTouchingDistance ||
                                           leftHit.distance <= capsuleTouchingDistance ||
                                           rearHit.distance <= capsuleTouchingDistance;
            groundInfo.isGrounded = frontHit.distance <= groundedDistance || rightHit.distance <= groundedDistance ||
                                    leftHit.distance <= groundedDistance || rearHit.distance <= groundedDistance;
            groundInfo.isCloseToGround = hitCount > 0;

            // No hits - normal = Vector3.up
            if (hitCount == 0)
            {
                groundInfo.normal = Vector3.up;
            }

            // 1 hit - normal = hit.normal
            else if (hitCount == 1)
            {
                if (didHitFront)
                    groundInfo.normal = frontHit.normal;
                else if (didHitRight)
                    groundInfo.normal = rightHit.normal;
                else if (didHitLeft)
                    groundInfo.normal = leftHit.normal;
                else if (didHitRear)
                    groundInfo.normal = rearHit.normal;
            }

            // 2 hits - normal = hits average
            else if (hitCount == 2)
            {
                groundInfo.normal = (frontHit.normal + rightHit.normal + leftHit.normal + rearHit.normal) * 0.5f;
            }

            // 3 hits - normal = normal of plane from 3 points
            else if (hitCount == 3)
            {
                if (!didHitFront)
                    groundInfo.normal = Vector3.Cross(rearHit.point - rightHit.point, leftHit.point - rightHit.point);

                if (!didHitRight)
                    groundInfo.normal = Vector3.Cross(rearHit.point - frontHit.point, leftHit.point - frontHit.point);

                if (!didHitLeft)
                    groundInfo.normal = Vector3.Cross(rightHit.point - frontHit.point, rearHit.point - frontHit.point);

                if (!didHitRear)
                    groundInfo.normal = Vector3.Cross(leftHit.point - rightHit.point, frontHit.point - rightHit.point);
            }

            // 4 hits - normal = average of normals from 4 planes
            else
            {
                var normal0 = Vector3.Cross(rearHit.point - rightHit.point, leftHit.point - rightHit.point);
                var normal1 = Vector3.Cross(rearHit.point - frontHit.point, leftHit.point - frontHit.point);
                var normal2 = Vector3.Cross(rightHit.point - frontHit.point, rearHit.point - frontHit.point);
                var normal3 = Vector3.Cross(leftHit.point - rightHit.point, frontHit.point - rightHit.point);

                groundInfo.normal = (normal0 + normal1 + normal2 + normal3) * 0.25f;
            }

            if (groundInfo.isGrounded)
            {
                var dot = Vector3.Dot(groundInfo.normal, _velocity.normalized);
                if (dot > VelocityNormalAirborneDot) groundInfo.isGrounded = false;
            }

            return groundInfo;
        }

        /// <summary>
        ///     Gets information about the nearest object hit by a raycast.
        /// </summary>
        private bool GetNearestFromRaycast(Ray ray, float rayDistance, int layerMask, QueryTriggerInteraction query,
            out RaycastHit hit)
        {
            var hits = Physics.RaycastNonAlloc(ray, _raycastHitBuffer, rayDistance, layerMask, query);

            hit = new RaycastHit();
            hit.distance = float.PositiveInfinity;

            var hitSelf = false;
            for (var i = 0; i < hits; i++)
            {
                if (_raycastHitBuffer[i].collider == _capsule)
                {
                    hitSelf = true;
                    continue;
                }

                if (_raycastHitBuffer[i].distance < hit.distance)
                    hit = _raycastHitBuffer[i];
            }

            if (hitSelf)
                hits--;

            return hits > 0;
        }

        /// <summary>
        ///     Checks and applies the modifier to the kart's stats if the kart is not grounded.
        /// </summary>
        private void ApplyAirborneModifier(GroundInfo currentGroundInfo)
        {
            if (airborneModifier != null)
            {
                if (_currentModifiers.Contains((IKartModifier)airborneModifier) && currentGroundInfo.isGrounded)
                    _currentModifiers.Remove((IKartModifier)airborneModifier);
                else if (!_currentModifiers.Contains((IKartModifier)airborneModifier) && !currentGroundInfo.isGrounded)
                    _currentModifiers.Add((IKartModifier)airborneModifier);
            }
        }

        /// <summary>
        ///     Affects the rotation stream so that the kart is level with the ground.
        /// </summary>
        private void GroundNormal(float deltaTime, ref Quaternion rotationStream, GroundInfo currentGroundInfo,
            GroundInfo nextGroundInfo)
        {
            var rigidbodyUp = _rigidbody.rotation * Vector3.up;
            var currentTargetRotation = Quaternion.FromToRotation(rigidbodyUp, currentGroundInfo.normal);
            var nextTargetRotation = Quaternion.FromToRotation(rigidbodyUp, nextGroundInfo.normal);
            if (nextGroundInfo.isCloseToGround)
                rotationStream = Quaternion.RotateTowards(currentTargetRotation, nextTargetRotation, 0.5f) *
                                 rotationStream;
            else
                rotationStream = Quaternion.RotateTowards(rotationStream, nextTargetRotation,
                    airborneOrientationSpeed * deltaTime);
        }

        /// <summary>
        ///     Affects the rotation stream based on the steering input.
        /// </summary>
        private void TurnKart(float deltaTime, ref Quaternion rotationStream)
        {
            var localVelocity = Quaternion.Inverse(rotationStream) * Quaternion.Inverse(_driftOffset) * _velocity;
            var forwardReverseSwitch = Mathf.Sign(localVelocity.z);
            var modifiedSteering = _hasControl ? _input.Steering * forwardReverseSwitch : 0f;
            if (_driftState == DriftState.FacingLeft)
                modifiedSteering = Mathf.Clamp(modifiedSteering, -1f, -minDriftingSteering);
            else if (_driftState == DriftState.FacingRight)
                modifiedSteering = Mathf.Clamp(modifiedSteering, minDriftingSteering, 1f);

            var speedProportion = _velocity.sqrMagnitude > 0f ? 1f : 0f;
            var turn = _modifiedStats.turnSpeed * modifiedSteering * speedProportion * deltaTime;
            var deltaRotation = Quaternion.Euler(0f, turn, 0f);
            rotationStream = rotationStream * deltaRotation;
        }

        /// <summary>
        ///     Calculates the velocity of the kart.
        /// </summary>
        private void CalculateDrivingVelocity(float deltaTime, GroundInfo groundInfo, Quaternion rotationStream)
        {
            var localVelocity = Quaternion.Inverse(rotationStream) * Quaternion.Inverse(_driftOffset) * _velocity;
            if (groundInfo.isGrounded)
            {
                localVelocity.x = Mathf.MoveTowards(localVelocity.x, 0f, _modifiedStats.grip * deltaTime);

                var acceleration = _hasControl ? _input.Acceleration : localVelocity.z > 0.05f ? -1f : 0f;

                if (acceleration > -Deadzone && acceleration < Deadzone) // No acceleration input.
                    localVelocity.z = Mathf.MoveTowards(localVelocity.z, 0f, _modifiedStats.coastingDrag * deltaTime);
                else if (acceleration > Deadzone) // Positive acceleration input.
                    localVelocity.z = Mathf.MoveTowards(localVelocity.z, _modifiedStats.topSpeed,
                        acceleration * _modifiedStats.acceleration * deltaTime);
                else if (localVelocity.z > Deadzone) // Negative acceleration input and going forwards.
                    localVelocity.z = Mathf.MoveTowards(localVelocity.z, 0f,
                        -acceleration * _modifiedStats.braking * deltaTime);
                else // Negative acceleration input and not going forwards.
                    localVelocity.z = Mathf.MoveTowards(localVelocity.z, -_modifiedStats.reverseSpeed,
                        -acceleration * _modifiedStats.reverseAcceleration * deltaTime);
            }

            if (groundInfo.isCapsuleTouching)
                localVelocity.y = Mathf.Max(0f, localVelocity.y);

            _velocity = _driftOffset * rotationStream * localVelocity;

            if (!groundInfo.isCapsuleTouching)
                _velocity += Vector3.down * _modifiedStats.gravity * deltaTime;
        }

        /// <summary>
        ///     Affects the velocity of the kart if it hops.
        /// </summary>
        private void Hop(Quaternion rotationStream, GroundInfo currentGroundInfo)
        {
            if (currentGroundInfo.isGrounded && _input.HopPressed && _hasControl)
            {
                _velocity += rotationStream * Vector3.up * _modifiedStats.hopHeight;
                _input.HopPressed = false;
                OnHop.Invoke();
            }
        }

        /// <summary>
        ///     Starts a drift if the kart lands with a sufficient turn.
        /// </summary>
        private void StartDrift(GroundInfo currentGroundInfo, GroundInfo nextGroundInfo, Quaternion rotationStream)
        {
            if (_input.HopHeld && !currentGroundInfo.isGrounded && nextGroundInfo.isGrounded && _hasControl &&
                _driftState == DriftState.NotDrifting)
            {
                var kartForward = rotationStream * Vector3.forward;
                kartForward.y = 0f;
                kartForward.Normalize();
                var flatVelocity = _velocity;
                flatVelocity.y = 0f;
                flatVelocity.Normalize();

                var signedAngle = Vector3.SignedAngle(kartForward, flatVelocity, Vector3.up);

                if (signedAngle > minDriftStartAngle && signedAngle < maxDriftStartAngle)
                {
                    _driftOffset = Quaternion.Euler(0f, signedAngle, 0f);
                    _driftState = DriftState.FacingLeft;

                    OnDriftStart();
                }
                else if (signedAngle < -minDriftStartAngle && signedAngle > -maxDriftStartAngle)
                {
                    _driftOffset = Quaternion.Euler(0f, signedAngle, 0f);
                    _driftState = DriftState.FacingRight;

                    OnDriftStart();
                }
            }
        }

        /// <summary>
        ///     State to activate drifting effects.
        /// </summary>
        private void OnDriftStart()
        {
            _leftRainbowParticle.Play();
            _rightRainbowParticle.Play();
            _driftAudio.Play();
        }

        /// <summary>
        ///     State to deactivate drifting effects.
        /// </summary>
        private void OnDriftEnd()
        {
            _leftRainbowParticle.Stop();
            _rightRainbowParticle.Stop();
            _driftAudio.Stop();
        }

        /// <summary>
        ///     Stops a drift if the hop input is no longer held.
        /// </summary>
        private void StopDrift(float deltaTime)
        {
            if (!_input.HopHeld || !_hasControl)
            {
                _driftOffset = Quaternion.RotateTowards(_driftOffset, Quaternion.identity,
                    rotationCorrectionSpeed * deltaTime);
                _driftState = DriftState.NotDrifting;

                OnDriftEnd();
            }
        }

        /// <summary>
        ///     Changes the velocity of the kart and processes collisions based on the velocity of the kart.
        /// </summary>
        private Vector3 ProcessVelocityCollisions(float deltaTime, Quaternion rotationStream, Vector3 penetrationOffset)
        {
            var rayDirection = _velocity * deltaTime + penetrationOffset;
            var rayLength = rayDirection.magnitude;
            var sphereCastRay = new Ray(Position, rayDirection);
            var hits = Physics.SphereCastNonAlloc(sphereCastRay, 0.5f, _raycastHitBuffer, rayLength,
                allCollidingLayers, QueryTriggerInteraction.Collide);

            for (var i = 0; i < hits; i++)
            {
                if (_raycastHitBuffer[i].collider == _capsule)
                    continue;

                var kartModifier = _raycastHitBuffer[i].collider.GetComponent<IKartModifier>();
                if (kartModifier != null)
                {
                    _currentModifiers.Add(kartModifier);
                    _tempModifiers.Add(kartModifier);
                }

                var kartCollider = _raycastHitBuffer[i].collider.GetComponent<IKartCollider>();
                if (Mathf.Approximately(_raycastHitBuffer[i].distance, 0f))
                    if (Physics.Raycast(Position, rotationStream * Vector3.down, out var hit, rayLength + 0.5f,
                            allCollidingLayers, QueryTriggerInteraction.Collide))
                        _raycastHitBuffer[i] = hit;

                if (kartCollider != null)
                {
                    _velocity = kartCollider.ModifyVelocity(this, _raycastHitBuffer[i]);

                    OnKartCollision.Invoke();
                    
                }
                else
                {
                    penetrationOffset = ComputePenetrationOffset(rotationStream, penetrationOffset);
                }
            }

            return penetrationOffset;
        }

        /// <summary>
        ///     So that the velocity doesn't keep forcing a kart into a collider, the velocity is reduced by the penetrationOffset
        ///     without flipping the direction of the velocity.
        /// </summary>
        /// <param name="deltaTime">The time between frames.</param>
        /// <param name="penetrationOffset">The amount the kart needs to be moved in order to not overlap other colliders.</param>
        private void AdjustVelocityByPenetrationOffset(float deltaTime, ref Vector3 penetrationOffset)
        {
            // Find how much of the velocity is in the penetration offset's direction.
            var penetrationProjection = Vector3.Project(_velocity * deltaTime, penetrationOffset);

            // If the projection and offset are in opposite directions (more than 90 degrees between the velocity and offset) ...
            if (Vector3.Dot(penetrationOffset, penetrationProjection) < 0f)
            {
                // ... and if the offset is larger than the projection...
                if (penetrationOffset.sqrMagnitude > penetrationProjection.sqrMagnitude)
                {
                    // ... then reduce the velocity by the equivalent velocity of the projection and the the offset by the projection.
                    _velocity -= penetrationProjection / deltaTime;
                    penetrationOffset += penetrationProjection;
                }
                else // If the offset is smaller than the projection...
                {
                    // ... then reduce the velocity by the equivalent velocity of the offset and then there is the offset remaining.
                    _velocity += penetrationOffset / deltaTime;
                    penetrationOffset = Vector3.zero;
                }
            }

            Movement = _velocity * deltaTime + penetrationOffset;
        }

        /// <summary>
        ///     This adds a modifier to the karts stats.  This might be something like a speed boost pickup being activated.
        /// </summary>
        /// <param name="kartModifier">The modifier to the kart's stats.</param>
        public void AddKartModifier(IKartModifier kartModifier)
        {
            _currentModifiers.Add(kartModifier);
        }

        /// <summary>
        ///     This removes a previously added modifier to the karts stats.  This might be something like a speed boost that has
        ///     just run out.
        /// </summary>
        /// <param name="kartModifier"></param>
        public void RemoveKartModifier(IKartModifier kartModifier)
        {
            _currentModifiers.Remove(kartModifier);
        }

        /// <summary>
        ///     This exists as part of the IKartCollider interface.  It is called when a kart collides with this kart.
        /// </summary>
        /// <param name="collidingKart">The kart that has collided with this kart.</param>
        /// <param name="collisionHit">Data for the collision.</param>
        /// <returns>The velocity of the colliding kart once it has been modified.</returns>
        public Vector3 ModifyVelocity(IKartInfo collidingKart, RaycastHit collisionHit)
        {
            var weightDifference = collidingKart.CurrentStats.weight - _modifiedStats.weight;
            if (weightDifference <= 0f)
            {
                var toCollidingKart = (collidingKart.Position - Position).normalized;
                return collidingKart.Velocity + toCollidingKart * (kartToKartBump - weightDifference);
            }

            return collidingKart.Velocity;
        }

        /// <summary>
        ///     This exists as part of the IMovable interface.  Typically it is called by the TrackManager when the race starts.
        /// </summary>
        public void EnableControl()
        {
            _hasControl = true;
        }

        /// <summary>
        ///     This exists as part of the IMovable interface.  Typically it is called by the TrackManager when the kart finishes
        ///     its final lap.
        /// </summary>
        public void DisableControl()
        {
            _hasControl = false;
            _driftState = DriftState.NotDrifting;
        }

        /// <summary>
        ///     This exists as part of the IMovable interface.  Typically it is called by the TrackManager to determine whether
        ///     control should be re-enabled after a reposition.
        /// </summary>
        /// <returns></returns>
        public bool IsControlled()
        {
            return _hasControl;
        }

        /// <summary>
        ///     This exists as part of the IMovable interface.  It is used to move the kart to a specific position for example to
        ///     replace it when the kart falls off the track.
        /// </summary>
        public void ForceMove(Vector3 positionDelta, Quaternion rotationDelta)
        {
            _velocity = Vector3.zero;
            _driftState = DriftState.NotDrifting;
            _repositionPositionDelta = positionDelta;
            _repositionRotationDelta = rotationDelta;
        }

        /// <summary>
        ///     This exists as part of the IMovable interface.
        /// </summary>
        /// <returns>The racer component implementation of the IRacer interface.</returns>
        public IRacer GetRacer()
        {
            return _racer;
        }

        /// <summary>
        ///     This exists as part of the IMovable interface.
        /// </summary>
        /// <returns>The implementation of IKartInfo for this script.</returns>
        public IKartInfo GetKartInfo()
        {
            return this;
        }
    }
}