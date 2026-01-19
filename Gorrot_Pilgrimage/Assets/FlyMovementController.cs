using System.Collections;
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

    public void SetPlayerStartSquare(int recX, int recY)
    {
        SquareController newSquareController = allSquares[recX, recY].GetComponent<SquareController>();

        this.transform.position = new Vector2(
            newSquareController.GetSquareXPosition(),
            newSquareController.GetSquareYPosition()
        );

        //newSquareController.ActivateSquareVisited();

        SetStartCurrentPosition(recX, recY);
    }

    void SetStartCurrentPosition(int startCurX, int startCurY)
    {
        currentPosition = new Vector2Int(startCurX, startCurY);
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

        StartCoroutine(MoveRoutine(newSquareController, newPositionX, newPositionY, newPosition, newSquareController));

        ResetCanMove();


    }

    IEnumerator MoveRoutine(
        SquareController targetSquare,
        int newX,
        int newY,
        Vector2 worldTargetPos,
        SquareController newSquareController)
    {

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
