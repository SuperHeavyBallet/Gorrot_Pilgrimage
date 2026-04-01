using UnityEngine;

public class PlayerCameraMovementController : MonoBehaviour
{
    public bool isPressingPanLeft;
    public bool isPressingPanRight;

[SerializeField]    Animator cameraContainerAnimator;

    public void Set_isPressingPanLeft(bool value)
    {
        isPressingPanLeft = value;
        cameraContainerAnimator.SetBool("isPressingPanLeft", value);
    }
    public void Set_isPressingPanRight(bool value)
    {
        isPressingPanRight = value;
        cameraContainerAnimator.SetBool("isPressingPanRight", value);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
