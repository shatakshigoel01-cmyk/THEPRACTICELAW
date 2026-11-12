using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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

    [Header("Proximity")]
    [Tooltip("Distance is measured from this object (the watch's Band). If left empty, a child named \"Band\" is used, otherwise this object.")]
    [SerializeField] private Transform proximityAnchor;

    [Tooltip("The panel appears when the headset is this close to the Band (metres).")]
    [Min(0.1f)]
    [SerializeField] private float showDistance = 1.5f;

    [Tooltip("The panel hides once the headset is this far away (metres). Keep it a little above Show Distance so the panel does not flicker at the edge.")]
    [Min(0.1f)]
    [SerializeField] private float hideDistance = 1.7f;

    [Tooltip("Measure across the floor only, so the player's height does not matter.")]
    [SerializeField] private bool ignoreHeight = true;

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
        if (proximityAnchor == null)
            proximityAnchor = FindChildNamed(transform, "Band") ?? transform;

        RefreshConfirmState();
        panel.SetActive(false);
    }

    private void OnValidate()
    {
        if (hideDistance < showDistance)
            hideDistance = showDistance;
    }

    /// <summary>Current distance from the headset to the Band, in metres (for debugging).</summary>
    public float CurrentDistance { get; private set; }

    private static Transform FindChildNamed(Transform root, string childName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child != root && string.Equals(child.name, childName, System.StringComparison.OrdinalIgnoreCase))
                return child;
        }
        return null;
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

        Transform anchor = proximityAnchor != null ? proximityAnchor : transform;
        Vector3 offset = head.position - anchor.position;
        if (ignoreHeight)
            offset.y = 0f;
        float distance = offset.magnitude;
        CurrentDistance = distance;

        // The gap between the two distances stops the panel flickering at the edge.
        if (!inRange && distance <= showDistance)
        {
            inRange = true;
        }
        else if (inRange && distance >= hideDistance && !grab.isSelected)
        {
            inRange = false;
            closedByPlayer = false;
        }

        bool shouldShow = inRange && !closedByPlayer;
        if (panel.activeSelf != shouldShow)
            panel.SetActive(shouldShow);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
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
        if (confirmLabel != null)
            confirmLabel.text = IsInspected ? confirmedText : confirmText;
        if (confirmButton != null)
            confirmButton.interactable = !IsInspected;
        if (confirmChevron != null)
            confirmChevron.SetActive(!IsInspected);
        if (confirmationMessage != null)
            confirmationMessage.SetActive(IsInspected);
    }
}
