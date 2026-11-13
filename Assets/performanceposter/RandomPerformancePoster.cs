
using UnityEngine;

public class RandomPerformancePoster : MonoBehaviour
{
    public GameObject posterDisplay;
    public Material[] posterMaterials;

    public GameObject posterSpotlight;

    private bool hasTriggered = false;

    private void Start()
    {
        posterDisplay.SetActive(false);
        posterSpotlight.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("MainCamera"))
        {
            hasTriggered = true;

            int randomIndex = Random.Range(0, posterMaterials.Length);

            posterDisplay.GetComponent<Renderer>().material =
                posterMaterials[randomIndex];

            posterDisplay.SetActive(true);
            posterSpotlight.SetActive(true);

            posterDisplay.GetComponent<AudioSource>().Play();
            posterSpotlight.GetComponent<AudioSource>().Play();

            Debug.Log("Random performance poster displayed!");
        }
    }
}
