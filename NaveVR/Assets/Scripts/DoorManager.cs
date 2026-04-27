using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Pivots")]
    public Transform leftPivot;
    public Transform rightPivot;
    [Header("Door Settings")]
    public float openMaxAngle = 90.0f; // Maximum angle to open the door
    public float openSpeed = 5.0f; // Speed at which the door opens
    /*[Header("Treasure Shinning (Material)")]
    public Renderer treasureRenderer;
    public Color colorShining = new Color(1.0f, 0.8f, 0.2f);
    public float maxIntensity = 5.0f;*/
    [Header("Treasure Shinning (Pointlight)")]
    public Light treasureLight;
    public float lightMaxIntensity = 5.0f;

    private Quaternion startingLeftRotation;
    private Quaternion startingRightRotation;
    private Quaternion goalLeftRotation;
    private Quaternion goalRightRotation;

    private Material treasureMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingLeftRotation = leftPivot.rotation; // Store the initial rotation of the left pivot
        startingRightRotation = rightPivot.rotation; // Store the initial rotation of the right pivot
        goalLeftRotation = startingLeftRotation;
        goalRightRotation = startingRightRotation;
        /*if(treasureRenderer != null)
        {
            treasureMaterial = treasureRenderer.material; // Get the material of the treasure renderer
            treasureMaterial.EnableKeyword("_EMISSION"); // Enable emission on the material
            treasureMaterial.SetColor("_EmissionColor", Color.black); // Set the emission color and intensity
        }*/
        if(treasureLight != null)
        {
            treasureLight.intensity = 0.0f; // Set the initial intensity of the treasure light
        }
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

        /*if(treasureMaterial != null)
        {
            float actualIntensity = maxIntensity * progress; // Calculate the actual intensity based on progress
            Color finalColor = colorShining * Mathf.Pow(actualIntensity, 2.0f); // Calculate the final color by multiplying the base color with the intensity squared
            treasureMaterial.SetColor("_EmissionColor", finalColor); // Set the emission color of the material
        }*/

        if(treasureLight != null)
        {
            float actualLightIntensity = lightMaxIntensity * progress; // Calculate the actual light intensity based on progress
            treasureLight.intensity = actualLightIntensity; // Set the intensity of the treasure light
        }
    }
}
