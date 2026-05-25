//using System.Collections;
//using Oculus.Interaction.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Drag the Enemy Prefabs")] public GameObject[] enemyPrefabs;
    [Tooltip("Drag the Enemy Spawners")] public Transform[] enemySpawners;

    public GameObject selectedEnemy;
    public AudioSource audioSource;
    public AudioClip spawnSound;

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
        PlaySpawnSound();
        Instantiate(selectedEnemy, selectedSpawner.position, selectedSpawner.rotation);
    }

    public void PlaySpawnSound()
    {
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
}