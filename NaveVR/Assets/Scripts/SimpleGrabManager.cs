using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleGrabManager : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject victory;
    public GameObject defeat;
    public GameObject yesD;
    public GameObject noD;
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
    public OVRHand leftHand;
    public OVRHand rightHand;
    private OVRHand activeHand;
    [Tooltip("El punto donde se sujetaran los robots")]
    public Transform leftPalm;
    public Transform rightPalm;
    private Transform activePalm;

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
    public RobotContainer robotContainer;
    public RobotSpawner robotSpawner;

    [Header("Timer Settings")]
    public float timer = 60.0f;
    public bool timerIsRunning = false;
    public float initialTimerValue;
    public int actualPhase = 0;//0 cuando tiene que cerrar la mano, 1 cuando tiene que abrirla

    public void OnClickNo()
    {
        SceneManager.LoadScene("Menu4");
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
        SceneManager.LoadScene("Calibracion4");
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 1);
        if(selectedHand == 0 && leftHand != null)
        {
            activeHand = leftHand;
            activePalm = leftPalm != null ? leftPalm : leftHand.transform;
        }
        else if(selectedHand == 1 && rightHand != null)
        {
            activeHand = rightHand;
            activePalm = rightPalm != null ? rightPalm : rightHand.transform;
        }
        else
        {
            Debug.Log("No se encontro una mano activa");
        }

        float maxGrabStrength = PlayerPrefs.GetFloat("MaxGrabStrength", 0.7f);
        grabThreshold = maxGrabStrength * 0.8f; // 80% of the max grab strength
        Debug.Log($"Meta de agarre: {(grabThreshold * 100):F0}% | Meta para soltar: {(releaseThreshold * 100):F0}%");
        if(warning != null)
        {
            warningOriginalScale = warning.transform.localScale;
            if(warningOriginalScale == Vector3.zero)
            {
                warningOriginalScale = Vector3.one; // Default to (1,1,1) if the scale is zero
            }
        }
        victory.SetActive(false);
        defeat.SetActive(false);
        yesV.SetActive(false);
        yesD.SetActive(false);
        noV.SetActive(false);
        noD.SetActive(false);
        initialTimerValue = timer;
        timerIsRunning = true;
        isVictoryAchieved = false;
        UpdateUI();
        UpdateReminderMessage();
    }

    float GetCurrentGrip()
    {
        float[] grips = new float[4];
        grips[0] = activeHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
        grips[1] = activeHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
        grips[2] = activeHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring);
        grips[3] = activeHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky);
        System.Array.Sort(grips);
        //float thumbGrip = activeHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb);
        //return Mathf.Max(indexGrip, middleGrip, ringGrip, pinkyGrip/*, thumbGrip*/);
        return (grips[2] + grips[3]) / 2.0f;
    }

    void Update()
    {
        if(isVictoryAchieved) return;
        if(timerIsRunning)
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
                DefeatAchieved();
            }
        }
        if (activeHand == null || !activeHand.IsTracked) return;
        float currentGrip = GetCurrentGrip();
        if(!isGrabbing && currentGrip >= grabThreshold)
        {
            TryGrabObject();
        }
        else if(isGrabbing && currentGrip <= releaseThreshold)
        {
            ReleaseObject();
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeRemainingText.text = "Tiempo restante: " + string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TryGrabObject()
    {
        if (activePalm == null) return;
        Collider[] hits = Physics.OverlapSphere(activePalm.position, grabRadius);
        float closestDistance = float.MaxValue;
        Rigidbody bestTarget = null;
        /*if (hits.Length > 0)
        {
            Debug.Log($"Cerraste el puño, objeto dentro de los 15 cm: {hits.Length}");
        }*/
        foreach (Collider hit in hits)
        {
            //Debug.Log($"Objeto detectado: {hit.gameObject.name} con tag {hit.tag}");
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
                /*else
                {
                    Debug.LogWarning($"El objeto {hit.name} tiene el tag {grabbableTag} pero no tiene un Rigidbody.");
                }*/
            }
        }
        if (bestTarget != null)
        {
            robotSpawner.ClearPlatform();
            isGrabbing = true;
            grabbedObject = bestTarget;
            grabbedObject.isKinematic = true;
            grabbedObject.transform.SetParent(activePalm);
            actualPhase = 1;
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
        actualPhase = 0;
        UpdateReminderMessage();
    }

    public void VictoryAchieved()
    {
        isVictoryAchieved = true;
        timerIsRunning = false;
        StopReminder();
        victory.SetActive(true);
        yesV.SetActive(true);
        noV.SetActive(true);
        Debug.Log("¡Victoria! Has recogido todos los robots.");
    }

    void DefeatAchieved()
    {
        isVictoryAchieved = false;
        timerIsRunning = false;
        StopReminder();
        yesD.SetActive(true);
        noD.SetActive(true);
        defeat.SetActive(true);
        Debug.Log("¡Derrota! No has recogido todos los robots.");
    }
   
    public void OnClickYesD()
    {
        yesD.SetActive(false);
        noD.SetActive(false);
        defeat.SetActive(false);
        winningStreak = 0;
        difficulty--;
        ResetRound();
    }

    public void OnClickYesV()
    {
        stage++;
        yesV.SetActive(false);
        noV.SetActive(false);
        victory.SetActive(false);
        if (timer >= (initialTimerValue * 0.5f))
        {
            winningStreak++;
            if (winningStreak >= 3)
            {
                difficulty++;
                winningStreak = 0;
            }
        }
        else
        {
            winningStreak = 0;
        }
        ResetRound();
    }

    void ResetRound()
    {
        timer = initialTimerValue;
        timerIsRunning = true;
        isVictoryAchieved = false;
        robotContainer.ResetContainer();
        if(robotSpawner != null)
        {
            robotSpawner.ClearCurrentRobot();
            robotSpawner.SpawnSingleRobot();
        }
        UpdateUI();
        UpdateReminderMessage();
    }

    void UpdateReminderMessage()
    {
        StopReminder();
        if(actualPhase == 0)
        {
            StartReminder("Cierra tu puño para agarrar al robot");
        }
        else
        {
            StartReminder("Abre tu puño para soltar al robot en el contenedor <color=red>rojo</color>");
        }
    }

    void UpdateUI()
    {
        if(stageText != null)
        {
            stageText.text = "Stage: " + stage;
        }
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
        if(warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
        }
        /*if(warningOriginalScale == Vector3.zero)
        {
            warningOriginalScale = Vector3.one; // Default to (1,1,1) if the scale is zero
        }*/
        
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
            //warningOriginalScale = warning.transform.localScale;
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
