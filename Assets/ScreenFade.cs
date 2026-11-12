
using UnityEngine;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    public CanvasGroup fadeGroup;

    public IEnumerator FadeToBlack()
    {
        fadeGroup.blocksRaycasts = true;

        float duration = 2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        fadeGroup.alpha = 1f;
    }

    public IEnumerator FadeFromBlack()
    {
        float duration = 2f;
        float timer = 0f;

        fadeGroup.blocksRaycasts = true;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }

        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
    }
}
