using UnityEngine;
using TMPro;

public class RobotContainer : MonoBehaviour
{
    [Header("Conexiones")]
    public RobotSpawner spawner;
    public SimpleGrabManager gameManager;
    [Tooltip("El centro del paciente")]
    public Transform playerCenter;

    [Header("Configuracion del Nivel")]
    public int maxRobotsPerRound = 10;
    public int robotsCollected = 0;
    [Tooltip("Altura fija del contenedor de robots")]
    public float containerHeight = 0.75f;
    private float maxReachRadius;
    public float spacing = 0.01f;
    public float verticalOffset = 0.1f;

    private void Awake()
    {
        maxReachRadius = PlayerPrefs.GetFloat("PlayerRadius", 0.4f);
        maxReachRadius = Mathf.Max(maxReachRadius - 0.05f, 0.2f); // Ensure a minimum radius
        MoveToNewPosition();
    }

    void MoveToNewPosition()
    {
        int currentDiff = gameManager != null ? gameManager.difficulty : 0;
        float diffFactor = Mathf.Clamp01(currentDiff / 5.0f);
        float currentMaxAngle = Mathf.Lerp(35.0f, 70.0f, diffFactor); // Adjust the range based on difficulty
        float minD = 0.25f;
        float currentMaxDist = Mathf.Lerp(Mathf.Max(0.30f, maxReachRadius*0.7f),maxReachRadius, diffFactor); // Adjust the range based on difficulty
        currentMaxDist = Mathf.Min(currentMaxDist, maxReachRadius); // Ensure it doesn't exceed the player's reach
        Vector3 newPosition = transform.position;
        bool validPosition = false;
        int attempts = 0;
        while(!validPosition && attempts < 100)
        {
            float randomAngle = Random.Range(-currentMaxAngle, currentMaxAngle);
            float randomDistance = Random.Range(minD, currentMaxDist);
            Vector3 offset = Quaternion.Euler(0, randomAngle, 0) * playerCenter.forward * randomDistance;
            newPosition = playerCenter.position + offset;
            newPosition.y = containerHeight; // Set the fixed height
            validPosition = true; // Assume it's valid for now
            if (spawner != null && spawner.robotDeployer != null)
            {
                Vector2 posContainer = new Vector2(newPosition.x, newPosition.z);
                Vector2 posPlatform = new Vector2(spawner.robotDeployer.position.x, spawner.robotDeployer.position.z);
                if(Vector2.Distance(posContainer, posPlatform) < 0.2f) // Ensure a minimum distance from the deployer
                {
                    validPosition = true;
                }
            }
            Vector2 currentPos2D = new Vector2(newPosition.x, newPosition.z);
            Vector2 newPos2D = new Vector2(newPosition.x, newPosition.z);
            if(Vector2.Distance(currentPos2D, newPos2D) < 0.15f) // Ensure a minimum distance from the previous position
            {
                validPosition = false;
            }
            attempts++;
        }
        transform.position = newPosition;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Grabbable"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if(rb != null && !rb.isKinematic) // Check if the object is almost stationary
            {
                CollectRobot(other.gameObject);
            }
        }
    }

    void CollectRobot(GameObject robot)
    {
        robot.tag = "Untagged"; // Prevent further collection
        Destroy(robot);
        robotsCollected++;
        if(gameManager != null && gameManager.clawText != null)
        {
            gameManager.clawText.text = $"Robots\nrestantes: {maxRobotsPerRound - robotsCollected}";
        }
        gameManager.UpdateMetricsUI();
        if(robotsCollected >= maxRobotsPerRound)
        {
            if(gameManager != null)
            {
                gameManager.VictoryAchieved();
            }
        }
        else
        {
            MoveToNewPosition();
            spawner.ClearCurrentRobot();
            spawner.SpawnRandomRobot();
        }
    }

    public void ResetContainer()
    {
        robotsCollected = 0;
        MoveToNewPosition();
        if(gameManager != null && gameManager.clawText != null)
        {
            gameManager.clawText.text = $"Robots\nrecolectados: {robotsCollected}/{maxRobotsPerRound}";
        }
    }
}