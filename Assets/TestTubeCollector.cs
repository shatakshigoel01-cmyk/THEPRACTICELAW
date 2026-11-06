using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TestTubeCollector : MonoBehaviour
{
    [Header("Blood Sample")]
    public GameObject bloodInside;

    [Tooltip("Socket at the tube opening that receives the swab.")]
    public XRSocketInteractor socket;

    /// <summary>Raised once when a bloodied swab has been stored in the tube.</summary>
    public event Action SampleStored;

    public bool SampleCollected { get; private set; }

    private void Start()
    {
        if (bloodInside != null)
        {
            bloodInside.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (socket != null)
            socket.selectEntered.AddListener(OnSwabPlaced);
    }

    private void OnDisable()
    {
        if (socket != null)
            socket.selectEntered.RemoveListener(OnSwabPlaced);
    }

    private void OnSwabPlaced(SelectEnterEventArgs args)
    {
        if (SampleCollected)
            return;

        CottonBloodSample swab = args.interactableObject.transform.GetComponent<CottonBloodSample>();

        if (swab == null || !swab.hasBlood)
            return;

        SampleCollected = true;
        StartCoroutine(StoreSwab(swab, args.interactableObject));
    }

    private IEnumerator StoreSwab(CottonBloodSample swab, IXRSelectInteractable interactable)
    {
        // Selection events must finish before the swab can be released from the socket.
        yield return null;

        if (bloodInside != null)
        {
            bloodInside.SetActive(true);
        }

        socket.socketActive = false;
        socket.interactionManager.SelectExit(socket, interactable);

        XRGrabInteractable grab = swab.GetComponent<XRGrabInteractable>();
        if (grab != null)
            grab.enabled = false;

        Rigidbody body = swab.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.isKinematic = true;
        }

        foreach (Collider swabCollider in swab.GetComponentsInChildren<Collider>())
            swabCollider.enabled = false;

        // The swab stays inside the tube, and follows it if the tube is carried.
        Transform tube = socket.transform.parent;
        Vector3 worldPosition = swab.transform.position;
        Vector3 tubeScale = tube.lossyScale;

        swab.transform.SetParent(tube, false);
        swab.transform.localRotation = Quaternion.identity;
        swab.transform.localScale = new Vector3(1f / tubeScale.x, 1f / tubeScale.y, 1f / tubeScale.z);
        swab.transform.position = worldPosition;

        Debug.Log("Blood sample successfully stored in test tube!");
        SampleStored?.Invoke();
    }
}
