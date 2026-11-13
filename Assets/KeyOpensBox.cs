
using UnityEngine;

public class KeyOpensBox : MonoBehaviour
{
    public Animator lidAnimator;

    public void OpenBox()
    {
        if (lidAnimator != null)
        {
            lidAnimator.SetTrigger("Open");
        }
    }
}
