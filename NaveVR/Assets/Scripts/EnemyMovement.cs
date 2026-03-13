using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    [Tooltip("Ventana de tiempo que el jugador tendra para destruir al enemigo")] public float timeToDestroy = 5.0f;
    private bool isStopped = false;
    private float waitTimer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        speed = Random.Range(1.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            return;
        }
        if (!isStopped)
        {
            transform.LookAt(player);
            transform.position += transform.forward * speed * Time.deltaTime;
            float distance = Vector3.Distance(transform.position, player.position);
        }
        else
        { 
            waitTimer += Time.deltaTime;
            if(waitTimer >= timeToDestroy)
            {
                Destroy(gameObject);
            }
        }
        /*transform.LookAt(player);
        float distance = Vector3.Distance(transform.position, player.position);
        if(distance > minDistance)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if(other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }*/
        if(other.gameObject.tag == "Limit")
        {
            isStopped = true;
        }
    }

    /*void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Limit")
        {
            Destroy(gameObject);
        }
    }*/
}
