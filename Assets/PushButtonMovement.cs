
using UnityEngine;

public class PushButtonMovement : MonoBehaviour
{
    public float pressDistance = 0.04f;
    public float moveSpeed = 15f;

    [Tooltip("Direction the button moves when pressed, in its parent's local space. The default keeps the original downward (-Y) behaviour.")]
    public Vector3 pressDirection = Vector3.down;

    [Tooltip("The visible part that moves. Leave empty to move this object. Moving a separate visual keeps the poke collider still, so a resting finger does not flicker the press.")]
    public Transform movingPart;

    private Transform Target => movingPart != null ? movingPart : transform;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;

    private void Start()
    {
        originalPosition = Target.localPosition;

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

        Target.localPosition = Vector3.Lerp(
            Target.localPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }
}
