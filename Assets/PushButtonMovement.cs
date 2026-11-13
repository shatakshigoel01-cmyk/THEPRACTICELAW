
using UnityEngine;

public class PushButtonMovement : MonoBehaviour
{
    public float pressDistance = 0.04f;
    public float moveSpeed = 15f;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;

    private void Start()
    {
        originalPosition = transform.localPosition;

        pressedPosition = originalPosition -
                         new Vector3(0f, pressDistance, 0f);
    }

    public void PressButton()
    {
        isPressed = true;
        Debug.Log("BUTTON PRESSED!");
    }

    public void ReleaseButton()
    {
        isPressed = false;
        Debug.Log("BUTTON RELEASED!");
    }

    private void Update()
    {
        Vector3 targetPosition = isPressed
            ? pressedPosition
            : originalPosition;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }
}
