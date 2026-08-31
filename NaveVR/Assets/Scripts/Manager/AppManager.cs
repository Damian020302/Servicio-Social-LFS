using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject handMenu;
    public GameObject mainMenu;

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
}