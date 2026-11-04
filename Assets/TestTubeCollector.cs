using UnityEngine;

public class TestTubeCollector : MonoBehaviour
{
    [Header("Blood Sample")]
    public GameObject bloodInside;

    private bool sampleCollected = false;

    private void Start()
    {
        if (bloodInside != null)
        {
            bloodInside.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sampleCollected)
            return;

        CottonBloodSample cotton = other.GetComponentInParent<CottonBloodSample>();

        if (cotton != null && cotton.hasBlood)
        {
            sampleCollected = true;

            other.GetComponentInParent<CottonBloodSample>()
                .gameObject.SetActive(false);

            if (bloodInside != null)
            {
                bloodInside.SetActive(true);
            }

            Debug.Log("Blood sample successfully stored in test tube!");
        }
    }
}