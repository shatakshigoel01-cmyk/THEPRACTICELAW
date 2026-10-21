
using UnityEngine;

public class ForensicLightReveal : MonoBehaviour
{
    public Light forensicLight;
    public Renderer fingerprintRenderer;
    public Transform fingerprintTarget;

    void Start()
    {
        if (fingerprintRenderer != null)
            fingerprintRenderer.enabled = false;
    }

    void Update()
    {
        if (forensicLight == null ||
            fingerprintRenderer == null ||
            fingerprintTarget == null)
            return;

        Vector3 direction =
            fingerprintTarget.position -
            forensicLight.transform.position;

        float distance = direction.magnitude;

        float angle = Vector3.Angle(
            forensicLight.transform.forward,
            direction
        );

        bool lightOnTarget =
            forensicLight.enabled &&
            distance <= forensicLight.range &&
            angle <= forensicLight.spotAngle * 0.5f;

        fingerprintRenderer.enabled = lightOnTarget;
    }
}
