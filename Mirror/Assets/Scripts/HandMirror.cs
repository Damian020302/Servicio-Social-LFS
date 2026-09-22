using UnityEngine;

public class HandMirror : MonoBehaviour
{
    [Header("Mirror References")]
    [Tooltip("Patient's solo hand")]
    public Transform sourceHandAnchor;
    public Transform sourceFingersRoot;
    //public Transform sourceFingersRoot;
    [Tooltip("Patient's center")]
    public Transform mirrorPivot;
    [Header("Mirror Configuration")]
    public bool mirrorFingers = true;
    public bool invertFingerX = false;
    public bool invertFingerY = true;
    public bool invertFingerZ = true;

    // Update is called once per frame
    void LateUpdate()
    {
        if(sourceHandAnchor == null || mirrorPivot == null) return;
        Vector3 localPos = mirrorPivot.InverseTransformPoint(sourceHandAnchor.position);
        localPos.x = -localPos.x;
        transform.position = mirrorPivot.TransformPoint(localPos);
        Quaternion localRot = Quaternion.Inverse(mirrorPivot.rotation) * sourceHandAnchor.rotation;
        localRot.y = -localRot.y;
        localRot.z = -localRot.z;
        transform.rotation = mirrorPivot.rotation * localRot;
        transform.Rotate(transform.up, 180.0f, Space.World);
        if(mirrorFingers/* && sourceFingersRoot != null*/) CopyBonesRecursive(/*sourceFingersRoot*/sourceFingersRoot, transform);
    }

    void CopyBonesRecursive(Transform source, Transform target)
    {
        for(int i = 0; i < source.childCount; i++)
        {
            if(i < target.childCount)
            {
                /*Transform sourceChild = source.GetChild(i);
                Transform targetChild = target.GetChild(i);
                Vector3 euler = sourceChild.localEulerAngles;
                euler.x = -euler.x;
                euler.y = -euler.y;
                targetChild.localEulerAngles = euler;
                CopyBonesRecursive(sourceChild, targetChild);*/
                Transform sourceChild = source.GetChild(i);
                Transform targetChild = target.GetChild(i);
                Vector3 euler = sourceChild.localEulerAngles;
                if (invertFingerX) euler.x = -euler.x;
                if(invertFingerY) euler.y = -euler.y;
                if (invertFingerZ) euler.z = -euler.z;
                targetChild.localEulerAngles = euler;
                //targetChild.localRotation = sourceChild.localRotation;
                CopyBonesRecursive(sourceChild, targetChild);
            }
        }
    }
}