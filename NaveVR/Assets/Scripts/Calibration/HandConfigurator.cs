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
    }
}