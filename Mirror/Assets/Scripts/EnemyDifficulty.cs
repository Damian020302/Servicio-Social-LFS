using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyDifficulty : MonoBehaviour
{
    public Slider sizeSlider;
    public Slider speedSlider;
    public Slider lifeTimeSlider;
    public Slider spawnRateSlider;
    
    void Start()
    {
        float savedEnemySpeed = PlayerPrefs.GetFloat("EnemySpeed", 3.0f);
        float savedEnemyLifetime = PlayerPrefs.GetFloat("EnemyLifetime", 10.0f);
        float savedEnemySize = PlayerPrefs.GetFloat("EnemySize", 1.0f);
        float savedSpawnRate = PlayerPrefs.GetFloat("TimeSpawnInterval", 2.0f);
        if (sizeSlider != null)
        {
            sizeSlider.value = savedEnemySize;
        }
        if (speedSlider != null)
        {
            speedSlider.value = savedEnemySpeed;
        }
        if (lifeTimeSlider != null)
        {
            lifeTimeSlider.value = savedEnemyLifetime;
        }
        if (spawnRateSlider != null)
        {
            spawnRateSlider.value = savedSpawnRate;
        }
    }

    /// <summary>
    /// Saves the current values of the sliders to PlayerPrefs and starts the game scene.
    /// </summary>
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