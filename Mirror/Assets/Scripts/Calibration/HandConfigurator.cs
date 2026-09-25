using UnityEngine;

public class HandConfigurator : MonoBehaviour
{
    [Header("Patient's reference")]
    public Transform trackingSpace;

    [Header("Left Hand")]
    public GameObject leftHandInteraction;
    public GameObject leftHandVisuals;
    public GameObject leftHandAnchor;

    [Header("Right Hand")]
    public GameObject rightHandInteraction;
    public GameObject rightHandVisuals;
    public GameObject rightHandAnchor;

    [Header("Mirror Therapy Settings")]
    public bool useMirrorTherapy = true;

    /*void Start()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 0);
        ApplyConfig(selectedHand);
    }*/
    
    public void ApplyConfig(int selectedHand)
    {
        if (selectedHand == 0)
        {
            SetHandState(leftHandInteraction, leftHandVisuals, leftHandAnchor, true);
            ToggleOculusTracking(leftHandVisuals, true);
            HandMirror leftMirror = leftHandVisuals.GetComponent<HandMirror>();
            if (leftMirror) leftMirror.enabled = false;
            SetHandState(rightHandInteraction, rightHandVisuals, rightHandAnchor, false, useMirrorTherapy);
            ToggleOculusTracking(rightHandVisuals, !useMirrorTherapy);
            if (useMirrorTherapy)
            {
                HandMirror rightMirror = SetupMirrorScript(rightHandVisuals, leftHandAnchor, leftHandVisuals);
                if (rightMirror) rightMirror.enabled = true;
            }
            Debug.Log("Terapia de Espejo: Mano Izquierda activa");
        }
        else /*if (selectedHand == 1)*/
        {
            SetHandState(rightHandInteraction, rightHandVisuals, rightHandAnchor, true);
            ToggleOculusTracking(rightHandVisuals, true);
            HandMirror rightMirror = rightHandVisuals.GetComponent<HandMirror>();
            if (rightMirror) rightMirror.enabled = false;
            SetHandState(leftHandInteraction, leftHandVisuals, leftHandAnchor, false, useMirrorTherapy);
            ToggleOculusTracking(leftHandVisuals, !useMirrorTherapy);
            if (useMirrorTherapy)
            {
                HandMirror leftMirror = SetupMirrorScript(leftHandVisuals, rightHandAnchor, rightHandVisuals);
                if (leftMirror) leftMirror.enabled = true;
            }
            Debug.Log("Terapia de Espejo: Mano Derecha activa");
        }
    }

    void SetHandState(GameObject interaction, GameObject visuals, GameObject anchor, bool isReal, bool forceVisuals = false)
    {
        if (interaction != null) interaction.SetActive(isReal);
        if (visuals != null) visuals.SetActive(isReal || forceVisuals);
        if (anchor != null) anchor.SetActive(isReal || forceVisuals);
    }

    HandMirror SetupMirrorScript(GameObject targetVisuals, GameObject sourceAnchor, GameObject sourceVisuals)
    {
        if (targetVisuals == null || sourceAnchor == null || sourceVisuals == null || trackingSpace == null) return null;
        HandMirror mirror = targetVisuals.GetComponent<HandMirror>();
        if (mirror == null) mirror = targetVisuals.AddComponent<HandMirror>();
        mirror.sourceAnchor = sourceAnchor.transform;
        mirror.sourceFingersRoot = sourceVisuals.transform;
        mirror.trackingSpace = trackingSpace;
        return mirror;
    }

    void ToggleOculusTracking(GameObject visuals, bool state)
    {
        if (visuals == null) return;
        MonoBehaviour[] scripts = visuals.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour s in scripts)
        {
            string name = s.GetType().Name;
            if (name == "OVRHand" || name == "OVRSkeleton" || name == "OVRMeshRenderer") s.enabled = state;
        }

        if(!state)
        {
            SkinnedMeshRenderer smr = visuals.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                smr.enabled = true;
                smr.updateWhenOffscreen = true;
            }   
        }
    }
}