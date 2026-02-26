using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] private float enemyInterval = 2.0f;
    public static int enemyCounter = 0;
    public const int maxEnemies = 5;
    public GameObject player;
    private Vector3 offset;

    void Start()
    {
        enemyCounter = 0;
        if (player != null)
        {
            offset = transform.position - player.transform.position;
        }       
        InvokeRepeating("spawnEnemy", 0f, enemyInterval);
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }

    void spawnEnemy()
    {
        if (enemyCounter < maxEnemies)
        {
            Instantiate(enemyPrefab, transform.position, transform.rotation);
            enemyCounter++;
        }
    }
}