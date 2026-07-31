using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleGrabManager : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject victory;
    public GameObject defeat;
    public GameObject yesD;
    public GameObject noD;
    public GameObject yesV;
    public GameObject noV;

    [Header("UI")]
    public TextMeshProUGUI clawText;
    public TextMeshProUGUI timeRemainingText;

    [Header("Hand Setup")]
    public OVRHand leftHand;
    public OVRHand rightHand;
    private OVRHand activeHand;
    [Tooltip("El punto donde se sujetaran los robots")]
    public Transform leftPalm;
    public Transform rightPalm;
    private Transform activePalm;

    [Header("Grab Settings")]
    public float grabRadius = 0.15f;
    public string grabbableTag = "Grabbable";

    [Header("Calibration Data")]
    private float grabThreshold;
    private float releaseThreshold = 0.2f;
    private bool isGrabbing = false;
    private Rigidbody grabbedObject;

    public void OnClickNo()
    {
        SceneManager.LoadScene("Menu4");
        Time.timeScale = 1.0f;
    }

    public void MenuGeneral()
    {
        SceneManager.LoadScene("MenuGeneral");
        Time.timeScale = 1.0f;
    }

    public void MainScene()
    {
        SceneManager.LoadScene("Juego4");
        Time.timeScale = 1.0f;
    }

    public void Calibrate()
    {
        SceneManager.LoadScene("Calibracion4");
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 1);
        if(selectedHand == 0 && leftHand != null)
        {
            activeHand = leftHand;
            activePalm = leftPalm != null ? leftPalm : leftHand.transform;
        }
        else if(selectedHand == 1 && rightHand != null)
        {
            activeHand = rightHand;
            activePalm = rightPalm != null ? rightPalm : rightHand.transform;
        }
        else
        {
            Debug.Log("No se encontro una mano activa");
        }

        float maxGrabStrength = PlayerPrefs.GetFloat("MaxGrabStrength", 0.7f);
        grabThreshold = maxGrabStrength * 0.8f; // 80% of the max grab strength
        Debug.Log($"Meta de agarre: {(grabThreshold * 100):F0}% | Meta para soltar: {(releaseThreshold * 100):F0}%");
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

    void Update()
    {
        if(activeHand == null || !activeHand.IsTracked) return;
        float currentGrip = GetCurrentGrip();
        if(!isGrabbing && currentGrip >= grabThreshold)
        {
            TryGrabObject();
        }
        else if(isGrabbing && currentGrip <= releaseThreshold)
        {
            ReleaseObject();
        }
    }

    void TryGrabObject()
    {
        if(activePalm == null) return;
        Collider[] hits = Physics.OverlapSphere(activePalm.position, grabRadius);
        if(hits.Length > 0)
        {
            Debug.Log($"Cerraste el puño, objeto dentro de los 15 cm: {hits.Length}");
        }
        float closestDistance = float.MaxValue;
        Rigidbody bestTarget = null;
        foreach(Collider hit in hits)
        {
            Debug.Log($"Objeto detectado: {hit.gameObject.name} con tag {hit.tag}");
            if (hit.CompareTag(grabbableTag))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float distance = Vector3.Distance(activePalm.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        bestTarget = rb;
                    }
                }
                else
                {
                    Debug.LogWarning($"El objeto {hit.name} tiene el tag {grabbableTag} pero no tiene un Rigidbody.");
                }
            }
        }
        if(bestTarget != null)
        {
            isGrabbing = true;
            grabbedObject = bestTarget;
            grabbedObject.isKinematic = true;
            grabbedObject.transform.SetParent(activePalm);
        }
    }

    void ReleaseObject()
    {
        if(grabbedObject != null)
        {
            grabbedObject.transform.SetParent(null);
            grabbedObject.isKinematic = false;
            grabbedObject = null;
        }
        isGrabbing = false;
    }

    private void OnDrawGizmosSelected()
    {
        if(activePalm != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(activePalm.position, grabRadius);
        }
    }
}
