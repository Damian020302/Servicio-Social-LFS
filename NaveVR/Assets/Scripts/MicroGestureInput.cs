using UnityEngine;

public class MicroGestureInput : MonoBehaviour
{
    public OVRHand hand;
    private Vector2 moveInput = Vector2.zero;
    public StarterAssets.StarterAssetsInputs inputs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OVRHand.MicrogestureType microGesture = hand.GetMicrogestureType();
        switch (microGesture)
        {
            case OVRHand.MicrogestureType.NoGesture:
                break;
            case OVRHand.MicrogestureType.SwipeLeft:
                Debug.Log("Swipe Left");
                moveInput = Vector2.left;
                break;
            case OVRHand.MicrogestureType.SwipeRight:
                Debug.Log("Swipe Right");
                moveInput = Vector2.right;
                break;
            case OVRHand.MicrogestureType.SwipeForward:
                Debug.Log("Swipe Forward");
                moveInput = Vector2.up;
                break;
            case OVRHand.MicrogestureType.SwipeBackward:
                Debug.Log("Swipe Backward");
                moveInput = Vector2.down;
                break;
            case OVRHand.MicrogestureType.ThumbTap:
                Debug.Log("Thumb Tap");
                moveInput = Vector2.zero;
                break;
            case OVRHand.MicrogestureType.Invalid:
                Debug.Log("Invalid Gesture");
                break;
            default:
                // No recognized microgesture
                break;
        }

        inputs.MoveInput(moveInput);
    }
}
