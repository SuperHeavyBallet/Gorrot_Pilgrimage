using UnityEngine;

public class FlyMovementController : MonoBehaviour
{
    Vector2Int currentPosition;
    Vector2Int previousPosition;

    int battleFieldSize;

    GameObject[,] allSquares;

    public bool canMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RollNewDirection()
    {
        if(canMove)
        {
            canMove = false;

            int randomInt = UnityEngine.Random.Range(0, 4);

            Vector2Int newDirection = new Vector2Int(0, 0);

            switch (randomInt)
            {
                case 0:
                    newDirection.x = 1;
                    break;
                case 1:
                    newDirection.x = -1;
                    break;
                case 2:
                    newDirection.y = 1;
                    break;
                case 3:
                    newDirection.y = -1;
                    break;
                default:
                    newDirection.x = 0;
                    break;
            }

            MoveFly(newDirection);
        }
       
    }

    void MoveFly(Vector2 newMoveValue)
    {
        previousPosition = currentPosition;

        int newPositionX = currentPosition.x + Mathf.RoundToInt(newMoveValue.x);
        int newPositionY = currentPosition.y + Mathf.RoundToInt(newMoveValue.y);

        // FIRST: check bounds BEFORE touching the array
        if (!IsInsideGrid(newPositionX, newPositionY))
        {
            ResetCanMove();
            return;
        }

        SquareController newSquareController = allSquares[newPositionX, newPositionY].GetComponent<SquareController>();

        Vector2 newPosition = new Vector2(
          newSquareController.GetSquareXPosition(),
          newSquareController.GetSquareYPosition()
           );

        this.transform.position = newPosition;

        ResetCanMove();


    }

    void ResetCanMove()
    {
        canMove = true;
    }

    public void SetBattleFieldSize(int newBattleFieldSize, GameObject[,] receivedAllSquares)
    {
        battleFieldSize = newBattleFieldSize;

        allSquares = receivedAllSquares;
    }

    bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < battleFieldSize &&
               y >= 0 && y < battleFieldSize;
    }
}
