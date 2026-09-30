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
    private SkinnedMeshRenderer smr;

    void Start()
    {
        smr = GetComponentInChildren<SkinnedMeshRenderer>(true);
    }

    void LateUpdate()
    {
        if (sourceAnchor == null || trackingSpace == null || sourceFingersRoot == null) return;
        Vector3 localPos = trackingSpace.InverseTransformPoint(sourceAnchor.position);
        localPos.x = -localPos.x;
        transform.position = trackingSpace.TransformPoint(localPos);
        Quaternion sourceLocalRot = Quaternion.Inverse(trackingSpace.rotation) * sourceFingersRoot.rotation;
        Quaternion mirroredLocalRol = new Quaternion(sourceLocalRot.x, -sourceLocalRot.y, -sourceLocalRot.z, sourceLocalRot.w);
        transform.rotation = trackingSpace.rotation * mirroredLocalRol;
        if(flipPalms) transform.Rotate(transform.up, 180.0f, Space.World);
        if(mirrorFingers) CopyBonesRecursive(sourceFingersRoot, transform);
        if(smr != null && !smr.enabled) smr.enabled = true;
    }

    void CopyBonesRecursive(Transform source, Transform target)
    {
        for (int i = 0; i < source.childCount; i++)
        {
            if (i < target.childCount)
            {
                Transform sourceChild = source.GetChild(i);
                Transform targetChild = target.GetChild(i);
                /*Vector3 euler = sourceChild.localEulerAngles;
                if (invertFingerX) euler.x = -euler.x;
                if (invertFingerY) euler.y = -euler.y;
                if (invertFingerZ) euler.z = -euler.z;
                targetChild.localEulerAngles = euler;*/
                Quaternion sourceFingerRot = sourceChild.localRotation;
                float x = sourceFingerRot.x;
                float y = invertFingerY ? -sourceFingerRot.y : sourceFingerRot.y;
                float z = invertFingerZ ? -sourceFingerRot.z : sourceFingerRot.z;
                float w = sourceFingerRot.w;
                targetChild.localRotation = new Quaternion(x, y, z, w);
                CopyBonesRecursive(sourceChild, targetChild);
            }
        }
    }
}