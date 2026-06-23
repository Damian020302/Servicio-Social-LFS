using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PuzzleWristCalibrator : MonoBehaviour
{
    [Header("Calibration Settings")]
    public Transform activeHand;
    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;
    private bool isCalibrated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instructionText != null)
        {
            instructionText.text = "Rota tu muñeca lo más que puedas.";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isCalibrated) return;

    }

    void SaveMeanRotation()
    {

    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Juego2");
        Time.timeScale = 1.0f;
    }
}
