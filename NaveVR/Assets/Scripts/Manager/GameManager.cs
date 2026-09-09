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
    [Header("Results Prompt")]
    public GameObject resultsLPanel;
    public GameObject resultsRPanel;
    public GameObject continuePanel;
    [Header("Variables del Juego")]
    private int score = 0;
    private int misses = 0;
    private int round = 1;
    public int enemiesPerRound = 10;
    public int enemiesTouchedL = 0;
    public int enemiesTouchedR = 0;
    public int totalEnemiesTouched = 0;
    public int enemiesExpired = 0;
    [Header("Dificultad Dinamica")]
    public float enemyLifetime = 10.0f;
    public float timeSpawnInterval = 2.0f;
    public float enemySpeed = 2.0f;
    public float enemySize = 1.0f;
    [Header("Sistema de semiesfera")]
    private float maxRadiusL;
    private float maxRadiusR;
    public float actualRadiusL;
    public float actualRadiusR;
    public float maxRadiusAchievedL = 0.0f;
    public float maxRadiusAchievedR = 0.0f;
    public bool roundOver = false;
    [Header("Timer Configuration")]
    public float timer;
    public bool timerIsRunning = false;
    public float initialTimerValue;
    private bool useTimerConfig;
    [Header("AverageTimes")]
    public float totalRoundTime = 0.0f;
    public float initialReactionTimeL = 0.0f;
    public float initialReactionTimeR = 0.0f;
    public float maxSpeedAchieved = 0.0f;
    //public float maxRadiusAchieved = 0.0f;
    public float averageInteractionTimeL = 0.0f;
    public float averageInteractionTimeR = 0.0f;
    public float averageArmAngleL = 0.0f;
    public float averageArmAngleR = 0.0f;
    public float averageSpawningTimeAchieved = 0.0f;
    [Header("UI Metrics")]
    public TextMeshProUGUI averageSpawningTimeAchievedText;
    public TextMeshProUGUI initialReactionTimeLText;
    public TextMeshProUGUI initialReactionTimeRText;
    public TextMeshProUGUI totalRoundTimeText;
    public TextMeshProUGUI maxSpeedAchievedText;
    public TextMeshProUGUI maxRadiusAchievedLText;
    public TextMeshProUGUI maxRadiusAchievedRText;
    public TextMeshProUGUI averageInteractionTimeLText;
    public TextMeshProUGUI averageInteractionTimeRText;
    public TextMeshProUGUI averageArmAngleLText;
    public TextMeshProUGUI averageArmAngleRText;
    public TextMeshProUGUI enemiesExpiredText;
    public TextMeshProUGUI enemiesTouchedLText;
    public TextMeshProUGUI enemiesTouchedRText;
    public TextMeshProUGUI totalEnemiesTouchedText;

    private float lastTouchTimeL = 0.0f;
    private float lastTouchTimeR = 0.0f;
    private float totalInteractionTimeL = 0.0f;
    private float totalInteractionTimeR = 0.0f;
    private int leftInteractionCount = 0;
    private int rightInteractionCount = 0;
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
        if (selectedTime > 30.0f) selectedTime -= 30.0f; // Decrementa en 10 segundos
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
        if (timerControls != null) timerControls.SetActive(useTimerToggle.isOn);
    }

    public void ConfirmAndStartGame()
    {
        PlayerPrefs.SetInt("UseTimer", useTimerToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("SessionTime", selectedTime);
        PlayerPrefs.Save();
        if (timerPanel != null) timerPanel.SetActive(false);
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
            if (timerControls != null) timerControls.SetActive(useTimerToggle.isOn);
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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
            maxRadiusL = PlayerPrefs.GetFloat("PlayerRadiusL", 0.7f);
            maxRadiusR = PlayerPrefs.GetFloat("PlayerRadiusR", 0.7f);
            actualRadiusL = 0.3f; // Comenzamos con un radio más pequeño para aumentar la dificultad gradualmente
            actualRadiusR = 0.3f; // Comenzamos con un radio más pequeño para aumentar la dificultad gradualmente
            maxSpeedAchieved = enemySpeed;
            maxRadiusAchievedL = actualRadiusL;
            maxRadiusAchievedR = actualRadiusR;
            float calibratedElbow = PlayerPrefs.GetFloat("PlayerElbowAngle", 0.0f);
            averageArmAngleL = PlayerPrefs.GetFloat("PlayerElbowAngleL", 0.0f);
            averageArmAngleR = PlayerPrefs.GetFloat("PlayerElbowAngleR", 0.0f);
            StartUI();
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

    void StartUI()
    {
        continuePanel.SetActive(false);
        resultsLPanel.SetActive(false);
        resultsRPanel.SetActive(false);
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
            }
            else
            {
                timer = 0;
                timerIsRunning = false;
                roundOver = true;
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
        enemiesTouchedL = 0;
        enemiesTouchedR = 0;
        enemiesExpired = 0;
        score = 0;
        roundOver = false;
        timer = initialTimerValue;
        timerIsRunning = useTimerConfig;
        roundStartTime = Time.time;
        lastTouchTimeL = Time.time;
        lastTouchTimeR = Time.time;
        totalRoundTime = 0.0f;
        initialReactionTimeL = 0.0f;
        initialReactionTimeR = 0.0f;
        totalInteractionTimeL = 0.0f;
        totalInteractionTimeR = 0.0f;
        leftInteractionCount = 0;
        rightInteractionCount = 0;
        totalSpawnIntervals = 0.0f;
        spawnCount = 0;
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

    public void EnemyTouched(int points, bool isLeftHand)
    {
        score += points;
        if(isLeftHand)
        {
            if(leftInteractionCount == 0) initialReactionTimeL = Time.time - roundStartTime;
            float interactionL = Time.time - lastTouchTimeL;
            totalInteractionTimeL += interactionL;
            leftInteractionCount++;
            averageInteractionTimeL = totalInteractionTimeL / leftInteractionCount;
            lastTouchTimeL = Time.time;
            enemiesTouchedL++;
            actualRadiusL = Mathf.Min(actualRadiusL + 0.2f, maxRadiusL);
        }
        else
        {
            if(rightInteractionCount == 0) initialReactionTimeR = Time.time - roundStartTime;
            float interactionR = Time.time - lastTouchTimeR;
            totalInteractionTimeR += interactionR;
            rightInteractionCount++;
            averageInteractionTimeR = totalInteractionTimeR / rightInteractionCount;
            lastTouchTimeR = Time.time;
            enemiesTouchedR++;
            actualRadiusR = Mathf.Min(actualRadiusR + 0.2f, maxRadiusR);
        }
        enemySpeed = Mathf.Min(enemySpeed + 0.1f, 10.0f);
        timeSpawnInterval = Mathf.Max(timeSpawnInterval - 0.05f, 0.5f);
        UpdateMetricsUI();
        UpdateUI();
        CheckRoundEnd();
    }

    public void EnemyExpired()
    {
        misses++;
        enemiesExpired++;
        lastTouchTimeL = Time.time;
        lastTouchTimeR = Time.time;
        enemySpeed = Mathf.Max(enemySpeed - 0.1f, 1.0f);
        timeSpawnInterval = Mathf.Min(timeSpawnInterval + 0.05f, 5.0f);
        actualRadiusL = Mathf.Max(actualRadiusL - 0.02f, 0.2f);
        actualRadiusR = Mathf.Max(actualRadiusR - 0.02f, 0.2f);
        enemyLifetime = Mathf.Min(enemyLifetime + 0.2f, 10.0f);
        UpdateMetricsUI();
        UpdateUI();
    }

    void CheckRoundEnd()
    {
        totalEnemiesTouched = enemiesTouchedL + enemiesTouchedR;
        if (totalEnemiesTouched >= enemiesPerRound && !roundOver)
        {
            roundOver = true;
            timerIsRunning = false;
            PlayerPrefs.SetFloat("TimeSpawnInterval", timeSpawnInterval);
            PlayerPrefs.SetFloat("EnemySpeed", enemySpeed);
            PlayerPrefs.SetFloat("EnemyLifetime", enemyLifetime);
            PlayerPrefs.Save();
            ShowContinuePrompt();
        }
    }

    void UpdateMetricsUI()
    {
        maxSpeedAchieved = Mathf.Max(maxSpeedAchieved, enemySpeed);
        maxRadiusAchievedL = Mathf.Max(maxRadiusAchievedL, actualRadiusL);
        maxRadiusAchievedR = Mathf.Max(maxRadiusAchievedR, actualRadiusR);
        if(totalEnemiesTouchedText != null) totalEnemiesTouchedText.text = string.Format("Aciertos en total: {0}", totalEnemiesTouched);
        if (maxSpeedAchievedText != null) maxSpeedAchievedText.text = string.Format("Velocidad Máxima alcanzada\npor las Naves: {0:F1}", maxSpeedAchieved);
        if (averageSpawningTimeAchievedText != null) averageSpawningTimeAchievedText.text = string.Format("Tiempo de Aparición\nde las Naves: {0:F1}s", averageSpawningTimeAchieved);
        if (enemiesExpiredText != null) enemiesExpiredText.text = string.Format("Fallos: {0}", enemiesExpired);
        if (initialReactionTimeLText != null) initialReactionTimeLText.text = string.Format("Tiempo de\nReacción: {0:F1}s", initialReactionTimeL);
        if (averageInteractionTimeLText != null) averageInteractionTimeLText.text = string.Format("Tiempo Promedio\nentre Interacción: {0:F1}s", averageInteractionTimeL);
        if(averageArmAngleLText != null) averageArmAngleLText.text = string.Format("Ángulo Promedio\ndel Brazo: {0:F1}º", averageArmAngleL);
        if(enemiesTouchedLText != null) enemiesTouchedLText.text = string.Format("Aciertos: {0}", enemiesTouchedL);
        if (maxRadiusAchievedLText != null) maxRadiusAchievedLText.text = string.Format("Radio Máximo \nalcanzado: {0:F1}m", maxRadiusAchievedL);
        if (initialReactionTimeRText != null)initialReactionTimeRText.text = string.Format("Tiempo de\nReacción: {0:F1}s", initialReactionTimeR);
        if (averageInteractionTimeRText != null) averageInteractionTimeRText.text = string.Format("Tiempo Promedio\nentre Interacción: {0:F1}s", averageInteractionTimeR);
        if (averageArmAngleRText != null) averageArmAngleRText.text = string.Format("Ángulo Promedio\ndel Brazo: {0:F1}º", averageArmAngleR);
        if (enemiesTouchedRText != null) enemiesTouchedRText.text = string.Format("Aciertos: {0}", enemiesTouchedR);
        if (maxRadiusAchievedRText != null) maxRadiusAchievedRText.text = string.Format("Radio Máximo \nalcanzado: {0:F1}m", maxRadiusAchievedR);
    }

    void ShowContinuePrompt()
    {
        if (continuePanel != null && resultsLPanel != null && resultsRPanel != null)
        {
            StopReminder();
            if(totalRoundTimeText != null)
            {
                float finalTime = useTimerConfig ? initialTimerValue : totalRoundTime;
                float minutes = Mathf.FloorToInt(finalTime / 60);
                float seconds = Mathf.FloorToInt(finalTime % 60);
                totalRoundTimeText.text = string.Format("Tiempo total: {0:00}:{1:00}", minutes, seconds);
            }
            EndUI();
        }
    }

    void EndUI()
    {
        continuePanel.SetActive(true);
        resultsLPanel.SetActive(true);
        resultsRPanel.SetActive(true);
    }

    public void OnClickYes()
    {
        StartUI();
        round++;
        UpdateUI();
        StartCoroutine(CoundownRutine());
    }

    public void OnClickNo()
    {
        EndUI();
        Debug.Log("Juego terminado. Gracias por jugar.");
        SceneManager.LoadScene("MenuGeneral");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    void UpdateUI()
    {
        int totalEnemiesTouched = enemiesTouchedL + enemiesTouchedR;
        scoreText.text = "Naves\nrestantes: " + (enemiesPerRound - totalEnemiesTouched);
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