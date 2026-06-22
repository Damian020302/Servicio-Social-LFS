using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class PuzzleManager : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject victoria;
    public GameObject derrota;
    public GameObject yesD;
    public GameObject noD;
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
    public int difficulty = 0; // Variable to track the current difficulty level
    public int winningStreak = 0;
    private bool isVictoryAchieved = false; // Flag to track if victory has been achieved
    public float timer = 60.0f; // Timer for defeat condition (if needed)
    public bool timerIsRunning = false; // Flag to track if the timer is running
    public float initialTimerValue;
    [Header("Puzzle Pieces")]
    private System.Collections.Generic.Dictionary<Collider, Quaternion> perfectRotations = new System.Collections.Generic.Dictionary<Collider, Quaternion>(); // Dictionary to store perfect rotations for each piece
    [Header("Phases")]
    private System.Collections.Generic.List<Collider[]> puzzlePhases = new System.Collections.Generic.List<Collider[]>(); // Dictionary to store puzzles for each phase
    public int actualPhase = 0; // Variable to track the current phase of the puzzle

    [Header("Victory configuration")]
    [Tooltip("Margin of error")]
    public float victoryMargin = 12.0f; // Margin of error for victory condition

    [Header("Door Manager")]
    public DoorManager doorManager; // Reference to the DoorManager script

    public void OnClickNo()
    {
        SceneManager.LoadScene("Menu2");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void MenuGeneral()
    {
        SceneManager.LoadScene("MenuGeneral");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void MainScene()
    {
        SceneManager.LoadScene("Juego2");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    void Start()
    {
        Collider[] allPieces = puzzleAdmin.GetComponentsInChildren<Collider>(true); // Get all colliders from the puzzle pieces
        foreach(Collider piece in allPieces)
        {
            perfectRotations.Add(piece, piece.transform.rotation); // Store the initial rotation as the perfect rotation for each piece
        }
        initialTimerValue = timer; // Store the initial timer value for potential resets
        yesV.SetActive(false); // Ensure the yes object is initially inactive
        noV.SetActive(false);
        yesD.SetActive(false); // Ensure the yes object is initially inactive
        noD.SetActive(false); // Ensure the no object is initially inactive
        victoria.SetActive(false); // Ensure the victory object is initially inactive
        derrota.SetActive(false); // Ensure the defeat object is initially inactive
        timerIsRunning = true; // Start the timer
        DinamicPuzzle();
        if (warning != null)
        {
            warningOriginalScale = warning.transform.localScale;
            if(warningOriginalScale == Vector3.zero)
            {
                warningOriginalScale = Vector3.one; // Fallback to a default scale if the original scale is not set
            }
        }
        UpdateReminderMessage();
    }

    void DinamicPuzzle()
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
    }

    void DefeatAchieved()
    {
        StopReminder(); // Stop any active reminders when defeat is achieved
        yesD.SetActive(true); // Activate the yes object
        noD.SetActive(true); // Activate the no object
        derrota.SetActive(true); // Activate the defeat object

        foreach(Collider[] phase in puzzlePhases)
        {
            foreach(Collider piece in phase)
            {
                if(piece != null)
                {
                    piece.gameObject.SetActive(false); // Deactivate all puzzle pieces
                }
            }
        }

        Debug.Log("Defeat logic executed.");
    }

    public void Update()
    {
        if(isVictoryAchieved)
        {
            return; // Exit if victory has already been achieved
        }
        Collider[] currentPhasePieces = puzzlePhases[actualPhase]; // Get the pieces for the current phase
        int correctPieces = GetCorrectPiecesCount(currentPhasePieces);
        if(doorManager != null/* && currentPhasePieces.Length > 0*/)
        {
            int totalLevelPieces = 0;
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
            }
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
                Debug.Log("All pieces are correctly aligned! Checking for victory condition...");
                isVictoryAchieved = true; // Set victory flag to true
                VictoryAchieved();
                Debug.Log("Victory Achieved! All pieces are aligned.");
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
        if (timerIsRunning)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime; // Decrease the timer by the time elapsed since the last frame
                DisplayTime(timer); // Update the UI text with the remaining time
            }
            else
            {
                timer = 0;
                timerIsRunning = false; // Stop the timer
                DefeatAchieved(); // Trigger defeat logic
                Debug.Log("Defeat Achieved! Time has run out.");
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
                if(angleDifference <= victoryMargin)
                {
                    count++; // Increment count if the piece is aligned within the margin
                }
            }
        }
        return count; // Return the total count of correctly aligned pieces
    }

    void VictoryAchieved()
    {
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

    public void OnClickYesD()
    {
        yesD.SetActive(false); // Deactivate the yes object
        noD.SetActive(false); // Deactivate the no object
        derrota.SetActive(false); // Deactivate the defeat object
        timer = initialTimerValue; // Reset the timer to its initial value
        timerIsRunning = true; // Restart the timer
        isVictoryAchieved = false; // Reset victory flag
        winningStreak = 0; // Reset winning streak on defeat
        difficulty--;
        DinamicPuzzle(); // Regenerate the puzzle with the updated difficulty level
        if(doorManager != null)
        {
            doorManager.UpdateOpening(0.0f); // Reset the door to closed position
        }
        Debug.Log("Restarting puzzle after defeat.");
        UpdateUI(); // Update the UI to reflect the reset state
        UpdateReminderMessage();
    }

    public void OnClickYesV()
    {
        dungeon++;
        yesV.SetActive(false); // Deactivate the yes object
        noV.SetActive(false); // Deactivate the no object
        victoria.SetActive(false); // Deactivate the victory object
        if(timer >= (initialTimerValue * 0.5f))
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
        }
        timer = initialTimerValue; // Reset the timer to its initial value
        timerIsRunning = true; // Restart the timer
        isVictoryAchieved = false; // Reset victory flag
        DinamicPuzzle(); // Regenerate the puzzle with the updated difficulty level
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
            if (difficulty > 0)
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
            if (difficulty > 0)
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
            dungeonText.text = "Calabozo " + dungeon; // Update the dungeon level text
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
        /*if(dungeon > 1)
        {
            warningCoroutine = StartCoroutine(WarningAnimationRoutine("Rota tu muñeca en las runas exteriores"));
        }*/
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