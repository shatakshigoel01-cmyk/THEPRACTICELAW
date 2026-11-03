
using UnityEngine;

public class UVFootprintReveal : MonoBehaviour
{
    public Light uvLight;
    public Renderer footprintRenderer;

    void Start()
    {
        if (footprintRenderer == null)
            footprintRenderer = GetComponent<Renderer>();

        if (footprintRenderer != null)
            footprintRenderer.enabled = false;
    }

    void Update()
    {
        if (uvLight == null || footprintRenderer == null)
            return;

        Ray ray = new Ray(uvLight.transform.position, uvLight.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, uvLight.range))
        {
            bool isFootprint = hit.collider.gameObject == gameObject;
            bool insideBeam = Vector3.Angle(uvLight.transform.forward,
                hit.point - uvLight.transform.position) < uvLight.spotAngle / 2f;

            footprintRenderer.enabled = isFootprint && insideBeam;
        }
        else
        {
            footprintRenderer.enabled = false;
        }
    }
}
