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
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private GameObject instructionGroup;
    [SerializeField] private GameObject completionGroup;
    [SerializeField] private TestTubeCollector collector;

    [Header("Distances (metres, measured across the floor)")]
    [SerializeField] private float showDistance = 1.8f;
    [SerializeField] private float hideDistance = 2.2f;

    [Header("Instructions")]
    [SerializeField] private string instructionTitle = "BLOOD SAMPLING";
    [SerializeField] private string instructionSubtitle = "COLLECT EVIDENCE";

    [Header("Completion")]
    [SerializeField] private string completionTitle = "BLOOD SAMPLE COLLECTED";
    [SerializeField] private string completionSubtitle = "EVIDENCE STORED";
    [SerializeField] private float completionSeconds = 5f;

    private Transform head;
    private bool completed;
    private float hideCompletionAt;

    private void Awake()
    {
        ApplyContent(instructionTitle, instructionSubtitle, false);
        panel.SetActive(false);
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
            panel.SetActive(true);
        else if (panel.activeSelf && distance >= hideDistance)
            panel.SetActive(false);
    }

    private void ApplyContent(string title, string subtitle, bool showCompletion)
    {
        titleText.text = title;
        subtitleText.text = subtitle;
        instructionGroup.SetActive(!showCompletion);
        completionGroup.SetActive(showCompletion);
    }

    private void OnSampleStored()
    {
        completed = true;
        hideCompletionAt = Time.time + completionSeconds;
        ApplyContent(completionTitle, completionSubtitle, true);
        panel.SetActive(true);
    }
}
