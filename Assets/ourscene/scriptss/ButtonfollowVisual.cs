using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ButtonFollowVisual : MonoBehaviour
{

    public Transform visualTarget;
    public Vector3 localaxis;
    private Vector3 offset;
    private Transform pokeAttachTransform;

    private XRBaseInteractable Interactable;
    private bool isfollowing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Interactable = GetComponent<XRBaseInteractable>();
        Interactable.hoverEntered.AddListener(follow);
    }
    public void follow(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor)
        {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;
            isfollowing = true;
            pokeAttachTransform = interactor.attachTransform;
            offset = visualTarget.position - pokeAttachTransform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isfollowing)
        {
            Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
            Vector3 constrainedLocalTargetPosition = Vector3.Project(localTargetPosition, localaxis);

            visualTarget.position = visualTarget.TransformPoint(constrainedLocalTargetPosition);
        }
    }
}