using TMPro;
using UnityEngine;

/// <summary>
/// Shows the blood sampling instructions while the headset is near the table,
/// and the completion message once the sample has been stored.
/// </summary>
public class BloodSamplingStation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private TestTubeCollector collector;

    [Header("Distances (metres, measured across the floor)")]
    [SerializeField] private float showDistance = 1.8f;
    [SerializeField] private float hideDistance = 2.2f;

    [Header("Instructions")]
    [SerializeField] private string instructionTitle = "BLOOD SAMPLE COLLECTION";
    [SerializeField, TextArea(4, 8)] private string instructionBody =
        "Collect a blood sample from the crime scene.\n\n" +
        "1. Grab the cotton swab.\n" +
        "2. Touch the blood stain to collect a sample.\n" +
        "3. Place the swab back inside the test tube.";

    [Header("Completion")]
    [SerializeField] private string completionTitle = "BLOOD SAMPLE COLLECTED";
    [SerializeField, TextArea(2, 4)] private string completionBody =
        "The sample has been stored in the test tube.";
    [SerializeField] private float completionSeconds = 5f;

    private Transform head;
    private bool completed;
    private float hideCompletionAt;

    private void Awake()
    {
        ShowInstructions(false);
    }

    private void OnEnable()
    {
        if (collector != null)
            collector.SampleStored += OnSampleStored;
    }

    private void OnDisable()
    {
        if (collector != null)
            collector.SampleStored -= OnSampleStored;
    }

    private void Update()
    {
        if (completed)
        {
            if (panel.activeSelf && Time.time >= hideCompletionAt)
                panel.SetActive(false);
            return;
        }

        if (head == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                return;
            head = mainCamera.transform;
        }

        Vector3 offset = head.position - transform.position;
        offset.y = 0f;
        float distance = offset.magnitude;

        if (!panel.activeSelf && distance <= showDistance)
            ShowInstructions(true);
        else if (panel.activeSelf && distance >= hideDistance)
            ShowInstructions(false);
    }

    private void ShowInstructions(bool visible)
    {
        titleText.text = instructionTitle;
        bodyText.text = instructionBody;
        panel.SetActive(visible);
    }

    private void OnSampleStored()
    {
        completed = true;
        hideCompletionAt = Time.time + completionSeconds;
        titleText.text = completionTitle;
        bodyText.text = completionBody;
        panel.SetActive(true);
    }
}
