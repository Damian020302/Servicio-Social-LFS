using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CauldronManager : MonoBehaviour
{
    [Header("Wrist tracking")]
    public Transform leftWrist;
    public Transform rightWrist;
    private Transform activeWrist;

    [Header("Buttons")]
    public GameObject victory;
    public GameObject defeat;
    public GameObject yesD;
    public GameObject noD;
    public GameObject yesV;
    public GameObject noV;

    [Header("UI")]
    public TextMeshProUGUI cauldronText;
    public TextMeshProUGUI timeRemainingText;

    [Header("Flexion/Extension Thresholds")]
    public float flexionThreshold = 30.0f; // Ángulo mínimo para considerar una flexión
    public float extensionThreshold = 30.0f; // Ángulo mínimo para considerar una extensión

    [Header("Wrist Flexion/Extension Counter")]
    public float flexionAngle = 0.0f;
    public float extensionAngle = 0.0f;
    private Quaternion neutralRotation;
    private int completeExercises = 0;
    private bool flexionCompleted = false;
    private bool extensionCompleted = false;

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
    public float timer = 60.0f; // Tiempo en segundos para completar el nivel
    public bool timerIsRunning = false;
    public float initialTimerValue;

    public int basePotionsNeeded = 1;
    private int currentPotionsNeeded;
    private int potionsThrown = 0;

    public void OnClickNo()
    {
        SceneManager.LoadScene("Menu3");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
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
    void Start()
    {
        initialTimerValue = timer;
        yesV.SetActive(false);
        noV.SetActive(false);
        yesD.SetActive(false);
        noD.SetActive(false);
        victory.SetActive(false);
        defeat.SetActive(false);
        //timerIsRunning = true;
        //neutralRotation = transform.rotation;
        if (warning != null)
        {
            warningOriginalScale = warning.transform.localScale;
            if(warningOriginalScale == Vector3.zero)
            {
                warningOriginalScale = Vector3.one;
            }
        }
        StartReminder();
        LevelConfig();
        UpdateActiveWrist();
        if(activeWrist != null)
        {
            neutralRotation = activeWrist.rotation;
        }
        else
        {
            Debug.LogError("No se ha asignado ninguna muñeca activa. Por favor, asigna una muñeca en el inspector.");
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
        currentPotionsNeeded = basePotionsNeeded + difficulty;
        potionsThrown = 0;
        timer = initialTimerValue;
        timerIsRunning = true;
        isVictoryAchieved = false;
        flexionCompleted = false;
        extensionCompleted = false;
        UpdateUI();
    }

    void DefeatAchieved()
    {
        StopReminder();
        yesD.SetActive(true);
        noD.SetActive(true);
        defeat.SetActive(true);
        Debug.Log("Defeat logic executed.");
    }

    // Update is called once per frame
    void Update()
    {
        if(isVictoryAchieved)
        {
            return; // Evita que se ejecute el código de actualización si ya se ha logrado la victoria
        }
        UpdateActiveWrist();
        if (activeWrist == null)
        {
            return; // Evita que se ejecute el código de actualización si no hay una muñeca activa asignada00
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
        if(flexionAngle >= flexionThreshold/* && !flexionCompleted*/)
        {
            flexionCompleted = true;
            Debug.Log("Flexión completa");
        }
        else if(extensionAngle >= extensionThreshold/* && !extensionCompleted*/)
        {
            extensionCompleted = true;
            Debug.Log("Extensión completa");
        }
        if(flexionCompleted && extensionCompleted)
        {
            if(currentPotion == null)
            {
                completeExercises++;
                Debug.Log($"Ejercicio completo #{completeExercises}");
                flexionCompleted = false;
                extensionCompleted = false;
                ThrowPotion();
            }
            else
            {
                flexionCompleted = false;
                extensionCompleted = false;
            }
            
        }

        if (timerIsRunning)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
                DisplayTime(timer);
                //UpdateUI();
            }
            else
            {
                timer = 0;
                timerIsRunning = false;
                DefeatAchieved();
                Debug.Log("Defeat achieved. Time has run out.");
            }
        }
        Debug.Log($"Flexion: {flexionAngle:F1}° | Extension: {extensionAngle:F1}°");
    }

    void ThrowPotion()
    { 
        if(prefabPotion != null && spawnPoint != null && cauldronEndpoint != null)
        {
            currentPotion = Instantiate(prefabPotion, spawnPoint.position, Quaternion.identity);
            //GameObject potion = Instantiate(prefabPotion, spawnPoint.position, Quaternion.identity);
            //Parabola parabola = potion.AddComponent<Parabola>();
            Parabola parabola = currentPotion.GetComponent<Parabola>();
            parabola.Launch(spawnPoint.position, cauldronEndpoint.position, launchSpeed, launchHeight);
            StartCoroutine(CheckPotionLanded(currentPotion));
        }        
    }

    IEnumerator CheckPotionLanded(GameObject potion)
    {
        while (potion != null)
        {
            yield return null;
        }
        if(!isVictoryAchieved && timerIsRunning)
        {
            potionsThrown++;
            Debug.Log($"Poción lanzada #{potionsThrown}");
            UpdateUI();
            if (potionsThrown >= currentPotionsNeeded)
            {
                isVictoryAchieved = true;
                timerIsRunning = false;
                VictoryAchieved();
            }
        }
    }

    public void RecalibrateNeutral()
    {
        UpdateActiveWrist();
        if(activeWrist != null)
        {
            neutralRotation = transform.rotation;
            flexionCompleted = false;
            extensionCompleted = false;
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
    
    public void OnClickYesD()
    {
        StartReminder();
        yesD.SetActive(false);
        noD.SetActive(false);
        defeat.SetActive(false);
        LevelConfig();
        //timerIsRunning = true;
        //isVictoryAchieved = false;
        winningStreak = 0;
        difficulty--;
        //UpdateUI();
    }

    public void OnClickYesV()
    {
        StartReminder();
        cauldron++;
        yesV.SetActive(false);
        noV.SetActive(false);
        victory.SetActive(false);
        if(timer >= (initialTimerValue * 0.5f))
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

    public void StartReminder()
    {
        if (warningCoroutine != null) StopCoroutine(warningCoroutine);
        warningCoroutine = StartCoroutine(WarningAnimationRoutine("Flexiona y extiende tu muñeca para lanzar el ingrediente"));
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
        }
    }
}