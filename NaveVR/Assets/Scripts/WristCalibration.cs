using UnityEngine;

public class WristCalibration : MonoBehaviour
{
    public bool isCalibrating = false;
    public float flexionAngle = 0.0f;
    public float extensionAngle = 0.0f;
    private Quaternion neutralRotation;
    private bool isCalibrated = false;
    
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
            float neutralAngle = Vector3.SignedAngle(forwardNeutral, transform.forward, rightNeutral);
            flexionAngle = 0.0f;
            extensionAngle = 0.0f;
            if (neutralAngle > 0)
            {
                flexionAngle = neutralAngle;
            }
            else if (neutralAngle < 0)
            {
                extensionAngle = Mathf.Abs(neutralAngle);
            }
            Debug.Log($"Flexion: {flexionAngle:F2}° | Extension: {extensionAngle:F1}°");
        }
    }

    public void CalibrateNeutralRotation()
    {
        neutralRotation = transform.rotation;
        isCalibrated = true;
        Debug.Log("Wrist calibrated. Neutral rotation set.");
    }
}
