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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int selectedHand = PlayerPrefs.GetInt("SelectedHand", 0); // 0 para mano izquierda, 1 para mano derecha
        ApplyConfig(selectedHand);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyConfig(int selectedHand)
    {
        if (selectedHand == 0)
        {
            // Configurar para mano izquierda
            if (leftHandInteraction != null) leftHandInteraction.SetActive(true);
            if (leftHandVisuals != null) leftHandVisuals.SetActive(true);
            if (leftHandAnchor != null) leftHandAnchor.SetActive(true);
            if (rightHandInteraction != null) rightHandInteraction.SetActive(false);
            if (rightHandVisuals != null) rightHandVisuals.SetActive(false);
            if (rightHandAnchor != null) rightHandAnchor.SetActive(false);
        }
        else if(selectedHand == 1)
        {
            // Configurar para mano derecha
            if (leftHandInteraction != null) leftHandInteraction.SetActive(false);
            if (leftHandVisuals != null) leftHandVisuals.SetActive(false);
            if (leftHandAnchor != null) leftHandAnchor.SetActive(false);
            if (rightHandInteraction != null) rightHandInteraction.SetActive(true);
            if (rightHandVisuals != null) rightHandVisuals.SetActive(true);
            if (rightHandAnchor != null) rightHandAnchor.SetActive(true);
        }
        else
        {
            // Configurar para ambas manos
            if (leftHandInteraction != null) leftHandInteraction.SetActive(true);
            if (leftHandVisuals != null) leftHandVisuals.SetActive(true);
            if (leftHandAnchor != null) leftHandAnchor.SetActive(true);
            if (rightHandInteraction != null) rightHandInteraction.SetActive(true);
            if (rightHandVisuals != null) rightHandVisuals.SetActive(true);
            if (rightHandAnchor != null) rightHandAnchor.SetActive(true);
        }
    }
}
