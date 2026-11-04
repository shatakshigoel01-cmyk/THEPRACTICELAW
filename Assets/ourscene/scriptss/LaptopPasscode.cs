
using UnityEngine;
using TMPro;

public class LaptopPasscode : MonoBehaviour
{
    public TMP_Text passcodeText;
    public GameObject evidenceScreen;

    private string enteredCode = "";
    private string correctCode = "4729";

    public void EnterNumber(int number)
    {
        Debug.Log("Laptop button pressed: " + number);

        if (enteredCode.Length >= 4)
            return;

        enteredCode += number.ToString();
        passcodeText.text = enteredCode;

        if (enteredCode.Length == 4)
        {
            CheckCode();
        }
    }

    public void ClearCode()
    {
        enteredCode = "";
        passcodeText.text = "ENTER PASSCODE";
    }

    private void CheckCode()
    {
        if (enteredCode == correctCode)
        {
            passcodeText.text = "ACCESS GRANTED";
            Invoke(nameof(ShowEvidence), 1.5f);
        }
        else
        {
            passcodeText.text = "WRONG PASSCODE";
            Invoke(nameof(ClearCode), 1.5f);
        }
    }

    private void ShowEvidence()
    {
        if (passcodeText != null)
            passcodeText.gameObject.SetActive(false);

        if (evidenceScreen != null)
            evidenceScreen.SetActive(true);
        else
            Debug.LogError("Evidence Screen is not assigned!");
    }
}
