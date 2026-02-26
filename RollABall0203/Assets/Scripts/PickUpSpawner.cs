using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    public GameObject[] pickUpPrefab;
    [SerializeField] private float pickUpInterval = 2.0f;
    public float spawnRatio = 3.0f;
    public GameObject player;
    private Vector3 offset;
    
    void Start()
    {
        if (player != null)
        {
            offset = transform.position - player.transform.position;
        }
        InvokeRepeating("spawnPickUp", 0f, pickUpInterval);
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }

    void spawnPickUp()
    {
        float angleP = 360.0f / pickUpPrefab.Length;
        for (int i = 0; i < pickUpPrefab.Length; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, angleP * i, 0);
            Vector3 spawnPosition = transform.position + (rotation * Vector3.forward * spawnRatio);
            Instantiate(pickUpPrefab[i], spawnPosition, Quaternion.identity);
        }
    }
}