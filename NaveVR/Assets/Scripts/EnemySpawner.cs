//using System.Collections;
//using Oculus.Interaction.Editor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Drag the Enemy Prefabs")] public GameObject[] enemyPrefabs;
    [Tooltip("Drag the Enemy Spawners")] public Transform[] enemySpawners;

    public GameObject selectedEnemy;

    public void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedEnemy", 0);
        if (enemyPrefabs != null && selectedIndex < enemyPrefabs.Length)
        {
            selectedEnemy = enemyPrefabs[selectedIndex];
        }
        else
        {
            if (enemyPrefabs.Length > 0)
            {
                selectedEnemy = enemyPrefabs[0];
            }
            Debug.LogWarning("Selected enemy index is out of range or enemyPrefabs is not assigned.");
            return;
        }

        if (enemySpawners.Length == 0)
        {
            Debug.LogWarning("No enemy spawners assigned!");
            return;
        }
    }

    public void SpawnSingleEnemy()
    {
        if(enemySpawners.Length == 0 || selectedEnemy == null)
        {
            Debug.LogWarning("No enemy spawners assigned or no enemy selected!");
            return;
        }
        int randomIndex = Random.Range(0, enemySpawners.Length);
        Transform selectedSpawner = enemySpawners[randomIndex];
        Instantiate(selectedEnemy, selectedSpawner.position, selectedSpawner.rotation);
    }
}