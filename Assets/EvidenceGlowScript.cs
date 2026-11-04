
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class EvidenceGlowScript : MonoBehaviour
{
    public Material glowMaterial;

    private XRGrabInteractable grab;
    private Renderer[] renderers;
    private Material[][] originalMaterials;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        renderers = GetComponentsInChildren<Renderer>();

        originalMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].materials;
    }

    void OnEnable()
    {
        if (grab == null)
            grab = GetComponent<XRGrabInteractable>();

        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
    }

    void OnDisable()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrab);
            grab.selectExited.RemoveListener(OnRelease);
        }

        RestoreMaterials();
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (glowMaterial == null) return;

        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.material = glowMaterial;
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        RestoreMaterials();
    }

    void RestoreMaterials()
    {
        if (renderers == null || originalMaterials == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                renderers[i].materials = originalMaterials[i];
        }
    }
}
