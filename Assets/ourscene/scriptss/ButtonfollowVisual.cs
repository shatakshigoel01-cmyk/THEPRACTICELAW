using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ButtonFollowVisual : MonoBehaviour
{
    public Transform visualTarget;
    public Vector3 localaxis;
    public float resetspeed = 5f;
    public float followAngletres = 45f;

    private Vector3 offset;
    private bool isFrozen = false;
    private Vector3 initiallocalpos;

    private Transform pokeAttachTransform;

    private XRBaseInteractable interactable;
    private bool isFollowing = false;

    void Start()
    {
        initiallocalpos = visualTarget.localPosition;

        interactable = GetComponent<XRBaseInteractable>();

        interactable.hoverEntered.AddListener(Follow);
        interactable.hoverExited.AddListener(ResetVisual);
        interactable.selectEntered.AddListener(Freeze);
    }

    public void Follow(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            XRPokeInteractor interactor =
                (XRPokeInteractor)hover.interactorObject;

            pokeAttachTransform = interactor.attachTransform;

            offset = visualTarget.position - pokeAttachTransform.position;

            float pokeangle = Vector3.Angle(
                offset,
                visualTarget.TransformDirection(localaxis)
            );

            if (pokeangle < followAngletres)
            {
                isFollowing = true;
                isFrozen = false;
            }
        }
    }

    public void ResetVisual(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            isFollowing = false;
            isFrozen = false;
        }
    }

    public void Freeze(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            isFrozen = true;
        }
    }

    void Update()
    {
        if (isFrozen)
            return;

        if (isFollowing)
        {
            Vector3 localTargetPosition =
                visualTarget.InverseTransformPoint(
                    pokeAttachTransform.position + offset
                );

            Vector3 constrainedLocalTargetPosition =
                Vector3.Project(
                    localTargetPosition,
                    localaxis
                );

            visualTarget.position =
                visualTarget.TransformPoint(
                    constrainedLocalTargetPosition
                );
        }
        else
        {
            visualTarget.localPosition = Vector3.Lerp(
                visualTarget.localPosition,
                initiallocalpos,
                Time.deltaTime * resetspeed
            );
        }
    }
}