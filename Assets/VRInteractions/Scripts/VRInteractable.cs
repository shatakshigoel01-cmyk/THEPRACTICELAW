
using UnityEngine;
using System.Collections.Generic;

public class VRInteractable : ExposableMonobehaviour
{
    public bool Interactable = true;

    private Rigidbody rb3d;
    private List<Collider> mColliders3D;
    private bool mCache;

    void Awake()
    {
        UpdateColliders3D();
        mCache = Interactable;
    }

    public void UpdateColliders3D()
    {
        rb3d = GetComponent<Rigidbody>();
        mColliders3D = new List<Collider>();

        if (rb3d != null)
        {
            mColliders3D.AddRange(
                rb3d.GetComponentsInChildren<Collider>()
            );
        }
        else
        {
            mColliders3D.AddRange(
                GetComponentsInChildren<Collider>()
            );
        }
    }

    public void IgnoreColliders(Rigidbody otherBody)
    {
        if (otherBody == null) return;

        IgnoreColliders3D(
            otherBody.GetComponentsInChildren<Collider>(),
            mColliders3D.ToArray()
        );
    }

    public void IgnoreColliders(Transform otherObject)
    {
        if (otherObject == null) return;

        IgnoreColliders3D(
            otherObject.GetComponentsInChildren<Collider>(),
            mColliders3D.ToArray()
        );
    }

    public void RemoveIgnoreColliders(Rigidbody otherBody)
    {
        if (otherBody == null) return;

        IgnoreColliders3D(
            otherBody.GetComponentsInChildren<Collider>(),
            mColliders3D.ToArray(),
            false
        );
    }

    public void RemoveIgnoreColliders(Transform otherObject)
    {
        if (otherObject == null) return;

        IgnoreColliders3D(
            otherObject.GetComponentsInChildren<Collider>(),
            mColliders3D.ToArray(),
            false
        );
    }

    public static void IgnoreColliders3D(
        Collider[] colliders,
        Collider[] otherColliders,
        bool ignore = true)
    {
        foreach (Collider col in colliders)
        {
            foreach (Collider otherCol in otherColliders)
            {
                if (col != null && otherCol != null)
                {
                    Physics.IgnoreCollision(
                        col,
                        otherCol,
                        ignore
                    );
                }
            }
        }
    }

    void Update()
    {
        if (mCache != Interactable)
        {
            // Controller collision handling from the old SteamVR
            // system has been removed.
            mCache = Interactable;
        }
    }
}
