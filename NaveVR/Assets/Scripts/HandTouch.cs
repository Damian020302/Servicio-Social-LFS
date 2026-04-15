using UnityEngine;

public class HandTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.enabled = false; // Desactiva el collider del enemigo para evitar múltiples colisiones
            if (GameManager.Instance != null && !GameManager.Instance.roundOver)
            {
                GameManager.Instance.EnemyTouched(1); // Incrementa el contador de enemigos tocados
            }
            Destroy(other.gameObject); // Destruye el objeto enemigo al tocarlo
        }
    }
}
