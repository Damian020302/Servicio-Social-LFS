using UnityEngine;

public class HandTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.ScorePoints(10); // Suma 10 puntos al puntaje del jugador
            }
            Destroy(other.gameObject); // Destruye el objeto enemigo al tocarlo
        }
    }
}
