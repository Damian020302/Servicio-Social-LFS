using UnityEngine;

public class RotatePuzzle : MonoBehaviour
{
    private bool isGrabbed = false;
    private Transform handInteraction;

    private Quaternion initialHandRotation;
    private Quaternion initialPuzzleRotation;

    [Header("Virtual Lock")]
    private Vector3 pos;
    private float xRotation;
    private float yRotation;

    private void Start()
    {
        pos = transform.position;
        xRotation = transform.localEulerAngles.x;
        yRotation = transform.localEulerAngles.y;
    }

    private void Update()
    {
        if (isGrabbed && handInteraction != null)
        {
            // Calculate the rotation based on the hand's movement
            Quaternion difRotation = handInteraction.rotation * Quaternion.Inverse(initialHandRotation);
            transform.rotation = difRotation * initialPuzzleRotation;
            transform.position = pos;
            Vector3 blockedRotation = transform.localEulerAngles;
            blockedRotation.x = xRotation;
            blockedRotation.y = yRotation;
            transform.localEulerAngles = blockedRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand") && !isGrabbed)
        {
            isGrabbed = true;
            handInteraction = other.transform;
            initialHandRotation = handInteraction.rotation;
            initialPuzzleRotation = transform.rotation;
            Debug.Log("Grabbed");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hand") && other.transform == handInteraction)
        {
            isGrabbed = false;
            handInteraction = null;
        }
    }
}
