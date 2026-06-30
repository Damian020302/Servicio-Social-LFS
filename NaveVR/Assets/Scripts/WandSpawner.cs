using UnityEngine;

public class WandSpawner : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Drag the Wand Prefabs")] public GameObject[] wandPrefabs;
    [Tooltip("Drag the Wand Spawner")] public Transform wandSpawner;

    public GameObject selectedWand;

    public void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedWand", 0);
        if(wandPrefabs != null && selectedIndex < wandPrefabs.Length)
        {
            selectedWand = wandPrefabs[selectedIndex];
        }
        else
        {
            if(wandPrefabs.Length > 0)
            {
                selectedWand = wandPrefabs[0];
            }
            Debug.LogWarning("Selected wand index is out of range or wandPrefabs is not assigned.");
            return;
        }
    }

    public void SpawnWand()
    {
        Instantiate(selectedWand, wandSpawner.position, wandSpawner.rotation);
    }
}
