using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using GorrotGame;

public class PlayerInputReceiver : MonoBehaviour
{
    Vector2 moveValue = new Vector2(0,0);
    bool hasPressedMove;
    bool moveInputIsBlocked;

    public    bool isPressingPanCameraLeft;
    public    bool isPressingPanCameraRight;

   // public GameObject player;
    PlayerMovementController playerMovementController;
    public DiceController diceController;
    PlayerCameraMovementController playerCameraMovementController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       ConnectToPlayer();
        
    }

   public void PanCameraLeft(InputAction.CallbackContext context)
    {
        if(!GorrotGame.GameFunctions.GameIsPaused())
        {
            if (context.performed)
            {
                playerCameraMovementController.Set_isPressingPanLeft(true);
                moveInputIsBlocked = true;
            }
            else if (context.canceled)
            {
                playerCameraMovementController.Set_isPressingPanLeft(false);
                moveInputIsBlocked = false;
            }
        }
    
    }
   public void PanCameraRight(InputAction.CallbackContext context)
    {
        if (!GorrotGame.GameFunctions.GameIsPaused())
        {
            if (context.performed)
            {
                playerCameraMovementController.Set_isPressingPanRight(true);
                moveInputIsBlocked = true;
            }
            else if (context.canceled)
            {
                playerCameraMovementController.Set_isPressingPanRight(false);
                moveInputIsBlocked = false;
            }
        }
    }

    public void CaptureDiceRollInput(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            diceController.RollDice();
        }
    }

    public void CaptureWaitInput(InputAction.CallbackContext context)
    {
        if (context.performed && !hasPressedMove)
        {
            hasPressedMove = true;
            playerMovementController.ReceiveMoveInput(Vector2.zero, false, true);
            
        }
        if (context.canceled)
        {
            hasPressedMove = false;
            moveValue = Vector2.zero;
        }
    }

 
    public void CaptureMovementInput(InputAction.CallbackContext context)
    {
        if (!GorrotGame.GameFunctions.GameIsPaused())
        {
            if (context.performed && !hasPressedMove && !moveInputIsBlocked)
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

                playerMovementController.ReceiveMoveInput(moveValue, false, false);
            }

            if (context.canceled)
            {
                hasPressedMove = false;
                moveValue = Vector2.zero;
            }
        }

    }

    public void ConnectToPlayer()
    {
        if (this.GetComponent<PlayerMovementController>() != null)
        {
            playerMovementController = this.GetComponent<PlayerMovementController>();
            playerCameraMovementController = this.GetComponent<PlayerCameraMovementController>();
        }
        else
        {
            Debug.LogError("No Player Found");
        }

        
    }
}
