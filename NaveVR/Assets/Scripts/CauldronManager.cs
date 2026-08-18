using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CauldronManager : MonoBehaviour
{
    public enum ExerciseState
    {
        WaitingForFlexion,
        WaitingForExtension,
        WatingForPotionToLand
    }

    private ExerciseState currentState = ExerciseState.WaitingForFlexion;

    public WandSpawner spawner;

    [Header("Timer Configuration")]
    public GameObject timerPanel;
    public TextMeshProUGUI timeDisplay;
    public Toggle useTimerToggle;
    public GameObject timerControls;
    private float selectedTime = 60.0f;

    [Header("Wrist tracking")]
    public Transform leftWrist;
    public Transform rightWrist;
    private Transform activeWrist;

    [Header("Buttons")]
    public GameObject victory;
    public GameObject yesV;
    public GameObject noV;

    [Header("UI")]
    public TextMeshProUGUI cauldronText;
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI potionsText;

    [Header("Flexion/Extension Thresholds")]
    public float flexionThreshold = 30.0f; // Ángulo mínimo para considerar una flexión
    public float extensionThreshold = 30.0f; // Ángulo mínimo para considerar una extensión

    [Header("Wrist Flexion/Extension Counter")]
    public float flexionAngle = 0.0f;
    public float extensionAngle = 0.0f;
    private Quaternion neutralRotation;
    private int completeExercises = 0;

    [Header("Constant Warning")]
    public TextMeshProUGUI warning;
    public float warningBlinkSpeed = 5.0f;
    public float warningScaleMultiplier = 1.2f;
    private Coroutine warningCoroutine;
    private Vector3 warningOriginalScale;

    [Header("Potion Launcher")]
    public GameObject prefabPotion;
    public Transform spawnPoint;
    public Transform cauldronEndpoint;
    public float launchSpeed = 10.0f;
    public float launchHeight = 2.0f;
    private GameObject currentPotion;

    [Header("Level Variables")]
    public int cauldron = 1;
    public int difficulty = 0;
    private bool isVictoryAchieved = false;
    public int winningStreak = 0;
    public WandManager wandManager;
    public int maxPotionsPerRound = 10;
    private int potionsThrown = 0;

    [Header("Timer Configuration")]
    public float timer;
    public bool timerIsRunning = false;
    public float initialTimerValue;
    private bool useTimerConfig;

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
        SceneManager.LoadScene("Calibracion3");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al iniciar el juego
    }


    public void MenuGeneral()
    {
        SceneManager.LoadScene("MenuGeneral");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }
    public void MainScene()
    {
        SceneManager.LoadScene("Juego3");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void Calibrate()
    {
        if(timerPanel != null)
        {
            timerPanel.SetActive(true);
            UpdateTimeDisplay();
            if(timerControls != null)
            {
                timerControls.SetActive(useTimerToggle.isOn);
            }
        }
        else
        {
            SceneManager.LoadScene("Calibracion3");
            Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
        }
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Juego3")
        {
            spawner.SpawnWand();
            wandManager = Object.FindFirstObjectByType<WandManager>();
            initialTimerValue = timer;
            yesV.SetActive(false);
            noV.SetActive(false);
            victory.SetActive(false);
            if (warning != null)
            {
                warningOriginalScale = warning.transform.localScale;
                if (warningOriginalScale == Vector3.zero)
                {
                    warningOriginalScale = Vector3.one;
                }
            }
            UpdateActiveWrist();

            if (activeWrist != null)
            {
                neutralRotation = activeWrist.rotation;
            }
            else
            {
                Debug.LogError("No se ha asignado ninguna muñeca activa. Por favor, asigna una muñeca en el inspector.");
            }
            extensionThreshold = PlayerPrefs.GetFloat("CauldronMaxExtension", extensionThreshold);
            flexionThreshold = PlayerPrefs.GetFloat("CauldronMaxFlexion", flexionThreshold);
            LevelConfig();
        }
        useTimerConfig = PlayerPrefs.GetInt("UseTimer", 1) == 1;
        initialTimerValue = PlayerPrefs.GetFloat("SessionTime", 60.0f);
        timer = initialTimerValue;
        timerIsRunning = useTimerConfig;
        if (!useTimerConfig && timeRemainingText != null)
        {
            timeRemainingText.gameObject.SetActive(false);
        }
    }

    void UpdateActiveWrist()
    {
        if (leftWrist != null && leftWrist.gameObject.activeInHierarchy)
        {
            activeWrist = leftWrist;
        }
        else if (rightWrist != null && rightWrist.gameObject.activeInHierarchy)
        {
            activeWrist = rightWrist;
        }
        else
        {
            activeWrist = null;
            Debug.LogError("No se ha asignado ninguna muñeca activa. Por favor, asigna una muñeca en el inspector.");
        }
    }

    void LevelConfig()
    {
        potionsThrown = 0;
        timer = initialTimerValue;
        timerIsRunning = useTimerConfig;
        isVictoryAchieved = false;
        if (currentPotion != null) Destroy(currentPotion);
        currentState = ExerciseState.WaitingForFlexion;
        StartReminder("Flexiona tu muñeca para tomar la poción");
        if(wandManager != null)
        {
            wandManager.UpdateRotation(0.0f);//90.0f
        }
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (isVictoryAchieved || activeWrist == null)
        {
            return; // Evita que se ejecute el código de actualización si ya se ha logrado la victoria
        }
        Vector3 forwardNeutral = neutralRotation * Vector3.forward;
        Vector3 rightNeutral = neutralRotation * Vector3.right;
        float flexExtAngle = Vector3.SignedAngle(forwardNeutral, activeWrist.forward, rightNeutral);
        flexionAngle = 0.0f;
        extensionAngle = 0.0f;
        if (flexExtAngle > 0)
        {
            flexionAngle = flexExtAngle;
        }
        else if (flexExtAngle < 0)
        {
            extensionAngle = Mathf.Abs(flexExtAngle);
        }
        float targetWandState = 0.0f;
        if (currentState == ExerciseState.WaitingForFlexion)
        {
            targetWandState = 0.0f;
            if (flexionAngle >= flexionThreshold)
            {
                SpawnPotion();
            }
        }
        else if (currentState == ExerciseState.WaitingForExtension)
        {
            targetWandState = 1.0f;
            if (extensionAngle >= extensionThreshold)
            {
                ThrowPotion();
            }
        }
        else if (currentState == ExerciseState.WatingForPotionToLand)
        {
            targetWandState = 0.0f; // No se muestra progreso mientras la poción está en el aire
        }
        if(wandManager != null)
        {
            wandManager.UpdateRotation(targetWandState);
        }

        if (useTimerConfig && timerIsRunning)
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
                isVictoryAchieved = true;
                VictoryAchieved();
            }
        }
        Debug.Log($"Flexion: {flexionAngle:F1}° | Extension: {extensionAngle:F1}°");
    }

    void SpawnPotion()
    {
        if(prefabPotion != null && spawnPoint != null)
        {
            currentPotion = Instantiate(prefabPotion, spawnPoint.position, spawnPoint.rotation, spawnPoint);
            currentPotion.transform.localScale = prefabPotion.transform.localScale;
            currentState = ExerciseState.WaitingForExtension;
            StartReminder("Flexiona tu mano para lanzar la poción");
            Debug.Log("Extensión completa, poción tomada");
        }
    }

    void ThrowPotion()
    { 
        if(prefabPotion != null && cauldronEndpoint != null)
        {
            completeExercises++;
            currentPotion.transform.SetParent(null);
            Parabola parabola = currentPotion.GetComponent<Parabola>();
            if(parabola == null)
            {
                parabola = currentPotion.AddComponent<Parabola>();
            }
            parabola.Launch(spawnPoint.position, cauldronEndpoint.position, launchSpeed, launchHeight);
            currentState = ExerciseState.WatingForPotionToLand;
            StartCoroutine(CheckPotionLanded(currentPotion));
        }        
    }

    IEnumerator CheckPotionLanded(GameObject potion)
    {
        while (potion != null)
        {
            yield return null;
        }
        if(!isVictoryAchieved)
        {
            potionsThrown++;
            Debug.Log($"Poción lanzada #{potionsThrown}");
            UpdateUI();
            if (potionsThrown >= maxPotionsPerRound)
            {
                isVictoryAchieved = true;
                timerIsRunning = false;
                VictoryAchieved();
            }
            else
            {
                currentState = ExerciseState.WaitingForFlexion;
                StartReminder("Flexiona tu muñeca para tomar la poción");
            }
        }
    }

    public void RecalibrateNeutral()
    {
        UpdateActiveWrist();
        if(activeWrist != null)
        {
            neutralRotation = activeWrist.rotation;
            Debug.Log("Neutral recalibrado");
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeRemainingText.text = string.Format("Tiempo Restante: {0:00}:{1:00}", minutes, seconds);
    }

    void VictoryAchieved()
    {
        StopReminder();
        yesV.SetActive(true);
        noV.SetActive(true);
        victory.SetActive(true);
        Debug.Log("Victory logic executed.");
    }

    public void OnClickYesV()
    {
        StopReminder();
        StartReminder("Extiende tu muñeca para lanzar la poción");
        cauldron++;
        yesV.SetActive(false);
        noV.SetActive(false);
        victory.SetActive(false);
        if(timer >= (initialTimerValue * 0.5f) && timer > 0)
        {
            winningStreak++;
            if(winningStreak >= 2)
            {
                difficulty++;
                winningStreak = 0;
            }
        }
        else
        {
            winningStreak = 0;
        }
        LevelConfig();
    }

    IEnumerator WarningAnimationRoutine(string message)
    {
        if (warning == null) yield break;
        warning.text = message;
        warning.gameObject.SetActive(true);
        Color originalColor = warning.color;
        float tiempo = 0.0f;
        while (true)
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

    public void StartReminder(string message)
    {
        if (warningCoroutine != null) StopCoroutine(warningCoroutine);
        warningCoroutine = StartCoroutine(WarningAnimationRoutine(message));
    }

    public void StopReminder()
    {
        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }
        if (warning != null)
        {
            warning.gameObject.SetActive(false);
            Color c = warning.color;
            c.a = 1.0f; // Reset alpha to fully visible
            warning.color = c;
        }
    }

    void UpdateUI()
    {
        if(cauldronText != null)
        {
            cauldronText.text = "Caldero " + cauldron;
            potionsText.text = "Pociones\nrestantes: " + (maxPotionsPerRound - potionsThrown);
        }
    }
}