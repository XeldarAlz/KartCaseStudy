using System;
using UnityEngine;

namespace Game.Settings
{
    [Serializable]
    public class PlayerInteractionSettings
    {
        [Header("Interaction")]
        public float RayCastFrequency = 0.1f;
        public bool CanInteract = true; 
        public float InteractionRange = 5f;
        public LayerMask InteractLayer;

        [Header("Debug")]
        public bool ShowDebugRay = true;
        public Color DebugRayColor = Color.blue;
    }
}