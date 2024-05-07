using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KartGame.KartSystems
{
    /// <summary>
    ///     This class handles all the canned and procedural animation for the kart, giving it a more pleasing appearance.
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class KartAnimation : MonoBehaviour
    {
        /// <summary>
        ///     A class representing an individual wheel on a kart.  This can be used to represent either front or back wheels.
        /// </summary>
        [Serializable]
        public class Wheel
        {
            [Tooltip("A reference to the transform of the wheel.")]
            public Transform wheelTransform;

            [Tooltip(
                "A vector representing the axel around which the wheel turns as the kart moves forward and backward.  This is relative to the wheel.")]
            public Vector3 axelAxis;

            [Tooltip(
                "A vector around which the wheel will turn as the kart steers.  This is relative to the kart and will be ignored for the back wheels.")]
            public Vector3 steeringAxis;

            private Vector3 _normalizedAxelAxis;
            private Vector3 _normalizedSteeringAxis;
            private Quaternion _steerlessLocalRotation;

            /// <summary>
            ///     This initialises the cached values that the Wheel uses for various operations and should be called once before any
            ///     other methods.
            /// </summary>
            public void Setup()
            {
                _normalizedAxelAxis = axelAxis.normalized;
                _normalizedSteeringAxis = steeringAxis.normalized;
                _steerlessLocalRotation = wheelTransform.localRotation;
            }

            /// <summary>
            ///     Some rotations are made relative to a default rotation.  This should be called to store that rotation.
            /// </summary>
            public void StoreDefaultRotation()
            {
                _steerlessLocalRotation = wheelTransform.localRotation;
            }

            /// <summary>
            ///     Some rotations are made relative to a default rotation.  This restores that default rotation.
            /// </summary>
            public void SetToDefaultRotation()
            {
                wheelTransform.localRotation = _steerlessLocalRotation;
            }

            /// <summary>
            ///     Rotates the wheel around its axel.
            /// </summary>
            /// <param name="rotationAngle">The angle in degrees by which the wheel rotates.</param>
            public void TurnWheel(float rotationAngle)
            {
                wheelTransform.Rotate(_normalizedAxelAxis, rotationAngle, Space.Self);
            }

            /// <summary>
            ///     Rotates the wheel around its steering axis.
            /// </summary>
            /// <param name="rotationAngle">The angle from a neutral position that the wheel should face.</param>
            public void SteerWheel(float rotationAngle)
            {
                wheelTransform.Rotate(_normalizedSteeringAxis, rotationAngle, Space.World);
            }
        }


        [Tooltip(
            "A reference to the input that the kart being animated is using for movement.  This must implement IInput.")]
        [RequireInterface(typeof(IInput))]
        public Object input;

        [Tooltip("A reference to the animator of the humanoid character driving the kart.")]
        public Animator playerAnimator;

        [Tooltip(
            "A reference to a script that provides information about the kart's movement, usually the KartMovmeent script.  This must implement IKartInfo.")]
        [RequireInterface(typeof(IKartInfo))]
        public Object kartMovement;

        [Space]
        [Tooltip(
            "The damping for the appearance of steering compared to the input.  The higher the number the less damping.")]
        public float steeringAnimationDamping = 10f;

        [Space] [Tooltip("A reference to the transform that represents the steering wheel.")]
        public Transform steeringWheel;

        [Tooltip(
            "The maximum angle in degrees that the steering wheel can be turned away from its default position, when the Steering input is either 1 or -1.")]
        public float maxSteeringWheelAngle = 90f;

        [Tooltip("The axis, local to the steering wheel, around which the steering wheel should turn when steering.")]
        public Vector3 steeringWheelRotationAxis;

        [Space]
        [Tooltip(
            "The radius of the front wheels of the kart.  Used to calculate how far the front wheels need to turn given the speed of the kart.")]
        public float frontWheelRadius;

        [Tooltip(
            "The radius of the rear wheels of the kart.  Used to calculate how far the rear wheels need to turn given the speed of the kart.")]
        public float rearWheelRadius;

        [Tooltip(
            "The maximum angle in degrees that the front wheels can be turned away from their default positions, when the Steering input is either 1 or -1.")]
        public float maxSteeringAngle;

        [Tooltip("Information referring to the front left wheel of the kart.")]
        public Wheel frontLeftWheel;

        [Tooltip("Information referring to the front right wheel of the kart.")]
        public Wheel frontRightWheel;

        [Tooltip("Information referring to the rear left wheel of the kart.")]
        public Wheel rearLeftWheel;

        [Tooltip("Information referring to the rear right wheel of the kart.")]
        public Wheel rearRightWheel;

        private IInput _input;
        private IKartInfo _kartMovement;
        private Quaternion _defaultSteeringWheelLocalRotation;
        private Vector3 _normalizedSteeringWheelRotationAxis;
        private float _inverseFrontWheelRadius;
        private float _inverseRearWheelRadius;
        private float _smoothedSteeringInput;

        private static readonly int _hashSteering = Animator.StringToHash("Steering");
        private static readonly int _hashGrounded = Animator.StringToHash("Grounded");

        private void Start()
        {
            frontLeftWheel.Setup();
            frontRightWheel.Setup();
            rearLeftWheel.Setup();
            rearRightWheel.Setup();

            _kartMovement = kartMovement as IKartInfo;

            _input = input as IInput;
            _defaultSteeringWheelLocalRotation = steeringWheel.localRotation;
            _normalizedSteeringWheelRotationAxis = steeringWheelRotationAxis.normalized;
            _inverseFrontWheelRadius = 1f / frontWheelRadius;
            _inverseRearWheelRadius = 1f / rearWheelRadius;
        }

        private void FixedUpdate()
        {
            _smoothedSteeringInput = Mathf.MoveTowards(_smoothedSteeringInput, _input.Steering,
                steeringAnimationDamping * Time.deltaTime);

            playerAnimator.SetFloat(_hashSteering, _smoothedSteeringInput);
            playerAnimator.SetBool(_hashGrounded, _kartMovement.IsGrounded);
        }

        private void LateUpdate()
        {
            SteeringWheelRotation();
            RotateWheels();
        }

        private void SteeringWheelRotation()
        {
            var axisRotation = maxSteeringWheelAngle * _smoothedSteeringInput * _normalizedSteeringWheelRotationAxis;
            steeringWheel.localRotation = _defaultSteeringWheelLocalRotation * Quaternion.Euler(axisRotation);
        }

        private void RotateWheels()
        {
            frontLeftWheel.SetToDefaultRotation();
            frontRightWheel.SetToDefaultRotation();

            var speed = _kartMovement.LocalSpeed;
            var rotationAngle = speed * Time.deltaTime * _inverseFrontWheelRadius * Mathf.Rad2Deg;
            frontLeftWheel.TurnWheel(rotationAngle);
            frontRightWheel.TurnWheel(rotationAngle);

            rotationAngle = speed * Time.deltaTime * _inverseRearWheelRadius * Mathf.Rad2Deg;
            rearLeftWheel.TurnWheel(rotationAngle);
            rearRightWheel.TurnWheel(rotationAngle);

            frontLeftWheel.StoreDefaultRotation();
            frontRightWheel.StoreDefaultRotation();

            rotationAngle = _smoothedSteeringInput * maxSteeringAngle;
            frontLeftWheel.SteerWheel(rotationAngle);
            frontRightWheel.SteerWheel(rotationAngle);
        }
    }
}