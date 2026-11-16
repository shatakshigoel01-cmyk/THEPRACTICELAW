
using UnityEngine;

public class PushButtonMovement : MonoBehaviour
{
    public float pressDistance = 0.04f;
    public float moveSpeed = 15f;

    [Tooltip("Direction the button moves when pressed, in its parent's local space. The default keeps the original downward (-Y) behaviour.")]
    public Vector3 pressDirection = Vector3.down;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;

    private void Start()
    {
        originalPosition = transform.localPosition;

        pressedPosition = originalPosition +
                         pressDirection.normalized * pressDistance;
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
