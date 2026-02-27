using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void MainScene()
    {
        SceneManager.LoadScene("MainVR");
        Time.timeScale = 1;
    }

    public void Tracking()
    {
        SceneManager.LoadScene("HandTracking");
        Time.timeScale = 1;
    }
}
