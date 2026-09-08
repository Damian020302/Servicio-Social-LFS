using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class VRCalibration : MonoBehaviour
{
    public enum CalibrationState
    {
        SettingNeutral,
        WaitingForStretch,
        ReturningToNeutral,
        Completed,
        Transitioning
    }
    private CalibrationState calibrationState;

    public enum CalibrationPhase
    {
        LeftArm,
        RightArm,
        Done
    }
    private CalibrationPhase currentPhase = CalibrationPhase.LeftArm;

    [Header("Calibration Settings")]
    [Tooltip("The center of the player")] public Transform playerCenter;
    [Tooltip("The reference object for measuring distance")] public Transform leftWrist;
    [Tooltip("The reference object for measuring distance")] public Transform rightWrist;
    private bool useLeftArm = false;
    private bool useRightArm = false;
    private Transform activeHand;
    public string currentArmName;
    public int totalReps = 3;
    [Tooltip("Time the arm must be held stretched")] public float holdTimeRequired = 3.0f;
    [Header("Medical Measurements")]
    [Tooltip("Arm length from shoulder to elbow")]
    public float upperArmLength = 0.30f;
    [Tooltip("Arm length from elbow to wrist")]
    public float forearmLength = 0.25f;
    [Header("Calibration Maths")]
    private int currentReps = 0;
    [Tooltip("Minimum distance in meters to start measuring")] public float minDist = 0.30f;
    [Tooltip("Distance the arm must return to count the repetition")] public float returnDist = 0.15f;
    private float holdTimer = 0.0f;
    private float maxDistanceThisRep = 0.0f;
    private float elbowAngleAtMaxReach = 180.0f;
    private List<float> recordedDistancesL = new List<float>();
    private List<float> recordedDistancesR = new List<float>();
    private List<float> recordedElbowL = new List<float>();
    private List<float> recordedElbowR = new List<float>();
    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;

    void Start()
    {
        upperArmLength = PlayerPrefs.GetFloat("UpperArmLength", upperArmLength);
        forearmLength = PlayerPrefs.GetFloat("ForearmLength", forearmLength);
        DetermineActiveHand();
        Invoke("StartCalibrationSequence", 3.0f);
    }

    void DetermineActiveHand()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 1);
        if (selectedHand == 0 && leftWrist != null)
        {
            useLeftArm = true;
            useRightArm = false;
        }
        else if (selectedHand == 1 && rightWrist != null)
        {
            useRightArm = true;
            useLeftArm = false;
        }
        else if (selectedHand == 2 && leftWrist != null && rightWrist != null)
        {
            useLeftArm = true;
            useRightArm = true;
        }
        else
        {
            Debug.Log("No se encontro una mano activa");
            instructionText.text = "No se encontro una mano activa";
        }        
    }

    void StartCalibrationSequence()
    {
        if(useLeftArm)
        {
            currentPhase = CalibrationPhase.LeftArm;
            activeHand = leftWrist;
        }
        else if(useRightArm)
        {
            currentPhase = CalibrationPhase.RightArm;
            activeHand = rightWrist;
        }
        else
        {
            currentPhase = CalibrationPhase.Done;
            SaveMeanDistance();
            return;
        }
        ResetRepTracking();
        calibrationState = CalibrationState.WaitingForStretch;
        UpdateUI();
    }

    void ResetRepTracking()
    {
        currentReps = 0;
        holdTimer = 0.0f;
        maxDistanceThisRep = 0.0f;
        elbowAngleAtMaxReach = 180.0f;
    }

    float CalculateElbowAngle(float distance)
    {
        float a = upperArmLength;
        float b = forearmLength;
        float c = Mathf.Clamp(distance, 0.0001f, a + b);
        if(c>(a+b))
        {
            float scale = c / (a + b);
            a *= scale;
            b *= scale;
        }
        float cosC = Mathf.Clamp((a * a + b * b - c * c) / (2 * a * b), -1.0f, 1.0f);
        return 180.0f - (Mathf.Acos(cosC) * Mathf.Rad2Deg);
    }

    void Update()
    {
        if(calibrationState == CalibrationState.Completed || calibrationState == CalibrationState.SettingNeutral || playerCenter == null) return;
        float currentDistance = Vector3.Distance(playerCenter.position, activeHand.position);
        float currentElbowAngle = (currentDistance > 0) ? CalculateElbowAngle(currentDistance) : 180.0f;
        currentArmName = (currentPhase == CalibrationPhase.LeftArm) ? "Izquierdo" : "Derecho";
        if(calibrationState == CalibrationState.WaitingForStretch)
        {
            if(currentDistance > minDist)
            {
                if(currentDistance > maxDistanceThisRep)
                {
                    maxDistanceThisRep = currentDistance;
                    elbowAngleAtMaxReach = currentElbowAngle;
                }
                if(currentDistance >= (maxDistanceThisRep - 0.05f))
                {
                    holdTimer += Time.deltaTime;
                    instructionText.text = $"Mantén el brazo {currentArmName} estirado.\n{(holdTimeRequired - holdTimer):F1}s";
                    if(holdTimer >= holdTimeRequired)
                    {
                        if(currentPhase == CalibrationPhase.LeftArm)
                        {
                            recordedDistancesL.Add(maxDistanceThisRep);
                            recordedElbowL.Add(elbowAngleAtMaxReach);
                        }
                        else if(currentPhase == CalibrationPhase.RightArm)
                        {
                            recordedDistancesR.Add(maxDistanceThisRep);
                            recordedElbowR.Add(elbowAngleAtMaxReach);
                        }
                        currentReps++;
                        holdTimer = 0;
                        if(currentReps >= totalReps) Invoke("AdvancePhase", 3.0f);
                        else calibrationState = CalibrationState.ReturningToNeutral;
                    }
                }
                else
                {
                    if(holdTimer > 0)
                    {
                        holdTimer = 0.0f;
                        UpdateUI();
                    }
                }
            }
            else
            {
                if(holdTimer > 0)
                {
                    holdTimer = 0.0f;
                    maxDistanceThisRep = 0.0f;
                    elbowAngleAtMaxReach = 180.0f;
                    UpdateUI();
                }
            }
        }
        else if(calibrationState == CalibrationState.ReturningToNeutral)
        {
            instructionText.text = $"Bien. ({currentReps}/{totalReps})\nDobla tu brazo {currentArmName} hacia tu cuerpo.";
            if(currentDistance <= (maxDistanceThisRep - returnDist) || currentDistance < minDist)
            {
                maxDistanceThisRep = 0.0f;
                elbowAngleAtMaxReach = 180.0f;
                calibrationState = CalibrationState.WaitingForStretch;
                UpdateUI();
            }
        }
    }

    void AdvancePhase()
    {
        calibrationState = CalibrationState.Transitioning;
        if(currentPhase == CalibrationPhase.LeftArm && useRightArm)
        {
            if(instructionText != null)
            {
                instructionText.text = "¡Excelente!\nAhora vamos a calibrar el brazo Derecho.\nPreparate...";
            }
            currentPhase = CalibrationPhase.RightArm;
            activeHand = rightWrist;
            Invoke("StartNextArm", 4.0f);
        }
        else
        {
            currentPhase = CalibrationPhase.Done;
            SaveMeanDistance();
        }
    }

    void StartNextArm()
    {
        ResetRepTracking();
        calibrationState = CalibrationState.WaitingForStretch;
        UpdateUI();
    }

    /**
     * Saves the mean distances and elbow angles to PlayerPrefs.
     * If only one arm is used, it copies the values to the other arm.
     */
    void SaveMeanDistance()
    {
        calibrationState = CalibrationState.Completed;
        float finalRadioL = 0.2f;
        float finalRadioR = 0.2f;
        float finalElbowL = 0.0f;
        float finalElbowR = 0.0f;
        if(recordedDistancesL.Count > 0)
        {
            float sumDistL = 0.0f;
            foreach(float dist in recordedDistancesL) sumDistL += dist;
            finalRadioL = Mathf.Max((sumDistL / recordedDistancesL.Count) - 0.05f, 0.2f);
            float sumElbowL = 0.0f;
            foreach(float angle in recordedElbowL) sumElbowL += angle;
            finalElbowL = sumElbowL / recordedElbowL.Count;
        }
        if(recordedDistancesR.Count > 0)
        {
            float sumDistR = 0.0f;
            foreach(float dist in recordedDistancesR) sumDistR += dist;
            finalRadioR = Mathf.Max((sumDistR / recordedDistancesR.Count) - 0.05f, 0.2f);
            float sumElbowR = 0.0f;
            foreach(float angle in recordedElbowR) sumElbowR += angle;
            finalElbowR = sumElbowR / recordedElbowR.Count;
        }
        if(useLeftArm && !useRightArm)
        {
            finalRadioR = finalRadioL;
            finalElbowR = finalElbowL;
        }
        if(useRightArm && !useLeftArm)
        {
            finalRadioL = finalRadioR;
            finalElbowL = finalElbowR;
        }
        PlayerPrefs.SetFloat("PlayerRadiusL", finalRadioL);
        PlayerPrefs.SetFloat("PlayerRadiusR", finalRadioR);
        PlayerPrefs.SetFloat("PlayerElbowAngleL", finalElbowL);
        PlayerPrefs.SetFloat("PlayerElbowAngleR", finalElbowR);
        PlayerPrefs.SetFloat("PlayerElbowAngle", (finalElbowL+finalElbowR)/2.0f);
        PlayerPrefs.SetFloat("PlayerRadius", Mathf.Max(finalRadioL, finalRadioR));
        PlayerPrefs.Save();
        if (instructionText != null)
        {
            instructionText.text = $"Calibración completa.\nRadio Izquierdo:{finalRadioL:F2}m\nRadio Derecho:{finalRadioR:F2}m\nÁngulo de Codo (Flexión): {(finalElbowL + finalElbowR) / 2.0f:F2}°\nIniciando terapia...";
        }
        Invoke("LoadNextScene", 3.0f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Enemigos");
        Time.timeScale = 1.0f;
    }

    void UpdateUI()
    {
        if(instructionText != null)
        {
            instructionText.text = $"Estira tu brazo {currentArmName} lo más que puedas y sostén la posición.\nRepetición: {currentReps + 1} de {totalReps}";
        }
    }
}