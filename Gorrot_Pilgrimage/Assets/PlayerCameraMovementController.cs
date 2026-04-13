using UnityEngine;

public class PlayerCameraMovementController : MonoBehaviour
{
    public bool isPressingPanLeft;
    public bool isPressingPanRight;
    public bool isPressingPanReverse;

[SerializeField]    Animator cameraContainerAnimator;
    [SerializeField] CameraTrackPlayer cameraTrackPlayer;

    int cameraNormalHash;


    public void Set_isPressingPanReverse(bool value)
    {
        isPressingPanReverse = value;
        cameraContainerAnimator.SetBool("isPressingPanReverse", value);
    }
    public void Set_isPressingPanLeft(bool value)
    {
       // cameraContainerAnimator.enabled = value;

        isPressingPanLeft = value;
        cameraContainerAnimator.SetBool("isPressingPanLeft", value);
        
        
    }
    public void Set_isPressingPanRight(bool value)
    {
        //cameraContainerAnimator.enabled = value;

        isPressingPanRight = value;
        cameraContainerAnimator.SetBool("isPressingPanRight", value);
    
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraNormalHash = Animator.StringToHash("CameraNormal");
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = cameraContainerAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == cameraNormalHash)
        {
            cameraTrackPlayer.DisableCameraTrack(false);
        }
        else
        {
            cameraTrackPlayer.DisableCameraTrack(true);
        }

    }
}
