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
        Completed
    }
    private CalibrationState calibrationState;
    [Header("Calibration Settings")]
    [Tooltip("El centro del jugador")] public Transform playerCenter;
    [Tooltip("El objeto de referencia para medir distancia")] public Transform leftWrist;
    [Tooltip("El objeto de referencia para medir distancia")] public Transform rightWrist;
    private Transform activeHand;
    public int totalReps = 5;
    [Tooltip("Tiempo que debe mantener el brazo estirado")] public float holdTimeRequired = 3.0f;
    //public float neutralThreshHold = 30.0f;
    [Header("Calibration Maths")]
    //private Quaternion neutralRotation;
    private int currentReps = 0;
    [Tooltip("Distancia minima en metros para empezar a medir")] public float minDist = 0.3f;
    private float holdTimer = 0.0f;
    private float maxDistanceThisRep = 0.0f;
    private List<float> recordedDistances = new List<float>();
    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;
    //private bool isCalibrated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(instructionText != null)
        {
            instructionText.text = "Estira tu brazo sano lo más que puedas y mantén la posición...";
        }
        DetermineActiveHand();
        Invoke("SetNeutralRotation", 10.0f);
    }

    void DetermineActiveHand()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 1);
        if (selectedHand == 0 && leftWrist != null)
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
            //neutralRotation = activeHand.transform.rotation;
            calibrationState = CalibrationState.WaitingForStretch;
            UpdateUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(calibrationState == CalibrationState.Completed || calibrationState == CalibrationState.SettingNeutral/*isCalibrated*/ || activeHand == null || playerCenter == null) return;
        /*Vector3 centerFlat = new Vector3(playerCenter.position.x, 0, playerCenter.position.z);
        Vector3 handFlat = new Vector3(activeHand.position.x, 0, activeHand.position.z);
        float currentDistance = Vector3.Distance(centerFlat, handFlat);*/
        float currentDistance = Vector3.Distance(playerCenter.position, activeHand.position);
        if(calibrationState == CalibrationState.WaitingForStretch)
        {
            if (currentDistance > minDist)
            {
                if (currentDistance > maxDistanceThisRep)
                {
                    maxDistanceThisRep = currentDistance;
                }
                if (currentDistance >= (maxDistanceThisRep - 0.05f))
                {
                    holdTimer += Time.deltaTime;
                    instructionText.text = $"Mantén el brazo estirado.\n{(holdTimeRequired - holdTimer):F1}s\n<size=50%>(Distancia actual: {currentDistance:F2}</size>)";
                    if (holdTimer >= holdTimeRequired)
                    {
                        recordedDistances.Add(maxDistanceThisRep);
                        currentReps++;
                        holdTimer = 0;
                        maxDistanceThisRep = 0.0f;
                        if(currentReps >= totalReps)
                        {
                            SaveMeanDistance();
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
                        holdTimer = 0.0f;
                        UpdateUI();
                    }
                }
            }
            else
            {
                if (holdTimer > 0)
                {
                    holdTimer = 0.0f;
                    maxDistanceThisRep = 0.0f;
                    UpdateUI();
                }
            }
        }
        else if(calibrationState == CalibrationState.ReturningToNeutral)
        {
            instructionText.text = $"Bien. ({currentReps}/{totalReps})\nRegresa el brazo cerca de tu cuerpo.\n<size=50%>(Distancia actual: {currentDistance:F2}</size>)";
            if(currentDistance < minDist)
            {
                calibrationState = CalibrationState.WaitingForStretch;
                UpdateUI();
            }
        }
        
        /*if (ovrHandL != null && ovrHandL.IsTracked)
        {
            bool isPinchingL = ovrHandL.GetFingerIsPinching(OVRHand.HandFinger.Index);
            if (isPinchingL)
            {
                Debug.Log("Pellizco detectado en mano izquierda");
                SaveMaxDistance(1);
            }
        }
        if (ovrHandR != null && ovrHandR.IsTracked)
        {
            bool isPinchingR = ovrHandR.GetFingerIsPinching(OVRHand.HandFinger.Index);
            if(isPinchingR)
            {
                Debug.Log("Pellizco detectado en mano derecha");
                SaveMaxDistance(2);
            }
        }*/
    }

    void SaveMeanDistance(/*int hand*/)
    {
        calibrationState = CalibrationState.Completed;
        float sum = 0;
        foreach(float dist in recordedDistances)
        {
            sum += dist;
        }
        float meanDistance = sum / recordedDistances.Count;
        /*isCalibrated = true;*/
        float finalRadio = meanDistance - 0.05f;
        finalRadio = Mathf.Max(finalRadio, 0.2f);
        PlayerPrefs.SetFloat("PlayerRadius", finalRadio);
        PlayerPrefs.Save();
        if(instructionText != null )
        {
            instructionText.text = $"Calibración completa.\nRadio guardado: {finalRadio:F2}m\nIniciando terapia...";
        }
        Invoke("LoadNextScene", 3.0f);
        /*if (hand == 1)
        {
            float extensionDistance = Vector3.Distance(playerCenter.position, referencePointL.position);
            float finalRadio = extensionDistance - 0.05f;
            PlayerPrefs.SetFloat("PlayerRadius", finalRadio);
            PlayerPrefs.Save();
            if(instructionText != null)
            {
                instructionText.text = $"Calibración completa.\nRadio guardado: {finalRadio:F2}m\n\nIniciando terapia...";
                Debug.Log($"Calibraci�n completa. Radio guardado: {finalRadio:F2}m");
                Invoke("LoadNextScene", 3f); // Espera 3 segundos antes de cargar la siguiente escena)
            }
        }
        else if (hand == 2)
        {
            float extensionDistance = Vector3.Distance(playerCenter.position, referencePointR.position);
            float finalRadio = extensionDistance - 0.05f;
            PlayerPrefs.SetFloat("PlayerRadius", finalRadio);
            PlayerPrefs.Save();
            if (instructionText != null)
            {
                instructionText.text = $"Calibración completa.\nRadio guardado: {finalRadio:F2}m\n\nIniciando terapia...";
                Debug.Log($"Calibraci�n completa. Radio guardado: {finalRadio:F2}m");
                Invoke("LoadNextScene", 3f); // Espera 3 segundos antes de cargar la siguiente escena)
            }
        }*/
        
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
            instructionText.text = $"Estira tu brazo sano lo más que puedas y sostén la posición.\nRepetición: {currentReps + 1} de {totalReps}";
        }
    }
}