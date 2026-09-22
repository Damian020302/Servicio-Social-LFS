using UnityEngine;

public class EnemySelection : MonoBehaviour
{
    /// <summary>
    /// Saves the selected enemy's ID to PlayerPrefs.
    /// </summary>
    /// <param name="enemyId">The ID of the selected enemy.</param>
    public void EnemySelected(int enemyId)
    {
        PlayerPrefs.SetInt("SelectedEnemy", enemyId);
        PlayerPrefs.Save();
        Debug.Log("Enemy " + enemyId + " selected and saved to PlayerPrefs.");
    }
}
