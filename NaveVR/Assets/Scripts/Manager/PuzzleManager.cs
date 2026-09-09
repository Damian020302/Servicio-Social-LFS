using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Timer Configuration")]
    public GameObject timerPanel;
    public TextMeshProUGUI timeDisplay;
    public Toggle useTimerToggle;
    public GameObject timerControls;
    private float selectedTime = 60.0f;

    [Header("Buttons")]
    public GameObject victoria;
    public GameObject yesV;
    public GameObject noV;

    [Header("UI")]
    public TextMeshProUGUI dungeonText;
    public TextMeshProUGUI timeRemainingText;

    [Header("Constant Warning")]
    public TextMeshProUGUI warning;
    public float warningBlinkSpeed = 5.0f;
    public float warningScaleMultiplier = 1.2f;
    private Coroutine warningCoroutine;
    private Vector3 warningOriginalScale;

    [Header("Puzzle Administrator")]
    public GameObject puzzleAdmin;

    [Header("Level Variables")]
    public int dungeon = 1;
    public int currentPuzzleIndex = 0;
    private int totalPuzzles = 0;
    //public int difficulty = 0; // Variable to track the current difficulty level
    //public int winningStreak = 0;
    private bool isVictoryAchieved = false; // Flag to track if victory has been achieved

    [Header("Timer Variables")]
    public float timer = 60.0f; // Timer for defeat condition (if needed)
    public bool timerIsRunning = false; // Flag to track if the timer is running
    public float initialTimerValue;
    private bool useTimerConfig;
    [Header("Puzzle Setup")]
    public float predefinedScrambleAngle = 120.0f;
    [Header("Victory configuration")]
    [Tooltip("Margin of error")]
    public float victoryMargin = 20.0f; // Margin of error for victory condition
    [Header("Puzzle Lists")]
    private Dictionary<Collider, Quaternion> perfectRotations = new Dictionary<Collider, Quaternion>();
    [Header("Phases")]
    private List<Collider[]> puzzlePhases = new List<Collider[]>(); // Dictionary to store puzzles for each phase
    public int actualPhase = 0; // Variable to track the current phase of the puzzle

    [Header("Door Manager")]
    public DoorManager doorManager; // Reference to the DoorManager script

    [Header("Average Times")]
    public float totalRoundTime = 0.0f;
    public float initialReactionTime = 0.0f;
    public float averageRotationTime = 0.0f;
    public float averageSolvingTime = 0.0f;
    public float averageRotationAngle = 0.0f;
    public float maxRotationAngle = 0.0f;
    public TextMeshProUGUI totalRoundTimeText;
    public TextMeshProUGUI initialReactionTimeText;
    public TextMeshProUGUI averageRotationTimeText;
    public TextMeshProUGUI averageSolvingTimeText;
    public TextMeshProUGUI averageRotationAngleText;
    public TextMeshProUGUI maxRotationAngleText;

    private float roundStartTime = 0.0f;
    private float phaseStartTime = 0.0f;
    private bool hasReacted = false;
    private int totalPhasesSolved = 0;
    private float totalPhaseSolvingTime = 0.0f;
    private int totalPiecesSolved = 0;
    private float totalPiecesRotationTime = 0.0f;
    private float totalPiecesRotationAngle = 0.0f;
    private Dictionary<Collider, Quaternion> scrambledRotations = new Dictionary<Collider, Quaternion>();
    private Dictionary<Collider, Quaternion> lastFrameRotations = new Dictionary<Collider, Quaternion>();
    private Dictionary<Collider, float> pieceActiveTime = new Dictionary<Collider, float>();

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
        SceneManager.LoadScene("Calibracion2");
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
        if (timerPanel != null)
        {
            timerPanel.SetActive(true);
            UpdateTimeDisplay();
            if (timerControls != null) timerControls.SetActive(useTimerToggle.isOn);
        }
        else
        {
            SceneManager.LoadScene("Calibracion2");
            Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
        }
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Juego2")
        {
            totalPuzzles = puzzleAdmin.transform.childCount;
            Collider[] allPieces = puzzleAdmin.GetComponentsInChildren<Collider>(true); // Get all colliders from the puzzle pieces
            foreach (Collider piece in allPieces)
            {
                perfectRotations.Add(piece, piece.transform.rotation); // Store the initial rotation as the perfect rotation for each piece
            }
            StartUI();
            currentPuzzleIndex = 0;
            LoadCurrentPuzzle();
            //DinamicPuzzle();
            if (warning != null)
            {
                warningOriginalScale = warning.transform.localScale;
                if (warningOriginalScale == Vector3.zero) warningOriginalScale = Vector3.one; // Fallback to a default scale if the original scale is not set
            }
            UpdateReminderMessage();
            isVictoryAchieved = false;
        }
        useTimerConfig = PlayerPrefs.GetInt("UseTimer", 1) == 1;
        initialTimerValue = PlayerPrefs.GetFloat("SessionTime", 60.0f);
        timer = initialTimerValue;
        timerIsRunning = useTimerConfig;
        if (!useTimerConfig && timeRemainingText != null)timeRemainingText.gameObject.SetActive(false);
    }

    void StartUI()
    {
        yesV.SetActive(false); // Ensure the yes object is initially inactive
        noV.SetActive(false);
        victoria.SetActive(false); // Ensure the victory object is initially inactive
    }

    void LoadCurrentPuzzle()
    {
        GameObject activePuzzle = null;
        for(int i = 0; i < totalPuzzles; i++)
        {
            bool isSelected = (i == currentPuzzleIndex);
            puzzleAdmin.transform.GetChild(i).gameObject.SetActive(isSelected);
            if(isSelected) activePuzzle = puzzleAdmin.transform.GetChild(i).gameObject;
        }
        puzzlePhases.Clear();
        actualPhase = 0;
        if(activePuzzle == null) return;
        int phases = activePuzzle.transform.childCount;
        for(int j = 0; j < phases; j++)
        {
            Transform nPuzzle = activePuzzle.transform.GetChild(j);
            int piecesInPhase = nPuzzle.childCount;
            Collider[] phasePieces = new Collider[piecesInPhase];
            for (int k = 0; k < piecesInPhase; k++)
            {
                Collider pieceCollider = nPuzzle.GetChild(k).GetComponent<Collider>();
                phasePieces[k] = pieceCollider; // Store the collider for each piece in the current phase
                pieceCollider.gameObject.SetActive(true); // Ensure the piece is active in the scene
                Rigidbody rb = pieceCollider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false; // Make the pieces non-physical to prevent them from falling or being interacted with
                    rb.angularVelocity = Vector3.zero; // Stop any existing angular velocity to prevent pieces from spinning
                    rb.angularDamping = 15.0f; // Apply angular damping to gradually stop rotation
                }
                pieceCollider.transform.rotation = perfectRotations[pieceCollider]; // Reset the piece to its perfect rotation
                float randomRotation = Random.Range(60.0f, 300.0f); // Generate a random Y rotation
                pieceCollider.transform.Rotate(0, 0, randomRotation, Space.Self); // Apply the random rotation
                pieceCollider.enabled = (j == 0); // Enable only the pieces of the first phase
                RotatePuzzle scriptRotation = pieceCollider.GetComponent<RotatePuzzle>();
                if (scriptRotation != null) scriptRotation.enabled = (j == 0); // Enable the RotatePuzzle script only for the pieces of the first phase
            }
            puzzlePhases.Add(phasePieces); // Add the current phase pieces to the list of phases
        }
        StartPhaseMetrics();
        UpdateReminderMessage();
    }

    void StartPhaseMetrics()
    {
        if (puzzlePhases.Count == 0) return;
        if(currentPuzzleIndex == 0 && actualPhase == 0)
        {
            roundStartTime = Time.time;
            hasReacted = false;
        }
        phaseStartTime = Time.time;
        foreach(Collider piece in puzzlePhases[actualPhase])
        {
            scrambledRotations[piece] = piece.transform.rotation;
            lastFrameRotations[piece] = piece.transform.rotation;
            pieceActiveTime[piece] = 0.0f;
        }
    }

    public void Update()
    {
        if (isVictoryAchieved) return;
        totalRoundTime = Time.time - roundStartTime;
        if (useTimerConfig && timerIsRunning)
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
                isVictoryAchieved = true;
                VictoryAchieved();
            }
        }
        else if (!useTimerConfig)
        {
            if (totalRoundTimeText != null)
            {
                float minutes = Mathf.FloorToInt(totalRoundTime / 60);
                float seconds = Mathf.FloorToInt(totalRoundTime % 60);
                totalRoundTimeText.text = string.Format("Tiempo total: {0:00}:{1:00}", minutes, seconds);
            }
        }
        if (puzzlePhases.Count == 0) return;
        Collider[] currentPhasePieces = puzzlePhases[actualPhase]; // Get the pieces for the current phase
        int correctPiecesInPhase = 0;
        foreach (Collider piece in currentPhasePieces)
        {
            if (piece == null) continue;
            RotatePuzzle scriptRotation = piece.GetComponent<RotatePuzzle>();
            bool isAlreadyLocked = (scriptRotation != null && !scriptRotation.enabled);
            if (isAlreadyLocked)
            {
                correctPiecesInPhase++;
                continue;
            }
            if (!hasReacted)
            {
                if (Quaternion.Angle(piece.transform.rotation, scrambledRotations[piece]) > 2.0f)
                {
                    initialReactionTime = Time.time - roundStartTime;
                    hasReacted = true;
                    UpdateMetricsUI();
                }
            }
            float frameDelta = Quaternion.Angle(piece.transform.rotation, lastFrameRotations[piece]);
            if (frameDelta > 0.05f)
            {
                pieceActiveTime[piece] += Time.deltaTime;
                float realPhysicalDelta = frameDelta;
                float currentDisplacement = Quaternion.Angle(piece.transform.rotation, scrambledRotations[piece]);
                float realPhysicalDisplacement = currentDisplacement;
                if (scriptRotation != null && scriptRotation.rotationMultiplier > 0)
                {
                    realPhysicalDelta /= scriptRotation.rotationMultiplier;
                    realPhysicalDisplacement /= scriptRotation.rotationMultiplier;
                }
                totalPiecesRotationAngle += realPhysicalDelta;
                if (realPhysicalDisplacement > maxRotationAngle) maxRotationAngle = realPhysicalDisplacement;
            }
            Quaternion targetRot = perfectRotations[piece];
            float angleDifference = Quaternion.Angle(piece.transform.rotation, targetRot);
            if (angleDifference <= victoryMargin)
            {
                piece.transform.rotation = targetRot;
                Rigidbody rb = piece.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                if (scriptRotation != null)
                {
                    scriptRotation.ToggleLights(true);
                    scriptRotation.enabled = false;
                }
                totalPiecesSolved++;
                totalPiecesRotationTime += pieceActiveTime[piece];
                correctPiecesInPhase++;
                UpdateMetricsUI();
            }
            else
            {
                if (scriptRotation != null) scriptRotation.ToggleLights(false);
            }
            lastFrameRotations[piece] = piece.transform.rotation;
        }
        if (doorManager != null && totalPuzzles > 0)
        {
            float baseProgress = (float)currentPuzzleIndex / totalPuzzles;
            float currentPuzzleProgress = ((float)correctPiecesInPhase / currentPhasePieces.Length) / totalPuzzles;
            doorManager.UpdateOpening(baseProgress + currentPuzzleProgress);
        }
        if (correctPiecesInPhase == currentPhasePieces.Length)
        {
            float phaseTime = Time.time - phaseStartTime;
            totalPhaseSolvingTime += phaseTime;
            totalPhasesSolved++;
            averageSolvingTime = totalPhaseSolvingTime / totalPhasesSolved;
            if (totalPiecesSolved > 0)
            {
                averageRotationTime = totalPiecesRotationTime / totalPiecesSolved;
                averageRotationAngle = totalPiecesRotationAngle / totalPiecesSolved;
            }
            UpdateMetricsUI();
            actualPhase++;
            if (actualPhase >= puzzlePhases.Count)
            {
                currentPuzzleIndex++;
                if (currentPuzzleIndex >= totalPuzzles)
                {
                    isVictoryAchieved = true;
                    timerIsRunning = false;
                    VictoryAchieved();
                }
                else LoadCurrentPuzzle();
            }
            else
            {
                Collider[] nextPhasePieces = puzzlePhases[actualPhase];
                foreach (Collider piece in nextPhasePieces)
                {
                    if (piece != null)
                    {
                        piece.enabled = true;
                        RotatePuzzle sr = piece.GetComponent<RotatePuzzle>();
                        if (sr != null) sr.enabled = true;
                    }
                }
                StartPhaseMetrics();
                UpdateReminderMessage();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1; // Add 1 second to account for the timer reaching 0
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); // Calculate minutes
        float seconds = Mathf.FloorToInt(timeToDisplay % 60); // Calculate seconds
        timeRemainingText.text = "Tiempo restante: " + string.Format("{0:00}:{1:00}", minutes, seconds); // Update the UI text with formatted time
    }

    void UpdateMetricsUI()
    {
        if(initialReactionTimeText != null) initialReactionTimeText.text = string.Format("Tiempo de\nReacción: {0:F1}s", initialReactionTime);
        if(averageRotationTimeText != null) averageRotationTimeText.text = string.Format("Tiempo Promedio\nde Rotación: {0:F1}s", averageRotationTime);
        if(averageSolvingTimeText != null) averageSolvingTimeText.text = string.Format("Tiempo Promedio\nde Resolución: {0:F1}s", averageSolvingTime);
        if(averageRotationAngleText != null) averageRotationAngleText.text = string.Format("Ángulo Promedio\nde Rotación: {0:F1}º", averageRotationAngle);
        if(maxRotationAngleText != null) maxRotationAngleText.text = string.Format("Ánglo Máximo\nde Rotación: {0:F1}º", maxRotationAngle);
    }

    void VictoryAchieved()
    {
        isVictoryAchieved = true;
        timerIsRunning = false;
        EndUI();
        if(totalRoundTimeText != null)
        {
            float finalTimeToShow = useTimerConfig ? initialTimerValue : totalRoundTime;
            float minutes = Mathf.FloorToInt(finalTimeToShow / 60);
            float seconds = Mathf.FloorToInt(finalTimeToShow % 60);
            totalRoundTimeText.text = string.Format("Tiempo Total: {0:00}:{1:00}", minutes, seconds);
        }
        if(doorManager != null)
        {
            StopReminder(); // Stop any active reminders when victory is achieved
            doorManager.UpdateOpening(1.0f); // Ensure the door is fully open on victory
        }
        UpdateMetricsUI();
        Debug.Log("Victory logic executed.");
    }

    void EndUI()
    {
        yesV.SetActive(true); // Activate the yes object
        noV.SetActive(true); // Activate the no object
        victoria.SetActive(true); // Activate the victory object
    }

    public void OnClickYes()
    {
        dungeon++;
        StartUI();
        timer = initialTimerValue;
        timerIsRunning = useTimerConfig;
        isVictoryAchieved = false;
        currentPuzzleIndex = 0;
        totalRoundTime = 0.0f;
        initialReactionTime = 0.0f;
        averageRotationTime = 0.0f;
        averageSolvingTime = 0.0f;
        averageRotationAngle = 0.0f;
        maxRotationAngle = 0.0f;
        hasReacted = false;
        totalPhasesSolved = 0;
        totalPhaseSolvingTime = 0.0f;
        totalPiecesSolved = 0;
        totalPiecesRotationTime = 0.0f;
        totalPiecesRotationAngle = 0.0f;
        if(doorManager != null)
        {
            doorManager.UpdateOpening(0.0f); // Reset the door to closed position
        }
        LoadCurrentPuzzle();
        Debug.Log("Avanzando al calabozo " + dungeon + " después de la victoria.");
        UpdateUI(); // Update the UI to reflect the new dungeon level*/
        UpdateMetricsUI();
        UpdateReminderMessage();
    }

    void UpdateReminderMessage()
    {
        StopReminder();
        if(actualPhase == 0)
        {
            if (currentPuzzleIndex > 0) StartReminder("Rota tu muñeca en las runas interiores"); 
            else StartReminder("Rota tu muñeca en la runa interior");
        }
        else
        {
            if (currentPuzzleIndex > 0) StartReminder("Rota tu muñeca en las runas exteriores");
            else StartReminder("Rota tu muñeca en la runa exterior");
        }
    }

    void UpdateUI()
    {
        if(dungeonText != null) dungeonText.text = "Catacumba " + dungeon; // Update the dungeon level text
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
}