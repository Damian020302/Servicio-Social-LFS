using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public float speed;
    [Tooltip("Ventana de tiempo que el jugador tiene para destruir al enemigo")] public float timeToDestroy = 5.0f;
    private bool isStopped = false;
    private float waitTimer = 0.0f;
    private float playerRadius;
    private float scale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if(GameManager.Instance != null)
        {
            speed = GameManager.Instance.enemySpeed;
            timeToDestroy = GameManager.Instance.enemyLifetime;
            playerRadius = GameManager.Instance.actualRadius;
            scale = GameManager.Instance.enemySize;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            speed = 3.0f;
            timeToDestroy = 5.0f;
            playerRadius = 0.7f;
        }
        playerRadius = PlayerPrefs.GetFloat("PlayerRadius", 0.7f);
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
            float playerDistance = Vector3.Distance(transform.position, player.position);
            if(playerDistance > playerRadius)
            {
                transform.position += transform.forward * speed * Time.deltaTime;
            }
            else
            {
                isStopped = true;
            }
        }
        else
        { 
            waitTimer += Time.deltaTime;
            if(waitTimer >= timeToDestroy)
            {
                if(GameManager.Instance != null)
                {
                    GameManager.Instance.EnemyExpired();
                }
                Destroy(gameObject);
            }
        }
    }
}
