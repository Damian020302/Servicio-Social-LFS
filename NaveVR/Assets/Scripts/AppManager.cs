using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using Meta.XR.ImmersiveDebugger.UserInterface.Generic;


public class AppManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject handMenu;
    public GameObject mainMenu;
    /*[Header("Timer Configuration")]
    public GameObject timerPanel;
    public TextMeshProUGUI timeDisplay;
    public Toggle useTimerToggle;
    private float selectedTime = 60.0f;
    private string sceneToLoad;*/

    public void Game1()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void Game2()
    {
        SceneManager.LoadScene("Menu2");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void Game3()
    {
        SceneManager.LoadScene("Menu3");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void Game4()
    {
        SceneManager.LoadScene("Menu4");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    public void LeftHandSelection()
    {
        handMenu.SetActive(false);
        mainMenu.SetActive(true);
        PlayerPrefs.SetInt("SelectedHand", 0); // 0 para mano izquierda
        PlayerPrefs.Save();
        HandConfigurator configurator = Object.FindFirstObjectByType<HandConfigurator>();
        if (configurator != null)
        {
            configurator.ApplyConfig(0); // Aplica la configuración para mano izquierda
        }
        Debug.Log("Mano izquierda seleccionada");
    }

    public void RightHandSelection()
    {
        handMenu.SetActive(false);
        mainMenu.SetActive(true);
        PlayerPrefs.SetInt("SelectedHand", 1); // 1 para mano derecha
        PlayerPrefs.Save();
        HandConfigurator configurator = Object.FindFirstObjectByType<HandConfigurator>();
        if (configurator != null)
        {
            configurator.ApplyConfig(1); // Aplica la configuración para mano derecha
        }
        Debug.Log("Mano derecha seleccionada");
    }
    /*
    public void ShowTimerPanel(string nextScene)
    {
        sceneToLoad = nextScene;
        if(timerPanel != null)
        {
            timerPanel.SetActive(true);
        }
        UpdateTimeDisplay();
    }

    public void IncreaseTime()
    {
        selectedTime += 30.0f; // Incrementa en 10 segundos
        UpdateTimeDisplay();
    }

    public void DecreaseTime()
    {
        if(selectedTime > 30.0f) // Evita que el tiempo sea menor a 10 segundos
        {
            selectedTime -= 30.0f; // Decrementa en 10 segundos
        }
        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        if(timeDisplay != null)
        {
            float minutes = Mathf.FloorToInt(selectedTime / 60);
            float seconds = Mathf.FloorToInt(selectedTime % 60);
            timeDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void ConfirmAndStartGame()
    {
        PlayerPrefs.SetInt("UseTimer", useTimerToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("SessionTime", selectedTime);
        PlayerPrefs.Save();
        if(timerPanel != null)
        {
            timerPanel.SetActive(false);
        }
        SceneManager.LoadScene(sceneToLoad);
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al iniciar el juego
    }*/
}
