using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CauldronWristCalibration : MonoBehaviour
{
    public enum CalibrationState
    {
        SettingNeutral,
        WaitingForExtension,
        ReturningFromExtension,
        WaitingForFlexion,
        ReturningFromFlexion,
        Completed
    }
    private CalibrationState calibrationState;

    [Header("Calibration Settings")]
    public Transform leftWrist;
    public Transform rightWrist;
    public Transform activeHand;
    public int totalReps = 3;
    public float holdTimeRequired = 2.0f;
    public float neutralThreshHold = 20.0f;

    [Header("Calibration Maths")]
    private Quaternion neutralRotation;
    private int currentReps = 0;
    private float holdTimer = 0.0f;
    private float maxAngleThisRep = 0.0f;
    private List<float> recordedExtensionAngles = new List<float>();
    private List<float> recordedFlexionAngles = new List<float>();

    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(instructionText != null)
        {
            instructionText.text = "Mantén tu mano relajada frente a ti unos segundos...";
        }
        DetermineActiveHand();
        Invoke("SetNeutralRotation", 5.0f);
    }

    void DetermineActiveHand()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 1);
        if (selectedHand == 0 && leftWrist != null)
        {
            activeHand = leftWrist;
        }
        else if(selectedHand == 1 && rightWrist != null)
        {
            activeHand = rightWrist;
        }
        else
        {
            Debug.Log("No se encontró una mano activa");
        }
    }

    void SetNeutralRotation()
    {
        if(activeHand != null)
        {
            neutralRotation = activeHand.rotation;
            calibrationState = CalibrationState.WaitingForExtension;
            Debug.Log("Mano detectada. Pasando a la calibración...");
            UpdateUI();
        }
        else
        {
            Debug.LogError("No hay una mano activa");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (calibrationState == CalibrationState.Completed || calibrationState == CalibrationState.SettingNeutral || activeHand == null) return;
        Vector3 forwardNeutral = neutralRotation * Vector3.forward;
        Vector3 rightNeutral = neutralRotation * Vector3.right;
        float flexExtAngle = Vector3.SignedAngle(forwardNeutral, activeHand.forward, rightNeutral);
        float flexionAngle = 0.0f;
        float extensionAngle = 0.0f;
        if (flexExtAngle > 0.0f) flexionAngle = flexExtAngle;
        else if (flexExtAngle < 0.0f) extensionAngle = Mathf.Abs(flexExtAngle);
        float currentAbsoluteAngle = Mathf.Abs(flexExtAngle);
        //Extension Calibration
        if(calibrationState == CalibrationState.WaitingForExtension)
        {
            if(extensionAngle > 15.0f)
            {
                holdTimer += Time.deltaTime;
                if(extensionAngle > maxAngleThisRep) maxAngleThisRep = extensionAngle;
                instructionText.text = $"¡Sostén la extensión!\n{(holdTimeRequired - holdTimer):F1}s\n<size=50%>(Ángulo: {extensionAngle:F0}°)</size>";
                if(holdTimer >= holdTimeRequired)
                {
                    recordedExtensionAngles.Add(maxAngleThisRep);
                    currentReps++;
                    ResetRepCounters();
                    if(currentReps >= totalReps)
                    {
                        currentReps = 0;
                        calibrationState = CalibrationState.ReturningFromExtension;
                    }
                    else
                    {
                        calibrationState = CalibrationState.ReturningFromExtension;
                    }
                }
            }
            else
            {
                if(holdTimer > 0)
                {
                    ResetRepCounters();
                    UpdateUI();
                }
            }
        }
        else if(calibrationState == CalibrationState.ReturningFromExtension)
        {
            instructionText.text = $"Bien. ({currentReps}/{totalReps})\nRelaja tu mano.\n<size=50%>(Faltan: {currentAbsoluteAngle:F0} para llegar a {neutralThreshHold}°)</size>";
            if(currentAbsoluteAngle <= neutralThreshHold)
            {
                //neutralRotation = activeHand.rotation;
                if (recordedExtensionAngles.Count >= totalReps) calibrationState = CalibrationState.WaitingForFlexion;
                else calibrationState = CalibrationState.WaitingForExtension;
                UpdateUI();
            }
        }
        else if(calibrationState == CalibrationState.WaitingForFlexion)
        {
            if (flexionAngle > 15.0f)
            {
                holdTimer += Time.deltaTime;
                if(flexionAngle > maxAngleThisRep) maxAngleThisRep = flexionAngle;
                instructionText.text = $"¡Sostén la flexión!\n{(holdTimeRequired - holdTimer):F1}s\n<size=50%>(Ángulo: {flexionAngle:F0}°)</size>";
                if(holdTimer >= holdTimeRequired)
                {
                    recordedFlexionAngles.Add(maxAngleThisRep);
                    currentReps++;
                    ResetRepCounters();
                    calibrationState = CalibrationState.ReturningFromFlexion;
                }
            }
            else
            {
                if(holdTimer > 0)
                {
                    ResetRepCounters();
                    UpdateUI();
                }
            }
        }
        else if(calibrationState == CalibrationState.ReturningFromFlexion)
        {
            instructionText.text = $"Bien. ({currentReps}/{totalReps})\nRelaja tu mano.\n<size=50%>(Faltan: {currentAbsoluteAngle:F0} para llegar a {neutralThreshHold}°)</size>";
            if (currentAbsoluteAngle <= neutralThreshHold)
            {
                //neutralRotation = activeHand.rotation;
                if (currentReps >= totalReps) SaveMeanRotations();
                else calibrationState = CalibrationState.WaitingForFlexion;
                UpdateUI();
            }
        }
    }

    void ResetRepCounters()
    {
        holdTimer = 0.0f;
        maxAngleThisRep = 0.0f;
    }

    void SaveMeanRotations()
    {
        calibrationState = CalibrationState.Completed;
        float extSum = 0;
        foreach (float a in recordedExtensionAngles) extSum += a;
        float meanExt = extSum / recordedExtensionAngles.Count;

        float flexSum = 0;
        foreach(float a in recordedFlexionAngles) flexSum += a;
        float meanFlex = flexSum / recordedFlexionAngles.Count;

        meanExt = Mathf.Clamp(meanExt, 20.0f, 90.0f);
        meanFlex = Mathf.Clamp(meanFlex, 20.0f, 90.0f);

        PlayerPrefs.SetFloat("CauldronMaxExtension", meanExt);
        PlayerPrefs.SetFloat("CauldronMaxFlexion", meanFlex);
        PlayerPrefs.Save();

        if(instructionText != null)
        {
            instructionText.text = $"¡Calibración Completada!\nExtension:{meanExt:F0}° | Flexión: {meanFlex:F0}°\n\nIniciando...";
        }
        Invoke("LoadNextScene", 3.0f);

    }

    void UpdateUI()
    {
        if (instructionText != null)
        {
            if (calibrationState == CalibrationState.WaitingForExtension)
                instructionText.text = $"Extiende tu mano hacia arriba lo más que puedas y sostén. \nRepetición {currentReps+1} de {totalReps}";
            else if (calibrationState == CalibrationState.WaitingForFlexion)
                instructionText.text = $"Flexiona tu mano hacia abajo lo más que puedas y sostén. \nRepetición {currentReps + 1} de {totalReps}";
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Juego3");
        Time.timeScale = 1.0f;
    }
}
