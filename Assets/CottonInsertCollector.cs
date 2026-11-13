
using UnityEngine;

public class CottonInsertCollector : MonoBehaviour
{
    public GameObject bloodFill;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        CottonBloodSample cotton =
            other.GetComponentInParent<CottonBloodSample>();

        if (cotton == null || !cotton.hasBlood)
            return;

        collected = true;

        if (bloodFill != null)
            bloodFill.SetActive(true);

        Destroy(cotton.gameObject);

        Debug.Log("Blood sample stored in test tube!");
    }
}
