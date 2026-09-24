using UnityEngine;

public class HandMirror : MonoBehaviour
{
    [Header("Mirror References")]
    public Transform sourceAnchor;
    public Transform sourceFingersRoot;
    public Transform trackingSpace;
    [Header("Mirror Configuration")]
    public bool mirrorFingers = true;
    public bool flipPalms = false;
    [Header("Oculus Fingers")]
    public bool invertFingerX = false;
    public bool invertFingerY = true;
    public bool invertFingerZ = true;

    void LateUpdate()
    {
        if (sourceAnchor == null || trackingSpace == null) return;
        Vector3 localPos = trackingSpace.InverseTransformPoint(sourceAnchor.position);
        localPos.x = -localPos.x;
        transform.position = trackingSpace.TransformPoint(localPos);
        Quaternion sourceLocalRot = Quaternion.Inverse(trackingSpace.rotation) * sourceFingersRoot.rotation;
        Quaternion mirroredLocalRol = new Quaternion(sourceLocalRot.x, -sourceLocalRot.y, -sourceLocalRot.z, sourceLocalRot.w);
        /*Quaternion localRot = Quaternion.Inverse(trackingSpace.rotation) * sourceFingersRoot.rotation; //sourceAnchor.rotation;
        localRot.y = -localRot.y;
        localRot.z = -localRot.z;
        transform.rotation = trackingSpace.rotation * localRot;*/
        transform.rotation = trackingSpace.rotation * mirroredLocalRol;
        if(flipPalms) transform.Rotate(transform.up, 180.0f, Space.World);
        if(mirrorFingers/* && sourceFingersRoot != null*/) CopyBonesRecursive(sourceFingersRoot, transform);
    }

    void CopyBonesRecursive(Transform source, Transform target)
    {
        for (int i = 0; i < source.childCount; i++)
        {
            if (i < target.childCount)
            {
                Transform sourceChild = source.GetChild(i);
                Transform targetChild = target.GetChild(i);
                Vector3 euler = sourceChild.localEulerAngles;
                if (invertFingerX) euler.x = -euler.x;
                if (invertFingerY) euler.y = -euler.y;
                if (invertFingerZ) euler.z = -euler.z;
                targetChild.localEulerAngles = euler;
                CopyBonesRecursive(sourceChild, targetChild);
            }
        }
    }
}