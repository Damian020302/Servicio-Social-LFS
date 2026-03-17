using Oculus.Interaction.Editor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;
    [SerializeField] private float enemyInterval;
    public GameObject player;
    private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("spawnEnemy", enemyInterval, enemyInterval);
    }

    void spawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }
}
