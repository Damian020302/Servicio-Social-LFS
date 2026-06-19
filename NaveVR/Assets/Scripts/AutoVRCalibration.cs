using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoVRCalibration : MonoBehaviour
{
    [Header("VR Reference")]
    public Transform vrCamera;
    public Transform vrRig;

    [Header("Wished Position")]
    public Transform targetHeadPosition;

    [Header("Fade Screentime")]
    public float fadeOutTime = 1.0f;
    public float delayBeforeCalibration = 0.45f;

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
        GameObject fadeScreen = new GameObject("FadeScreen");
        fadeScreen.transform.SetParent(vrCamera);
        fadeScreen.transform.localPosition = new Vector3(0, 0, 0.5f);
        fadeScreen.transform.localRotation = Quaternion.identity;
        Canvas canvas = fadeScreen.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 3200;
        RectTransform rect = fadeScreen.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(10.0f, 10.0f);
        Image blackScreen = fadeScreen.AddComponent<Image>();
        blackScreen.color = new Color(0, 0, 0, 1.0f);
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
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeOutTime);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        Destroy(fadeScreen);
        Debug.Log("AutoVRCalibration: Calibration completed.");
    }
}