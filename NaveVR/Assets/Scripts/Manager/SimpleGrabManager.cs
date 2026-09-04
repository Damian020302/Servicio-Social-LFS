using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SimpleGrabManager : MonoBehaviour
{
    [Header("Timer Configuration")]
    public GameObject timerPanel;
    public TextMeshProUGUI timeDisplay;
    public Toggle useTimerToggle;
    public GameObject timerControls;
    private float selectedTime = 60.0f;

    [Header("Buttons")]
    public GameObject victory;
    public GameObject yesV;
    public GameObject noV;

    [Header("UI")]
    public TextMeshProUGUI clawText;
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI stageText;

    [Header("Constant Warning")]
    public TextMeshProUGUI warning;
    public float warningBlinkSpeed = 5.0f;
    public float warningScaleMultiplier = 1.2f;
    private Coroutine warningCoroutine;
    private Vector3 warningOriginalScale;

    [Header("Hand Setup")]
    public int selectedHand;
    public OVRHand leftHand;
    public OVRHand rightHand;
    private OVRHand activeHand;
    [Tooltip("El punto donde se sujetaran los robots")]
    public Transform leftPalm;
    public Transform rightPalm;
    private Transform activePalm;
    [Header("Visual Feedback")]
    public Light leftPalmLight;
    public Light rightPalmLight;
    private Light activePalmLight;

    [Header("Grab Settings")]
    public float grabRadius = 0.15f;
    public string grabbableTag = "Grabbable";

    [Header("Calibration Data")]
    private float grabThreshold;
    private float releaseThreshold = 0.2f;
    private bool isGrabbing = false;
    private Rigidbody grabbedObject;

    [Header("Level Variables and Manager")]
    public int stage = 1;
    public int difficulty = 0;
    public int winningStreak = 0;
    private bool isVictoryAchieved = false;
    public int droppedRobots = 0;
    public RobotContainer robotContainer;
    public RobotSpawner robotSpawner;

    [Header("Timer Configuration")]
    public float timer;
    public bool timerIsRunning = false;
    public float initialTimerValue;
    private bool useTimerConfig;
    public int actualPhase = 0;//0 cuando tiene que cerrar la mano, 1 cuando tiene que abrirla

    [Header("Average Times")]
    public float averageTimeToGrab = 0.0f;
    public float averageHoldTime = 0.0f;
    public float totalRoundTime = 0.0f;
    public float initialReactionTime = 0.0f;
    public TextMeshProUGUI initialReactionTimeText;
    public TextMeshProUGUI averageTimeToGrabText;
    public TextMeshProUGUI averageHoldTimeText;
    public TextMeshProUGUI totalRoundTimeText;
    public TextMeshProUGUI totalGrabsText;
    public TextMeshProUGUI successGrabsText;
    public TextMeshProUGUI failGrabsText;
    public TextMeshProUGUI selectedArmText;
    private float totalTimeToGrab = 0.0f;
    private float totalHoldTime = 0.0f;
    private int grabCount = 0;
    private int holdCount = 0;
    private float timeRobotAppeared = 0.0f;
    private float timeRobotGrabbed = 0.0f;
    private float roundStartTime = 0.0f;

    public void IncreaseTime()
    {
        selectedTime += 30.0f;
        UpdateTimeDisplay();
    }

    public void DecreaseTime()
    {
        if(selectedTime > 30.0f)
        {
            selectedTime -= 30.0f;
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
        if(timerControls != null) timerControls.SetActive(useTimerToggle.isOn);
    }

    public void ConfirmAndStartGame()
    {
        PlayerPrefs.SetInt("UseTimer", useTimerToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("SessionTime", selectedTime);
        PlayerPrefs.Save();
        if (timerPanel != null) timerPanel.SetActive(false);
        SceneManager.LoadScene("Calibracion4");
        Time.timeScale = 1.0f;
    }

    public void MenuGeneral()
    {
        SceneManager.LoadScene("MenuGeneral");
        Time.timeScale = 1.0f;
    }

    public void MainScene()
    {
        SceneManager.LoadScene("Juego4");
        Time.timeScale = 1.0f;
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
            SceneManager.LoadScene("Calibracion4");
            Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
        }
    }

    private void Start()
    {
        selectedHand = PlayerPrefs.GetInt("SelectedHand", 1);
        if(selectedHand == 0 && leftHand != null)
        {
            activeHand = leftHand;
            activePalm = leftPalm != null ? leftPalm : leftHand.transform;
            activePalmLight = leftPalmLight;
        }
        else if(selectedHand == 1 && rightHand != null)
        {
            activeHand = rightHand;
            activePalm = rightPalm != null ? rightPalm : rightHand.transform;
            activePalmLight = rightPalmLight;
        }
        else
        {
            Debug.Log("No se encontro una mano activa");
        }
        if(leftPalmLight != null) leftPalmLight.enabled = false;
        if (rightPalmLight != null) rightPalmLight.enabled = false;
        if (SceneManager.GetActiveScene().name == "Juego4")
        {
            float maxGrabStrength = PlayerPrefs.GetFloat("MaxGrabStrength", 0.7f);
            grabThreshold = maxGrabStrength * 0.8f; // 80% of the max grab strength
            Debug.Log($"Meta de agarre: {(grabThreshold * 100):F0}% | Meta para soltar: {(releaseThreshold * 100):F0}%");
            if (warning != null)
            {
                warningOriginalScale = warning.transform.localScale;
                if (warningOriginalScale == Vector3.zero) warningOriginalScale = Vector3.one; // Default to (1,1,1) if the scale is zero
            }
            victory.SetActive(false);
            yesV.SetActive(false);
            noV.SetActive(false);
            initialTimerValue = timer;
            timerIsRunning = true;
            isVictoryAchieved = false;
            timeRobotAppeared = Time.time;
            roundStartTime = Time.time;
            totalRoundTime = 0.0f;
            UpdateUI();
            UpdateReminderMessage();
        }
        useTimerConfig = PlayerPrefs.GetInt("UseTimer", 1) == 1;
        initialTimerValue = PlayerPrefs.GetFloat("SessionTime", 60.0f);
        timer = initialTimerValue;
        timerIsRunning = useTimerConfig;
        if(!useTimerConfig && timeRemainingText != null) timeRemainingText.gameObject.SetActive(false);
    }

    float GetCurrentGrip()
    {
        float[] grips = new float[4];
        grips[0] = activeHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        grips[1] = activeHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
        grips[2] = activeHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring);
        grips[3] = activeHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky);
        System.Array.Sort(grips);
        return (grips[2] + grips[3]) / 2.0f;
    }

    void Update()
    {
        if(isVictoryAchieved) return;
        totalRoundTime = Time.time - roundStartTime;
        if(useTimerConfig && timerIsRunning)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                DisplayTime(timer);
            }
            else
            {
                timer = 0;
                timerIsRunning = false;
                isVictoryAchieved = true;
                VictoryAchieved();
            }
        }
        else if(!useTimerConfig)
        {
            if(totalRoundTimeText != null)
            {
                float minutes = Mathf.FloorToInt(totalRoundTime / 60);
                float seconds = Mathf.FloorToInt(totalRoundTime % 60);
                totalRoundTimeText.text = string.Format("Tiempo Total: {0:00}:{1:00}", minutes, seconds);
            }
        }
        if (activeHand == null || !activeHand.IsTracked) return;
        float currentGrip = GetCurrentGrip();
        if(!isGrabbing && currentGrip >= grabThreshold) TryGrabObject();
        else if(isGrabbing && currentGrip <= releaseThreshold) ReleaseObject();
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeRemainingText.text = string.Format("Tiempo: {0:00}:{1:00}", minutes, seconds);
    }

    void TryGrabObject()
    {
        if (activePalm == null) return;
        Collider[] hits = Physics.OverlapSphere(activePalm.position, grabRadius);
        float closestDistance = float.MaxValue;
        Rigidbody bestTarget = null;
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(grabbableTag))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float distance = Vector3.Distance(activePalm.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestTarget = rb;
                    }
                }
            }
        }
        if (bestTarget != null)
        {
            robotSpawner.ClearPlatform();
            isGrabbing = true;
            if (activePalmLight != null) activePalmLight.enabled = true;
            grabbedObject = bestTarget;
            grabbedObject.isKinematic = true;
            grabbedObject.transform.SetParent(activePalm);
            actualPhase = 1;
            float timeTakenToGrab = Time.time - timeRobotAppeared;
            if(grabCount == 0) initialReactionTime = timeTakenToGrab;
            totalTimeToGrab += timeTakenToGrab;
            grabCount++;
            averageTimeToGrab = totalTimeToGrab / grabCount;
            timeRobotGrabbed = Time.time;
            UpdateMetricsUI();
            UpdateReminderMessage();
        }
    }

    void ReleaseObject()
    {
        if (grabbedObject != null)
        {
            grabbedObject.transform.SetParent(null);
            grabbedObject.isKinematic = false;
            grabbedObject = null;
        }
        isGrabbing = false;
        if(activePalmLight != null) activePalmLight.enabled = false;
        actualPhase = 0;
        float holdDuration = Time.time - timeRobotGrabbed;
        totalHoldTime += holdDuration;
        holdCount++;
        averageHoldTime = totalHoldTime / holdCount;
        timeRobotAppeared = Time.time;
        UpdateMetricsUI();
        UpdateReminderMessage();
    }

    public void RegisterDroppedRobot()
    {
        droppedRobots++;
        UpdateMetricsUI();
    }

    public void UpdateMetricsUI()
    {
        if (selectedHand == 0 && leftHand != null && selectedArmText != null)
        {
            selectedArmText.text = "Brazo\nTrabajado: Izquierdo";
        }
        else if (selectedHand == 1 && rightHand != null && selectedArmText != null)
        {
            selectedArmText.text = "Brazo\nTrabajado: Derecho";
        }
        if (averageHoldTimeText != null) averageHoldTimeText.text = string.Format("Tiempo Promedio de Agarre: {0:F1}s", averageHoldTime);
        if (initialReactionTimeText != null) initialReactionTimeText.text = string.Format("Tiempo de Reacción: {0:F1}s", initialReactionTime);
        if (averageHoldTimeText != null) averageTimeToGrabText.text = string.Format("Tiempo Promedio entre Agarre: {0:F1}s", averageTimeToGrab);
        if(totalGrabsText != null)
        {
            totalGrabsText.text = string.Format("Total de Agarres: {0}", (droppedRobots + robotContainer.robotsCollected));
            successGrabsText.text = string.Format("Aciertos: {0}", robotContainer.robotsCollected);
            failGrabsText.text = string.Format("Fallos: {0}", droppedRobots);
        }
    }

    public void VictoryAchieved()
    {
        isVictoryAchieved = true;
        timerIsRunning = false;
        if(totalRoundTimeText != null)
        {
            float minutes = Mathf.FloorToInt(totalRoundTime / 60);
            float seconds = Mathf.FloorToInt(totalRoundTime % 60);
            totalRoundTimeText.text = string.Format("Tiempo Total: {0:00}:{1:00}", minutes, seconds);
        }
        StopReminder();
        victory.SetActive(true);
        yesV.SetActive(true);
        noV.SetActive(true);
        UpdateMetricsUI();
        Debug.Log("¡Victoria! Has recogido todos los robots.");
    }

    public void OnClickYesV()
    {
        stage++;
        yesV.SetActive(false);
        noV.SetActive(false);
        victory.SetActive(false);
        bool wasFast = timer >= (initialTimerValue * 0.25f) && timer > 0;
        bool goodMotorControl = droppedRobots <= 1;
        if (wasFast && goodMotorControl)
        {
            winningStreak++;
            if (winningStreak >= 3)
            {
                difficulty++;
                winningStreak = 0;
            }
        }
        else winningStreak = 0;
        ResetRound();
    }

    void ResetRound()
    {
        timer = initialTimerValue;
        timerIsRunning = useTimerConfig;
        isVictoryAchieved = false;
        droppedRobots = 0;
        totalTimeToGrab = 0.0f;
        totalHoldTime = 0.0f;
        grabCount = 0;
        holdCount = 0;
        averageTimeToGrab = 0.0f;
        averageHoldTime = 0.0f;
        initialReactionTime = 0.0f;
        timeRobotAppeared = Time.time;
        roundStartTime = Time.time;
        totalRoundTime = 0.0f;
        UpdateMetricsUI();
        if(activePalmLight != null) activePalmLight.enabled = false;
        robotContainer.ResetContainer();
        if(robotSpawner != null)
        {
            robotSpawner.ClearCurrentRobot();
            robotSpawner.SpawnRandomRobot();
        }
        UpdateUI();
        UpdateReminderMessage();
    }

    void UpdateReminderMessage()
    {
        StopReminder();
        if(actualPhase == 0) StartReminder("Cierra tu puño para agarrar al robot");
        else StartReminder("Abre tu puño para soltar al robot en el contenedor <color=red>rojo</color>");
    }

    void UpdateUI()
    {
        if(stageText != null) stageText.text = "Stage " + stage;
    }

    IEnumerator WarningAnimationRoutine(string message)
    {
        if(warning == null) yield break;
        warning.text = message;
        warning.gameObject.SetActive(true);
        Color originalColor = warning.color;
        float time = 0.0f;
        while(true)
        {
            time += Time.deltaTime * warningBlinkSpeed;
            float alpha = (Mathf.Sin(time) + 1.0f) / 2.0f;
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
        if(warningCoroutine != null) StopCoroutine(warningCoroutine);
        warningCoroutine = StartCoroutine(WarningAnimationRoutine(message));
    }

    public void StopReminder()
    {
        if(warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
            warningCoroutine = null;
        }
        if(warning != null)
        {
            warning.gameObject.SetActive(false);
            Color c = warning.color;
            c.a = 1.0f; // Reset alpha to fully opaque
            warning.color = c;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(activePalm != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(activePalm.position, grabRadius);
        }
    }
}