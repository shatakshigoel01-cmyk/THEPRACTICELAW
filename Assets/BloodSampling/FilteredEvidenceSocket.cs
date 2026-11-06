using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// A socket that accepts one specific object, optionally only once it holds a blood sample.
/// </summary>
public class FilteredEvidenceSocket : XRSocketInteractor
{
    [Header("Evidence Filter")]
    [SerializeField] private XRBaseInteractable allowedObject;
    [SerializeField] private bool requireBloodSample;

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        return base.CanHover(interactable) && IsAccepted(interactable.transform);
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && IsAccepted(interactable.transform);
    }

    private bool IsAccepted(Transform candidate)
    {
        if (allowedObject == null || candidate != allowedObject.transform)
            return false;

        if (!requireBloodSample)
            return true;

        CottonBloodSample swab = candidate.GetComponent<CottonBloodSample>();
        return swab != null && swab.hasBlood;
    }
}
