namespace KartSystem.KartSystems
{
    /// <summary>
    ///     This can work as a placeholder for kart modifiers that have yet to be created.  It does not modify the stats at
    ///     all.
    /// </summary>
    public struct DefaultKartModifier : IKartModifier
    {
        public float ModifyAcceleration(float acceleration) => acceleration;
        public float ModifyBraking(float braking) => braking;
        public float ModifyCoastingDrag(float coastingDrag) => coastingDrag;
        public float ModifyGravity(float gravity) => gravity;
        public float ModifyGrip(float grip) => grip;
        public float ModifyHopHeight(float hopHeight) => hopHeight;
        public float ModifyReverseAcceleration(float reverseAcceleration) => reverseAcceleration;
        public float ModifyReverseSpeed(float reverseSpeed) => reverseSpeed;
        public float ModifyTopSpeed(float topSpeed) => topSpeed;
        public float ModifyTurnSpeed(float turnSpeed) => turnSpeed;
        public float ModifyWeight(float weight) => weight;
    }
}