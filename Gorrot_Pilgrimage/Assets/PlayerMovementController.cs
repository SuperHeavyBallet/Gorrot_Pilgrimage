using Unity.Mathematics;
using UnityEngine;
using System.Collections;


public class PlayerMovementController : MonoBehaviour
{
    //int[,] battleFieldCoordinates;
    public GameObject square;
    int battleFieldSize = 0;

    GameObject[,] allSquares;

    

    public Vector2Int currentPosition;

    public bool isPlayerTurn;
    public TurnOrganiser turnOrganiser;

    public AudioManager audioManager;

    PlayerStatsController playerStatsController;

    public bool playerIsAlive;

   public BattlefieldBuilder battlefieldBuilder;
    public string nextSquareQuantity = "medium";

    PlayerInventory playerInventory;

    public FateCounter fateCounter;

    bool isMoving;

    public GameObject playerSprite;

   [SerializeField] PlayerAnimationManager playerAnimationManager;

    enum facingPositions
    {
        up, down, left, right
    }

    facingPositions nextFacingPosition = facingPositions.up;
    facingPositions currentFacingPosition = facingPositions.up;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStatsController = this.GetComponent<PlayerStatsController>();
        playerIsAlive = CheckPlayerAlive();
        playerInventory = this.GetComponent<PlayerInventory>();
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

    public void ReceiveMoveInput(Vector2 receivedMoveValue)
    {
        


        Vector2 normalizedMoveValue = receivedMoveValue;
        if (receivedMoveValue.x > 0) { normalizedMoveValue.x = 1;}

        if (receivedMoveValue.y > 0) { normalizedMoveValue.y = 1; }

        SetFacing(normalizedMoveValue.x, normalizedMoveValue.y);


        if (isMoving) return;

        if (turnOrganiser.GetIsInMerchant()) return;

        if (turnOrganiser.GetLandedOnGoal()) return;



        playerIsAlive = CheckPlayerAlive();

    

        if (isPlayerTurn && playerIsAlive)
        {
            if (turnOrganiser.currentPhase == TurnOrganiser.ActivePhase.movement)
                MovePlayer(normalizedMoveValue);
        }

    }

    bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < battleFieldSize &&
               y >= 0 && y < battleFieldSize;
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

    public void MovePlayer(Vector2 newMoveValue)
    {
        


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


        StartCoroutine(MoveRoutine(newSquareController, newPositionX, newPositionY, newPosition, newSquareController));

       


       



    }

    IEnumerator MoveRoutine(
        SquareController targetSquare,
    int newX,
    int newY,
    Vector2 worldTargetPos,
    SquareController newSquareController
    )
    {
        isMoving = true;
        playerAnimationManager.SetIsWalking(true);

        audioManager.playPlayerMoveSoundEffect();

        Vector3 start = transform.position;
        Vector3 end = new Vector3(worldTargetPos.x, worldTargetPos.y, transform.position.z);

        float duration = 0.25f; // tune feel
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);

            // Smoothstep-ish curve (feels nicer than linear)
            u = u * u * (3f - 2f * u);

            transform.position = Vector3.Lerp(start, end, u);
            yield return null;
        }

        transform.position = end;

        // Commit grid position *after* movement finishes
        currentPosition = new Vector2Int(newX, newY);

        

        isMoving = false;
        playerAnimationManager.SetIsWalking(false);
        ApplyMoveResults(newSquareController);

        // Now the turn can progress
        turnOrganiser.BuildNextTurn();
    }

    void ApplyMoveResults(SquareController newSquareController)
    {
        newSquareController.ActivateSquareVisited();



        if (newSquareController.isGoalSquare)
        {
            turnOrganiser.LandedOnGoal();
            fateCounter.resetFateCounter();
            return;
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
            turnOrganiser.SetLandedOnEnemySquare(true);
            fateCounter.resetFateCounter();
            newSquareController.MakeEmptySquare();
            return;
        }


        fateCounter.alterFateCounter(1);

        if (newSquareController.isEmptySquare)
        {
            addMovementSuffering();
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
                audioManager.playCannotMoveSoundEffect();
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

            playerStatsController.alterMoney(amount);
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
            int sufferingAmount = Mathf.Clamp(amount, 0, amount - 2);

            playerStatsController.alterSuffering(sufferingAmount * -1);
            newSquareController.MakeEmptySquare();
        }
    }

    void addMovementSuffering()
    {

        playerStatsController.alterSuffering(1);
    }


    void BlockedSquare()
    {
        audioManager.playCannotMoveSoundEffect();
    }

    public void ReceiveBattlefieldSize(int size, GameObject[,] receivedAllSquares)
    {
        battleFieldSize = size;

        allSquares = receivedAllSquares;
    }

    public void SetPlayerStartSquare(int recX, int recY)
    {
        SquareController newSquareController = allSquares[recX, recY].GetComponent<SquareController>();

        this.transform.position = new Vector2(
            newSquareController.GetSquareXPosition(),
            newSquareController.GetSquareYPosition()
        );

        newSquareController.ActivateSquareVisited();

        SetStartCurrentPosition(recX, recY);
    }

    void SetStartCurrentPosition(int startCurX, int startCurY)
    {
        currentPosition = new Vector2Int(startCurX, startCurY);
    }





}
