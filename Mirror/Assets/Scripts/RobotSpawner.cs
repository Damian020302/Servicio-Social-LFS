using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Drag the Robot Prefabs")] public GameObject[] robotPrefabs;
    public GameObject platform;
    public Transform robotDeployer;
    private GameObject currentRobot;
    public RobotContainer robotContainer;
    public SimpleGrabManager gameManager;
    private int currentRobotIndex = -1; //To track the index of the currently spawned robot
    public float fallThreshold;

    void Start()
    {
        if(robotDeployer != null) fallThreshold = robotDeployer.position.y - 5.0f; //Set the threshold below the deployer position
        else fallThreshold = -5.0f; //Default threshold if deployer is not assigned
        SpawnRandomRobot();
    }

    private void Update()
    {
        if(currentRobot != null )
        {
            if(currentRobot.transform.position.y < fallThreshold)
            {
                Rigidbody rb = currentRobot.GetComponent<Rigidbody>();
                if(rb != null && !rb.isKinematic)
                {
                    if(gameManager != null)
                    {
                        gameManager.RegisterDroppedRobot();
                        Debug.Log("Se debe de llevar esta cantidad de caidos " + gameManager.droppedRobots);
                    }
                    Destroy(currentRobot);
                    ClearCurrentRobot();
                    RespawnCurrentRobot();
                }
            }
        }
    }

    /// <summary>
    /// Spawns a random robot from the robotPrefabs array. It also ensures that the platform is activated and the robot container is deactivated when a new robot is spawned.
    /// </summary>
    public void SpawnRandomRobot()
    {
        if(robotPrefabs.Length > 0) currentRobotIndex = Random.Range(0, robotPrefabs.Length);
        RespawnCurrentRobot();
    }

    /// <summary>
    /// Spawns the current robot based on the currentRobotIndex.
    /// </summary>
    public void RespawnCurrentRobot()
    {
        platform.SetActive(true); // Activate the platform when respawning a robot
        if(robotContainer != null) robotContainer.gameObject.SetActive(false); // Deactivate the container when respawning a robot
        if(robotPrefabs.Length == 0 || robotDeployer == null)
        {
            Debug.LogWarning("No robot prefabs assigned or no deployer assigned!");
            return;
        }
        if(currentRobot != null) return;
        if(currentRobotIndex == -1) currentRobotIndex = Random.Range(0, robotPrefabs.Length);
        GameObject selectedRobot = robotPrefabs[currentRobotIndex];
        currentRobot = Instantiate(selectedRobot, robotDeployer.position, robotDeployer.rotation);
    }

    /// <summary>
    /// Clears the reference to the current robot, allowing for a new robot to be spawned. This method is called when the current robot is destroyed or falls below the threshold.
    /// </summary>
    public void ClearCurrentRobot()
    { 
        currentRobot = null;
    }

    /// <summary>
    /// Deactivates the platform and activates the robot container if it exists.
    /// </summary>
    public void ClearPlatform()
    {
        platform.SetActive(false); // Deactivate the platform when the robot is cleared
        if(robotContainer != null) robotContainer.gameObject.SetActive(true); // Activate the container when the platform is cleared
    }
}