using UnityEngine;

public class WandSelection : MonoBehaviour
{
    public void WandSelected(int wandId)
    {
        PlayerPrefs.SetInt("SelectedWand", wandId);
        PlayerPrefs.Save();
        Debug.Log("Wand " + wandId + " selected and saved to PlayerPrefs.");
    }
}
