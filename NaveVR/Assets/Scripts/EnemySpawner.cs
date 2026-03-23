using System.Collections;
using Oculus.Interaction.Editor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject enemyPrefab;
    [Tooltip("Drag the Enemy Spawners")] public Transform[] enemySpawners;

    public void SpawnSingleEnemy()
    {
        if (enemySpawners.Length == 0)
        {
            Debug.LogWarning("No enemy spawners assigned!");
            return;
        }
        int randomIndex = Random.Range(0, enemySpawners.Length);
        Transform selectedSpawner = enemySpawners[randomIndex];

        Instantiate(enemyPrefab, selectedSpawner.position, selectedSpawner.rotation);
    }
}