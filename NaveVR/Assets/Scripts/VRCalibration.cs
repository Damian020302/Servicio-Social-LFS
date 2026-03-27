using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class VRCalibration : MonoBehaviour
{
    [Header("Calibration Settings")]
    [Tooltip("El centro del jugador")] public Transform playerCenter;
    [Tooltip("El objeto de referencia para medir distancia")] public Transform referencePointL;
    [Tooltip("El objeto de referencia para medir distancia")] public Transform referencePointR;
    [Tooltip("El componente OVRHand para detectar el pellizco")] public OVRHand ovrHandL;
    [Tooltip("El componente OVRHand para detectar el pellizco")] public OVRHand ovrHandR;

    [Header("UI Elements")]
    public TextMeshProUGUI instructionText;
    private bool isCalibrated = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(instructionText != null)
        {
            instructionText.text = "Estira tu brazo sano lo más que puedas.\nSi puedes, haz el gesto de pellizco con tu pulgar e indice.";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isCalibrated) return;
        if(ovrHandL != null && ovrHandL.IsTracked)
        {
            bool isPinchingL = ovrHandL.GetFingerIsPinching(OVRHand.HandFinger.Index);
            if(isPinchingL)
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
        }
    }

    void SaveMaxDistance(int hand)
    {
        isCalibrated = true;
        if (hand == 1)
        {
            float extensionDistance = Vector3.Distance(playerCenter.position, referencePointL.position);
            float finalRadio = extensionDistance - 0.05f;
            PlayerPrefs.SetFloat("PlayerRadius", finalRadio);
            PlayerPrefs.Save();
            if(instructionText != null)
            {
                instructionText.text = $"Calibraci�n completa.\nRadio guardado: {finalRadio:F2}m\n\nIniciando terapia...";
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
                instructionText.text = $"Calibraci�n completa.\nRadio guardado: {finalRadio:F2}m\n\nIniciando terapia...";
                Debug.Log($"Calibraci�n completa. Radio guardado: {finalRadio:F2}m");
                Invoke("LoadNextScene", 3f); // Espera 3 segundos antes de cargar la siguiente escena)
            }
        }
        
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Dificultad");
        Time.timeScale = 1.0f;
    }
}
