using System;
using UnityEngine;

namespace KartSystem.KartSystems
{
    /// <summary>
    ///     A plain serializable class that takes the current stats and multiplies each.
    /// </summary>
    [Serializable]
    public class MultiplicativeKartModifier : MonoBehaviour, IKartModifier
    {
        [Tooltip("The stats to be multiplied by the current stats.")]
        public KartStats modifiers;

        /// <summary>
        ///     Initialises the KartStats to a default value of 1.
        /// </summary>
        public MultiplicativeKartModifier() => modifiers = new KartStats(1f);
        public float ModifyAcceleration(float acceleration) => modifiers.acceleration * acceleration;
        public float ModifyBraking(float braking) => modifiers.braking * braking;
        public float ModifyCoastingDrag(float coastingDrag) => modifiers.coastingDrag * coastingDrag;
        public float ModifyGravity(float gravity) => modifiers.gravity * gravity;
        public float ModifyGrip(float grip) => modifiers.grip * grip;
        public float ModifyHopHeight(float hopHeight) => modifiers.hopHeight * hopHeight;
        public float ModifyReverseAcceleration(float reverseAcceleration) => modifiers.reverseAcceleration * reverseAcceleration;
        public float ModifyReverseSpeed(float reverseSpeed) => modifiers.reverseSpeed * reverseSpeed;
        public float ModifyTopSpeed(float topSpeed) => modifiers.topSpeed * topSpeed;
        public float ModifyTurnSpeed(float turnSpeed) => modifiers.turnSpeed * turnSpeed;
        public float ModifyWeight(float weight) => modifiers.weight * weight;
    }
}