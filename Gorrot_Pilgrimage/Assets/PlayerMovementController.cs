using Unity.Mathematics;
using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    int[,] battleFieldCoordinates;
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

        playerIsAlive = CheckPlayerAlive();

        if(isPlayerTurn && playerIsAlive)
        {
            MovePlayer(normalizedMoveValue);
        }

    }

    bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < battleFieldSize &&
               y >= 0 && y < battleFieldSize;
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
        nextSquareQuantity = newSquareController.getSquareQuantity();


        if (newSquareController == null)
        {
            BlockedSquare();
            return;
        }

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

        if (newSquareController.isGoalSquare)
        {
            playerStatsController.resetSuffering();
            battlefieldBuilder.BuildNewBattlefield();
            return; // <- stop here, don't move to old newPosition

        }

        newSquareController.ActivateSquareVisited();

        if (newSquareController.isEmptySquare)
        {
            addMovementSuffering();
        }

        if(newSquareController.isItemSquare)
        {
            int amount = 0;
            string potionSize = "Med Pot";

            string squareContentsID = newSquareController.GetContentsID();



            switch (nextSquareQuantity)
            {
                case "small":
                    potionSize = "Sma Pot";
                    break;
                case "medium":
                    potionSize = "Med Pot";
                    break;
                case "large":
                    potionSize = "Big Pot";
                    break;
                default:
                    potionSize = "Med Pot";
                    break;
            }

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

            switch(nextSquareQuantity)
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

            playerStatsController.alterSuffering(amount * -1);

            //playerStatsController.subtractSuffering(amount);
            newSquareController.MakeEmptySquare();
        }

        if(newSquareController.isEnemySquare)
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

            turnOrganiser.landedOnEnemySquare(amount);
            newSquareController.MakeEmptySquare();
        }

        if(newSquareController.isHealthSquare)
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

            //playerStatsController.addHealth(amount);
            playerStatsController.alterHealth(amount);
            int sufferingAmount = Mathf.Clamp(amount, 0, amount-2);

            playerStatsController.alterSuffering(sufferingAmount * -1);

            //playerStatsController.subtractSuffering(sufferingAmount);
            newSquareController.MakeEmptySquare();
        }

        
                
        this.transform.position = newPosition;
        currentPosition = new Vector2Int(newPositionX, newPositionY);

        fateCounter.alterFateCounter(1);

        turnOrganiser.disablePlayerTurn();



       

           
       
       

    }

    void addMovementSuffering()
    {

        playerStatsController.alterSuffering(1);
//        playerStatsController.addSuffering(1);
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
