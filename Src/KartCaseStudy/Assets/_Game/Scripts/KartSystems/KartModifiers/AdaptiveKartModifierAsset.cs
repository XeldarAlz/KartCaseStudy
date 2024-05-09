using UnityEngine;

namespace KartSystem.KartSystems
{
    /// <summary>
    ///     A ScriptableObject that takes the current stats and adds to each.
    /// </summary>
    [CreateAssetMenu]
    public class AdaptiveKartModifierAsset : ScriptableObject, IKartModifier
    {
        [Tooltip("The stats to be added to the current stats.")]
        public KartStats statAdditions = new(0f);

        public float ModifyAcceleration(float acceleration) => acceleration + statAdditions.acceleration;
        public float ModifyBraking(float braking) => braking + statAdditions.braking;
        public float ModifyCoastingDrag(float coastingDrag) => coastingDrag + statAdditions.coastingDrag;
        public float ModifyGravity(float gravity) => gravity + statAdditions.gravity;
        public float ModifyGrip(float grip) => grip + statAdditions.grip;
        public float ModifyHopHeight(float hopHeight) => hopHeight + statAdditions.hopHeight;
        public float ModifyReverseAcceleration(float reverseAcceleration) => reverseAcceleration + statAdditions.reverseAcceleration;
        public float ModifyReverseSpeed(float reverseSpeed) => reverseSpeed + statAdditions.reverseSpeed;
        public float ModifyTopSpeed(float topSpeed) => topSpeed + statAdditions.topSpeed;
        public float ModifyTurnSpeed(float turnSpeed) => turnSpeed + statAdditions.turnSpeed;
        public float ModifyWeight(float weight) => weight + statAdditions.weight;
    }
}