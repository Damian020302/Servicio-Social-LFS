using UnityEngine;

public class HandConfigurator : MonoBehaviour
{
    [Header("Left Hand References")]
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

    public void ApplyConfig(int selectedHand)
    {
        if (selectedHand == 0)
        {
            //Configure for left hand
            if (leftHandInteraction != null) leftHandInteraction.SetActive(true);
            if (leftHandVisuals != null) leftHandVisuals.SetActive(true);
            if (leftHandAnchor != null) leftHandAnchor.SetActive(true);
            if (rightHandInteraction != null) rightHandInteraction.SetActive(false);
            if (rightHandVisuals != null) rightHandVisuals.SetActive(false);
            if (rightHandAnchor != null) rightHandAnchor.SetActive(false);
        }
        else if(selectedHand == 1)
        {
            //Configure for right hand
            if (leftHandInteraction != null) leftHandInteraction.SetActive(false);
            if (leftHandVisuals != null) leftHandVisuals.SetActive(false);
            if (leftHandAnchor != null) leftHandAnchor.SetActive(false);
            if (rightHandInteraction != null) rightHandInteraction.SetActive(true);
            if (rightHandVisuals != null) rightHandVisuals.SetActive(true);
            if (rightHandAnchor != null) rightHandAnchor.SetActive(true);
        }
        else
        {
            //Configure for both hands
            if (leftHandInteraction != null) leftHandInteraction.SetActive(true);
            if (leftHandVisuals != null) leftHandVisuals.SetActive(true);
            if (leftHandAnchor != null) leftHandAnchor.SetActive(true);
            if (rightHandInteraction != null) rightHandInteraction.SetActive(true);
            if (rightHandVisuals != null) rightHandVisuals.SetActive(true);
            if (rightHandAnchor != null) rightHandAnchor.SetActive(true);
        }
    }
}