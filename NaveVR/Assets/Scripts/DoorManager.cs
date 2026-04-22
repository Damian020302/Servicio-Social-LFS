using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Pivots")]
    public Transform leftPivot;
    public Transform rightPivot;
    [Header("Door Settings")]
    public float openMaxAngle = 90.0f; // Maximum angle to open the door
    public float openSpeed = 5.0f; // Speed at which the door opens

    private Quaternion startingLeftRotation;
    private Quaternion startingRightRotation;
    private Quaternion goalLeftRotation;
    private Quaternion goalRightRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingLeftRotation = leftPivot.rotation; // Store the initial rotation of the left pivot
        startingRightRotation = rightPivot.rotation; // Store the initial rotation of the right pivot
        goalLeftRotation = startingLeftRotation;
        goalRightRotation = startingRightRotation;
    }

    // Update is called once per frame
    void Update()
    {
        leftPivot.rotation = Quaternion.Slerp(leftPivot.rotation, goalLeftRotation, Time.deltaTime * openSpeed);
        rightPivot.rotation = Quaternion.Slerp(rightPivot.rotation, goalRightRotation, Time.deltaTime * openSpeed);
    }

    public void UpdateOpening(float progress)
    {
        float actualLeftAngle = -openMaxAngle * progress; // Calculate the actual angle for the left pivot based on progress
        float actualRightAngle = openMaxAngle * progress; // Calculate the actual angle for the right pivot based on progress
        goalLeftRotation = startingLeftRotation * Quaternion.Euler(0, actualLeftAngle, 0);
        goalRightRotation = startingRightRotation * Quaternion.Euler(0, actualRightAngle, 0);
    }
}
