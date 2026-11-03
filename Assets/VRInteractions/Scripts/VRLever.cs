
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Event fired when the lever value changes.
/// </summary>
[System.Serializable]
public class LeverChangeEvent : UnityEvent<VRLever, float, float>
{
}

/// <summary>
/// Physical lever with a value similar to a slider.
/// </summary>
public class VRLever : VRInteractable
{
    public LeverChangeEvent LeverListeners;

    [Tooltip("Minimum lever angle in degrees")]
    public float Min = 0f;

    [Tooltip("Maximum lever angle in degrees")]
    public float Max = 90f;

    private HingeJoint CurrentHinge;

    protected float mValue = 0f;
    protected float valueCache = 0f;

    public float Value
    {
        get { return mValue; }
        set
        {
            if (Mathf.Approximately(mValue, value))
                return;

            float previousValue = mValue;
            mValue = Mathf.Clamp(value, 0f, 1f);

            CheckHingeValue();

            if (LeverListeners != null)
            {
                try
                {
                    LeverListeners.Invoke(this, mValue, previousValue);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("VRLever event failed: " + e.Message);
                }
            }

            valueCache = mValue;
        }
    }

    private float mMinCache;
    private float mMaxCache;

    private VRGripper controller;
    private bool isActing = false;

    public float VibrationStrength = 0.2f;
    public bool triggerEnabled = false;

    private void Awake()
    {
        CurrentHinge = GetComponent<HingeJoint>();
    }

    private void OnEnable()
    {
        SetAngleToValue();
    }

    private void Reset()
    {
        mMinCache = 0f;
        mMaxCache = 0f;
        Validate();
    }

    private void OnValidate()
    {
        Validate();
    }

    private void Validate()
    {
        CurrentHinge = GetComponent<HingeJoint>();

        if (CurrentHinge != null)
            EditorUpdateHinges();
    }

    private void EditorUpdateHinges()
    {
        JointLimits limits = CurrentHinge.limits;

        if (!Mathf.Approximately(limits.min, mMinCache))
        {
            mMinCache = limits.min;
            Min = limits.min;
        }
        else if (!Mathf.Approximately(mMinCache, Min))
        {
            mMinCache = Min;
            limits.min = Min;
            CurrentHinge.limits = limits;
        }

        limits = CurrentHinge.limits;

        if (!Mathf.Approximately(limits.max, mMaxCache))
        {
            mMaxCache = limits.max;
            Max = limits.max;
        }
        else if (!Mathf.Approximately(mMaxCache, Max))
        {
            mMaxCache = Max;
            limits.max = Max;
            CurrentHinge.limits = limits;
        }
    }

    private void CheckHingeValue()
    {
        if (!isActing)
            SetAngleToValue();
    }

    private void SetAngleToValue()
    {
        Vector3 rotation = transform.localEulerAngles;
        rotation.x = ValueToAngle();
        transform.localEulerAngles = rotation;
    }

    private float AngleToValue()
    {
        float angle = transform.localEulerAngles.x;

        if (angle > 180f)
            angle -= 360f;

        float range = Max - Min;

        if (Mathf.Approximately(range, 0f))
            return 0f;

        return Mathf.Clamp01((angle - Min) / range);
    }

    private float ValueToAngle()
    {
        return Mathf.Lerp(Min, Max, Mathf.Clamp01(mValue));
    }

    public void Update()
    {
        //base.Update();

        if (!Interactable)
            return;

        float currentValue = AngleToValue();

        if (!Mathf.Approximately(currentValue, valueCache))
        {
            Value = currentValue;
        }

        // Legacy SteamVR haptic feedback has been removed.
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == null)
            return;

        controller = collision.rigidbody.GetComponent<VRGripper>();

        if (controller != null)
            BeginAction();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody == null)
            return;

        VRGripper gripper =
            collision.rigidbody.GetComponent<VRGripper>();

        if (controller != gripper)
            return;

        EndAction();
        controller = null;
    }

    private void BeginAction()
    {
        isActing = true;
    }

    private void EndAction()
    {
        isActing = false;
    }

    public void OnGripBegin()
    {
        BeginAction();
    }

    public void OnGripEnd()
    {
        EndAction();
    }
}
