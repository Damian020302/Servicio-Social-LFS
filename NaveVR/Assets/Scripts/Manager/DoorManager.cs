using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Pivots")]
    public Transform leftPivot;
    public Transform rightPivot;
    public GameObject[] leftDoors;
    public GameObject[] rightDoors;
    [Header("Door Settings")]
    public float openMaxAngle = 90.0f; //Maximum angle to open the door
    public float openSpeed = 5.0f; //Speed at which the door opens
    [Header("Treasure Shinning (Pointlight)")]
    public Light treasureLight;
    public float lightMaxIntensity = 5.0f;

    private Quaternion startingLeftRotation;
    private Quaternion startingRightRotation;
    private Quaternion goalLeftRotation;
    private Quaternion goalRightRotation;

    void Start()
    {
        startingLeftRotation = leftPivot.rotation; //Store the initial rotation of the left pivot
        startingRightRotation = rightPivot.rotation; //Store the initial rotation of the right pivot
        goalLeftRotation = startingLeftRotation;
        goalRightRotation = startingRightRotation;
        if(treasureLight != null)
        {
            treasureLight.intensity = 0.0f; //Set the initial intensity of the treasure light
        }
        SelectRandomDoors(); //Select random door models to display
    }

    void Update()
    {
        leftPivot.rotation = Quaternion.Slerp(leftPivot.rotation, goalLeftRotation, Time.deltaTime * openSpeed);
        rightPivot.rotation = Quaternion.Slerp(rightPivot.rotation, goalRightRotation, Time.deltaTime * openSpeed);
    }

    /// <summary>
    /// Updates the opening of the door based on the given progress value (0 to 1).
    /// </summary>
    /// <param name="progress">A value between 0 and 1 representing the progress of the door opening.</param>
    public void UpdateOpening(float progress)
    {
        float actualLeftAngle = -openMaxAngle * progress; //Calculate the actual angle for the left pivot based on progress
        float actualRightAngle = openMaxAngle * progress; //Calculate the actual angle for the right pivot based on progress
        goalLeftRotation = startingLeftRotation * Quaternion.Euler(0, actualLeftAngle, 0);
        goalRightRotation = startingRightRotation * Quaternion.Euler(0, actualRightAngle, 0);
        if(treasureLight != null)
        {
            float actualLightIntensity = lightMaxIntensity * progress; //Calculate the actual light intensity based on progress
            treasureLight.intensity = actualLightIntensity; //Set the intensity of the treasure light
        }
    }

    private void SelectRandomDoors()
    {
        foreach (GameObject model in leftDoors) if (model != null) model.SetActive(false);
        foreach (GameObject model in rightDoors) if (model != null) model.SetActive(false);

        if(leftDoors.Length > 0)
        {
            int randomLeftIndex = Random.Range(0, leftDoors.Length);
            leftDoors[randomLeftIndex].SetActive(true); //Activate a random left door model
        }
        if(rightDoors.Length > 0)
        {
            int randomRightIndex = Random.Range(0, rightDoors.Length);
            rightDoors[randomRightIndex].SetActive(true); //Activate a random right door model
        }
    }
}
