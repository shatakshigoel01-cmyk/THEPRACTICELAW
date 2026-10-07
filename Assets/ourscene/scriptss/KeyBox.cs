using UnityEngine;

public class KeyBox : MonoBehaviour
{
    public Transform lid;
    public Transform openPosition;

    public void OpenBox()
    {
        lid.position = openPosition.position;
        lid.rotation = openPosition.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            OpenBox();
        }
    }
}