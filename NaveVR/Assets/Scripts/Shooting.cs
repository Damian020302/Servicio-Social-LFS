using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("OVRHand")]
    [Tooltip("Objeto que contenga OVRHand")] public OVRHand hand;
    [Header("Bullet")] public GameObject bulletPrefab;
    [Header("Fire Point")]
    [Tooltip("Punto de salida del disparo")] public Transform firePoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
