using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Timer Configuration")]
    public GameObject timerPanel;
    public TextMeshProUGUI timeDisplay;
    public Toggle useTimerToggle;
    public GameObject timerControls;
    private float selectedTime = 60.0f;
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    //public TextMeshProUGUI missText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timeRemainingText;
    [Header("Constant Warning")]
    public TextMeshProUGUI warning;
    public float warningBlinkSpeed = 5.0f;
    public float warningScaleMultiplier = 1.2f;
    private Coroutine warningCoroutine;
    private Vector3 warningOriginalScale;
    [Header("Continue Prompt")]
    public GameObject continuePanel;
    public TextMeshProUGUI continueText;
    [Header("Variables del Juego")]
    private int score = 0;
    private int misses = 0;
    private int round = 1;
    public int enemiesPerRound = 10;
    public int enemiesTouched = 0;
    public int enemiesExpired = 0;
    [Header("Dificultad Dinamica")]
    public float enemyLifetime = 10.0f;
    public float timeSpawnInterval = 2.0f;
    public float enemySpeed = 2.0f;
    public float enemySize = 1.0f;
    [Header("Sistema de semiesfera")]
    private float maxRadius;
    public float actualRadius;
    public bool roundOver = false;
    [Header("Timer Configuration")]
    public float timer;
    public bool timerIsRunning = false;
    public float initialTimerValue;
    private bool useTimerConfig;
    [Header("AverageTimes")]
    public float totalRoundTime = 0.0f;
    public float maxSpeedAchieved = 0.0f;
    public float maxRadiusAchieved = 0.0f;
    public float averageInteractionTime = 0.0f;
    public float averageArmAngle = 0.0f;
    public float averageSpawningTimeAchieved = 0.0f;
    public TextMeshProUGUI averageSpawningTimeAchievedText;
    public TextMeshProUGUI totalRoundTimeText;
    public TextMeshProUGUI maxSpeedAchievedText;
    public TextMeshProUGUI maxRadiusAchievedText;
    public TextMeshProUGUI averageInteractionTimeText;
    public TextMeshProUGUI averageArmAngleText;
    public TextMeshProUGUI enemiesExpiredText;
    public TextMeshProUGUI enemiesTouchedText;

    private float lastTouchTime = 0.0f;
    private float totalInteractionTime = 0.0f;
    private int interactionCount = 0;
    private float totalSpawnIntervals = 0.0f;
    private int spawnCount = 0;
    private float roundStartTime = 0.0f;

    public void IncreaseTime()
    {
        selectedTime += 30.0f; // Incrementa en 10 segundos
        UpdateTimeDisplay();
    }

    public void DecreaseTime()
    {
        if (selectedTime > 30.0f) // Evita que el tiempo sea menor a 10 segundos
        {
            selectedTime -= 30.0f; // Decrementa en 10 segundos
        }
        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if (timeDisplay != null)
        {
            float minutes = Mathf.FloorToInt(selectedTime / 60);
            float seconds = Mathf.FloorToInt(selectedTime % 60);
            timeDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void OnToggleTimer()
    {
        if (timerControls != null)
        {
            timerControls.SetActive(useTimerToggle.isOn);
        }
    }

    public void ConfirmAndStartGame()
    {
        PlayerPrefs.SetInt("UseTimer", useTimerToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("SessionTime", selectedTime);
        PlayerPrefs.Save();
        if (timerPanel != null)
        {
            timerPanel.SetActive(false);
        }
        SceneManager.LoadScene("Calibracion");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al iniciar el juego
    }

    public void MenuGeneral()
    {
        SceneManager.LoadScene("MenuGeneral");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void MainScene()
    {
        SceneManager.LoadScene("Juego");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void Calibrate()
    {
        if (timerPanel != null)
        {
            timerPanel.SetActive(true);
            UpdateTimeDisplay();
            if (timerControls != null)
            {
                timerControls.SetActive(useTimerToggle.isOn);
            }
        }
        else
        {
            SceneManager.LoadScene("Calibracion");
            Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
        }
    }

    public void Difficulty()
    {
        SceneManager.LoadScene("Dificultad");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    /*public void StartGame()
    {
        SceneManager.LoadScene("Juego");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al iniciar el juego
    }*/

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
        if(SceneManager.GetActiveScene().name == "Juego")
        {
            enemySpeed = PlayerPrefs.GetFloat("EnemySpeed", 3.0f);
            enemyLifetime = PlayerPrefs.GetFloat("EnemyLifetime", 10.0f);
            timeSpawnInterval = PlayerPrefs.GetFloat("TimeSpawnInterval", 2.0f);
            enemySize = PlayerPrefs.GetFloat("EnemySize", 1.0f);
            maxRadius = PlayerPrefs.GetFloat("PlayerRadius", 0.7f);
            averageArmAngle = PlayerPrefs.GetFloat("PlayerShoulderAngle", 0.0f);
            actualRadius = 0.3f; // Comenzamos con un radio más pequeño para aumentar la dificultad gradualmente
            maxSpeedAchieved = enemySpeed;
            maxRadiusAchieved = actualRadius;
            if (continuePanel != null) continuePanel.SetActive(false);
            if (countdownText != null) countdownText.gameObject.SetActive(false);
            if (warning != null)
            {
                warningOriginalScale = warning.transform.localScale;
                warning.gameObject.SetActive(false);
            }
            UpdateUI();
            StartCoroutine(CoundownRutine());
        }
        useTimerConfig = PlayerPrefs.GetInt("UseTimer", 1) == 1;
        initialTimerValue = PlayerPrefs.GetFloat("SessionTime", 60.0f);
        timer = initialTimerValue;
        timerIsRunning = useTimerConfig;
        if(!useTimerConfig && timeRemainingText != null) timeRemainingText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (roundOver) return;
        totalRoundTime = Time.time - roundStartTime;
        if(useTimerConfig && timerIsRunning)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                DisplayTime(timer);
                //UpdateUI();
            }
            else
            {
                timer = 0;
                timerIsRunning = false;
                roundOver = true;
                //EvaluateDifficulty();
                ShowContinuePrompt();
            }
        }
        else if(!useTimerConfig && totalRoundTimeText != null)
        {
            float minutes = Mathf.FloorToInt(totalRoundTime / 60);
            float seconds = Mathf.FloorToInt(totalRoundTime % 60);
            totalRoundTimeText.text = string.Format("Tiempo Total: {0:00}:{1:00}", minutes, seconds);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if(timeRemainingText != null) timeRemainingText.text = string.Format("Tiempo: {0:00}:{1:00}", minutes, seconds);
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
        score = 0;
        roundOver = false;
        timer = initialTimerValue;
        timerIsRunning = useTimerConfig;
        roundStartTime = Time.time;
        lastTouchTime = Time.time;
        totalRoundTime = 0.0f;
        StartReminder();
        StartCoroutine(SpawnWaveRoutine());
    }

    IEnumerator SpawnWaveRoutine()
    {
        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        while(!roundOver)
        {
            if (spawner != null) spawner.SpawnSingleEnemy();
            totalSpawnIntervals += timeSpawnInterval;
            spawnCount++;
            averageSpawningTimeAchieved = totalSpawnIntervals / spawnCount;
            UpdateMetricsUI();
            yield return new WaitForSeconds(timeSpawnInterval);
        }
    }

    public void EnemyTouched(int points)
    {
        score += points;
        enemiesTouched++;
        float interaction = Time.time - lastTouchTime;
        totalInteractionTime += interaction;
        interactionCount++;
        averageInteractionTime = totalInteractionTime / interactionCount;
        lastTouchTime = Time.time;
        enemySpeed = Mathf.Min(enemySpeed + 0.1f, 10.0f);
        timeSpawnInterval = Mathf.Max(timeSpawnInterval - 0.05f, 0.5f);
        actualRadius = Mathf.Min(actualRadius + 0.2f, maxRadius);
        UpdateMetricsUI();
        UpdateUI();
        CheckRoundEnd();
    }

    public void EnemyExpired()
    {
        misses++;
        enemiesExpired++;
        lastTouchTime = Time.time;
        enemySpeed = Mathf.Max(enemySpeed - 0.1f, 1.0f);
        timeSpawnInterval = Mathf.Min(timeSpawnInterval + 0.05f, 5.0f);
        actualRadius = Mathf.Max(actualRadius - 0.02f, 0.2f);
        enemyLifetime = Mathf.Min(enemyLifetime + 0.2f, 10.0f);
        UpdateMetricsUI();
        UpdateUI();
       // CheckRoundEnd();
    }

    void CheckRoundEnd()
    {
        //int totalEnemies = enemiesTouched + enemiesExpired;
        if (enemiesTouched >= enemiesPerRound && !roundOver)
        {
            roundOver = true;
            timerIsRunning = false;
            PlayerPrefs.SetFloat("TimeSpawnInterval", timeSpawnInterval);
            PlayerPrefs.SetFloat("EnemySpeed", enemySpeed);
            PlayerPrefs.SetFloat("EnemyLifetime", enemyLifetime);
            PlayerPrefs.Save();
            //EvaluateDifficulty();
            ShowContinuePrompt();
        }
    }

    /*void EvaluateDifficulty()
    {
        // Calculamos el porcentaje de éxito (0.0 a 1.0)
        int totalEnemiesSpawned = enemiesTouched + enemiesExpired;
        if (totalEnemiesSpawned == 0) return;
        float successPercentage = (float)enemiesTouched / totalEnemiesSpawned;
        Debug.Log($"Éxito de la ronda: {successPercentage * 100}%");

        if (successPercentage > 0.7f && enemiesTouched >= (enemiesPerRound * 0.5f)) // 80% o más de éxito = Subir dificultad
        {
            enemyLifetime = Mathf.Max(3.0f, enemyLifetime - 1.5f); // Menos tiempo para tocarlo
            timeSpawnInterval = Mathf.Max(0.5f, timeSpawnInterval - 0.2f); // Salen más rápido
            enemySpeed += 0.5f; // Caminan más rápido
            float increase = 0.15f;
            actualRadius = Mathf.Min(actualRadius + increase, maxRadius); // Aumenta el radio del jugador para hacerlo más difícil
            if(maxRadius - actualRadius <= 0.01f)
            {
                actualRadius = maxRadius; // Asegura que no se pase del radio máximo
            }
            Debug.Log($"Subiendo dificultad para la próxima ronda. {actualRadius}");
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
    }*/

    void UpdateMetricsUI()
    {
        maxSpeedAchieved = Mathf.Max(maxSpeedAchieved, enemySpeed);
        maxRadiusAchieved = Mathf.Max(maxRadiusAchieved, actualRadius);
        if(maxSpeedAchievedText != null)
        {
            maxSpeedAchievedText.text = string.Format("Velocidad Máxima alcanzada\npor las Naves: {0:F1}", maxSpeedAchieved);
        }
        if (maxRadiusAchievedText != null)
        {
            maxRadiusAchievedText.text = string.Format("Radio Máximo \nalcanzado: {0:F1}m", maxRadiusAchieved);
        }
        if (averageInteractionTimeText != null)
        {
            averageInteractionTimeText.text = string.Format("Tiempo Promedio\nentre Interacción: {0:F1}s", averageInteractionTime);
        }
        if (averageSpawningTimeAchievedText != null)
        {
            averageSpawningTimeAchievedText.text = string.Format("Tiempo de Aparición\nde las Naves: {0:F1}s", averageSpawningTimeAchieved);
        }
        if (averageArmAngleText != null)
        {
            averageArmAngleText.text = string.Format("Ángulo Promedio\ndel Brazo: {0:F1}º", averageArmAngle);
        }
        if(enemiesTouchedText != null)
        {
            enemiesTouchedText.text = string.Format("Aciertos: {0}", enemiesTouched);
        }
        if(enemiesExpiredText != null)
        {
            enemiesExpiredText.text = string.Format("Fallos: {0}", enemiesExpired);
        }
    }

    void ShowContinuePrompt()
    {
        if (continuePanel != null)
        {
            StopReminder();
            if(totalRoundTimeText != null)
            {
                float finalTime = useTimerConfig ? initialTimerValue : totalRoundTime;
                float minutes = Mathf.FloorToInt(finalTime / 60);
                float seconds = Mathf.FloorToInt(finalTime % 60);
                totalRoundTimeText.text = string.Format("Tiempo total: {0:00}:{1:00}", minutes, seconds);
            }
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
        SceneManager.LoadScene("MenuGeneral");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    void UpdateUI()
    {
        scoreText.text = "Naves\nrestantes: " + (enemiesPerRound - score);
        //missText.text = "Fallos: " + misses;
        roundText.text = "Round " + round;
    }
    
    IEnumerator WarningAnimationRoutine(string message)
    {
        if(warning == null) yield break;
        warning.text = message;
        warning.gameObject.SetActive(true);
        Color originalColor = warning.color;
        float tiempo = 0.0f;
        while(true)
        {
            tiempo += Time.deltaTime * warningBlinkSpeed;
            float alpha = (Mathf.Sin(tiempo) + 1.0f) / 2.0f; // Oscila entre 0 y 1
            Color nuevoColor = originalColor;
            nuevoColor.a = Mathf.Lerp(0.5f, 1.0f, alpha); // Cambia la transparencia entre 50% y 100%
            warning.color = nuevoColor;
            float scaleMultiplier = Mathf.Lerp(1.0f, warningScaleMultiplier, alpha); // Cambia el tamaño entre 100% y el multiplicador
            warning.transform.localScale = warningOriginalScale * scaleMultiplier;
            yield return null;
        }
    }

    public void StartReminder()
    {
        if(warningCoroutine != null) StopCoroutine(warningCoroutine);
        warningCoroutine = StartCoroutine(WarningAnimationRoutine("Estírate para destruir las naves"));
    }

    public void StopReminder()
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }
        if(warning != null) warning.gameObject.SetActive(false);
    }
}