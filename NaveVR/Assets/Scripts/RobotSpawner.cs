using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Drag the Robot Prefabs")] public GameObject[] robotPrefabs;
    public GameObject platform;
    public Transform robotDeployer;
    private GameObject currentRobot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnSingleRobot();
    }

    public void SpawnSingleRobot()
    {
        platform.SetActive(true); // Deactivate the current robot if it exists
        if (robotPrefabs.Length == 0 || robotDeployer == null)
        {
            Debug.LogWarning("No robot prefabs assigned or no deployer assigned!");
            return;
        }
        if(currentRobot != null) return; // Prevent spawning if a robot already exists
        int randomIndex = Random.Range(0, robotPrefabs.Length);
        GameObject selectedRobot = robotPrefabs[randomIndex];
        currentRobot = Instantiate(selectedRobot, robotDeployer.position, robotDeployer.rotation);
    }

    public void ClearCurrentRobot()
    { 
        currentRobot = null;
    }

    public void ClearPlatform()
    {
        platform.SetActive(false); // Deactivate the platform when the robot is cleared
    }
}
