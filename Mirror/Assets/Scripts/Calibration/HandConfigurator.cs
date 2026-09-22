using UnityEngine;

public class HandConfigurator : MonoBehaviour
{
    /*[Header("Left Hand References")]
    public GameObject leftHandInteraction;
    public GameObject leftHandVisuals;
    public GameObject leftHandAnchor;

    [Header("Right Hand References")]
    public GameObject rightHandInteraction;
    public GameObject rightHandVisuals;
    public GameObject rightHandAnchor;

    void Start()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 0); //0 for left hand, 1 for right hand, 2 for both hands
        ApplyConfig(selectedHand);
    }

    /// <summary>
    /// Applies the hand configuration based on the selected hand preference.
    /// </summary>
    /// <param name="selectedHand">0 for left hand, 1 for right hand, 2 for both hands</param>
    public void ApplyConfig(int selectedHand)
    {
        if (selectedHand == 0)
        {
            if (leftHandInteraction != null) leftHandInteraction.SetActive(true);
            if (leftHandVisuals != null) leftHandVisuals.SetActive(true);
            if (leftHandAnchor != null) leftHandAnchor.SetActive(true);
            if (rightHandInteraction != null) rightHandInteraction.SetActive(false);
            if (rightHandVisuals != null) rightHandVisuals.SetActive(false);
            if (rightHandAnchor != null) rightHandAnchor.SetActive(false);
        }
        else if(selectedHand == 1)
        {
            if (leftHandInteraction != null) leftHandInteraction.SetActive(false);
            if (leftHandVisuals != null) leftHandVisuals.SetActive(false);
            if (leftHandAnchor != null) leftHandAnchor.SetActive(false);
            if (rightHandInteraction != null) rightHandInteraction.SetActive(true);
            if (rightHandVisuals != null) rightHandVisuals.SetActive(true);
            if (rightHandAnchor != null) rightHandAnchor.SetActive(true);
        }
        else
        {
            if (leftHandInteraction != null) leftHandInteraction.SetActive(true);
            if (leftHandVisuals != null) leftHandVisuals.SetActive(true);
            if (leftHandAnchor != null) leftHandAnchor.SetActive(true);
            if (rightHandInteraction != null) rightHandInteraction.SetActive(true);
            if (rightHandVisuals != null) rightHandVisuals.SetActive(true);
            if (rightHandAnchor != null) rightHandAnchor.SetActive(true);
        }
    }*/
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
        ApplyConfig(selectedHand);
    }

    public void ApplyConfig(int selectedHand)
    {
        //HandMirror leftMirror = SetupMirrorScript(leftHandVisuals, rightHandVisuals);
        //HandMirror rightMirror = SetupMirrorScript(rightHandVisuals, leftHandVisuals);
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
                if(rightMirror) rightMirror.enabled = true;
            } 
            /*SetHandState(leftHandInteraction, leftHandVisuals, leftHandAnchor, true);
            SetHandState(rightHandInteraction, rightHandVisuals, rightHandAnchor, false);
            if(useMirrorTherapy && fakeRightHand != null)
            {
                fakeRightHand.SetActive(true);
                SetupMirrorScript(fakeRightHand, leftHandAnchor.gameObject, leftHandVisuals);
            }*/
            Debug.Log("Terapia de Espejo: Mano Izquierda activa");
        }
        else if (selectedHand == 1)
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
            /*SetHandState(leftHandInteraction, leftHandVisuals, leftHandAnchor, false, useMirrorTherapy);
            ToggleOculusTracking(leftHandVisuals, !useMirrorTherapy);
            if (leftMirror) leftMirror.enabled = useMirrorTherapy;
            SetHandState(rightHandInteraction, rightHandVisuals, rightHandAnchor, true);
            ToggleOculusTracking(rightHandVisuals, true);
            if (rightMirror) rightMirror.enabled = false;*/
            /*SetHandState(rightHandInteraction, rightHandVisuals, rightHandAnchor, true);
            SetHandState(leftHandInteraction, leftHandVisuals, leftHandAnchor, false);
            if (useMirrorTherapy && fakeLeftHand != null)
            {
                fakeLeftHand.SetActive(true);
                SetupMirrorScript(fakeLeftHand, rightHandAnchor.gameObject, rightHandVisuals);
            }*/
            Debug.Log("Terapia de Espejo: Mano Derecha activa");
        }
    }

    void SetHandState(GameObject interaction, GameObject visuals, GameObject anchor, bool isReal, bool forceVisuals = false)
    {
        if (interaction != null) interaction.SetActive(isReal);
        if (visuals != null) visuals.SetActive(isReal || forceVisuals);
        if (anchor != null) anchor.SetActive(isReal || forceVisuals);
    }

    /*HandMirror SetupMirrorScript(GameObject targetVisuals, GameObject sourceVisuals)
    {
        if (targetVisuals == null || sourceVisuals == null || playerCenter == null) return null;
        HandMirror mirror = targetVisuals.GetComponent<HandMirror>();
        if (mirror == null) mirror = targetVisuals.AddComponent<HandMirror>();
        mirror.sourceHand = sourceVisuals.transform;
        mirror.mirrorPivot = playerCenter;
        return mirror;
    }*/

    HandMirror SetupMirrorScript(GameObject targetVisuals, GameObject sourceAnchor, GameObject sourceVisuals)
    {
        if (targetVisuals == null || sourceAnchor == null || sourceVisuals == null || playerCenter == null) return null;
        HandMirror mirror = targetVisuals.GetComponent<HandMirror>();
        if (mirror == null) mirror = targetVisuals.AddComponent<HandMirror>();
        mirror.sourceHandAnchor = sourceAnchor.transform;
        mirror.sourceFingersRoot = sourceVisuals.transform;
        mirror.mirrorPivot = playerCenter;
        return mirror;
    }

    void ToggleOculusTracking(GameObject visuals, bool state)
    {
        if (visuals == null) return;
        MonoBehaviour[] scripts = visuals.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour s in scripts)
        {
            string name = s.GetType().Name;
            if (name == "OVRHand" || name == "OVRSkeleton") s.enabled = state;
        }
    }
}