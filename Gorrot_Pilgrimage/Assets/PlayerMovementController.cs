using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using GorrotGame;
using System.Collections.Generic;



public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] float yOffset = 0.65f;

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
    public SquareSize nextSquareQuantity = SquareSize.Medium;

    PlayerInventory playerInventory;

    public FateCounter fateCounter;

    bool isMoving;

    bool reachedGoalSquare;

    public GameObject playerSprite;

   [SerializeField] PlayerAnimationManager playerAnimationManager;
     Vector3 standeeForwardRotationEuler; // for editing in inspector
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

    int previousPositionX = 0;
    int previousPositionY = 0;
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
        // Stop movement mid-flight so coroutine doesn't keep touching old squares
        if (turnRoutine != null)
        {
            StopCoroutine(turnRoutine);
            turnRoutine = null;
        }
        StopAllCoroutines();

        isMoving = false;
        reachedGoalSquare = false;

        // Drop dead references
        currentSquareController = null;
        allSquares = null;

        // Optional: if you want to fully reset facing/anim
        playerAnimationManager.SetIsWalking(false);
        if (standeeAnimator) standeeAnimator.SetBool("isMoving", false);
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

    bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth &&
         y >= 0 && y < gridHeight;
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

        switch (nextFacingPosition)
        {
            case facingPositions.down: playerAnimationManager.SetFrontSprites(); break;
            case facingPositions.right: playerAnimationManager.SetSideSprites("right"); break;
            case facingPositions.left: playerAnimationManager.SetSideSprites("left"); break;
            default: playerAnimationManager.SetBackSprites(); break;
        }



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

        nextSquareQuantity = newSquareController.ThisSquareSize;

        Vector3 nextSquareStandPosition = newSquareController.ThisSquarePlayerPosition;
        

        //bool isMoveableSquare = newSquareController.IsMoveableSquare;

        if (!newSquareController.IsMoveableSquare)
        {
            BlockedSquare();
            return;
        }



        // Compare Positions between this and proposed next squareSize to set the entry direction
        Vector2Int newMoveVector = new Vector2Int(newPositionX, newPositionY);
        //newSquareController.SetEntryDirection(currentPosition, newMoveVector);

        Vector2 newPosition = new Vector2(
           newSquareController.SquareXPosition,
           newSquareController.SquareYPosition
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
        if (squareController.IsWater)
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
        playerAnimationManager.SetIsWalking(true);
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
        playerAnimationManager.SetIsWalking(false);
        standeeAnimator.SetBool("isMoving", false);
        ApplyMoveResults(newSquareController, freeMove, isWaiting);
        turnOrganiser.BuildNextTurn();
    }

    void ApplyMoveResults(SquareController newSquareController, bool freeMove, bool isWaiting)
    {
        //newSquareController.ActivateSquareVisited();

        if (newSquareController.IsGoalSquare)
        {
            //newSquareController.MakeGoalSquarePressed();
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

        if(newSquareController.IsTrapSquare)
        {
            if(newSquareController.TrapActivated == false)
            {
               
                newSquareController.ActivateTrap(playSoundEffect : true);
                playerStatsController.alterHealth(-1);
                MovePlayerBackOneSquare();
            }
            
        }

        if(newSquareController.IsMerchantSquare)
        {
            turnOrganiser.LandedOnMerchantSquare();
            return;
        }

        if (newSquareController.IsEnemy)
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
            turnOrganiser.SetLandedOnEnemySquare(true, newSquareController);
            return;
        }


        if (!freeMove)
        {
            if (!isWaiting) fateCounter.alterFateCounter(1);
            else fateCounter.alterFateCounter(2);

            if (newSquareController.IsWater)
            {
                playerStatsController.alterSuffering(2); // or 1, whatever intended
            }
            else if (newSquareController.IsEmptySquare && !isWaiting && !newSquareController.IsSacred)
            {
                playerStatsController.alterSuffering(1);
            }
        }




        if (newSquareController.IsItemSquare)
        {
            string squareContentsID = newSquareController.ContentsID;
            bool canAddItem = playerInventory.TryToAddItem(squareContentsID);
            

            if (canAddItem)
            {
                newSquareController.MakeSquare(SquareType.Empty, newSquareController.ThisMap);
            }
            else
            {
                AudioManager.Instance.playCannotMoveSoundEffect();
            }

        }

        if (newSquareController.IsTreasureSquare)
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
            newSquareController.MakeSquare(SquareType.Empty, newSquareController.ThisMap);
        }

        

        if (newSquareController.IsHealthSquare)
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
            newSquareController.MakeSquare(SquareType.Empty, newSquareController.ThisMap);
        }
    }
    
    public GameObject[] GetEightSurroundingSquares()
    {
        Vector2Int playerCurrentPosition = currentPosition;

        List<GameObject> neighbours = new List<GameObject>();

        int x = currentPosition.x;
        int y = currentPosition.y;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue; // skip the centre square

                int nx = x + dx;
                int ny = y + dy;

                // boundary check (important!)
                if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
                {
                    neighbours.Add(allSquares[nx, ny]);
                }
            }
        }

        // Maybe later
        Vector2Int[] offsets =
        {
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1)
        };

       

        return neighbours.ToArray();

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
            newSquareController.SquareYPosition
        );

        SetStartCurrentPosition(recX, recY);
    }

    void SetStartCurrentPosition(int startCurX, int startCurY)
    {
        currentPosition = new Vector2Int(startCurX, startCurY);
    }



    void LateUpdate()
    {
        /*
        Vector3 p = transform.position;
        if (p.z != zOffset)
        {
            p.z = zOffset;
            transform.position = p;
        }*/
    }

}
