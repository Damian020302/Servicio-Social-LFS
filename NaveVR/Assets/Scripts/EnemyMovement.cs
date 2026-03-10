using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float minDistance = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        speed = Random.Range(2.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            return;
        }
        transform.LookAt(player);
        float distance = Vector3.Distance(transform.position, player.position);
        if(distance > minDistance)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
}
