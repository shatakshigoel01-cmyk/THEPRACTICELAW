
using UnityEngine;
using System.Collections;

public class GuidePosterTiming : MonoBehaviour
{
    public GameObject poster;

    IEnumerator Start()
    {
        poster.SetActive(false);

        yield return new WaitForSeconds(3f);

        poster.SetActive(true);

        yield return new WaitForSeconds(10f);

        poster.SetActive(false);
    }
}
