
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class EvidenceMessage : MonoBehaviour
{
    public GameObject evidencePanel;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (evidencePanel != null)
            evidencePanel.SetActive(false);
    }

    private void OnEnable()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (evidencePanel != null)
            evidencePanel.SetActive(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (evidencePanel != null)
            evidencePanel.SetActive(false);
    }
}
