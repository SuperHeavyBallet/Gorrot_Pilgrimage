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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStatsController = this.GetComponent<PlayerStatsController>();
        playerIsAlive = CheckPlayerAlive();
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


        Debug.Log("Player Received Move Command: " + receivedMoveValue);
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

        turnOrganiser.disablePlayerTurn();

        Vector2 newPosition = new Vector2(
           newSquareController.GetSquareXPosition(),
           newSquareController.GetSquareYPosition()
            );

        if(newSquareController.isTreasureSquare)
        {
            playerStatsController.subtractSuffering(3);
        }

        if(newSquareController.isEnemySquare)
        {
            playerStatsController.subtractHealth(1);
            playerStatsController.resetSuffering();
        }

        if(newSquareController.isHealthSquare)
        {
            playerStatsController.addHealth(3);
            playerStatsController.subtractSuffering(3);
        }

        if(newSquareController.isGoalSquare)
        {
            playerStatsController.resetSuffering();
            battlefieldBuilder.BuildNewBattlefield();
            return; // <- stop here, don't move to old newPosition

        }
                
        this.transform.position = newPosition;
        currentPosition = new Vector2Int(newPositionX, newPositionY);

        UpdateStats();

           
       
       

    }

    void UpdateStats()
    {
        playerStatsController.addSuffering(1);
    }

    void BlockedSquare()
    {
        audioManager.playCannotMoveSoundEffect();
        Debug.Log("Blocked Square");
    }

    public void ReceiveBattlefieldSize(int size, GameObject[,] receivedAllSquares)
    {
        battleFieldSize = size;

        allSquares = receivedAllSquares;


        int randomX = UnityEngine.Random.Range(0, size);
        int randomY = UnityEngine.Random.Range(0, size);

        GameObject randomStartSquare = allSquares[randomX, randomY];
        SquareController squareController = randomStartSquare.GetComponent<SquareController>();
        Transform squareCentre = squareController.GetSquareCentre();
        squareController.ActivateSquareVisited();

    }

    public void SetPlayerStartSquare(int recX, int recY)
    {
        SquareController newSquareController = allSquares[recX, recY].GetComponent<SquareController>();

        this.transform.position = new Vector2(
            newSquareController.GetSquareXPosition(),
            newSquareController.GetSquareYPosition()
        );

        SetStartCurrentPosition(recX, recY);
    }

    void SetStartCurrentPosition(int startCurX, int startCurY)
    {
        currentPosition = new Vector2Int(startCurX, startCurY);
    }





}
