using UnityEngine;

public class DestroyedEnemy : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 2.0f;
    public float explosionForce = 200.0f;
    public AudioSource explosionSound;
    public AudioClip explosionClip;
    [Header("Cleanup Settings")]
    public float cleanupDelay = 2.0f;
    
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
        Destroy(gameObject, cleanupDelay);
    }

    public void PlayExplosionSound()
    {
        if (explosionSound != null && explosionClip != null)
        {
            explosionSound.PlayOneShot(explosionClip);
        }
    }
}