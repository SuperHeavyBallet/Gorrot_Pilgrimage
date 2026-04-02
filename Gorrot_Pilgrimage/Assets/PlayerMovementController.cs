using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using GorrotGame;
using System.Collections.Generic;



public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] float yOffset = 0f;


   int gridWidth;
    int gridHeight;


    GameObject[,] allSquares;


    Vector2Int previousPosition;
    Vector2Int currentPosition;

    bool isPlayerTurn;
    [SerializeField] TurnOrganiser turnOrganiser;


    PlayerStatsController playerStatsController;

    public bool playerIsAlive;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;
    SquareSize nextSquareQuantity = SquareSize.Medium;

    PlayerInventory playerInventory;

    [SerializeField] FateCounter fateCounter;

    bool isMoving;

    bool reachedGoalSquare;

    //public GameObject playerSprite;

   //[SerializeField] PlayerAnimationManager playerAnimationManager;
    private Quaternion standeeForwardQ;
    [SerializeField] GameObject standee;
    [SerializeField] Animator standeeAnimator;

    private Quaternion standeeRightQ, standeeLeftQ, standeeBackQ;


    SquareController currentSquareController;

    public event System.Action OnPlayerMoved;

    enum facingPositions
    {
        up, down, left, right
    }

    facingPositions nextFacingPosition = facingPositions.up;
    facingPositions currentFacingPosition = facingPositions.up;

    [SerializeField] float turnDuration = 0.15f; // small = snappy
    Coroutine turnRoutine;

    [SerializeField] Transform board; // something whose “up” is your turn axis

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStatsController = this.GetComponent<PlayerStatsController>();
        playerIsAlive = CheckPlayerAlive();
        playerInventory = this.GetComponent<PlayerInventory>();

        standeeForwardQ = standee.transform.rotation;

        Vector3 turnAxis = board.up; // or board.forward / board.right depending on your scene

        standeeRightQ = standeeForwardQ * Quaternion.AngleAxis(90f, turnAxis);
        standeeLeftQ = standeeForwardQ * Quaternion.AngleAxis(-90f, turnAxis);
        standeeBackQ = standeeForwardQ * Quaternion.AngleAxis(180f, turnAxis);

        standeeAnimator.SetBool("isMoving", false);

    }

    public void PrepareForMapRebuild()
    {
        if (turnRoutine != null)
        {
            StopCoroutine(turnRoutine);
            turnRoutine = null;
        }
        StopAllCoroutines();

        isMoving = false;
        reachedGoalSquare = false;
        currentSquareController = null;
        allSquares = null;

        //playerAnimationManager.SetIsWalking(false);
        if (standeeAnimator) standeeAnimator.SetBool("isMoving", false);
    }

    public void SetReachedGoalSquare(bool value)
    {
        reachedGoalSquare = value;
    }
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
            Vector2 normalizedMoveValue = new Vector2(
                 receivedMoveValue.x == 0 ? 0 : Mathf.Sign(receivedMoveValue.x),
                 receivedMoveValue.y == 0 ? 0 : Mathf.Sign(receivedMoveValue.y)
             );

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

    void SetFacing(float normX, float normY)
    {
        currentFacingPosition = nextFacingPosition;

        if (normX < 0) nextFacingPosition = facingPositions.left;
        else if (normX > 0) nextFacingPosition = facingPositions.right;
        else if (normY < 0) nextFacingPosition = facingPositions.down;
        else nextFacingPosition = facingPositions.up;

        // 1) Always compute rotation from the current facing
        Quaternion target;
        switch (nextFacingPosition)
        {
            case facingPositions.down: target = standeeBackQ; break;
            case facingPositions.right: target = standeeRightQ; break;
            case facingPositions.left: target = standeeLeftQ; break;
            case facingPositions.up:
            default: target = standeeForwardQ; break;
        }

       

        // Smooth rotate (even if facing didn't change, this is harmless)
        if (turnRoutine != null) StopCoroutine(turnRoutine);
        turnRoutine = StartCoroutine(TurnToRotation(target));

        // Only swap sprites when facing changes
        if (currentFacingPosition == nextFacingPosition) return;
        /*
        switch (nextFacingPosition)
        {
            case facingPositions.down: playerAnimationManager.SetFrontSprites(); break;
            case facingPositions.right: playerAnimationManager.SetSideSprites("right"); break;
            case facingPositions.left: playerAnimationManager.SetSideSprites("left"); break;
            default: playerAnimationManager.SetBackSprites(); break;
        }*/



    }

    IEnumerator TurnToRotation(Quaternion target)
    {
        Quaternion start = standee.transform.rotation;
        float t = 0f;

        // quick early-out
        if (Quaternion.Angle(start, target) < 0.1f)
            yield break;

        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, turnDuration);
            float u = t * t * (3f - 2f * t); // smoothstep
            standee.transform.rotation = Quaternion.Slerp(start, target, u);
            yield return null;
        }

        standee.transform.rotation = target;
        turnRoutine = null;
    }

    public void MovePlayer(Vector2 newMoveValue, bool freeMove, bool isWaiting)
    {
        
        previousPosition = currentPosition;

        int newPositionX = currentPosition.x + Mathf.RoundToInt(newMoveValue.x);
        int newPositionY = currentPosition.y + Mathf.RoundToInt(newMoveValue.y);

        
        

        // FIRST: check bounds BEFORE touching the array
        
        if(!GridUtilities.IsInsideGrid(newPositionX, newPositionY, gridWidth, gridHeight))
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

        nextSquareQuantity = newSquareController.ThisSquareSize;

        Vector3 nextSquareStandPosition = newSquareController.ThisSquarePlayerPosition;
        

        if (!newSquareController.IsMoveableSquare)
        {
            BlockedSquare();
            return;
        }



        // Compare Positions between this and proposed next squareSize to set the entry direction
        Vector2Int newMoveVector = new Vector2Int(newPositionX, newPositionY);

        Vector2 newPosition = new Vector2(
           newSquareController.SquareXPosition,
           newSquareController.SquareZPosition
            );


        StartCoroutine(MoveRoutine(
            newSquareController, 
            newPositionX, 
            newPositionY, 
            newPosition, 
            newSquareController, 
            freeMove,
            isWaiting,
            nextSquareStandPosition
            ));

    }

    void PlayFootStepSound(SquareController squareController)
    {
        AudioManager.Instance.PlayPlayerMoveSoundEffect(squareController.IsWater);
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
            if (squareController.IsWater)
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
        bool isWaiting,
        Vector3 targetPos
        )
    {
        isMoving = true;
        //playerAnimationManager.SetIsWalking(true);
        standeeAnimator.SetBool("isMoving", true);

        PlayFootStepSound(newSquareController);

        Vector3 start = transform.position;
        start.y = yOffset;

        Vector3 end = new Vector3(targetPos.x, yOffset, targetPos.z);

        float duration = 0.25f; // tune feel
        float t = 0f;

        MakeSquareHoldPlayer(currentSquareController, false);
        ActivateWaterStep(currentSquareController, false);
        bool waterSplashTriggered = false;

        bool isGoalSquare = newSquareController.IsGoalSquare;
        if (isGoalSquare)
        {
            SetFacing(0, 1);
        }

        while (t < duration)
        {
            
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);

            if (!waterSplashTriggered && u >= 0.5f && newSquareController.IsWater)
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

        OnPlayerMoved?.Invoke();
        

        isMoving = false;
        //playerAnimationManager.SetIsWalking(false);
        standeeAnimator.SetBool("isMoving", false);
        ApplyMoveResults(newSquareController, freeMove, isWaiting);
        turnOrganiser.BuildNextTurn();
    }
    public int GridHeight => gridHeight;
    public Vector2Int PlayerPosition => currentPosition;

    void SteppedOnGoal()
    {
        SetReachedGoalSquare(true);
        turnOrganiser.LandedOnGoal();
        fateCounter.resetFateCounter();
    }


    void SteppedOnPottard()
    {
        playerStatsController.resetSuffering();
        MovePlayerBackOneSquare();
    }

    void SteppedOnTrap(SquareController newSqCon)
    {
        newSqCon.ActivateTrap(playSoundEffect: true);
        playerStatsController.alterHealth(-1);
        MovePlayerBackOneSquare();
    }

    void SteppedOnMerchant()
    {
        turnOrganiser.LandedOnMerchantSquare();
    }

    void SteppedOnEnemy(SquareController newSqCon)
    {
        int amount = 0;

        switch (nextSquareQuantity)
        {
            case SquareSize.Small:
                amount = 1;
                break;
            case SquareSize.Medium:
                amount = 3;
                break;
            case SquareSize.Large:
                amount = 5;
                break;
            default:
                amount = 3;
                break;
        }

        turnOrganiser.UpdateCurrentEnemySize(amount);
        turnOrganiser.SetLandedOnEnemySquare(true, newSqCon);
    }

    void ApplyMoveTax(SquareController newSqCon, bool isWaiting)
    {
        if (!isWaiting) fateCounter.alterFateCounter(1);
        else fateCounter.alterFateCounter(2);

        if (newSqCon.IsWater)
        {
            playerStatsController.alterSuffering(2);
        }
        else if (newSqCon.IsEmptySquare && !isWaiting && !newSqCon.IsSacred)
        {
            playerStatsController.alterSuffering(1);
        }
    }

    void SteppedOnItem(SquareController newSqCon)
    {
        string squareContentsID = newSqCon.ContentsID;
        bool canAddItem = playerInventory.TryToAddItem(squareContentsID);


        if (canAddItem)
        {
            newSqCon.MakeSquare(SquareType.Empty, newSqCon.ThisMap);
        }
        else
        {
            AudioManager.Instance.playCannotMoveSoundEffect();
        }
    }

    void SteppedOnTreasure(SquareController newSqCon)
    {
        int amount = 0;

        switch (nextSquareQuantity)
        {
            case SquareSize.Small:
                amount = 1;
                break;
            case SquareSize.Medium:
                amount = 3;
                break;
            case SquareSize.Large:
                amount = 5;
                break;
            default:
                amount = 3;
                break;
        }

        playerStatsController.AlterMoney(amount);
        playerStatsController.alterSuffering(amount * -1);
        newSqCon.MakeSquare(SquareType.Empty, newSqCon.ThisMap);
    }

    void SteppedOnHealth(SquareController newSqCon)
    {
        int amount = 0;

        switch (nextSquareQuantity)
        {
            case SquareSize.Small:
                amount = 1;
                break;
            case SquareSize.Medium:
                amount = 3;
                break;
            case SquareSize.Large:
                amount = 5;
                break;
            default:
                amount = 3;
                break;
        }

        playerStatsController.alterHealth(amount);
        int sufferingAmount = 1;


        playerStatsController.alterSuffering(sufferingAmount * -1);
        newSqCon.MakeSquare(SquareType.Empty, newSqCon.ThisMap);
    }
    void ApplyMoveResults(SquareController newSquareController, bool freeMove, bool isWaiting)
    {
        if (newSquareController.IsGoalSquare)
        {
           SteppedOnGoal();
            return;
        }

        if(newSquareController.ThisSquareHoldsPottard) SteppedOnPottard();

        if(newSquareController.IsTrapSquare && newSquareController.TrapActivated == false) SteppedOnTrap(newSquareController);

        if(newSquareController.IsMerchantSquare)
        {
            SteppedOnMerchant();
            return;
        }

        if (newSquareController.IsEnemy)
        {
            SteppedOnEnemy(newSquareController);
            return;
        }

        if (!freeMove) ApplyMoveTax(newSquareController, isWaiting);

        if (newSquareController.IsItemSquare) SteppedOnItem(newSquareController);

        if (newSquareController.IsTreasureSquare)
        {
            SteppedOnTreasure(newSquareController);
        }

        if (newSquareController.IsHealthSquare)
        {
           SteppedOnHealth(newSquareController);
        }
    }
    
    public GameObject[] GetEightSurroundingSquares()
    {
        return GridUtilities.GetEightSurroundingSquares(currentPosition, gridWidth, gridHeight, allSquares);
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
            newSquareController.SquareXPosition,
            yOffset,
            newSquareController.SquareZPosition
        );

        SetStartCurrentPosition(recX, recY);
    }

    void SetStartCurrentPosition(int startCurX, int startCurY)
    {
        currentPosition = new Vector2Int(startCurX, startCurY);
    }



    void LateUpdate()
    {
        
       ApplyVerticalOffset();
    }

    void ApplyVerticalOffset()
    {
        // To Ensure player is consistently above the floor
        Vector3 p = transform.position;
        if (p.y != yOffset)
        {
            p.y = yOffset;
            transform.position = p;
        }
    }

}
