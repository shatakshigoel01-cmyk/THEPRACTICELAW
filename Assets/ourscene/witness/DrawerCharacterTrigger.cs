
using UnityEngine;
using System.Collections;

public class DrawerCharacterTrigger : MonoBehaviour
{
    public GameObject character;
    public AudioSource dialogue;
    public Animator animator;
    public GameObject zoneVisual;

    private bool hasPlayed = false;

    private void Start()
    {
        character.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;

        if (other.CompareTag("MainCamera"))
        {
            hasPlayed = true;

            if (zoneVisual != null)
                zoneVisual.SetActive(false);

            StartCoroutine(PlayCharacterScene());
        }
    }

    IEnumerator PlayCharacterScene()
    {
        character.SetActive(true);

        if (dialogue != null)
        {
            dialogue.Play();
            yield return new WaitForSeconds(dialogue.clip.length);
        }

        character.SetActive(false);
    }
}
