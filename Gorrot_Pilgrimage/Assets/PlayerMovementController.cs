using Unity.Mathematics;
using UnityEngine;
using System.Collections;


public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] float zOffset = -0.1f;

    //int[,] battleFieldCoordinates;
    public GameObject square;
    int battleFieldSize = 0;
    int gridWidth;
    int gridHeight;


    GameObject[,] allSquares;


    public Vector2Int previousPosition;
    public Vector2Int currentPosition;

    public bool isPlayerTurn;
    public TurnOrganiser turnOrganiser;

   // public AudioManager audioManager;

    PlayerStatsController playerStatsController;

    public bool playerIsAlive;

   public BattlefieldBuilder battlefieldBuilder;
    public string nextSquareQuantity = "medium";

    PlayerInventory playerInventory;

    public FateCounter fateCounter;

    bool isMoving;

    bool reachedGoalSquare;

    public GameObject playerSprite;

   [SerializeField] PlayerAnimationManager playerAnimationManager;

    SquareController currentSquareController;

    enum facingPositions
    {
        up, down, left, right
    }

    facingPositions nextFacingPosition = facingPositions.up;
    facingPositions currentFacingPosition = facingPositions.up;

    int previousPositionX = 0;
    int previousPositionY = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStatsController = this.GetComponent<PlayerStatsController>();
        playerIsAlive = CheckPlayerAlive();
        playerInventory = this.GetComponent<PlayerInventory>();
    }

    public void SetReachedGoalSquare(bool value)
    {
        reachedGoalSquare = value;
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerTurn = turnOrganiser.GetPlayerTurn();
        
    }

    bool CheckPlayerAlive()
    {
        return playerStatsController.playerIsAlive;
    }

    public void ReceiveMoveInput(Vector2 receivedMoveValue, bool freeMove, bool isWaiting)
    {
        

        if(!reachedGoalSquare)
        {
            Vector2 normalizedMoveValue = receivedMoveValue;
            if (receivedMoveValue.x > 0) { normalizedMoveValue.x = 1; }

            if (receivedMoveValue.y > 0) { normalizedMoveValue.y = 1; }

            SetFacing(normalizedMoveValue.x, normalizedMoveValue.y);


            if (isMoving) return;

            if (turnOrganiser.GetIsInMerchant()) return;

            if (turnOrganiser.GetLandedOnGoal()) return;



            playerIsAlive = CheckPlayerAlive();



            if (isPlayerTurn && playerIsAlive)
            {
                if (turnOrganiser.currentPhase == TurnOrganiser.ActivePhase.movement)
                {
                   
                        MovePlayer(normalizedMoveValue, freeMove, isWaiting);
                    

                   
                }
                    
            }
        }
       

    }

    bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth &&
         y >= 0 && y < gridHeight;
    }


    
    void SetFacing(float normX, float normY)
    {
        currentFacingPosition = nextFacingPosition;

        if (normX < 0) { nextFacingPosition = facingPositions.left; }
        else if (normX > 0) { nextFacingPosition = facingPositions.right; }
        else if (normY < 0) nextFacingPosition = facingPositions.down;
        else { nextFacingPosition = facingPositions.up; }

        if (currentFacingPosition != nextFacingPosition)
        {
            switch (nextFacingPosition)
            {
                case facingPositions.down:
                    playerAnimationManager.SetFrontSprites();
                    break;

                case facingPositions.right:
                    playerAnimationManager.SetSideSprites("right");
                    break;

                case facingPositions.left:
                    playerAnimationManager.SetSideSprites("left");
                    break;

                case facingPositions.up:
                default:
                    playerAnimationManager.SetBackSprites();
                    break;
            }

        }

       

    
    }

    public void MovePlayer(Vector2 newMoveValue, bool freeMove, bool isWaiting)
    {
        
        previousPosition = currentPosition;

        int newPositionX = currentPosition.x + Mathf.RoundToInt(newMoveValue.x);
        int newPositionY = currentPosition.y + Mathf.RoundToInt(newMoveValue.y);

        
        

        // FIRST: check bounds BEFORE touching the array
        if (!IsInsideGrid(newPositionX, newPositionY))
        {
            BlockedSquare();
            return;
        }

        SquareController newSquareController = allSquares[newPositionX, newPositionY].GetComponent<SquareController>();

        if (newSquareController == null)
        {
            BlockedSquare();
            return;
        }

        nextSquareQuantity = newSquareController.getSquareQuantity();

        bool isMoveableSquare = newSquareController.isMoveableSquare();

        if (!isMoveableSquare)
        {
            BlockedSquare();
            return;
        }



        // Compare Positions between this and proposed next square to set the entry direction
        Vector2Int newMoveVector = new Vector2Int(newPositionX, newPositionY);
        newSquareController.SetEntryDirection(currentPosition, newMoveVector);

        Vector2 newPosition = new Vector2(
           newSquareController.GetSquareXPosition(),
           newSquareController.GetSquareYPosition()
            );


        StartCoroutine(MoveRoutine(
            newSquareController, 
            newPositionX, 
            newPositionY, 
            newPosition, 
            newSquareController, 
            freeMove,
            isWaiting
            ));

    }

    void PlayFootStepSound(SquareController squareController)
    {
        if (squareController.isWater)
        {
            AudioManager.Instance.playPlayerMoveWaterSoundEffect();
        }
        else
        {
            AudioManager.Instance.playPlayerMoveSoundEffect();
        }
    }

    void MakeSquareHoldPlayer(SquareController squareController, bool value)
    {
        if (squareController != null)
        {
            squareController.MakeThisSquareHoldPlayer(value);

            
        }
    }

    void ActivateWaterStep(SquareController squareController, bool value)
    {
        if (squareController != null)
        {
            if (squareController.isWater)
            {
                squareController.ActivateStepInWaterSprite(value);
            }
        }
    }

    public void UpdateCurrentSquareController(SquareController squareController)
    {
        if (squareController != null)
        {
            currentSquareController = squareController;
        }
    }

    void UpdateCurrentPosition(int newPosX, int newPosY)
    {
        currentPosition = new Vector2Int(newPosX, newPosY);
    }

    IEnumerator MoveRoutine(
        SquareController targetSquare,
    int newX,
    int newY,
    Vector2 worldTargetPos,
    SquareController newSquareController,
    bool freeMove,
    bool isWaiting
    )
    {
        isMoving = true;
        playerAnimationManager.SetIsWalking(true);

        PlayFootStepSound(newSquareController);

        Vector3 start = transform.position;
        start.z = zOffset;

        Vector3 end = new Vector3(worldTargetPos.x, worldTargetPos.y, zOffset);

        float duration = 0.25f; // tune feel
        float t = 0f;

        MakeSquareHoldPlayer(currentSquareController, false);
        ActivateWaterStep(currentSquareController, false);
        bool waterSplashTriggered = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);

            if (!waterSplashTriggered && u >= 0.5f && newSquareController.isWater)
            {
                newSquareController.ActivateStepInWaterSprite(true);
                waterSplashTriggered = true;
            }

            u = u * u * (3f - 2f * u);
            transform.position = Vector3.Lerp(start, end, u);
            yield return null;
        }
        transform.position = end;

        // Commit grid position *after* movement finishes
        UpdateCurrentPosition(newX, newY);
        MakeSquareHoldPlayer(newSquareController, true);
        UpdateCurrentSquareController(newSquareController);
        

        isMoving = false;
        playerAnimationManager.SetIsWalking(false);
        ApplyMoveResults(newSquareController, freeMove, isWaiting);
        turnOrganiser.BuildNextTurn();
    }

    void ApplyMoveResults(SquareController newSquareController, bool freeMove, bool isWaiting)
    {
        newSquareController.ActivateSquareVisited();

        if (newSquareController.isGoalSquare)
        {
            newSquareController.MakeGoalSquarePressed();
            SetReachedGoalSquare(true);
            turnOrganiser.LandedOnGoal();
            fateCounter.resetFateCounter();
            return;
        }

        if(newSquareController.ThisSquareHoldsPottard)
        {
            playerStatsController.resetSuffering();
            MovePlayerBackOneSquare();
        }

        if(newSquareController.Type == SquareController.SquareType.Trap)
        {
            if(newSquareController.GetTrapActivated == false)
            {
                AudioManager.Instance.PlayTrapTriggerSoundEffect();
                newSquareController.ActivateTrap();
                playerStatsController.alterHealth(-1);
                MovePlayerBackOneSquare();
            }
            
        }

        if(newSquareController.GetIsMerchantSquare())
        {
            turnOrganiser.LandedOnMerchantSquare();
            return;
        }

        if (newSquareController.isEnemySquare)
        {
            int amount = 0;

            switch (nextSquareQuantity)
            {
                case "small":
                    amount = 1;
                    break;
                case "medium":
                    amount = 3;
                    break;
                case "large":
                    amount = 5;
                    break;
                default:
                    amount = 3;
                    break;
            }

            turnOrganiser.UpdateCurrentEnemySize(amount);
            turnOrganiser.SetLandedOnEnemySquare(true, newSquareController);
            return;
        }


        if (!freeMove)
        {
            if (!isWaiting) fateCounter.alterFateCounter(1);
            else fateCounter.alterFateCounter(2);

            if (newSquareController.GetIsWater())
            {
                playerStatsController.alterSuffering(2); // or 1, whatever intended
            }
            else if (newSquareController.isEmptySquare && !isWaiting)
            {
                playerStatsController.alterSuffering(1);
            }
        }




        if (newSquareController.isItemSquare)
        {
            string squareContentsID = newSquareController.GetContentsID();
            bool canAddItem = playerInventory.TryToAddItem(squareContentsID);

            if (canAddItem)
            {
                newSquareController.MakeEmptySquare();
            }
            else
            {
                AudioManager.Instance.playCannotMoveSoundEffect();
            }

        }

        if (newSquareController.isTreasureSquare)
        {
            int amount = 0;

            switch (nextSquareQuantity)
            {
                case "small":
                    amount = 1;
                    break;
                case "medium":
                    amount = 3;
                    break;
                case "large":
                    amount = 5;
                    break;
                default:
                    amount = 3;
                    break;
            }

            playerStatsController.AlterMoney(amount);
            playerStatsController.alterSuffering(amount * -1);
            newSquareController.MakeEmptySquare();
        }

        

        if (newSquareController.isHealthSquare)
        {
            int amount = 0;

            switch (nextSquareQuantity)
            {
                case "small":
                    amount = 1;
                    break;
                case "medium":
                    amount = 3;
                    break;
                case "large":
                    amount = 5;
                    break;
                default:
                    amount = 3;
                    break;
            }

            playerStatsController.alterHealth(amount);
            int sufferingAmount = 1;

            
            playerStatsController.alterSuffering(sufferingAmount * -1);
            newSquareController.MakeEmptySquare();
        }
    }



    public void MovePlayerBackOneSquare()
    {
       Vector2Int delta = previousPosition - currentPosition;   // e.g. (-1,0)
        MovePlayer(new Vector2(delta.x, delta.y), true, false);
    }
    void BlockedSquare()
    {
        AudioManager.Instance.playCannotMoveSoundEffect();
    }

    public void ReceiveBattlefieldSize(GameObject[,] receivedAllSquares)
    {
        

        allSquares = receivedAllSquares;
        gridWidth = allSquares.GetLength(0);
        gridHeight = allSquares.GetLength(1);
    }

    public void SetPlayerStartSquare(int recX, int recY)
    {
        SquareController newSquareController = allSquares[recX, recY].GetComponent<SquareController>();

        this.transform.position = new Vector3(
        newSquareController.GetSquareXPosition(),
        newSquareController.GetSquareYPosition(),
        zOffset
    );

        newSquareController.ActivateSquareVisited();

        SetStartCurrentPosition(recX, recY);
    }

    void SetStartCurrentPosition(int startCurX, int startCurY)
    {
        currentPosition = new Vector2Int(startCurX, startCurY);
    }



    void LateUpdate()
    {
        Vector3 p = transform.position;
        if (p.z != zOffset)
        {
            p.z = zOffset;
            transform.position = p;
        }
    }

}
