using System.Collections;
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

    [Header("Claw Configuration")]
    public Transform clawTransform;
    public Animator clawAnimator;
    [Tooltip("Trigger for the Animator Controller")] public string dropTriggerName = "Drop"; // Name of the trigger in the Animator Controller
    public float clawUpY = 2.0f;
    public float clawDownY = 1.0f;
    public float clawMoveSpeed = 5.0f;
    [Tooltip("Time in seconds the claw waits for a robot to spawn")] public float spawnDelay = 0.2f;
    [Tooltip("Time in seconds the claw stays down before moving back up")] public float returnDelay = 0.4f;

    private Coroutine spawnCoroutine;

    void Start()
    {
        if(robotDeployer != null) fallThreshold = robotDeployer.position.y - 5.0f; //Set the threshold below the deployer position
        else fallThreshold = -5.0f; //Default threshold if deployer is not assigned
        if(clawTransform != null)
        {
            Vector3 pos = clawTransform.position;
            pos.y = clawUpY;
            clawTransform.position = pos;
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
                        gameManager.RegisterDroppedRobot();
                        Debug.Log("Se debe de llevar esta cantidad de fallos " + gameManager.droppedRobots);
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
        if(clawTransform != null)
        {
            if(spawnCoroutine != null) StopCoroutine(spawnCoroutine);
            spawnCoroutine = StartCoroutine(ClawSpawnRoutine());
        }
        else
        {
            InstantiateSelectedRobot();
        }
        /*GameObject selectedRobot = robotPrefabs[currentRobotIndex];
        currentRobot = Instantiate(selectedRobot, robotDeployer.position, robotDeployer.rotation);*/
    }

    private IEnumerator ClawSpawnRoutine()
    {
        yield return StartCoroutine(MoveClawToY(clawDownY));
        if(clawAnimator != null && !string.IsNullOrEmpty(dropTriggerName))
        {
            clawAnimator.SetTrigger(dropTriggerName);
        }
        if(spawnDelay > 0f) yield return new WaitForSeconds(spawnDelay);
        InstantiateSelectedRobot();
        if (returnDelay > 0.0f) yield return new WaitForSeconds(returnDelay);
        yield return StartCoroutine(MoveClawToY(clawUpY));
        spawnCoroutine = null;
    }

    private IEnumerator MoveClawToY(float targetY)
    {
        while(Mathf.Abs(clawTransform.position.y - targetY) > 0.01f)
        {
            Vector3 newPosition = clawTransform.position;
            newPosition.y = Mathf.MoveTowards(newPosition.y, targetY, clawMoveSpeed * Time.deltaTime);
            clawTransform.position = newPosition;
            yield return null;
        }
        Vector3 finalPos = clawTransform.position;
        finalPos.y = targetY;
        clawTransform.position = finalPos;
    }

    private void InstantiateSelectedRobot()
    {
        if(currentRobot != null) return; // Prevent multiple instantiations if the current robot is already set
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