using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [Header("Puzzle Pieces")]
    private System.Collections.Generic.Dictionary<Collider, Quaternion> perfectRotations = new System.Collections.Generic.Dictionary<Collider, Quaternion>(); // Dictionary to store perfect rotations for each piece
    [Header("Phases")]
    private System.Collections.Generic.List<Collider[]> puzzlePhases = new System.Collections.Generic.List<Collider[]>(); // Dictionary to store puzzles for each phase
    public int actualPhase = 0; // Variable to track the current phase of the puzzle

    [Header("Victory configuration")]
    [Tooltip("Margin of error")]
    public float victoryMargin = 20.0f; // Margin of error for victory condition

    [Header("Door Manager")]
    public DoorManager doorManager; // Reference to the DoorManager script

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
            if (timerControls != null)
            {
                timerControls.SetActive(useTimerToggle.isOn);
            }
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
            yesV.SetActive(false); // Ensure the yes object is initially inactive
            noV.SetActive(false);
            victoria.SetActive(false); // Ensure the victory object is initially inactive
            currentPuzzleIndex = 0;
            LoadCurrentPuzzle();
            //DinamicPuzzle();
            if (warning != null)
            {
                warningOriginalScale = warning.transform.localScale;
                if (warningOriginalScale == Vector3.zero)
                {
                    warningOriginalScale = Vector3.one; // Fallback to a default scale if the original scale is not set
                }
            }
            UpdateReminderMessage();
            isVictoryAchieved = false;
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

    void LoadCurrentPuzzle()
    {
        GameObject activePuzzle = null;
        for(int i = 0; i < totalPuzzles; i++)
        {
            bool isSelected = (i == currentPuzzleIndex);
            puzzleAdmin.transform.GetChild(i).gameObject.SetActive(isSelected);
            if(isSelected)
            {
                activePuzzle = puzzleAdmin.transform.GetChild(i).gameObject;
            }
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
                if (scriptRotation != null)
                {
                    scriptRotation.enabled = (j == 0); // Enable the RotatePuzzle script only for the pieces of the first phase
                }
            }
            puzzlePhases.Add(phasePieces); // Add the current phase pieces to the list of phases
        }
        UpdateReminderMessage();
    }

    /*void DinamicPuzzle()
    {
        difficulty = Mathf.Clamp(difficulty, 0, puzzleAdmin.transform.childCount - 1); // Increase difficulty every 3 dungeons
        GameObject activePuzzle = null;
        for(int i = 0; i < puzzleAdmin.transform.childCount; i++)
        {
            bool isSelected = (i == difficulty); // Select the puzzle based on the current difficulty level
            puzzleAdmin.transform.GetChild(i).gameObject.SetActive(isSelected); // Activate the selected puzzle and deactivate others
            if(isSelected)
            {
                activePuzzle = puzzleAdmin.transform.GetChild(i).gameObject; // Store reference to the active puzzle
            }
        }
        puzzlePhases.Clear(); // Clear any existing phases
        actualPhase = 0; // Reset to the first phase
        int phases = activePuzzle.transform.childCount;
        for (int f = 0; f < phases; f++)
        {
            Transform nPuzzle = activePuzzle.transform.GetChild(f);
            int piecesInPhase = nPuzzle.childCount;
            Collider[] phasePieces = new Collider[piecesInPhase];
            for(int p = 0; p < piecesInPhase; p++)
            {
                Collider pieceCollider = nPuzzle.GetChild(p).GetComponent<Collider>();
                phasePieces[p] = pieceCollider; // Store the collider for each piece in the current phase
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
                pieceCollider.enabled = (f == 0); // Enable only the pieces of the first phase
                RotatePuzzle scriptRotation = pieceCollider.GetComponent<RotatePuzzle>();
                if (scriptRotation != null)
                {
                    scriptRotation.enabled = (f == 0); // Enable the RotatePuzzle script only for the pieces of the first phase
                }
            }
            puzzlePhases.Add(phasePieces); // Add the current phase pieces to the list of phases
        }
    }*/

    public void Update()
    {
        if(isVictoryAchieved)
        {
            return; // Exit if victory has already been achieved
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
        Collider[] currentPhasePieces = puzzlePhases[actualPhase]; // Get the pieces for the current phase
        int correctPieces = GetCorrectPiecesCount(currentPhasePieces);
        if(doorManager != null && totalPuzzles > 0)
        {
            int totalPiecesInCurrentPuzzle = 0;
            int correctPiecesInCurrentPuzzle = 0;
            foreach (Collider[] phase in puzzlePhases)
            {
                totalPiecesInCurrentPuzzle += phase.Length;
                correctPiecesInCurrentPuzzle += GetCorrectPiecesCount(phase);
            }
            if(totalPiecesInCurrentPuzzle > 0)
            {
                float baseProgress = (float)currentPuzzleIndex / totalPuzzles;
                float currentPuzzleProgress = ((float)correctPiecesInCurrentPuzzle / totalPiecesInCurrentPuzzle) / totalPuzzles;
                doorManager.UpdateOpening(baseProgress + currentPuzzleProgress);
            }
            /*int totalLevelPieces = 0;
            int totalCorrectPieces = 0;
            foreach (Collider[] phase in puzzlePhases)
            {
                totalLevelPieces += phase.Length; // Count total pieces across all phases
                totalCorrectPieces += GetCorrectPiecesCount(phase); // Count total correct pieces across all phases
            }
            if(totalLevelPieces > 0)
            {
                float progress = (float)totalCorrectPieces / totalLevelPieces; // Calculate overall progress as a percentage
                doorManager.UpdateOpening(progress); // Update the door opening based on overall progress
            }*/
        }
        if (correctPieces == currentPhasePieces.Length)
        {
            foreach (Collider piece in currentPhasePieces)
            {
                if (piece != null)
                {
                    piece.enabled = false; // Disable the colliders for the current phase pieces
                    Rigidbody rb = piece.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.angularVelocity = Vector3.zero; // Stop any existing angular velocity to prevent pieces from spinning
                        rb.isKinematic = true; // Make the pieces non-physical to prevent them from falling or being interacted with
                    }
                    RotatePuzzle scriptRotation = piece.GetComponent<RotatePuzzle>();
                    if (scriptRotation != null)
                    {
                        scriptRotation.enabled = false; // Disable the RotatePuzzle script to prevent further interaction
                    }
                    piece.gameObject.SetActive(false); // Deactivate the pieces of the current phase to prevent further interaction
                    piece.gameObject.SetActive(true); // Reactivate the pieces to ensure they remain visible but non-interactive
                }
            }
            actualPhase++; // Move to the next phase
            if (actualPhase >= puzzlePhases.Count)
            {
                currentPuzzleIndex++;
                if(currentPuzzleIndex >= totalPuzzles)
                {
                    Debug.Log("All pieces are correctly aligned! Checking for victory condition...");
                    isVictoryAchieved = true; // Set victory flag to true
                    timerIsRunning = false;
                    VictoryAchieved();
                    Debug.Log("Victory Achieved! All pieces are aligned.");
                }
                else
                {
                    LoadCurrentPuzzle();
                }
            }
            else
            {
                Collider[] nextPhasePieces = puzzlePhases[actualPhase]; // Get the pieces for the next phase
                foreach (Collider piece in nextPhasePieces)
                {
                    if (piece != null)
                    {
                        piece.enabled = true; // Enable the colliders for the next phase pieces
                        RotatePuzzle scriptRotation = piece.GetComponent<RotatePuzzle>();
                        if (scriptRotation != null)
                        {
                            scriptRotation.enabled = true; // Enable the RotatePuzzle script to allow interaction
                        }
                    }
                }
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

    private int GetCorrectPiecesCount(Collider[] pieces)
    {
        int count = 0;
        for(int i = 0; i < pieces.Length; i++)
        {
            if(pieces[i] != null)
            {
                Quaternion target = perfectRotations[pieces[i]];
                float angleDifference = Quaternion.Angle(pieces[i].transform.rotation, target);
                bool isCorrect = (angleDifference <= victoryMargin);
                RotatePuzzle scriptRotation = pieces[i].GetComponent<RotatePuzzle>();
                if(scriptRotation != null)
                {
                    scriptRotation.ToggleLights(isCorrect); // Increment count if the piece is aligned within the margin
                }
                if(isCorrect)
                {
                    count++;
                }
            }
        }
        return count; // Return the total count of correctly aligned pieces
    }

    void VictoryAchieved()
    {
        isVictoryAchieved = true;
        timerIsRunning = false;
        yesV.SetActive(true); // Activate the yes object
        noV.SetActive(true); // Activate the no object
        victoria.SetActive(true); // Activate the victory object
        if(doorManager != null)
        {
            StopReminder(); // Stop any active reminders when victory is achieved
            doorManager.UpdateOpening(1.0f); // Ensure the door is fully open on victory
        }
        Debug.Log("Victory logic executed.");
    }

    public void OnClickYesV()
    {
        dungeon++;
        yesV.SetActive(false); // Deactivate the yes object
        noV.SetActive(false); // Deactivate the no object
        victoria.SetActive(false); // Deactivate the victory object
        /*if(timer >= (initialTimerValue * 0.5f) && timer > 0)
        {
            winningStreak++;
            if(winningStreak >= 2)
            {
                difficulty++; // Increase difficulty if the player won with more than 50% of the time remaining
                winningStreak = 0; // Reset winning streak after increasing difficulty
            }
        }
        else
        {
            winningStreak = 0; // Reset winning streak if the player won with less than 50% of the time remaining
        }*/
        timer = initialTimerValue; // Reset the timer to its initial value
        timerIsRunning = useTimerConfig; // Restart the timer
        isVictoryAchieved = false; // Reset victory flag
        //DinamicPuzzle(); // Regenerate the puzzle with the updated difficulty level
        currentPuzzleIndex = 0;
        if(doorManager != null)
        {
            doorManager.UpdateOpening(0.0f); // Reset the door to closed position
        }
        Debug.Log("Avanzando al calabozo " + dungeon + " después de la victoria.");
        UpdateUI(); // Update the UI to reflect the new dungeon level*/
        UpdateReminderMessage();
    }

    void UpdateReminderMessage()
    {
        StopReminder();
        if(actualPhase == 0)
        {
            if (currentPuzzleIndex > 0)
            {
                StartReminder("Rota tu muñeca en las runas interiores");
            }
            else
            {
                StartReminder("Rota tu muñeca en la runa interior");
            }
        }
        else
        {
            if (currentPuzzleIndex > 0)
            {
                StartReminder("Rota tu muñeca en las runas exteriores");
            }
            else
            {
                StartReminder("Rota tu muñeca en la runa exterior");
            }
        }
    }

    void UpdateUI()
    {
        if(dungeonText != null)
        {
            dungeonText.text = "Catacumba " + dungeon; // Update the dungeon level text
        }
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