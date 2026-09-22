using UnityEngine;

public class MirrorTherapyManager : MonoBehaviour
{
    [Header("Patient's reference")]
    public Transform playerCenter;

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

    void Start()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 0);
        ConfigureMirrorTherapy(selectedHand);
    }

    void ConfigureMirrorTherapy(int selectedHand)
    {
        HandMirror leftMirror = SetupMirrorScript(leftHandVisuals, rightHandVisuals);
        HandMirror rightMirror = SetupMirrorScript(rightHandVisuals, leftHandVisuals);
        if(selectedHand == 0)
        {
            SetHandState(leftHandInteraction, leftHandVisuals, leftHandAnchor, true);
            ToggleOculusTracking(leftHandVisuals, true);
            if(leftMirror) leftMirror.enabled = false;
            SetHandState(rightHandInteraction, rightHandVisuals, rightHandAnchor, false, useMirrorTherapy);
            ToggleOculusTracking(leftHandVisuals, !useMirrorTherapy);
            if (leftMirror) leftMirror.enabled = useMirrorTherapy;
            Debug.Log("Terapia de Espejo: Mano Izquierda activa");
        }
        else if(selectedHand == 1)
        {
            SetHandState(leftHandInteraction, leftHandVisuals, leftHandAnchor, false, useMirrorTherapy);
            ToggleOculusTracking(leftHandVisuals, !useMirrorTherapy);
            if (leftMirror) leftMirror.enabled = useMirrorTherapy;
            SetHandState(rightHandInteraction, rightHandVisuals, rightHandAnchor, true);
            ToggleOculusTracking(leftHandVisuals, true);
            if (leftMirror) leftMirror.enabled = false;
            Debug.Log("Terapia de Espejo: Mano Derecha activa");
        }
    }

    void SetHandState(GameObject interaction, GameObject visuals, GameObject anchor, bool isReal, bool forceVisuals = false)
    {
        if (interaction != null) interaction.SetActive(isReal);
        if(visuals != null) visuals.SetActive(isReal || forceVisuals);
        if(anchor != null) anchor.SetActive(isReal || forceVisuals);
    }

    HandMirror SetupMirrorScript(GameObject targetVisuals, GameObject sourceVisuals)
    {
        if(targetVisuals == null || sourceVisuals == null || playerCenter == null) return null;
        HandMirror mirror = targetVisuals.GetComponent<HandMirror>();
        if (mirror == null) mirror = targetVisuals.AddComponent<HandMirror>();
        //mirror.sourceHand = sourceVisuals.transform;
        mirror.mirrorPivot = playerCenter;
        return mirror;
    }

    void ToggleOculusTracking(GameObject visuals, bool state)
    {
        if (visuals == null) return;
        MonoBehaviour[] scripts = visuals.GetComponentsInChildren<MonoBehaviour>();
        foreach(MonoBehaviour s in scripts)
        {
            string name = s.GetType().Name;
            if(name == "OVRHand" || name == "OVRSkeleton") s.enabled = state;
        }
    }
}
