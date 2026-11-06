using UnityEngine;

public class CottonBloodSample : MonoBehaviour
{
    public Renderer cottonRenderer;
    public Material redCottonMaterial;

    [Tooltip("Trigger volume placed over the blood stain.")]
    public Collider bloodZone;

    [Tooltip("Red tip shown after collection. Its position is the point that must touch the stain.")]
    public GameObject redTip;

    public bool hasBlood = false;

    private void Awake()
    {
        if (redTip != null)
            redTip.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (hasBlood || bloodZone == null || other != bloodZone)
            return;

        // The swab's whole collider enters the zone, but only its tip takes a sample.
        if (redTip != null && !bloodZone.bounds.Contains(redTip.transform.position))
            return;

        CollectSample();
    }

    private void CollectSample()
    {
        hasBlood = true;

        if (redTip != null)
            redTip.SetActive(true);
        else if (cottonRenderer != null)
            cottonRenderer.material = redCottonMaterial;

        Debug.Log("Blood sample collected!");
    }
}
