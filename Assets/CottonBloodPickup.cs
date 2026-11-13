
using UnityEngine;

public class CottonBloodPickup : MonoBehaviour
{
    public Renderer cottonRenderer;
    public Material redCottonMaterial;

    private bool hasBlood = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBlood) return;

        if (other.gameObject.name == "BloodSampleZone")
        {
            cottonRenderer.material = redCottonMaterial;
            hasBlood = true;

            Debug.Log("Blood collected! Cotton turned red.");
        }
    }
}
