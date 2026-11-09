
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections;

/// <summary>
/// Opens the box lid when the key is placed in the keyhole.
/// With a keyhole socket assigned, the key must be released into the keyhole and snap in;
/// without one, touching the keyhole zone with the key is enough.
/// </summary>
public class BoxUnlocker : MonoBehaviour
{
    public Transform keyObject;
    public Transform lidPivot;
    public float openAngle = -100f;
    public float openSpeed = 1f;

    [Tooltip("Socket in the keyhole. When set, the lid opens only once the key is seated in it.")]
    public XRSocketInteractor keyholeSocket;

    [Tooltip("Raised once when the lid starts to open, for example to play an unlock sound.")]
    public UnityEvent onUnlocked = new UnityEvent();

    public bool IsOpen => isOpen;

    private bool isOpen = false;

    private void OnEnable()
    {
        if (keyholeSocket != null)
            keyholeSocket.selectEntered.AddListener(OnKeyInserted);
    }

    private void OnDisable()
    {
        if (keyholeSocket != null)
            keyholeSocket.selectEntered.RemoveListener(OnKeyInserted);
    }

    private void OnKeyInserted(SelectEnterEventArgs args)
    {
        if (IsKey(args.interactableObject.transform))
            Unlock();
    }

    private void OnTriggerEnter(Collider other)
    {
        // With a socket the key has to be seated in the keyhole, not just touch it.
        if (keyholeSocket != null)
            return;

        Transform touchedObject = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform;

        if (IsKey(touchedObject))
            Unlock();
    }

    private bool IsKey(Transform candidate)
    {
        return keyObject != null && candidate != null &&
               (candidate == keyObject || candidate.IsChildOf(keyObject));
    }

    private void Unlock()
    {
        if (isOpen || lidPivot == null)
            return;

        StartCoroutine(OpenLid());
    }

    private IEnumerator OpenLid()
    {
        isOpen = true;
        onUnlocked.Invoke();

        Quaternion startRotation = lidPivot.localRotation;
        Quaternion targetRotation =
            startRotation * Quaternion.Euler(openAngle, 0f, 0f);

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime * openSpeed;
            lidPivot.localRotation =
                Quaternion.Slerp(startRotation, targetRotation, time);
            yield return null;
        }

        lidPivot.localRotation = targetRotation;
    }
}
