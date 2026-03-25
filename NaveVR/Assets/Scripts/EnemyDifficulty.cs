using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyDifficulty : MonoBehaviour
{
    public Slider speedSlider;
    public Slider lifeTimeSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float savedEnemySpeed = PlayerPrefs.GetFloat("EnemySpeed", 3.0f);
        float savedEnemyLifetime = PlayerPrefs.GetFloat("EnemyLifetime", 10.0f);
        if (speedSlider != null)
        {
            speedSlider.value = savedEnemySpeed;
        }
        if (lifeTimeSlider != null)
        {
            lifeTimeSlider.value = savedEnemyLifetime;
        }
    }

    /**
     * Guarda las configuraciones de dificultad seleccionadas por el jugador y carga la escena del juego.
     */
    public void SaveAndPlay()
    {
        PlayerPrefs.SetFloat("EnemySpeed", speedSlider.value);
        PlayerPrefs.SetFloat("EnemyLifetime", lifeTimeSlider.value);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Juego");
        Time.timeScale = 1.0f;
    }

}
