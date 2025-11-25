using Unity.Mathematics;
using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    int[,] battleFieldCoordinates;
    public GameObject square;
    int battleFieldSize = 0;

    GameObject[,] allSquares;

    BattlefieldSquare[,] grid;

    public Vector2 currentPosition = new Vector2 (0,0);

    public bool isPlayerTurn;
    public TurnOrganiser turnOrganiser;


    public class BattlefieldSquare
    {
        public int x;
        public int y;

        public bool blocked;
        public bool visited;
        public string tileType;

        public BattlefieldSquare(int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerTurn = turnOrganiser.GetPlayerTurn();
        
    }

    public void ReceiveMoveInput(Vector2 receivedMoveValue)
    {


        Debug.Log("Player Received Move Command: " + receivedMoveValue);
        Vector2 normalizedMoveValue = receivedMoveValue;
        if (receivedMoveValue.x > 0) { normalizedMoveValue.x = 1;}

        if (receivedMoveValue.y > 0) { normalizedMoveValue.y = 1; }


        if(isPlayerTurn)
        {
            MovePlayer(normalizedMoveValue);
        }
       

        
        

        

    }

    public void MovePlayer(Vector2 newMoveValue)
    {
        turnOrganiser.disablePlayerTurn();
        Vector2 newPosition = new Vector2(currentPosition.x + newMoveValue.x, currentPosition.y + newMoveValue.y);


        this.transform.position = newPosition;
        currentPosition = newPosition;
    }

    public void ReceiveBattlefieldSize(int size, GameObject[,] receivedAllSquares)
    {
        Debug.Log("Battlefield SIze: " + size + " x " + size);
        battleFieldSize = size;

        allSquares = receivedAllSquares;
        Debug.Log(allSquares);


        int randomX = UnityEngine.Random.Range(0, size);
        int randomY = UnityEngine.Random.Range(0, size);

        GameObject randomStartSquare = allSquares[randomX, randomY];
        SquareController squareController = randomStartSquare.GetComponent<SquareController>();
        Transform squareCentre = squareController.GetSquareCentre();
        squareController.ActivateSquareVisited();


        //currentPosition = new Vector2(randomX,randomY);
        //this.transform.position = allSquares[randomX, randomY].transform.position;


    }

    public void SetStartCurrentPosition(Vector2 startPosition)
    {
        currentPosition = startPosition;
    }


}
