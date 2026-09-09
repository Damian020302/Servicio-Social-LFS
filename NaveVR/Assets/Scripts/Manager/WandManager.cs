using UnityEngine;

public class WandManager : MonoBehaviour
{
    [Header("Pivot")]
    public Transform wandPivot;
    public float rotateMaxAngle = 90.0f; // Maximum angle to rotate the wand
    public float rotateSpeed = 5.0f; // Speed of rotation
    private Quaternion startingRotation;
    private Quaternion goalRotation;
    
    void Start()
    {
        startingRotation = wandPivot.localRotation; // Store the initial rotation of the wand pivot
        goalRotation = startingRotation;
    }

    void Update()
    {
        wandPivot.localRotation = Quaternion.Slerp(wandPivot.localRotation, goalRotation, Time.deltaTime * rotateSpeed);
    }

    public void UpdateRotation(float progress)
    {
        float actualAngle = rotateMaxAngle * progress; // Calculate the actual angle for the wand pivot based on progress
        goalRotation = startingRotation * Quaternion.Euler(actualAngle, 0, 0);
    }
}
