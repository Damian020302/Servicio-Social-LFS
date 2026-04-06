using UnityEngine;

public class RealityManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camara principal del Jugador")] public Camera mainCamera;
    [Tooltip("Componente de OVRCameraRig")] public OVRPassthroughLayer passthroughLayer;

    private bool isMixedReality = false;

    private void Start()
    {
        ActivateVR();
    }

    public void AlternateReality()
    {
        isMixedReality = !isMixedReality;
        if(isMixedReality)
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
        passthroughLayer.hidden = false;
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = new Color(0,0,0,0);
        Debug.Log("Mixed Reality Activated");
    }

    void ActivateVR()
    {
        passthroughLayer.hidden = true;
        mainCamera.clearFlags = CameraClearFlags.Skybox;
        Debug.Log("Virtual Reality Activated");
    }
}
