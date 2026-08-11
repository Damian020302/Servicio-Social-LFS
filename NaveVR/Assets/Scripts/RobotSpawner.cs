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
    private int currentRobotIndex = -1; // To track the index of the currently spawned robot
    public float fallThreshold;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(robotDeployer != null)
        {
            fallThreshold = robotDeployer.position.y - 5.0f; // Set the threshold below the deployer position
        }
        else
        {
            fallThreshold = -5.0f; // Default threshold if deployer is not assigned
        }
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
                        gameManager.droppedRobots++;
                    }
                    Destroy(currentRobot);
                    ClearCurrentRobot();
                    RespawnCurrentRobot();
                }
            }
        }
    }

    public void SpawnRandomRobot()
    {
        if(robotPrefabs.Length > 0)
        {
            currentRobotIndex = Random.Range(0, robotPrefabs.Length);
        }
        RespawnCurrentRobot();
    }

    public void RespawnCurrentRobot()
    {
        platform.SetActive(true); // Activate the platform when respawning a robot
        if(robotContainer != null)
        {
            robotContainer.gameObject.SetActive(false); // Deactivate the container when respawning a robot
        }
        if(robotPrefabs.Length == 0 || robotDeployer == null)
        {
            Debug.LogWarning("No robot prefabs assigned or no deployer assigned!");
            return;
        }
        if(currentRobot != null)
        {
            return;
        }
        if(currentRobotIndex == -1)
        {
            currentRobotIndex = Random.Range(0, robotPrefabs.Length);
        }
        GameObject selectedRobot = robotPrefabs[currentRobotIndex];
        currentRobot = Instantiate(selectedRobot, robotDeployer.position, robotDeployer.rotation);
    }

    public void ClearCurrentRobot()
    { 
        currentRobot = null;
    }

    public void ClearPlatform()
    {
        platform.SetActive(false); // Deactivate the platform when the robot is cleared
        if(robotContainer != null)
        {
            robotContainer.gameObject.SetActive(true); // Activate the container when the platform is cleared
        }
    }
}