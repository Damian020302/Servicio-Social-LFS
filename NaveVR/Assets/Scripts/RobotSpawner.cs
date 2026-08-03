using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RobotSpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Drag the Robot Prefabs")] public GameObject[] robotPrefabs;
    public Transform robotDeployer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (robotPrefabs != null)
        {
            if (robotPrefabs.Length == 0)
            {
                Debug.LogWarning("No robot prefabs assigned!");
            }
        }
        else
        {
            Debug.LogWarning("Robot prefabs array is not assigned!");
        }
    }

    void SpawnSingleRobot()
    {
        if(robotPrefabs.Length == 0)
        {
            Debug.LogWarning("No robot prefabs assigned or no deployer assigned!");
            return;
        }
        int randomIndex = Random.Range(0, robotPrefabs.Length);
        GameObject selectedRobot = robotPrefabs[randomIndex];
        Instantiate(selectedRobot, robotDeployer.position, robotDeployer.rotation);
    }
}
