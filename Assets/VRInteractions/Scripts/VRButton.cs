
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class VRButtonEvent : UnityEvent<VRButton> { }

public class VRButton : VRInteractable
{
    public VRButtonEvent ButtonListeners;

    private List<VRGripper> ActiveControllers =
        new List<VRGripper>();

    public float TriggerHapticStrength = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        if (Interactable && other.name == "Switch")
        {
            TriggerButton();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Interactable && collision.collider.name == "Switch")
        {
            TriggerButton();
        }

        if (collision.rigidbody == null)
            return;

        VRGripper gripper =
            collision.rigidbody.GetComponent<VRGripper>();

        if (gripper != null && !ActiveControllers.Contains(gripper))
        {
            ActiveControllers.Add(gripper);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody == null)
            return;

        VRGripper gripper =
            collision.rigidbody.GetComponent<VRGripper>();

        if (gripper != null)
        {
            ActiveControllers.Remove(gripper);
        }
    }

    void TriggerButton()
    {
        if (!Interactable)
            return;

        if (ButtonListeners != null)
        {
            ButtonListeners.Invoke(this);
        }

        // Legacy SteamVR haptic feedback is disabled.
    }
}
