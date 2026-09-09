using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject evidencePanel;

    public void StartInvestigation()
    {
        startPanel.SetActive(false);
        evidencePanel.SetActive(true);
    }
}