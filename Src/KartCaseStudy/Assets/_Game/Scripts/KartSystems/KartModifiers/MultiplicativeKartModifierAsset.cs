using UnityEngine;

namespace KartSystem.KartSystems
{
    /// <summary>
    ///     A ScriptableObject that takes the current stats and multiplies each.
    /// </summary>
    [CreateAssetMenu]
    public class MultiplicativeKartModifierAsset : ScriptableObject, IKartModifier
    {
        [Tooltip("The stats to be multiplied by the current stats.")]
        public KartStats statMultipliers = new(1f);

        public float ModifyAcceleration(float acceleration) => acceleration * statMultipliers.acceleration;
        public float ModifyBraking(float braking) => braking * statMultipliers.braking;
        public float ModifyCoastingDrag(float coastingDrag) => coastingDrag * statMultipliers.coastingDrag;
        public float ModifyGravity(float gravity) => gravity * statMultipliers.gravity;
        public float ModifyGrip(float grip) => grip * statMultipliers.grip;
        public float ModifyHopHeight(float hopHeight) => hopHeight * statMultipliers.hopHeight;
        public float ModifyReverseAcceleration(float reverseAcceleration) => reverseAcceleration * statMultipliers.reverseAcceleration;
        public float ModifyReverseSpeed(float reverseSpeed) => reverseSpeed * statMultipliers.reverseSpeed;
        public float ModifyTopSpeed(float topSpeed) => topSpeed * statMultipliers.topSpeed;
        public float ModifyTurnSpeed(float turnSpeed) => turnSpeed * statMultipliers.turnSpeed;
        public float ModifyWeight(float weight) => weight * statMultipliers.weight;
    }
}