using System.Collections;
using Oculus.Interaction.Editor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;
    public float rangeX = 2.0f;
    public void StartGeneration(int enemies)
    {
        StartCoroutine(SpawnEnemies(enemies));
    }

    IEnumerator SpawnEnemies(int count)
    {
        for(int i = 0; i < count; i++)
        {
            float wait = 2.0f;
            if(GameManager.Instance != null)
            {
                wait = GameManager.Instance.timeSpawnInterval;
            }
            yield return new WaitForSeconds(wait);
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-rangeX, rangeX), 0, 0);
            Instantiate(enemyPrefab, spawnPosition, transform.rotation);
        }
    }
    //[SerializeField] private float enemyInterval;
    /*public GameObject player;
    private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("spawnEnemy", enemyInterval, enemyInterval);
    }

    void spawnEnemy()
    {
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }*/
}
