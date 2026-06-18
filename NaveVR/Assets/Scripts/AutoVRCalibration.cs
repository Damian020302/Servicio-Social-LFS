using UnityEngine;
using System.Collections;

public class AutoVRCalibration : MonoBehaviour
{
    [Header("VR Reference")]
    public Transform vrCamera;
    public Transform vrRig;

    [Header("Wished Position")]
    public Transform targetHeadPosition;

    public float delayBeforeCalibration = 0.01f;

    private void Start()
    {
        StartCoroutine(CalibrateRoutine());
    }

    public void ManualCalibration()
    {
        StartCoroutine(CalibrateRoutine());
    }

    private IEnumerator CalibrateRoutine()
    {
        yield return new WaitForSeconds(delayBeforeCalibration);
        if(vrRig != null && vrCamera != null && targetHeadPosition != null)
        {
            Vector3 offsetPosition = targetHeadPosition.position - vrCamera.position;
            vrRig.position += offsetPosition;
            float offsetRotation = targetHeadPosition.eulerAngles.y - vrCamera.eulerAngles.y;
            vrRig.RotateAround(vrCamera.position, Vector3.up, offsetRotation);
        }
        else
        {
            Debug.LogWarning("AutoVRCalibration: Missing reference(s) for calibration.");
        }
    }
}