using UnityEngine;

public class HandTouch : MonoBehaviour
{
    [Header("Hand Configuration")]
    public bool isLeftHand = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.enabled = false;
            EnemyMovement enemyMovement = other.GetComponent<EnemyMovement>();
            if (enemyMovement != null)
            {
                enemyMovement.Pieces();
            }
            if (GameManager.Instance != null && !GameManager.Instance.roundOver)
            {
                GameManager.Instance.EnemyTouched(1, isLeftHand);
            }
            Destroy(other.gameObject);
        }
    }
}