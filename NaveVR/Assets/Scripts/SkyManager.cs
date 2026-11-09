using UnityEngine;

public class SkyManager : MonoBehaviour
{
    public float skySpeed;
    
    void Update()
    {
        if(RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * skySpeed);
        }
    }
}