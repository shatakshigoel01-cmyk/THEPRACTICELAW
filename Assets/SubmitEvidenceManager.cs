
using UnityEngine;
using System.Collections;

public class SubmitEvidenceManager : MonoBehaviour
{
    public ScreenFade screenFade;
    public Transform xrOrigin;
    public Transform timelineSpawnPoint;

    public void SubmitEvidence()
    {
        StartCoroutine(MoveToTimelineRoom());
    }

    IEnumerator MoveToTimelineRoom()
    {
        // Fade the screen to black
        yield return StartCoroutine(screenFade.FadeToBlack());

        // Move the VR player into the timeline room
        xrOrigin.SetPositionAndRotation(
            timelineSpawnPoint.position,
            timelineSpawnPoint.rotation
        );

        // Small pause while the player arrives
        yield return new WaitForSeconds(0.5f);

        // Reveal the timeline room
        yield return StartCoroutine(screenFade.FadeFromBlack());
    }
}
