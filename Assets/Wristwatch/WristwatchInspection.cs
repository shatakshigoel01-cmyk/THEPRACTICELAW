using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Shows the wristwatch inspection panel when the player approaches or picks up the watch,
/// and records the watch as inspected when the player confirms it.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class WristwatchInspection : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text confirmLabel;
    [SerializeField] private GameObject confirmChevron;
    [SerializeField] private GameObject confirmationMessage;
    [SerializeField] private Button closeButton;

    [Header("Text")]
    [SerializeField] private string confirmText = "CONFIRM EVIDENCE";
    [SerializeField] private string confirmedText = "EVIDENCE CONFIRMED";

    [Tooltip("Also confirm when the player pulls the trigger while holding the watch.")]
    [SerializeField] private bool confirmWithTrigger = true;

    [Header("Proximity (metres, measured across the floor)")]
    [Tooltip("Where distance is measured from. Defaults to the panel, which stays fixed near the watch.")]
    [SerializeField] private Transform proximityAnchor;
    [SerializeField] private float showDistance = 1.5f;
    [SerializeField] private float hideDistance = 1.9f;

    [Tooltip("Raised once when the wristwatch is confirmed as inspected.")]
    public UnityEvent onEvidenceConfirmed = new UnityEvent();

    public bool IsInspected { get; private set; }

    private XRGrabInteractable grab;
    private Transform head;
    private bool inRange;
    private bool closedByPlayer;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        RefreshConfirmState();
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrabbed);
        grab.selectExited.AddListener(OnReleased);
        grab.activated.AddListener(OnActivated);
        confirmButton.onClick.AddListener(ConfirmEvidence);
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
        grab.selectExited.RemoveListener(OnReleased);
        grab.activated.RemoveListener(OnActivated);
        confirmButton.onClick.RemoveListener(ConfirmEvidence);
        if (closeButton != null)
            closeButton.onClick.RemoveListener(ClosePanel);
    }

    private void Update()
    {
        if (head == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                return;
            head = mainCamera.transform;
        }

        Transform anchor = proximityAnchor != null ? proximityAnchor : panel.transform;
        Vector3 offset = head.position - anchor.position;
        offset.y = 0f;
        float distance = offset.magnitude;

        // The gap between the two distances stops the panel flickering at the edge.
        if (!inRange && distance <= showDistance)
        {
            inRange = true;
        }
        else if (inRange && distance >= hideDistance && !IsHeldByHand())
        {
            inRange = false;
            closedByPlayer = false;
        }

        bool shouldShow = inRange && !closedByPlayer;
        if (panel.activeSelf != shouldShow)
            panel.SetActive(shouldShow);
    }

    /// <summary>True while a hand holds the watch. A socket, such as the evidence board, does not count.</summary>
    private bool IsHeldByHand()
    {
        foreach (var interactor in grab.interactorsSelecting)
        {
            if (!(interactor is XRSocketInteractor))
                return true;
        }
        return false;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Snapping into a socket (the evidence board) is not a pick-up, so it must not open the panel.
        if (args.interactorObject is XRSocketInteractor)
            return;

        // Picking the watch up always brings the panel back, even after it was closed.
        inRange = true;
        closedByPlayer = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // The panel stays while the player is nearby; Update hides it once they walk away.
    }

    private void OnActivated(ActivateEventArgs args)
    {
        if (confirmWithTrigger && panel.activeSelf)
            ConfirmEvidence();
    }

    public void ConfirmEvidence()
    {
        if (IsInspected)
            return;

        IsInspected = true;
        RefreshConfirmState();
        Debug.Log("Wristwatch registered as inspected.");
        onEvidenceConfirmed.Invoke();
    }

    public void ClosePanel()
    {
        // Stays closed until the player walks away and comes back, or picks the watch up again.
        closedByPlayer = true;
        panel.SetActive(false);
    }

    private void RefreshConfirmState()
    {
        confirmLabel.text = IsInspected ? confirmedText : confirmText;
        confirmButton.interactable = !IsInspected;
        if (confirmChevron != null)
            confirmChevron.SetActive(!IsInspected);
        if (confirmationMessage != null)
            confirmationMessage.SetActive(IsInspected);
    }
}
