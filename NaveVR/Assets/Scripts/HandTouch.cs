using UnityEngine;

public class HandTouch : MonoBehaviour
{
    [Header("Hand Configuration")]
    public bool isLeftHand = true; // Configuración para determinar si es la mano izquierda o derecha
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.enabled = false; // Desactiva el collider del enemigo para evitar múltiples colisiones
            EnemyMovement enemyMovement = other.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.Pieces(); // Llama al método Pieces() del enemigo para que se destruya en partes
            }
            if (GameManager.Instance != null && !GameManager.Instance.roundOver)
            {
                GameManager.Instance.EnemyTouched(1, isLeftHand); // Incrementa el contador de enemigos tocados
            }
            Destroy(other.gameObject); // Destruye el objeto enemigo al tocarlo
        }
    }
}
