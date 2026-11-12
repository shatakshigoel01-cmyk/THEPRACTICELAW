
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class KnifeEvidenceFeedback : MonoBehaviour
{
    public AudioSource voice;
    public CanvasGroup panel;

    private XRGrabInteractable grab;
    private Coroutine fadeRoutine;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();

        if (panel != null)
        {
            panel.alpha = 0;
            panel.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (grab != null)
            grab.selectEntered.AddListener(OnKnifePicked);
    }

    void OnDisable()
    {
        if (grab != null)
            grab.selectEntered.RemoveListener(OnKnifePicked);
    }

    void OnKnifePicked(SelectEnterEventArgs args)
    {
        if (voice != null)
            voice.Play();

        if (panel == null)
        {
            Debug.LogError("Knife panel is not assigned!");
            return;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(ShowAndHide());
    }

    IEnumerator ShowAndHide()
    {
        panel.gameObject.SetActive(true);
        panel.alpha = 0;

        yield return Fade(0, 1, 0.5f);

        yield return new WaitForSeconds(5f);

        yield return Fade(1, 0, 0.5f);

        panel.gameObject.SetActive(false);
        fadeRoutine = null;
    }

    IEnumerator Fade(float start, float end, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            panel.alpha = Mathf.Lerp(start, end, time / duration);
            yield return null;
        }

        panel.alpha = end;
    }
}
