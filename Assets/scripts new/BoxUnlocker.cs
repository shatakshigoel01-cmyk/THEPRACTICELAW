
using UnityEngine;
using System.Collections;

public class BoxUnlocker : MonoBehaviour
{
    public Transform keyObject;
    public Transform lidPivot;
    public float openAngle = -100f;
    public float openSpeed = 1f;

    private bool isOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isOpen || keyObject == null || lidPivot == null)
            return;

        Transform touchedObject = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform;

        if (touchedObject != keyObject &&
            !touchedObject.IsChildOf(keyObject))
            return;

        StartCoroutine(OpenLid());
    }

    private IEnumerator OpenLid()
    {
        isOpen = true;

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
