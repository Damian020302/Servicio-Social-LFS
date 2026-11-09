using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RealityManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camara principal del Jugador")] public Camera mainCamera;
    [Tooltip("Componente de OVRCameraRig")] public OVRPassthroughLayer passthroughLayer;
    private bool isMixedReality = false;
    public GameObject vrSign;
    public GameObject mrSign;

    private Material savedSkyboxMaterial;

    private void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if(passthroughLayer == null)
        {
            passthroughLayer = Object.FindFirstObjectByType<OVRPassthroughLayer>();
        }
        savedSkyboxMaterial = RenderSettings.skybox;
        if (OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = true;
        }
        if(passthroughLayer != null)
        {
            passthroughLayer.enabled = true;
        }
        int savedReality = PlayerPrefs.GetInt("RealityMode", 0);
        isMixedReality = (savedReality == 1);
        if(isMixedReality) ActivateMR();
        else ActivateVR();
    }

    public void AlternateReality()
    {
        isMixedReality = !isMixedReality;
        PlayerPrefs.SetInt("RealityMode", isMixedReality ? 1 : 0);
        PlayerPrefs.Save();
        if (isMixedReality) ActivateMR();
        else ActivateVR();
    }

    /// <summary>
    /// Activates Mixed Reality mode.
    /// </summary>
    void ActivateMR()
    {
        if(vrSign != null)
        {
            vrSign.SetActive(false);
        }
        if(mrSign != null)
        {
            mrSign.SetActive(true);
        }
        /*if(OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = true;
            Debug.Log("Ya debería funcionar");
        }*/
        if(passthroughLayer != null)
        {
            //passthroughLayer.enabled = true;
            passthroughLayer.hidden = false;
            passthroughLayer.textureOpacity = 1.0f;
        }
        RenderSettings.skybox = null;
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0, 0, 0, 0);
            mainCamera.allowHDR = false;
            var urpCameraData = mainCamera.GetUniversalAdditionalCameraData();
            if(urpCameraData != null)
            {
                urpCameraData.renderPostProcessing = false;
            }
        }
        Debug.Log("Mixed Reality Activated");
    }

    /// <summary>
    /// Activates Virtual Reality mode.
    /// </summary>
    void ActivateVR()
    {
        if(mrSign != null)
        {
            mrSign.SetActive(false);
        }
        if(vrSign != null)
        {
            vrSign.SetActive(true);
        }
        /*if (OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = false;
        }*/
        if (passthroughLayer != null)
        {
            //passthroughLayer.enabled = false;
            passthroughLayer.hidden = true;
        }
        if(savedSkyboxMaterial != null)
        {
            RenderSettings.skybox = savedSkyboxMaterial;
        }
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.Skybox;
        }
        Debug.Log("Virtual Reality Activated");
    }
}