using UnityEngine;
using TMPro;

public class WristCalibration : MonoBehaviour
{
    public bool isCalibrating = false;
    public float flexionAngle = 0.0f;
    public float extensionAngle = 0.0f;
    public float supinationAngle = 0.0f;
    public float pronationAngle = 0.0f;
    private Quaternion neutralRotation;
    private bool isCalibrated = false;
    public TextMeshProUGUI counterFlexion;
    public TextMeshProUGUI counterExtension;
    public TextMeshProUGUI counterSupination;
    public TextMeshProUGUI counterPronation;

    // Update is called once per frame
    void Update()
    {
        if (isCalibrating)
        {
            CalibrateNeutralRotation();
            isCalibrating = false;
        }
        if (isCalibrated)
        {
            Vector3 forwardNeutral = neutralRotation * Vector3.forward;
            Vector3 rightNeutral = neutralRotation * Vector3.right;
            float flexExtAngle = Vector3.SignedAngle(forwardNeutral, transform.forward, rightNeutral);
            flexionAngle = 0.0f;
            extensionAngle = 0.0f;
            if (flexExtAngle > 0)
            {
                flexionAngle = flexExtAngle;
            }
            else if (flexExtAngle < 0)
            {
                extensionAngle = Mathf.Abs(flexExtAngle);
            }
            Debug.Log($"Flexion: {flexionAngle:F1}° | Extension: {extensionAngle:F1}°");
            SetCounterExtension();
            SetCounterFlexion();
            float supProAngle = Vector3.SignedAngle(rightNeutral, transform.right, transform.forward);
            supinationAngle = 0.0f;
            pronationAngle = 0.0f;
            if (supProAngle > 0)
            {
                supinationAngle = supProAngle;
            }
            else if (supProAngle < 0)
            {
                pronationAngle = Mathf.Abs(supProAngle);
            }
            Debug.Log($"Supination: {supinationAngle:F1}° | Pronation: {pronationAngle:F1}°");
            SetCounterPronation();
            SetCounterSupination();
        }
    }

    public void CalibrateNeutralRotation()
    {
        neutralRotation = transform.rotation;
        isCalibrated = true;
        Debug.Log("Wrist calibrated. Neutral rotation set.");
    }
    
    public void SetCounterFlexion()
    {
        if (counterFlexion != null)
        {
            counterFlexion.text = $"Flexion: {flexionAngle:F1}°";
        }
    }

    public void SetCounterExtension()
    {
        if (counterExtension != null)
        {
            counterExtension.text = $"Extension: {extensionAngle:F1}°";
        }
    }

    public void SetCounterSupination()
    {
        if (counterSupination != null)
        {
            counterSupination.text = $"Supination: {supinationAngle:F1}°";
        }
    }

    public void SetCounterPronation()
    {
        if (counterPronation != null)
        {
            counterPronation.text = $"Pronation: {pronationAngle:F1}°";
        }
    }
}