using System;
using UnityEngine;

namespace Game.Settings
{
    [Serializable]
    public class PlayerMovementSettings
    {
        [Header("Movement")]
        public bool CanMove = true;
        public bool IsGrounded = true;
        public float ForwardSpeed;
        public float RevSpeed;
        public float TurnSpeed;
        public float NormalDrag;
        public float ModifiedDrag;
        public float AlignToGroundTime;
        public LayerMask GroundLayer;
    }
}