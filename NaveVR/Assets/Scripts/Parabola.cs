using UnityEngine;

public class Parabola : MonoBehaviour
{
    public float speed;
    public float archHeight;
    private Vector3 start;
    private Vector3 end;
    private float totalDistance;
    private float progress = 0.0f;
    private bool isFinished = false;

    public void Launch(Vector3 origin, Vector3 destination, float launchSpeed, float height)
    {
        start = origin;
        end = destination;
        archHeight = height;
        speed = launchSpeed;
        totalDistance = Vector3.Distance(start, end);
        progress = 0.0f;

        if (totalDistance < 0.1f)
        { 
            transform.position = end;
            Destroy(gameObject);
            return;
        }
        isFinished = true;
    }

    void Update()
    {
        if (!isFinished) return;
        progress += (speed * Time.deltaTime) / totalDistance;
        progress = Mathf.Clamp01(progress);
        float heightMultiplier = Mathf.Sin(Mathf.PI * progress);
        Vector3 nextPosition = Vector3.Lerp(start, end, progress);
        nextPosition.y += archHeight * heightMultiplier;
        transform.position = nextPosition;
        if (progress >= 1.0f)
        {
            isFinished = false;
            Destroy(gameObject);
        }
    }
}
