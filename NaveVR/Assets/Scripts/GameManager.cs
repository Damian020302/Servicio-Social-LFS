using UnityEngine;
using TMPro;
using System.Collections;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI continueText;
    public TextMeshProUGUI countdownText;
    //private int enemiesDestroyed;
    private int score = 0;
    private int round = 1;
    public int enemiesPerRound = 10;
    private int enemiesTouched = 0;
    private int enemiesExpired = 0;
    public float enemyLifetime = 10.0f;
    public float timeSpawnInterval = 2.0f;
    public float enemySpeed = 2.0f;
    /*private int countdown;
    private bool continueYN;*/
    //private int successRate;

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
        continueText.gameObject.SetActive(false);
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
        int countdown = 5;
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

        if (successPercentage >= 0.8f) // 80% o más de éxito = Subir dificultad
        {
            enemyLifetime = Mathf.Max(3f, enemyLifetime - 1.5f); // Menos tiempo para tocarlo
            timeSpawnInterval = Mathf.Max(0.5f, timeSpawnInterval - 0.2f); // Salen más rápido
            enemySpeed += 0.5f; // Caminan más rápido
        }
        else if (successPercentage <= 0.4f) // 40% o menos = Bajar dificultad
        {
            enemyLifetime += 2f; 
            timeSpawnInterval += 0.5f;
            enemySpeed = Mathf.Max(1f, enemySpeed - 0.5f);
        }
        // Si está entre 41% y 79%, la dificultad se mantiene igual.
    }

    void ShowContinuePrompt()
    {
        continueText.gameObject.SetActive(true);
        continueText.text = "¿Continuar a la siguiente ronda? (Y/N)";
        StartCoroutine(WaitForContinueInput());
    }

    public void NextRound()
    {
        continueText.gameObject.SetActive(false);
        round++;
        UpdateUI();
        StartCoroutine(CoundownRutine());
    }

    void UpdateUI()
    {
        UpdateScoreText();
        UpdateRoundText();
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

    void UpdateScoreText()
    {
        scoreText.text = "Puntuaci�n: " + score;
    }

    void UpdateRoundText()
    {
        roundText.text = "Round\n" + round;
    }

    /*void UpdateCountdownText()
    {
        for(int i = countdown; i > 0; i--)
        {
            countdownText.text = "" + i;
        }
    }*/
}
