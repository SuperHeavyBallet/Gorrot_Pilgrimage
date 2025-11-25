using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputReceiver : MonoBehaviour
{
    Vector2 moveValue = new Vector2(0,0);
    bool hasPressedMove;

   // public GameObject player;
    PlayerMovementController playerMovementController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       ConnectToPlayer();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CaptureMovementInput(InputAction.CallbackContext context)
    {
        if (context.performed && !hasPressedMove)
        {
            hasPressedMove = true;
            var input = context.ReadValue<Vector2>();

            // Treat tiny noise from sticks as zero
            float deadZone = 0.1f;
            bool hasX = Mathf.Abs(input.x) > deadZone;
            bool hasY = Mathf.Abs(input.y) > deadZone;

            // If both directions pressed, cancel movement
            if (hasX && hasY)
            {
                moveValue = Vector2.zero;
            }
            else if (hasX)
            {
                moveValue = new Vector2(Mathf.Sign(input.x), 0f);
            }
            else if (hasY)
            {
                moveValue = new Vector2(0f, Mathf.Sign(input.y));
            }
            else
            {
                moveValue = Vector2.zero;
            }

            playerMovementController.ReceiveMoveInput(moveValue);
        }

        if (context.canceled)
        {
            hasPressedMove = false;
            moveValue = Vector2.zero;
        }

    }

    public void ConnectToPlayer()
    {
        if (this.GetComponent<PlayerMovementController>() != null)
        {
            playerMovementController = this.GetComponent<PlayerMovementController>();
        }
        else
        {
            Debug.LogError("No Player Found");
        }
    }
}
