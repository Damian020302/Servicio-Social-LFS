using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManagerPuzzle : MonoBehaviour
{
    public void MainScene()
    {
        SceneManager.LoadScene("Menu2");
        Time.timeScale = 1.0f; // Asegura que el tiempo se reanude al volver al menú
    }

    //public void 
}
