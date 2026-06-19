using UnityEngine;

public class WandManager : MonoBehaviour
{
    [Header("Pivot")]
    public Transform wandPivot;
    public float rotateMaxAngle = 90.0f; // Maximum angle to rotate the wand
    public float rotateSpeed = 5.0f; // Speed of rotation
    private Quaternion startingRotation;
    private Quaternion goalRotation;
    public Vector3 rotationAxis = new Vector3(1, 0, 0); // Default rotation axis (X-axis)
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (wandPivot != null)
        {
            startingRotation = wandPivot.localRotation; // Store the initial rotation of the wand pivot
            goalRotation = startingRotation;
        }
        startingRotation = wandPivot.rotation; // Store the initial rotation of the wand pivot
        goalRotation = startingRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(wandPivot != null)
        {
            wandPivot.localRotation = Quaternion.Slerp(wandPivot.localRotation, goalRotation, Time.deltaTime * rotateSpeed);
        }
    }

    public void UpdateRotation(float progress)
    {
        if(wandPivot == null)
        {
            return; // Exit if wandPivot is not assigned
        }
        float actualAngle = rotateMaxAngle * progress; // Calculate the actual angle for the wand pivot based on progress
        goalRotation = startingRotation * Quaternion.AngleAxis(actualAngle, rotationAxis);
    }
}
