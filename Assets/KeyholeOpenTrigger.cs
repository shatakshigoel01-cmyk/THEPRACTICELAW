
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeyholeOpenTrigger : MonoBehaviour
{
    public Animator lidAnimator;

    private XRGrabInteractable currentKey;

    private void OnTriggerStay(Collider other)
    {
        XRGrabInteractable key =
            other.GetComponentInParent<XRGrabInteractable>();

        if (key != null && key.isSelected)
        {
            currentKey = key;
            lidAnimator.SetBool("NearKeyhole", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        XRGrabInteractable key =
            other.GetComponentInParent<XRGrabInteractable>();

        if (key != null && key == currentKey)
        {
            currentKey = null;
            lidAnimator.SetBool("NearKeyhole", false);
        }
    }
}
