
using UnityEngine;
using System.Collections;

public class MissionIntroduction : MonoBehaviour
{
    public GameObject character;
    public AudioClip dialogue;

    public float idleDuration = 3f;

    private IEnumerator Start()
    {
        character.SetActive(true);

        Animator animator = character.GetComponentInChildren<Animator>();
        AudioSource voice = character.GetComponentInChildren<AudioSource>();

        if (animator != null)
        {
            animator.SetBool("IsTalking", false);
        }

        yield return new WaitForSeconds(idleDuration);

        if (animator != null)
        {
            animator.SetBool("IsTalking", true);
        }

        if (voice != null && dialogue != null)
        {
            voice.clip = dialogue;
            voice.Play();

            yield return new WaitForSeconds(dialogue.length);
        }

        character.SetActive(false);
    }
}
