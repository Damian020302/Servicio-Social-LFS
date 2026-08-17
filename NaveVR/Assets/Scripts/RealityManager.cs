using TMPro;
using UnityEngine;

public class RealityManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camara principal del Jugador")] public Camera mainCamera;
    [Tooltip("Componente de OVRCameraRig")] public OVRPassthroughLayer passthroughLayer;
    private bool isMixedReality = false;
    public GameObject vrSign;
    public GameObject mrSign;

    private void Start()
    {
        int savedReality = PlayerPrefs.GetInt("RealityMode", 0);
        isMixedReality = (savedReality == 1);
        if(isMixedReality)
        {
            ActivateMR();
        }
        else
        {
            ActivateVR();
        }
    }

    public void AlternateReality()
    {
        isMixedReality = !isMixedReality;
        PlayerPrefs.SetInt("RealityMode", isMixedReality ? 1 : 0);
        PlayerPrefs.Save();
        if (isMixedReality)
        {
            ActivateMR();
        }
        else
        {
            ActivateVR();
        }
    }

    void ActivateMR()
    {
        vrSign.SetActive(false);
        mrSign.SetActive(true);
        if(OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = true;
            Debug.Log("Ya debería funcionar");
        }

        if(passthroughLayer != null)
        {
            passthroughLayer.enabled = true;
            passthroughLayer.hidden = false;
        }
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = new Color(0,0,0,0);
        Debug.Log("Mixed Reality Activated");
    }

    void ActivateVR()
    {
        mrSign.SetActive(false);
        vrSign.SetActive(true);
        if(OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = false;
        }
        if (passthroughLayer != null)
        {
            passthroughLayer.enabled = false;
            passthroughLayer.hidden = true;
        }
        mainCamera.clearFlags = CameraClearFlags.Skybox;
        Debug.Log("Virtual Reality Activated");
    }
}
