using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI countdownText;
    [Header("Continue Prompt")]
    public GameObject continuePanel;
    public TextMeshProUGUI continueText;
    [Header("Variables del Juego")]
    private int score = 0;
    private int round = 1;
    public int enemiesPerRound = 10;
    private int enemiesTouched = 0;
    private int enemiesExpired = 0;
    [Header("Dificultad Dinamica")]
    public float enemyLifetime = 10.0f;
    public float timeSpawnInterval = 2.0f;
    public float enemySpeed = 2.0f;

    public void MainScene()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void Calibrate()
    {
        SceneManager.LoadScene("Calibracion");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void Difficulty()
    {
        SceneManager.LoadScene("Dificultad");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Juego");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al iniciar el juego
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemySpeed = PlayerPrefs.GetFloat("EnemySpeed", 3.0f);
        enemyLifetime = PlayerPrefs.GetFloat("EnemyLifetime", 10.0f);
        UpdateUI();
        if (continuePanel != null)
        {
            continuePanel.SetActive(false);
        }
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        StartCoroutine(CoundownRutine());
    }

    IEnumerator CoundownRutine()
    {
        countdownText.gameObject.SetActive(true);
        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        countdownText.text = "¡Comienza!";
        yield return new WaitForSeconds(1.0f);
        countdownText.gameObject.SetActive(false);
        StartRound();
    }

    void StartRound()
    {
        enemiesTouched = 0;
        enemiesExpired = 0;
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        for (int i = 0; i < enemiesPerRound; i++)
        {
            if (spawner != null)
            {
                spawner.SpawnSingleEnemy();
            }
            yield return new WaitForSeconds(timeSpawnInterval);
        }
    }

    public void EnemyTouched(int points)
    {
        score += points;
        enemiesTouched++;
        UpdateUI();
        CheckRoundEnd();
    }

    public void EnemyExpired()
    {
        enemiesExpired++;
        CheckRoundEnd();
    }

    void CheckRoundEnd()
    {
        int totalEnemies = enemiesTouched + enemiesExpired;
        if (totalEnemies >= enemiesPerRound)
        {
            EvaluateDifficulty();
            ShowContinuePrompt();
        }
    }

    void EvaluateDifficulty()
    {
        // Calculamos el porcentaje de éxito (0.0 a 1.0)
        float successPercentage = (float)enemiesTouched / enemiesPerRound;
        Debug.Log($"Éxito de la ronda: {successPercentage * 100}%");

        if (successPercentage > 0.7f) // 80% o más de éxito = Subir dificultad
        {
            Debug.Log("Subiendo dificultad para la próxima ronda.");
            enemyLifetime = Mathf.Max(3f, enemyLifetime - 1.5f); // Menos tiempo para tocarlo
            timeSpawnInterval = Mathf.Max(0.5f, timeSpawnInterval - 0.2f); // Salen más rápido
            enemySpeed += 0.5f; // Caminan más rápido
        }
        else if (successPercentage < 0.5f) // 40% o menos = Bajar dificultad
        {
            Debug.Log("Bajando dificultad para la próxima ronda.");
            enemyLifetime += 2f;
            timeSpawnInterval += 0.5f;
            enemySpeed = Mathf.Max(1f, enemySpeed - 0.5f);
        }
        else
        {
            Debug.Log("Manteniendo dificultad para la próxima ronda.");
        }
    }

    void ShowContinuePrompt()
    {
        if (continuePanel != null)
        {
            continuePanel.SetActive(true);
        }
    }

    public void OnClickYes()
    {
        continuePanel.SetActive(false);
        round++;
        UpdateUI();
        StartCoroutine(CoundownRutine());
    }

    public void OnClickNo()
    {
        continuePanel.SetActive(true);
        // Aquí podrías agregar lógica para terminar el juego o volver al menú principal
        Debug.Log("Juego terminado. Gracias por jugar.");
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    void UpdateUI()
    {
        scoreText.text = "Puntuacion: " + score;
        roundText.text = "Round\n" + round;
    }
}