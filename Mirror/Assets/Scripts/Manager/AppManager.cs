using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject leftHandMenu;
    public GameObject rightHandMenu;
    public GameObject handMenu;
    public GameObject mainMenu;
    public GameObject bothHandsGame;
    public GameObject leftOrRightHandGame;
    HandConfigurator configurator;
    public TextMeshProUGUI instructions;

    /*[Header("Loading System")]
    public GameObject loadingTextUI;
    public GameObject selectionButtonsUI;*/

    void Start()
    {
        configurator = Object.FindFirstObjectByType<HandConfigurator>();
        if(configurator.leftHandReady && configurator.rightHandReady)
        {
            Debug.Log("Hardware listo para usar");
            instructions.text = "¡Perfecto!.\nSistema calibrado, puede alternar mano en cualquier momento.";
        }
        else
        {
            Debug.Log("Hardware no listo para usar");
            handMenu.SetActive(true);
            leftHandMenu.SetActive(true);
            rightHandMenu.SetActive(false);
            mainMenu.SetActive(false);
            instructions.text = "Bienvenid@s.\nComencemos con la calibración del sistema:\nPaciente - Si puede, estire su brazo y ponga su palma izquierda frente al visor. \nTerapeuta - Seleccione la mano Izquierda.";
        }
        /*if(loadingTextUI != null) loadingTextUI.SetActive(true);
        if(selectionButtonsUI != null) selectionButtonsUI.SetActive(false);*/
    }

    /*public void UnlockHandSelection()
    {
        if(loadingTextUI != null) loadingTextUI.SetActive(false);
        if(selectionButtonsUI != null) selectionButtonsUI.SetActive(true);
        Debug.Log("Botones de selección de mano desbloqueados");
    }*/

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

    public void EndGame()
    {
        Application.Quit();
    }

    public void LeftHandSelection()
    {
        //Menus();
        configurator.leftHandReady = true;
        if (configurator.leftHandReady && configurator.rightHandReady)
        {
            Debug.Log("Hardware listo para usar");
            instructions.text = "¡Perfecto!.\nSistema calibrado, se puede alternar de mano en cualquier momento.";
        }
        else
        {
            Debug.Log("Hardware no listo para usar");
            bothHandsGame.SetActive(false);
            leftOrRightHandGame.SetActive(true);
            rightHandMenu.SetActive(true);
            leftHandMenu.SetActive(false);
            instructions.text = "¡Excelente!.\nContinuemos con la calibración del sistema:\nPaciente - Si puede, estire su brazo y ponga su palma derecha frente al visor \nTerapeuta - Seleccione la mano Derecha.";
        }
        //handMenu.SetActive(false);
        PlayerPrefs.SetInt("SelectedHand", 0); //0 for left hand
        PlayerPrefs.Save();
        //HandConfigurator configurator = Object.FindFirstObjectByType<HandConfigurator>();
        if (configurator != null) configurator.ApplyConfig(0); //Applies configuration to left hand
        Debug.Log("Mano izquierda seleccionada");
    }

    public void RightHandSelection()
    {
        configurator.rightHandReady = true;
        //Menus();
        if (configurator.leftHandReady && configurator.rightHandReady)
        {
            bothHandsGame.SetActive(false);
            leftOrRightHandGame.SetActive(true);
            rightHandMenu.SetActive(true);
            leftHandMenu.SetActive(true);
            instructions.text = "¡Perfecto!.\nSistema calibrado, puede alternar mano en cualquier momento.";
        }
        //handMenu.SetActive(false);
        PlayerPrefs.SetInt("SelectedHand", 1); //1 for right hand
        PlayerPrefs.Save();
        //HandConfigurator configurator = Object.FindFirstObjectByType<HandConfigurator>();
        if (configurator != null) configurator.ApplyConfig(1); //Applies configuration to right hand
        Debug.Log("Mano derecha seleccionada");
    }
}