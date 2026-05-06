using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
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
}
