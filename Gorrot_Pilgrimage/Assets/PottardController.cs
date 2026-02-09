using UnityEngine;
using System.Collections;
public class PottardController : MonoBehaviour
{
    Coroutine pottardMoveRoutine;

    Vector2Int[] freeSquares;
    GameObject[,] allSquares;

    Vector2Int currentPosition;
    Vector2Int prevDirection;

    bool landOnGoal;

    [SerializeField, Range(0f, 1f)] float keepGoingChance = 0.65f;

    SquareController currentSquareController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       prevDirection = Vector2Int.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCurrentBattlefield(Vector2Int[] recFreeSquares, GameObject[,] recAllSquares)
    {

        freeSquares = recFreeSquares;
        allSquares = recAllSquares;

    }

    public void SetStartPosition(Vector2 startPos)
    {
        this.transform.position = startPos;

        StartNewMoveRoutine();
    }

    void StartNewMoveRoutine()
    {
        if (pottardMoveRoutine != null)
        {
            StopCoroutine(pottardMoveRoutine);
            pottardMoveRoutine = null;
        }

        landOnGoal = false;

        Vector2Int current = Vector2Int.RoundToInt(transform.position);

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        var candidates = new System.Collections.Generic.List<Vector2Int>();

        foreach (var d in dirs)
        {
            Vector2Int p = current + d;

            if (p.x < 0 || p.y < 0 ||
                p.x >= allSquares.GetLength(0) ||
                p.y >= allSquares.GetLength(1))
                continue;

            var sq = allSquares[p.x, p.y]?.GetComponent<SquareController>();
            if (sq != null && (sq.isEmptySquare || sq.isGoalSquare || sq.isTreasureSquare))
            {
                candidates.Add(p);
            }



        }

        if (candidates.Count == 0)
        {
            // No valid moves: either wait, or try again later
            pottardMoveRoutine = StartCoroutine(PottardWaitThenRetry());
            return;
        }


        Vector2Int next = candidates[UnityEngine.Random.Range(0, candidates.Count)];

        Vector2Int chosenDir = next - current;
        prevDirection = chosenDir;

        Vector2Int forwardPos = current + prevDirection;

        bool canGoForward = prevDirection != Vector2Int.zero && candidates.Contains(forwardPos);

        if (canGoForward && UnityEngine.Random.value < keepGoingChance)
        {
            next = forwardPos;
        }
        else
        {
            next = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        // Store heading as a direction, not a position
        prevDirection = next - current;

        SquareController squareController = allSquares[next.x, next.y].GetComponent<SquareController>();
        if (squareController != null)
        {
           
                if (squareController.isGoalSquare)
                {
                    landOnGoal = true;
                }

                if(squareController.isTreasureSquare)
                {
                pottardMoveRoutine = StartCoroutine(PottardMovement(next, squareController));
            }
            else
            {
                pottardMoveRoutine = StartCoroutine(PottardMovement(next, null));
            }




                
        }
    }

        IEnumerator PottardWaitThenRetry()
        {
            yield return new WaitForSeconds(1f);
            StartNewMoveRoutine();
        }



        IEnumerator PottardMovement(Vector2Int newPosition, SquareController squareToLand)
        {


            yield return new WaitForSeconds(1);

            Vector3 start = transform.position;
            Vector3 end = new Vector3(newPosition.x, newPosition.y, transform.position.z);

            float duration = 0.25f;
            float t = 0f;

            if(currentSquareController != null)
            {
            Debug.LogError("Square Cont Not Null");
                currentSquareController.MakePottardSquare(false);
            }
        

            while (t < duration)
            {
                
                t += Time.deltaTime;
                float u = Mathf.Clamp01(t / duration);

                // Smoothstep-ish curve (feels nicer than linear)
                u = u * u * (3f - 2f * u);

                transform.position = Vector3.Lerp(start, end, u);
                yield return null;
            }

            this.transform.position = new Vector2(newPosition.x, newPosition.y);
            yield return null;

        if( squareToLand != null)
        {
            squareToLand.MakeEmptySquare();
            squareToLand.MakePottardSquare(true);
            currentSquareController = squareToLand;
        }

            currentPosition = new Vector2Int(newPosition.x, newPosition.y);

            if(landOnGoal)
            {
            Destroy(gameObject);
        }

            else
            {
                yield return new WaitForSeconds(1);

                StartNewMoveRoutine();
            }
            
        }
    
}
