using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float speed = 0.0f;
    public float minDist = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        speed = Random.Range(4.0f, 6.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }
        transform.LookAt(player);
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > minDist)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        EnemySpawner.enemyCounter--;
        if(EnemySpawner.enemyCounter  < 0 )
        {
            EnemySpawner.enemyCounter = 0;
        }
    }
}