using UnityEngine;
using TMPro;

public class LaptopPasscode : MonoBehaviour
{
    public TMP_Text passcodeText;
    public GameObject evidenceScreen;

    string enteredCode = "";
    string correctCode = "4729";

    public void EnterNumber(string number)
    {
        if (enteredCode.Length >= 4)
            return;

        enteredCode += number;
        passcodeText.text = enteredCode;

        if (enteredCode.Length == 4)
        {
            CheckCode();
        }
    }

    void CheckCode()
    {
        if (enteredCode == correctCode)
        {
            passcodeText.text = "ACCESS GRANTED";

            Invoke(nameof(ShowEvidence), 1.5f);
        }
        else
        {
            passcodeText.text = "WRONG PASSCODE";
            Invoke(nameof(ResetCode), 1.5f);
        }
    }

    void ShowEvidence()
    {
        passcodeText.gameObject.SetActive(false);
        evidenceScreen.SetActive(true);
    }

    void ResetCode()
    {
        enteredCode = "";
        passcodeText.text = "ENTER PASSCODE";
    }
}