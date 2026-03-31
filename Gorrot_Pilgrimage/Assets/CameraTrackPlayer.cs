using UnityEngine;

public class CameraTrackPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] PlayerMovementController playerMovementController;

    [SerializeField] float maxForwardOffset = 6f;
    [SerializeField] float minForwardOffset = 1f;
    [SerializeField] float smoothTime = 0.2f;

    Vector3 velocity = Vector3.zero;

    public int gridHeight;
    public int playerZPosition;

     float startXRotation = 75f;
    float endXRotation = 55f;
    [SerializeField] float rotationSmoothSpeed = 5f;



    void Update()
    {
        TrackPlayer();
    }

    void TrackPlayer()
    {
        gridHeight = playerMovementController.GridHeight;
        playerZPosition = playerMovementController.PlayerPosition.y;

        float t = gridHeight > 1 ? playerZPosition / (float)(gridHeight - 1) : 0;
        t = Mathf.Clamp01(t);

        float forwardOffset = Mathf.Lerp(0f, maxForwardOffset, t);

        Vector3 targetPosition = new Vector3(
            player.transform.position.x,
            transform.position.y,
            player.transform.position.z - forwardOffset
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        float targetXRotation = Mathf.Lerp(startXRotation, endXRotation, t);

        Quaternion targetRotation = Quaternion.Euler(
     targetXRotation,
     transform.localEulerAngles.y,
     transform.localEulerAngles.z
 );

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}
