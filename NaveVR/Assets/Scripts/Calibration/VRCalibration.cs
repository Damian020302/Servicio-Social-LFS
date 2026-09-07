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
    private Transform activeHandL;
    private Transform activeHandR;
    public int totalReps = 3;
    [Tooltip("Tiempo que debe mantener el brazo estirado")] public float holdTimeRequired = 3.0f;
    [Header("Medical Measurements")]
    [Tooltip("Arm length from shoulder to elbow")]
    public float upperArmLength = 0.30f;
    [Tooltip("Arm length from elbow to wrist")]
    public float forearmLength = 0.25f;
    //public float neutralThreshHold = 30.0f;
    [Header("Calibration Maths")]
    //private Quaternion neutralRotation;
    private int currentReps = 0;
    [Tooltip("Distancia minima en metros para empezar a medir")] public float minDist = 0.30f;
    [Tooltip("Distancia que debe retroceder para contar la repeticion")] public float returnDist = 0.15f;
    private float holdTimer = 0.0f;
    private float maxDistanceL = 0.0f;
    private float maxDistanceR = 0.0f;
    private List<float> recordedDistancesL = new List<float>();
    private List<float> recordedDistancesR = new List<float>();
    private float maxElbowL = 180.0f;
    private float maxElbowR = 180.0f;
    private List<float> recordedElbowL = new List<float>();
    private List<float> recordedElbowR = new List<float>();
    /*[Header("Rastreadores de altura")]
    private float maxHeightThisRep = -100.0f;
    private List<float> recordedHeights = new List<float>();
    private float maxShoulderAngleThisRep = -90.0f;
    private List<float> recordedShoulderAngles = new List<float>();
    private float elbowAngleAtMaxReach = 180.0f;
    private List<float> recordedElbowAngles = new List<float>();*/
    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;
    //private bool isCalibrated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upperArmLength = PlayerPrefs.GetFloat("UpperArmLength", upperArmLength);
        forearmLength = PlayerPrefs.GetFloat("ForearmLength", forearmLength);
        if (instructionText != null)
        {
            instructionText.text = "Estira tus brazos lo más que puedas y mantén la posición...";
        }
        DetermineActiveHand();
        Invoke("SetNeutralRotation", 3.0f);
    }

    void DetermineActiveHand()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 1);
        if (selectedHand == 0 && leftWrist != null) activeHandL = leftWrist;
        else if (selectedHand == 1 && rightWrist != null) activeHandR = rightWrist;
        else if (selectedHand == 2 && leftWrist != null)
        {
            activeHandL = leftWrist;
            activeHandR = rightWrist;
        }
        else Debug.Log("No se encontro una mano activa");
    }

    void SetNeutralRotation()
    {
        if (activeHandL != null || activeHandR != null)
        {
            calibrationState = CalibrationState.WaitingForStretch;
            //UpdateUI();
            Invoke("UpdateUI", 1.5f);
        }
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

    // Update is called once per frame
    void Update()
    {
        if(calibrationState == CalibrationState.Completed || calibrationState == CalibrationState.SettingNeutral || playerCenter == null) return;
        float distL = (activeHandL != null) ? Vector3.Distance(playerCenter.position, activeHandL.position) : 0.0f;
        float distR = (activeHandR != null) ? Vector3.Distance(playerCenter.position, activeHandR.position) : 0.0f;
        float elbowL = (distL > 0) ? CalculateElbowAngle(distL) : 180.0f;
        float elbowR = (distR > 0) ? CalculateElbowAngle(distR) : 180.0f;
        float primaryDistance = Mathf.Max(distL, distR);
        float maxDistanceThisRepPrimary = Mathf.Max(maxDistanceL, maxDistanceR);
        if(calibrationState == CalibrationState.WaitingForStretch)
        {
            if(primaryDistance > minDist)
            {
                if(distL > maxDistanceL)
                {
                    maxDistanceL = distL;
                    maxElbowL = elbowL;
                }
                if(distR > maxDistanceR)
                {
                    maxDistanceR = distR;
                    maxElbowR = elbowR;
                }
                if(primaryDistance >= (maxDistanceThisRepPrimary - 0.05f))
                {
                    holdTimer += Time.deltaTime;
                    instructionText.text = $"Mantén el brazo estirado.\n{(holdTimeRequired - holdTimer):F1}s\n<size=50%>(Distancia actual: {primaryDistance:F2}</size>)";
                    if(holdTimer >= holdTimeRequired)
                    {
                        if(activeHandL != null)
                        {
                            recordedDistancesL.Add(maxDistanceL);
                            recordedElbowL.Add(maxElbowL);
                        }
                        if(activeHandR != null)
                        {
                            recordedDistancesR.Add(maxDistanceR);
                            recordedElbowR.Add(maxElbowR);
                        }
                        currentReps++;
                        holdTimer = 0;
                        if(currentReps >= totalReps) SaveMeanDistance();
                        else calibrationState = CalibrationState.ReturningToNeutral;
                    }
                }
                else
                {
                    if(holdTimer > 0)
                    {
                        holdTimer = 0.0f;
                        Invoke("UpdateUI", 1.5f);
                    }
                }
            }
            else
            {
                if(holdTimer > 0)
                {
                    holdTimer = 0.0f;
                    maxDistanceL = 0.0f;
                    maxDistanceR = 0.0f;
                    maxElbowL = 180.0f;
                    maxElbowR = 180.0f;
                    Invoke("UpdateUI", 1.5f);
                }
            }
        }
        else if(calibrationState == CalibrationState.ReturningToNeutral)
        {
            instructionText.text = $"Bien. ({currentReps}/{totalReps})\nDobla tu brazo y regresa el brazo cerca de tu cuerpo.\n<size=50%>(Distancia actual: {primaryDistance:F2}</size>)";
            if(primaryDistance <= (maxDistanceThisRepPrimary - returnDist) || primaryDistance < minDist)
            {
                maxDistanceL = 0.0f;
                maxDistanceR = 0.0f;
                maxElbowL = 180.0f;
                maxElbowR = 180.0f;
                calibrationState = CalibrationState.WaitingForStretch;
                Invoke("UpdateUI", 1.5f);
            }
        }
        /*float currentDistance = Vector3.Distance(playerCenter.position, activeHandR.position);
        float currentHeight = activeHandR.position.y - playerCenter.position.y;
        float currentShoulderAngle = 0.0f;
        if(currentDistance > 0) currentShoulderAngle = Mathf.Asin(currentHeight / currentDistance) * Mathf.Rad2Deg;
        if(currentDistance > (upperArmLength + forearmLength))
        {
            float scale = currentDistance / (upperArmLength + forearmLength);
            upperArmLength *= scale;
            forearmLength *= scale;
        }
        float a = upperArmLength;
        float b = forearmLength;
        float c = currentDistance;
        c = Mathf.Clamp(c, 0.0001f, a + b);
        float cosC = (a * a + b * b - c * c) / (2 * a * b);
        cosC = Mathf.Clamp(cosC, -1.0f, 1.0f);
        float interiorElbowAngle = Mathf.Acos(cosC) * Mathf.Rad2Deg;
        float currentElbowAngle = 180.0f - interiorElbowAngle;
        if (calibrationState == CalibrationState.WaitingForStretch)
        {
            if (currentDistance > minDist)
            {
                if (currentDistance > maxDistanceThisRep)
                {
                    maxDistanceThisRep = currentDistance;
                    elbowAngleAtMaxReach = currentElbowAngle;
                }
                if(currentHeight > maxHeightThisRep) maxHeightThisRep = currentHeight;
                if(currentShoulderAngle > maxShoulderAngleThisRep) maxShoulderAngleThisRep = currentShoulderAngle;
                if (currentDistance >= (maxDistanceThisRep - 0.05f))
                {
                    holdTimer += Time.deltaTime;
                    instructionText.text = $"Mantén el brazo estirado.\n{(holdTimeRequired - holdTimer):F1}s\n<size=50%>(Distancia actual: {currentDistance:F2}</size>)";
                    if (holdTimer >= holdTimeRequired)
                    {
                        recordedDistances.Add(maxDistanceThisRep);
                        recordedHeights.Add(maxHeightThisRep);
                        recordedShoulderAngles.Add(maxShoulderAngleThisRep);
                        recordedElbowAngles.Add(elbowAngleAtMaxReach);
                        currentReps++;
                        holdTimer = 0;
                        if(currentReps >= totalReps) SaveMeanDistance();
                        else calibrationState = CalibrationState.ReturningToNeutral;
                    }
                }
                else
                {
                    if (holdTimer > 0)
                    {
                        holdTimer = 0.0f;
                        //UpdateUI();
                        Invoke("UpdateUI", 1.5f);
                    }
                }
            }
            else
            {
                if (holdTimer > 0)
                {
                    holdTimer = 0.0f;
                    maxDistanceThisRep = 0.0f;
                    maxHeightThisRep = -100.0f;
                    maxShoulderAngleThisRep = -90.0f;
                    elbowAngleAtMaxReach = 180.0f;
                    //UpdateUI();
                    Invoke("UpdateUI", 1.5f);
                }
            }
        }
        else if(calibrationState == CalibrationState.ReturningToNeutral)
        {
            instructionText.text = $"Bien. ({currentReps}/{totalReps})\nDobla tu brazo y regresa el brazo cerca de tu cuerpo.\n<size=50%>(Distancia actual: {currentDistance:F2}</size>)";
            if(currentDistance <= (maxDistanceThisRep - returnDist) || currentDistance < minDist)
            {
                maxDistanceThisRep = 0.0f;
                maxHeightThisRep = -100.0f;
                maxShoulderAngleThisRep = -90.0f;
                elbowAngleAtMaxReach = 180.0f;
                calibrationState = CalibrationState.WaitingForStretch;
                Invoke("UpdateUI", 1.5f);
                //UpdateUI();
            }
        }*/
    }

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
        if(activeHandL != null && activeHandR == null)
        {
            finalRadioR = finalRadioL;
            finalElbowR = finalElbowL;
        }
        if(activeHandR != null && activeHandL == null)
        {
            finalRadioL = finalRadioR;
            finalElbowL = finalElbowR;
        }
        PlayerPrefs.SetFloat("PlayerRadiusL", finalRadioL);
        PlayerPrefs.SetFloat("PlayerRadiusR", finalRadioR);
        PlayerPrefs.SetFloat("PlayerElbowAngle", (finalElbowL+finalElbowR)/2.0f);
        PlayerPrefs.Save();
        if (instructionText != null)
        {
            instructionText.text = $"Calibración completa.\nRadio guardado: L:{finalRadioL:F2}m R:{finalRadioR:F2}m\nÁngulo de Codo (Flexión): {(finalElbowL + finalElbowR) / 2.0f:F2}°\nIniciando terapia...";
        }
        Invoke("LoadNextScene", 3.0f);
        //foreach(float dist in recordedDistances) sumDist += dist;
        //float meanDistance = sum / recordedDistances.Count;
        /*isCalibrated = true;*/
        //float finalRadio = Mathf.Max((sumDist / recordedDistances.Count) - 0.05f, 0.2f);//meanDistance - 0.05f;
        //finalRadio = Mathf.Max(finalRadio, 0.2f);
        /*float sumHeight = 0;
        foreach(float height in recordedHeights) sumHeight += height;
        float meanHeight = sumHeight / recordedHeights.Count;
        float sumShoulder = 0;
        foreach(float angle in recordedShoulderAngles) sumShoulder += angle;
        float meanShoulderAngle = sumShoulder / recordedShoulderAngles.Count;
        float sumElbow = 0;
        foreach(float angle in recordedElbowAngles) sumElbow += angle;
        float meanElbowAngle = sumElbow / recordedElbowAngles.Count;
        PlayerPrefs.SetFloat("PlayerRadius", finalRadio);
        PlayerPrefs.SetFloat("PlayerMaxHeight", meanHeight);
        PlayerPrefs.SetFloat("PlayerShoulderAngle", meanShoulderAngle);
        PlayerPrefs.SetFloat("PlayerElbowAngle", meanElbowAngle);
        PlayerPrefs.Save();
        if(instructionText != null ) instructionText.text = $"Calibración completa.\nRadio guardado: {finalRadio:F2}m\nÁngulo de Codo (Flexión): {meanElbowAngle:F2}°\nIniciando terapia...";
        Invoke("LoadNextScene", 3.0f);*/
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