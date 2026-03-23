using UnityEngine;

public class HandTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.EnemyTouched(10); // Incrementa el contador de enemigos tocados
            }
            Destroy(other.gameObject); // Destruye el objeto enemigo al tocarlo
        }
    }
}
