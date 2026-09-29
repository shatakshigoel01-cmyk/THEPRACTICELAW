using UnityEngine;

public class WitnessTalk : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;

    public void StartTalking()
    {
        animator.SetTrigger("Talk");
        audioSource.Play();
    }

    void Update()
    {
        if (!audioSource.isPlaying && animator.GetCurrentAnimatorStateInfo(0).IsName("Talking"))
        {
            animator.Play("Idle");
        }
    }
}