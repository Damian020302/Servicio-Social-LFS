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

    [Header("Therapeutic Restriction")]
    [Tooltip("Set the maximum rotation angle for each interaction")]
    public float maxRotationPerInteraction = 90.0f;

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
            Quaternion targetRotation = difRotation * initialPuzzleRotation;
            //transform.rotation = difRotation * initialPuzzleRotation;
            transform.rotation = Quaternion.RotateTowards(initialPuzzleRotation, targetRotation, maxRotationPerInteraction);
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
            Debug.Log("Released. 90 degrees accomplished.");
        }
    }
}
