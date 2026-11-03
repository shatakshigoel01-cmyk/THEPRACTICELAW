
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// VR gripped button. Responds when an XR controller interacts with it.
/// </summary>
[RequireComponent(typeof(VRButton))]
public class VRGrippedButton : MonoBehaviour
{
    [Header("Button Animation")]
    public Animation ButtonAnim;

    private VRButton Button;
    private bool isActivated = false;

    private void Awake()
    {
        Button = GetComponent<VRButton>();

        if (Button == null)
        {
            Debug.LogError("VRGrippedButton: VRButton component is missing.");
        }

        Collider buttonCollider = GetComponent<Collider>();

        if (buttonCollider != null)
        {
            buttonCollider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("VRGrippedButton: Add a Collider to this GameObject.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Button == null || !Button.Interactable)
            return;

        // Check whether the entering object belongs to an XR controller.
        XRBaseInteractor interactor =
            other.GetComponentInParent<XRBaseInteractor>();

        if (interactor == null)
            return;

        // Activate when the XR controller is interacting with the button.
        ActivateButton();
    }

    public void ActivateButton()
    {
        if (Button == null || !Button.Interactable)
            return;

        if (isActivated)
            return;

        isActivated = true;

        if (ButtonAnim != null)
        {
            ButtonAnim.Play();
        }

        // Notify the VRButton component and its configured listeners.
        SendMessage(
            "TriggerButton",
            SendMessageOptions.DontRequireReceiver
        );
    }

    private void OnTriggerExit(Collider other)
    {
        XRBaseInteractor interactor =
            other.GetComponentInParent<XRBaseInteractor>();

        if (interactor != null)
        {
            isActivated = false;
        }
    }
}
