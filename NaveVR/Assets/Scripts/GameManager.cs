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
    //public TextMeshProUGUI continueText;
    public GameObject continuePanel;
    public TextMeshProUGUI continueText;
    //private int enemiesDestroyed;
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
    /*private int countdown;
    private bool continueYN;*/
    //private int successRate;

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
        if(Instance == null)
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
        UpdateUI();
        if(continuePanel != null)
        {
            continuePanel.SetActive(false);
        }
        if(countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        //continueText.gameObject.SetActive(false);
        StartCoroutine(CoundownRutine());
        /*score = 0;
        round = 1;
        countdown = 5;
        //enemiesDestroyed = 0;
        continueYN = true;
        UpdateScoreText();
        UpdateRoundText();*/
    }

    IEnumerator CoundownRutine()
    {
        countdownText.gameObject.SetActive(true);
        //int countdown = 5;
        for(int i = 5; i > 0; i--)
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
        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        if(spawner != null)
        {
            spawner.StartGeneration(enemiesPerRound);
        }
        Debug.Log("Inicia la ronda " + round);
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
        //UpdateUI();
        CheckRoundEnd();
    }

    void CheckRoundEnd()
    {
        int totalEnemies = enemiesTouched + enemiesExpired;
        if(totalEnemies >= enemiesPerRound)
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
            enemyLifetime = Mathf.Max(3f, enemyLifetime - 1.5f); // Menos tiempo para tocarlo
            timeSpawnInterval = Mathf.Max(0.5f, timeSpawnInterval - 0.2f); // Salen más rápido
            enemySpeed += 0.5f; // Caminan más rápido
        }
        else if (successPercentage < 0.5f) // 40% o menos = Bajar dificultad
        {
            enemyLifetime += 2f; 
            timeSpawnInterval += 0.5f;
            enemySpeed = Mathf.Max(1f, enemySpeed - 0.5f);
        }
        // Si está entre 41% y 79%, la dificultad se mantiene igual.
    }

    void ShowContinuePrompt()
    {
        if(continuePanel != null)
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
        //UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // Ejemplo de volver al menú
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    /*public void NextRound()
    {
        continueText.gameObject.SetActive(false);
        round++;
        UpdateUI();
        StartCoroutine(CoundownRutine());
    }*/

    void UpdateUI()
    {
        scoreText.text = "Puntuacion: " + score;
        roundText.text = "Round\n" + round;
        //UpdateScoreText();
        //UpdateRoundText();
    }

    /*public void ScorePoints(int points)
    {
        score += points;
        UpdateScoreText();
    }*/

    /*public void EnemiesDestroyed(int count)
    {
        enemiesDestroyed += count;

    }*/

    /*void UpdateScoreText()
    {
        scoreText.text = "Puntuaci�n: " + score;
    }*/

   /* void UpdateRoundText()
    {
        roundText.text = "Round\n" + round;
    }*/

    /*void UpdateCountdownText()
    {
        for(int i = countdown; i > 0; i--)
        {
            countdownText.text = "" + i;
        }
    }*/
}