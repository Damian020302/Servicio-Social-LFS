using UnityEngine;

public class WristDebbuger : MonoBehaviour
{
    /*[Header("OpenXRHand")] public Transform wristBone;
    [Header("forearm_stub")] public Transform forearmBone;*/
    private OVRSkeleton skeleton;

    private void Start()
    {
        skeleton = GetComponentInChildren<OVRSkeleton>();
        if (skeleton == null)
        {
            Debug.LogWarning($"No se encontro un OVRSkeleton dentro de {gameObject.name} ¿El Handtracing esta activo?");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(skeleton != null && skeleton.IsInitialized && skeleton.IsDataValid)
        {
            Transform wrist = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_WristRoot].Transform;
            Transform forearm = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_ForearmStub].Transform;
            float flexionAngle = Vector3.SignedAngle(forearm.forward, wrist.forward, forearm.right);
            Debug.Log($"Flexion/Extension: {flexionAngle:F1}");
        }
        //if(wristBone != null && forearmBone != null)
        //{
            /*Vector3 wristRotation = wristBone.localEulerAngles;
            float xAngle = ConvertAngle(wristRotation.x);
            float yAngle = ConvertAngle(wristRotation.y);
            float zAngle = ConvertAngle(wristRotation.x);
            Debug.Log($"Wrist Rotation - X: {xAngle:f2}°, Y: {yAngle:f2}°, Z: {zAngle:f2}°");*/
        /*    float flexionAngle = Vector3.SignedAngle(forearmBone.forward, wristBone.forward, forearmBone.right);
            float extensionAngle = Vector3.SignedAngle(forearmBone.right, wristBone.right, forearmBone.up);
            Debug.Log($"Flexion/Extension: {flexionAngle:F1}| Desviacion: {extensionAngle:F1}");
        }*/
    }

    /*float ConvertAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
        {
            return angle - 360;
        }
        return angle;
    }*/
}
