
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class VRHandle : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Rigidbody leverBody;
    private FixedJoint handleJoint;

    public float breakForces = 1000f;

    void Awake()
    {
        leverBody = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Allow the XR Interaction Toolkit to handle grabbing.
        grabInteractable.selectMode = InteractableSelectMode.Single;
        grabInteractable.throwOnDetach = false;
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
        RemoveJoint();
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        RemoveJoint();

        Transform controllerTransform = args.interactorObject.transform;
        Rigidbody controllerBody =
            controllerTransform.GetComponentInParent<Rigidbody>();

        if (controllerBody == null)
        {
            Debug.LogWarning(
                "VRHandle: No Rigidbody found on the grabbing controller."
            );
            return;
        }

        handleJoint = gameObject.AddComponent<FixedJoint>();
        handleJoint.connectedBody = controllerBody;
        handleJoint.breakForce = breakForces;
        handleJoint.breakTorque = breakForces;
        handleJoint.enableCollision = false;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        RemoveJoint();
    }

    void RemoveJoint()
    {
        if (handleJoint != null)
        {
            Destroy(handleJoint);
            handleJoint = null;
        }
    }
}
