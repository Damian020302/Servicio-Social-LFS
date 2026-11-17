using UnityEngine;

public class RobotDespawnTrigger : MonoBehaviour
{
    [SerializeField] private string robotTag = "Grabbable";
    private void OnTriggerEnter(Collider other)
    {
        GameObject target = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;
        if (other.CompareTag(robotTag))
        {
            target.SetActive(false);
        }
    }
}
