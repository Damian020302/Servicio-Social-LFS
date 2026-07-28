using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GrabWristCalibration : MonoBehaviour
{
    public enum CalibrationState
    {
        SettingNeutral,
        WaitingForGrab,
        ReturningToNeutral,
        Completed
    }
    private CalibrationState calibrationState;
    [Header("Calibration Settings")]
    [Tooltip("Compoente OVRHand de la mano izquierda")] public OVRHand leftHand;
    [Tooltip("Componente OVRHand de la mano derecha")] public OVRHand rightHand;
    private OVRHand activeHand;
    public int totalReps = 3;
    public float holdTimeRequired = 3.0f;
    public float neutralThreshold = 0.2f;
    public float graceTime = 0.5f;
    [Header("Calibration Maths")]
    private int currentRep = 0;
    private float holdTimer = 0.0f;
    private float graceTimer = 0.0f;
    private float maxGripThisRep = 0.0f;
    private List<float> recordedGrips = new List<float>();
    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instructionText != null)
        {
            instructionText.text = "Mantén tu mano abierta y relajada frente a ti unos segundos...";
        }
        DetermineActiveHand();
        Invoke("StartCalibration", 5.0f);
    }

    void DetermineActiveHand()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 1);
        if (selectedHand == 0 && leftHand != null)
        {
            activeHand = leftHand;
        }
        else if (selectedHand == 1 && rightHand != null)
        {
            activeHand = rightHand;
        }
        else
        {
            Debug.Log("No se encontro una mano activa");
        }
    }

    void StartCalibration()
    {
        if (activeHand != null)
        {
            calibrationState = CalibrationState.WaitingForGrab;
            UpdateUI();
        }
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

    // Update is called once per frame
    void Update()
    {
        if (calibrationState == CalibrationState.Completed || calibrationState == CalibrationState.SettingNeutral || activeHand == null) return;
        if(!activeHand.IsTracked)
        {
            instructionText.text = "Mano no detectada, por favor coloca tu mano frente al visor.";
            return;
        }
        float currentGrip = GetCurrentGrip();
        if(calibrationState == CalibrationState.WaitingForGrab)
        {
            bool isValidGrip = (currentGrip > 0.3f) && (currentGrip >= (maxGripThisRep - 0.2f));
            if (isValidGrip)
            {
                if (currentGrip > maxGripThisRep)
                {
                    maxGripThisRep = currentGrip;
                }
                graceTimer = 0.0f;
                holdTimer += Time.deltaTime;
                instructionText.text = $"¡Mantén tu puño cerrado! \n{(holdTimeRequired - holdTimer):F1}s\n<size=50%>(Fuerza actual: {(currentGrip * 100):F0}%)</size>";
                if (holdTimer >= holdTimeRequired)
                {
                    recordedGrips.Add(maxGripThisRep);
                    currentRep++;
                    holdTimer = 0.0f;
                    maxGripThisRep = 0.0f;
                    if (currentRep >= totalReps)
                    {
                        SaveMeanGrip();
                    }
                    else
                    {
                        calibrationState = CalibrationState.ReturningToNeutral;
                    }
                }
            }
            else
            {
                if (holdTimer > 0)
                {
                    graceTimer += Time.deltaTime;
                    if (graceTimer > graceTime)
                    {
                        holdTimer = 0.0f;
                        maxGripThisRep = 0.0f;
                        UpdateUI();
                    }
                    else
                    {
                        instructionText.text = $"¡Mantén tu puño cerrado! \n{(holdTimeRequired - holdTimer):F1}s\n<size=50%>(Fuerza actual: {(currentGrip * 100):F0}%)</size>\n<size=50%>(Tiempo de gracia: {(graceTime - graceTimer):F1}s)</size>";
                    }
                }
            }
        }
        else if (calibrationState == CalibrationState.ReturningToNeutral)
        {
            instructionText.text = $"Bien. ({currentRep}/{totalReps})\nAbre tu mano completamente para descansar.\n<size=50%>(Fuerza actual:{(currentGrip * 100):F0}%)</size>";
            if (currentGrip <= neutralThreshold)
            {
                calibrationState = CalibrationState.WaitingForGrab;
                UpdateUI();
            }
        }
    }

    void SaveMeanGrip()
    {
        calibrationState = CalibrationState.Completed;
        float sum = 0;
        foreach (float grip in recordedGrips)
        {
            sum += grip;
        }
        float meanGrip = sum / recordedGrips.Count;
        float finalCalibration = Mathf.Clamp(meanGrip, 0.3f, 0.95f);
        PlayerPrefs.SetFloat("MaxGrabStrength", finalCalibration);
        PlayerPrefs.Save();
        if (instructionText != null)
        {
            instructionText.text = $"Calibración completada.\nFuerza de puño guardada: {(finalCalibration * 100):F0}%\nIniciando...";
        }
        Invoke("LoadNextScene", 3.0f);
    }

    void UpdateUI()
    {
        if (instructionText != null)
        {
            instructionText.text = $"Cierra tu mano para hacer un puño y sostén. \nRepetición: {currentRep + 1} de {totalReps}";
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Juego4");
        Time.timeScale = 1.0f;
    }
}