
using UnityEngine;

public class VRGripper : MonoBehaviour
{
    private AudioSource currentAudio;
    private bool isColliding = false;

    public float HapticPulseStrength = 0.5f;
    public float VibrationLength = 0.1f;

    void Awake()
    {
        currentAudio = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isColliding)
            return;

        isColliding = true;

        if (currentAudio != null && !currentAudio.isPlaying)
        {
            currentAudio.Play();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

    public void HapticVibration(float strength, float duration)
    {
        // Haptic feedback is not implemented in this version.
    }
}
