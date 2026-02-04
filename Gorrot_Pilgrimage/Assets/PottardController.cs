using UnityEngine;
using System.Collections;
public class PottardController : MonoBehaviour
{
    Coroutine pottardMoveRoutine;

    Vector2Int[] freeSquares;
    GameObject[,] allSquares;

    Vector2Int currentPosition;

    bool landOnGoal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
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
            if (sq != null && sq.isEmptySquare || sq.isGoalSquare)
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

        SquareController squareController = allSquares[next.x, next.y].GetComponent<SquareController>();
        if (squareController != null)
        {
            {
                if (squareController.isGoalSquare)
                {
                    landOnGoal = true;
                }
            }

            pottardMoveRoutine = StartCoroutine(PottardMovement(next));
        }
    }

        IEnumerator PottardWaitThenRetry()
        {
            yield return new WaitForSeconds(1f);
            StartNewMoveRoutine();
        }



        IEnumerator PottardMovement(Vector2Int newPosition)
        {


            yield return new WaitForSeconds(1);

            Vector3 start = transform.position;
            Vector3 end = new Vector3(newPosition.x, newPosition.y, transform.position.z);

            float duration = 0.25f;
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

            this.transform.position = new Vector2(newPosition.x, newPosition.y);

            currentPosition = new Vector2Int(newPosition.x, newPosition.y);

            if(landOnGoal)
            {
                Destroy(this);
            }

            else
            {
                yield return new WaitForSeconds(1);

                StartNewMoveRoutine();
            }
            
        }
    
}
