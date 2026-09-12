using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvidencePriority : MonoBehaviour
{
    public TMP_Dropdown priorityDropdown;
    public Image priorityIndicator;

    public void ChangePriority()
    {
        int priority = priorityDropdown.value;

        if (priority == 0)
        {
            // Low
            priorityIndicator.color = Color.gray;
        }
        else if (priority == 1)
        {
            // Normal
            priorityIndicator.color = Color.white;
        }
        else if (priority == 2)
        {
            // High
            priorityIndicator.color = new Color(1f, 0.5f, 0f);
        }
        else if (priority == 3)
        {
            // Critical
            priorityIndicator.color = Color.red;
        }
    }
}