using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PuzzleWristCalibrator : MonoBehaviour
{
    public enum CalibrationState
    {
        SettingNeutral,
        WaitingForRotation,
        ReturningToNeutral,
        Completed
    }
    private CalibrationState calibrationState;
    [Header("Calibration Settings")]
    public Transform leftWrist;
    public Transform rightWrist;
    private Transform activeHand;
    public int totalReps = 5;
    public float holdTimeRequired = 3.0f;
    public float neutralThreshHold = 30.0f;

    [Header("Calibration Maths")]
    private Quaternion neutralRotation;
    private int currentReps = 0;
    private float holdTimer = 0.0f;
    private float maxAngleThisRep = 0.0f;
    private List<float> recordedAngles = new List<float>();

    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instructionText != null)
        {
            instructionText.text = "Mantén tu mano relajada frente a ti unos segundos...";
        }
        DetermineActiveHand();
        Invoke("SetNeutralRotation", 10.0f);
    }

    void DetermineActiveHand()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 1);
        if(selectedHand == 0 && leftWrist != null)
        {
            activeHand = leftWrist;
        }
        else if (selectedHand == 1 && rightWrist != null)
        {
            activeHand = rightWrist;
        }
        else
        {
            Debug.Log("No se encontro una mano activa");
        }
    }

    void SetNeutralRotation()
    {
        if(activeHand != null)
        {
            neutralRotation = activeHand.transform.rotation;
            calibrationState = CalibrationState.WaitingForRotation;
            UpdateUI();
        }
        else
        {
            Debug.LogError("No hay mano activa");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(calibrationState == CalibrationState.Completed || calibrationState == CalibrationState.SettingNeutral || activeHand == null)
        {
            return;
        }
        float currentAngle = Quaternion.Angle(neutralRotation, activeHand.rotation);
        if(calibrationState == CalibrationState.WaitingForRotation)
        {
            if(currentAngle > 20.0f)
            {
                holdTimer += Time.deltaTime;
                if(currentAngle > maxAngleThisRep)
                {
                    maxAngleThisRep = currentAngle;
                }
                instructionText.text = $"¡Mantén la posición ahí!\n{(holdTimeRequired - holdTimer):F1}s";
                if(holdTimer >= holdTimeRequired)
                {
                    recordedAngles.Add(maxAngleThisRep);
                    currentReps++;
                    holdTimer = 0.0f;
                    maxAngleThisRep = 0.0f;
                    if(currentReps >= totalReps)
                    {
                        SaveMeanRotation();
                    }
                    else
                    {
                        calibrationState = CalibrationState.ReturningToNeutral;
                    }
                }
            }
            else
            {
                if(holdTimer > 0)
                {
                    holdTimer = 0.0f;
                    maxAngleThisRep = 0.0f;
                    UpdateUI();
                }
            }
        }
        else if(calibrationState == CalibrationState.ReturningToNeutral)
        {
            instructionText.text = $"Buen trabajo. ({currentReps}/{totalReps})\nAhora relaja y baja tu mano.";
            if (currentAngle <= neutralThreshHold)
            {
                neutralRotation = activeHand.rotation;
                calibrationState = CalibrationState.WaitingForRotation;
                UpdateUI();
            }
        }

    }

    void SaveMeanRotation()
    {
        calibrationState = CalibrationState.Completed;
        float sum = 0;
        foreach(float angle in recordedAngles)
        {
            sum += angle;
        }
        float meanAngle = sum / recordedAngles.Count;
        float finalCalibration = Mathf.Clamp(meanAngle, 30.0f, 180.0f);
        PlayerPrefs.SetFloat("MaxPuzzleRotation", finalCalibration);
        PlayerPrefs.Save();
        if(instructionText != null)
        {
            instructionText.text = $"Calibración existosa. \nRotación máxima promedio: {finalCalibration:F1}°\n\nIniciando terapia...";
        }
        Invoke("LoadNextScene", 3.0f);
    }

    void UpdateUI()
    {
        if(instructionText != null)
        {
            instructionText.text = $"Gira tu muñeca lo más que puedas y sostén el esfuerzo.\nRepetición: {currentReps + 1} de {totalReps}";
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Juego2");
        Time.timeScale = 1.0f;
    }
}
