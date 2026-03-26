using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyDifficulty : MonoBehaviour
{
    public Slider sizeSlider;
    public Slider speedSlider;
    public Slider lifeTimeSlider;
    public Slider spawnRateSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float savedEnemySpeed = PlayerPrefs.GetFloat("EnemySpeed", 3.0f);
        float savedEnemyLifetime = PlayerPrefs.GetFloat("EnemyLifetime", 10.0f);
        float savedEnemySize = PlayerPrefs.GetFloat("EnemySize", 1.0f);
        float savedSpawnRate = PlayerPrefs.GetFloat("TimeSpawnInterval", 2.0f);
        if (sizeSlider != null)
        {
            sizeSlider.value = savedEnemySize;
            Debug.Log($"Cargando tamaño de enemigo guardado: {savedEnemySize}");
        }
        if (speedSlider != null)
        {
            speedSlider.value = savedEnemySpeed;
            Debug.Log($"Cargando velocidad de enemigo guardada: {savedEnemySpeed}");
        }
        if (lifeTimeSlider != null)
        {
            lifeTimeSlider.value = savedEnemyLifetime;
            Debug.Log($"Cargando tiempo de vida de enemigo guardado: {savedEnemyLifetime}");
        }
        if (spawnRateSlider != null)
        {
            spawnRateSlider.value = savedSpawnRate;
            Debug.Log($"Cargando tasa de aparición guardada: {savedSpawnRate}");
        }
    }

    /**
     * Guarda las configuraciones de dificultad seleccionadas por el jugador y carga la escena del juego.
     */
    public void SaveAndPlay()
    {
        PlayerPrefs.SetFloat("TimeSpawnInterval", spawnRateSlider.value);
        PlayerPrefs.SetFloat("EnemySize", sizeSlider.value);
        PlayerPrefs.SetFloat("EnemySpeed", speedSlider.value);
        PlayerPrefs.SetFloat("EnemyLifetime", lifeTimeSlider.value);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Juego");
        Time.timeScale = 1.0f;
    }

}
