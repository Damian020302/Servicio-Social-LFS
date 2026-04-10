using UnityEngine;

public class EnemySelection : MonoBehaviour
{
    public void EnemySelected(int enemyId)
    {
        PlayerPrefs.SetInt("SelectedEnemy", enemyId);
        PlayerPrefs.Save();
        Debug.Log("Enemy " + enemyId + " selected and saved to PlayerPrefs.");
    }
}
