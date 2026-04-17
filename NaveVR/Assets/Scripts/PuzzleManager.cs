using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Pieces")]
    public Collider[] puzzlePieces; // Array to hold references to the puzzle piece colliders
    private int currentPieceIndex = 0; // Index to track the current active piece

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < puzzlePieces.Length; i++)
        {
            if(puzzlePieces[i] != null)
            {
                puzzlePieces[i].enabled = (i == 0); // Disable all colliders at the start
            }
        }
    }

    public void ActivatePiece(int index)
    {
        if(puzzlePieces[currentPieceIndex] != null)
        {
            puzzlePieces[currentPieceIndex].enabled = false; // Enable the collider for the specified piece
        }
        if(puzzlePieces[index] != null)
        {
            puzzlePieces[index].enabled = true; // Disable the collider for the current piece
        }
        currentPieceIndex = index; // Update the current piece index
        Debug.Log($"Activated piece: {index}"); // Log the activated piece index
    }
}
