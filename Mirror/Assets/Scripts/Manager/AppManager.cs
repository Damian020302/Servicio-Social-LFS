using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject handMenu;
    public GameObject mainMenu;
    public GameObject bothHandsGame;
    public GameObject leftOrRightHandGame;

    public void Game1()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1.0f;
    }

    public void Game2()
    {
        SceneManager.LoadScene("Menu2");
        Time.timeScale = 1.0f;
    }

    public void Game3()
    {
        SceneManager.LoadScene("Menu3");
        Time.timeScale = 1.0f;
    }

    public void Game4()
    {
        SceneManager.LoadScene("Menu4");
        Time.timeScale = 1.0f;
    }

    void Menus()
    {
        handMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void LeftHandSelection()
    {
        Menus();
        bothHandsGame.SetActive(false);
        leftOrRightHandGame.SetActive(true);
        PlayerPrefs.SetInt("SelectedHand", 0); //0 for left hand
        PlayerPrefs.Save();
        HandConfigurator configurator = Object.FindFirstObjectByType<HandConfigurator>();
        if (configurator != null) configurator.ApplyConfig(0); //Applies configuration to left hand
        Debug.Log("Mano izquierda seleccionada");
    }

    public void RightHandSelection()
    {
        Menus();
        bothHandsGame.SetActive(false);
        leftOrRightHandGame.SetActive(true);
        PlayerPrefs.SetInt("SelectedHand", 1); //1 for right hand
        PlayerPrefs.Save();
        HandConfigurator configurator = Object.FindFirstObjectByType<HandConfigurator>();
        if (configurator != null) configurator.ApplyConfig(1); //Applies configuration to right hand
        Debug.Log("Mano derecha seleccionada");
    }

    public void BothHandsSelection()
    {
        Menus();
        bothHandsGame.SetActive(true);
        leftOrRightHandGame.SetActive(false);
        PlayerPrefs.SetInt("SelectedHand", 2); //2 for both hands
        PlayerPrefs.Save();
        HandConfigurator configurator = Object.FindFirstObjectByType<HandConfigurator>();
        if (configurator != null) configurator.ApplyConfig(2); //Applies configuration to both hands
        Debug.Log("Ambas manos seleccionadas");
    }
}