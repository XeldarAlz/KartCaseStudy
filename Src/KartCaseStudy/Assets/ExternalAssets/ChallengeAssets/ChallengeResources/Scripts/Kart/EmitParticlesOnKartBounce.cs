using UnityEngine;

public class EmitParticlesOnKartBounce : MonoBehaviour
{
    private ParticleSystem p;

    private void Awake()
    {
        p = GetComponent<ParticleSystem>();
        var kart = GetComponentInParent<KartSystem.KartSystems.KartMovement>();
        var capsule = kart.GetComponent<CapsuleCollider>();
        capsule.height = Mathf.Clamp(capsule.height, 0, 1f);

        kart.OnKartCollision.AddListener(KartCollision_OnExecute);
    }

    private void KartCollision_OnExecute()
    {
        p.Play();
    }
}