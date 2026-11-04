using UnityEngine;

public class CottonBloodSample : MonoBehaviour
{
    public Renderer cottonRenderer;
    public Material redCottonMaterial;

    public bool hasBlood = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "BloodSampleZone" && !hasBlood)
        {
            cottonRenderer.material = redCottonMaterial;
            hasBlood = true;

            Debug.Log("Blood sample collected!");
        }
    }
}