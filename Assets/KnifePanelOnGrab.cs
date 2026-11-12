
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KnifePanelOnGrab : MonoBehaviour
{
    public GameObject knifePanel;

    private XRGrabInteractable grab;
    private Coroutine hideRoutine;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        if (knifePanel != null)
            knifePanel.SetActive(false);
    }

    void OnEnable()
    {
        if (grab != null)
            grab.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        if (grab != null)
            grab.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        knifePanel.SetActive(true);
        hideRoutine = StartCoroutine(HidePanel());
    }

    IEnumerator HidePanel()
    {
        yield return new WaitForSeconds(5f);

        knifePanel.SetActive(false);
        hideRoutine = null;
    }
}
