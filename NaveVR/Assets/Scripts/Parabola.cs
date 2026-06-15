using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola : MonoBehaviour
{
    //public Transform objective;
    public float speed/* = 1.0f*/;
    public float archHeight/* = 5.0f*/;
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
            OnTargetReached();
            return;
        }
        isFinished = true;
    }

    void Update()
    {
        if (!isFinished)
            return;
        progress += (speed * Time.deltaTime) / totalDistance;
        progress = Mathf.Clamp01(progress);
        float heightMultiplier = Mathf.Sin(Mathf.PI * progress);
        Vector3 nextPosition = Vector3.Lerp(start, end, progress);
        nextPosition.y += archHeight * heightMultiplier;
        transform.position = nextPosition;
        if (progress >= 1.0f)
        {
            isFinished = false;
            OnTargetReached();
        }
    }

    private void OnTargetReached()
    {
        Destroy(gameObject);
        // Implement any logic you want to execute when the target is reached
        Debug.Log("Target reached!");
    }
}
