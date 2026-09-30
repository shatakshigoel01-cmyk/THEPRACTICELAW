using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public GameObject incidentNightPanel;
    public GameObject evidenceText;
    public GameObject incidentNightButton;

    public void OpenIncidentNight()
    {
        evidenceText.SetActive(false);
        incidentNightButton.SetActive(false);

        incidentNightPanel.SetActive(true);
    }
}