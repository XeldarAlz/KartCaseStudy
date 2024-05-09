using System;
using UnityEngine;

namespace KartSystem.Track
{
    /// <summary>
    /// This is used to mark out key points on the track that a racer must pass through in order to count as having completed a lap.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class Checkpoint : MonoBehaviour
    {
        /// <summary>
        /// This is subscribed to by the TrackManager in order to measure a racer's progress around the track.
        /// </summary>

        public delegate void KartHitDelegate(IRacer racer, Checkpoint checkpoint);
        public event KartHitDelegate OnKartHit;

        public event Action<IRacer, Checkpoint> OnKartHitCheckpoint;

        [Tooltip("Whether or not this checkpoint is the start/finish line.")]
        public bool isStartFinishLine;
        [Tooltip("The layers to check for a kart passing through this trigger.")]
        public LayerMask kartLayers;
        [Tooltip("The layers to check for the ground.  Used to determine where the reset position for a kart is.")]
        public LayerMask groundLayers;

        private Vector3 _resetPosition;
        private Quaternion _resetRotation;

        public Vector3 ResetPosition => _resetPosition;
        public Quaternion ResetRotation => _resetRotation;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            kartLayers = LayerMask.GetMask("Default");
        }

        private void Start()
        {
            float boxColliderHeight = GetComponent<BoxCollider>().size.y;
            Ray ray = new(transform.position + Vector3.up * boxColliderHeight * 0.5f, -Vector3.up);

            RaycastHit[] hits = Physics.RaycastAll(ray, boxColliderHeight, groundLayers, QueryTriggerInteraction.Ignore);

            if (hits.Length == 0)
                throw new UnityException("This checkpoint is not above ground and has no set reset position.");

            RaycastHit closestGround = new()
            {
                distance = float.PositiveInfinity
            };

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < closestGround.distance)
                    closestGround = hits[i];
            }

            _resetPosition = closestGround.point;
            _resetRotation = Quaternion.LookRotation(transform.forward, closestGround.normal);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (kartLayers.ContainsGameObjectsLayer(other.gameObject))
            {
                IRacer racer = other.GetComponent<IRacer>();
                if (racer != null)
                {
                    OnKartHitCheckpoint?.Invoke(racer, this);
                }
            }
        }
    }
}