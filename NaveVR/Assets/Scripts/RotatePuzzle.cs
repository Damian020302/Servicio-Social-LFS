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
    public float physicalRotationLimit = 90.0f;
    public float rotationMultiplier = 1.0f;

    [Header("Victory Lights")]
    private Light[] runeLights;

    private void Start()
    {
        pos = transform.position;
        xRotation = transform.localEulerAngles.x;
        yRotation = transform.localEulerAngles.y;
        runeLights = GetComponentsInChildren<Light>(true);
        ToggleLights(false);
        physicalRotationLimit = PlayerPrefs.GetFloat("MaxPuzzleRotation", physicalRotationLimit);
        physicalRotationLimit = Mathf.Max(physicalRotationLimit, 5.0f);
        float virtualRotationNeeded = 120.0f;
        PuzzleManager puzzleManager = Object.FindFirstObjectByType<PuzzleManager>();
        if(puzzleManager != null)
        {
            virtualRotationNeeded = puzzleManager.predefinedScrambleAngle;
        }
        rotationMultiplier = virtualRotationNeeded / physicalRotationLimit;
        Debug.Log($"RotatePuzzle: Limite Fisico = {physicalRotationLimit}, Meta virtual = {virtualRotationNeeded}, Multiplicador = {rotationMultiplier:F2}");
        //maxRotationPerInteraction = PlayerPrefs.GetFloat("MaxPuzzleRotation", maxRotationPerInteraction);
    }

    private void Update()
    {
        if (isGrabbed && handInteraction != null)
        {
            // Calculate the rotation based on the hand's movement
            Quaternion difRotation = handInteraction.rotation * Quaternion.Inverse(initialHandRotation);
            difRotation.ToAngleAxis(out float handAngle, out Vector3 handAxis);
            if (handAngle > 180.0f) handAngle += 360.0f;
            float virtualAngle = handAngle * rotationMultiplier;
            Quaternion scaledDifRotation = Quaternion.AngleAxis(virtualAngle, handAxis);
            transform.rotation = scaledDifRotation * initialPuzzleRotation;
            //Quaternion targetRotation = difRotation * initialPuzzleRotation;
            //transform.rotation = Quaternion.RotateTowards(initialPuzzleRotation, targetRotation, maxRotationPerInteraction);
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

    public void ToggleLights(bool isOn)
    {
        if(runeLights == null)
        {
            return;
        }
        foreach(Light l in runeLights)
        {
            if (l != null)
            {
                l.enabled = isOn;
            }
        }
    }
}