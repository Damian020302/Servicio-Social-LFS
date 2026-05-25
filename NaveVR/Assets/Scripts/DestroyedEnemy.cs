using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedEnemy : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 2.0f; // Radio de la explosión
    public float explosionForce = 200.0f; // Fuerza de la explosión
    public AudioSource explosionSound; // Sonido de la explosión
    public AudioClip explosionClip; // Clip de audio para la explosión
    [Header("Cleanup Settings")]
    public float cleanupDelay = 2.0f; // Tiempo antes de limpiar los restos
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayExplosionSound();
        Rigidbody[] pieces = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody piece in pieces)
        {
            if(piece != null)
            {
                piece.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                piece.AddTorque(Random.insideUnitSphere * 5.0f, ForceMode.Impulse); // Agrega una rotación aleatoria
            }
        }
        Destroy(gameObject, cleanupDelay); // Destruye el objeto después de un tiempo para limpiar los restos
    }

    public void PlayExplosionSound()
    {
        if (explosionSound != null && explosionClip != null)
        {
            explosionSound.PlayOneShot(explosionClip);
        }
    }
}