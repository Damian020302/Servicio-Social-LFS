using TMPro;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public GameObject victoria;
    public GameObject derrota;
    [Header("Puzzle Pieces")]
    public Collider[] puzzlePieces; // Array to hold references to the puzzle piece colliders
    //private int currentPieceIndex = 0; // Index to track the current active piece

    [Header("Victory configuration")]
    [Tooltip("Margin of error")]
    public float victoryMargin = 12.0f; // Margin of error for victory condition

    [Header("Door Manager")]
    public DoorManager doorManager; // Reference to the DoorManager script

    private Quaternion[] targetRotations; // Array to hold target rotations for each piece
    //private int actualActiveIndex = 0; // Index to track the actual active piece for victory condition
    private bool isVictoryAchieved = false; // Flag to track if victory has been achieved
    
    public float timer = 60.0f; // Timer for defeat condition (if needed)
    public bool timerIsRunning = false; // Flag to track if the timer is running
    public TextMeshProUGUI timeRemaining;

    void Start()
    {
        victoria.SetActive(false); // Ensure the victory object is initially inactive
        derrota.SetActive(false); // Ensure the defeat object is initially inactive
        targetRotations = new Quaternion[puzzlePieces.Length];
        for(int i = 0; i < puzzlePieces.Length; i++)
        {
            if(puzzlePieces[i] != null)
            {
                targetRotations[i] = puzzlePieces[i].transform.rotation; // Store the initial rotation as the target rotation
                float randomRotation = Random.Range(60.0f, 300.0f); // Generate a random Y rotation
                //puzzlePieces[i].transform.rotation = Quaternion.Euler(0, randomYRotation, 0); // Apply the random rotation
                puzzlePieces[i].transform.Rotate(0, 0, randomRotation, Space.Self); // Apply the random rotation
                puzzlePieces[i].enabled = true; // Disable all colliders at the start
                
            }
        }
    }

    /*public void ActivatePiece(int index)
    {
        if(isVictoryAchieved)
        {
            Debug.Log("Victory already achieved. No more pieces can be activated.");
            return; // Exit if victory has already been achieved
        }
        if (puzzlePieces[currentPieceIndex] != null)
        {
            puzzlePieces[currentPieceIndex].enabled = false; // Enable the collider for the specified piece
        }
        if(puzzlePieces[index] != null)
        {
            puzzlePieces[index].enabled = true; // Disable the collider for the current piece
            actualActiveIndex = index; // Update the actual active index for victory condition
                                       // Check for victory condition
        }
        currentPieceIndex = index; // Update the current piece index
        Debug.Log($"Activated piece: {index}"); // Log the activated piece index
    }*/

    public void Update()
    {
        if(isVictoryAchieved)
        {
            return; // Exit if victory has already been achieved
        }
        int correctPieces = GetCorrectPiecesCount();
        float progress = 0.0f;
        if(puzzlePieces.Length > 0)
        {
            progress = (float)correctPieces / puzzlePieces.Length; // Calculate progress as a percentage
        }
        if (doorManager != null)
        {
            doorManager.UpdateOpening(progress); // Update the door opening based on progress
        }
        if(correctPieces == puzzlePieces.Length)
        {
            Debug.Log("All pieces are correctly aligned! Checking for victory condition...");
        /*}
        if (/*AlignVerification())
        {*/
            isVictoryAchieved = true; // Set victory flag to true
            VictoryAchieved();
            Debug.Log("Victory Achieved! All pieces are aligned.");
        }
        if(timerIsRunning)
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
        timeRemaining.text = "Tiempo restante: " + string.Format("{0:00}:{1:00}", minutes, seconds); // Update the UI text with formatted time
    }

    private int GetCorrectPiecesCount()
    {
        int count = 0;
        for(int i = 0; i < puzzlePieces.Length; i++)
        {
            if(puzzlePieces[i] != null)
            {
                float angleDifference = Quaternion.Angle(puzzlePieces[i].transform.rotation, targetRotations[i]);
                if(angleDifference <= victoryMargin)
                {
                    count++; // Increment count if the piece is aligned within the margin
                }
            }
        }
        return count; // Return the total count of correctly aligned pieces
    }

    /*bool AlignVerification()
    {
        for(int i = 0; i < puzzlePieces.Length; i++)
        {
            if(puzzlePieces[i] != null)
            {
                float angleDifference = Quaternion.Angle(puzzlePieces[i].transform.rotation, targetRotations[i]);
                if(angleDifference > victoryMargin)
                {
                    return false; // If any piece is not aligned within the margin, return false
                }
            }
        }
        return true; // All pieces are aligned within the margin
    }*/

    void VictoryAchieved()
    {
        victoria.SetActive(true); // Activate the victory object
        if(doorManager != null)
        {
            doorManager.UpdateOpening(1.0f); // Ensure the door is fully open on victory
        }
        Debug.Log("Victory logic executed.");
    }

    void DefeatAchieved()
    {
        derrota.SetActive(true); // Activate the defeat object
        Debug.Log("Defeat logic executed.");
    }
}