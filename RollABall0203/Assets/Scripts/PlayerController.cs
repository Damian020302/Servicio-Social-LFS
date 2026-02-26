using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public TextMeshProUGUI counterPoints;
    public TextMeshProUGUI counterLives;
    public GameObject winTextObject;
    public GameObject winMenu;
    public GameObject loseTextObject;
    public GameObject loseMenu;
    public GameObject pauseButton;
    private Rigidbody rb;
    private int count;
    private int lives;
    private float movementX;
    private float movementY;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        lives = 9;
        SetCounterPoints();
        SetCounterLives();
        winTextObject.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCounterPoints()
    {
        counterPoints.text = "Count: " + count.ToString();
        if (count >= 12 && SceneManager.GetActiveScene().name == "Level1")
        {
            winTextObject.SetActive(true);
            Time.timeScale = 0;
            winMenu.SetActive(true);
        }
    }

    void SetCounterLives()
    {
        counterLives.text = "Lives: " + lives.ToString();
        if(lives <= 0)
        {
            loseTextObject.SetActive(true);
            this.gameObject.SetActive(false);
            Time.timeScale = 0;
            loseMenu.SetActive(true);
            pauseButton.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LifeUp") && lives < 9)
        {
            other.gameObject.SetActive(false);
            lives++;
            SetCounterLives();
        } 
        else if(other.gameObject.CompareTag("Lava"))
        {
            lives = 0;
            SetCounterLives();
        }
        else if(SceneManager.GetActiveScene().name == "Level2" && other.gameObject.CompareTag("Goal"))
        {
            other.gameObject.SetActive(false);
            winTextObject.SetActive(true);
            Time.timeScale = 0;
            winMenu.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            lives--;
            SetCounterLives();
        }
        else if(collision.gameObject.CompareTag("PickUp"))
        {
            Destroy(collision.gameObject);
            count++;
            SetCounterPoints();
        }
    }

    public void Level2()
    {
        SceneManager.LoadScene("Level2");
        Time.timeScale = 1;
    }

    public void Victoria()
    {
        SceneManager.LoadScene("Menu");
    }
}