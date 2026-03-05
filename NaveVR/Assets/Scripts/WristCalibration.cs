using UnityEngine;

public class WristCalibration : MonoBehaviour
{
    public bool isCalibrating = false;
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
            float flexionAngle = Vector3.SignedAngle(forwardNeutral, transform.forward, rightNeutral);

            Debug.Log($"Flexion/Extension: {flexionAngle:F1}");
        }
    }

    public void CalibrateNeutralRotation()
    {
        neutralRotation = transform.rotation;
        isCalibrated = true;
        Debug.Log("Wrist calibrated. Neutral rotation set.");
    }
}
