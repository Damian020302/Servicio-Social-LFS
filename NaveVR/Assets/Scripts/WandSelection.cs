using UnityEngine;

public class WandSelection : MonoBehaviour
{
    /// <summary>
    /// Sets the selected wand ID from the UI and saves it to PlayerPrefs.
    /// </summary>
    /// <param name="wandId">The ID of the selected wand.</param>
    public void WandSelected(int wandId)
    {
        PlayerPrefs.SetInt("SelectedWand", wandId);
        PlayerPrefs.Save();
        Debug.Log("Wand " + wandId + " selected and saved to PlayerPrefs.");
    }
}
