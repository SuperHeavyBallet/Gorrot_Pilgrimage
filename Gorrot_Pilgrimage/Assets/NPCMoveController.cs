using UnityEngine;
using System.Collections;

public class NPCMoveController : MonoBehaviour
{

    Coroutine moveRoutine;

    [SerializeField] GameObject standee;
    [SerializeField] float turnDuration = 0.25f; // small = snappy
    Coroutine turnRoutine;

    Vector2Int[] freeSquares;
    GameObject[,] allSquares;

    Vector2Int currentPosition;
    Vector2Int previousPosition;
    Vector2Int prevDirection;

    [SerializeField, Range(0f, 1f)] float keepGoingChance = 0.65f;

    SquareController currentSquareController;

    enum facingPositions
    {
        up, down, left, right
    }

    facingPositions nextFacingPosition = facingPositions.up;
    facingPositions currentFacingPosition = facingPositions.up;
    private Quaternion standeeForwardQ, standeeRightQ, standeeLeftQ, standeeBackQ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prevDirection = Vector2Int.zero;

        standeeForwardQ = standee.transform.rotation;
        Vector3 turnAxis = Vector3.up; // or board.forward / board.right depending on your scene

        standeeRightQ = standeeForwardQ * Quaternion.AngleAxis(90f, turnAxis);
        standeeLeftQ = standeeForwardQ * Quaternion.AngleAxis(-90f, turnAxis);
        standeeBackQ = standeeForwardQ * Quaternion.AngleAxis(180f, turnAxis);
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

    public void SetStartPosition(Vector2Int startGridPos)
    {
        currentPosition = startGridPos;

        SquareController sq = allSquares[startGridPos.x, startGridPos.y].GetComponent<SquareController>();

        transform.position = sq.ThisSquarePlayerPosition;

        currentSquareController = sq;
        sq.MakePottardSquare(true);

        StartNewMoveRoutine(Vector2Int.zero);
    }

    void StartNewMoveRoutine(Vector2Int overrideDelta)
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }



        Vector2Int current = currentPosition;



        Vector2Int next;

        // If we are forced to move (back one squareSize), treat it as a DELTA
        if (overrideDelta != Vector2Int.zero)
        {
            next = current + overrideDelta;

            // Validate bounds
            if (next.x < 0 || next.y < 0 ||
                next.x >= allSquares.GetLength(0) ||
                next.y >= allSquares.GetLength(1))
            {
                // Can't move there; just retry normal movement
                moveRoutine = StartCoroutine(WaitThenRetry());
                return;
            }

            var sq = allSquares[next.x, next.y]?.GetComponent<SquareController>();
            if (!IsCandidateSquare(sq))
            {
                moveRoutine = StartCoroutine(WaitThenRetry());
                return;
            }
        }
        else
        {
            // Normal wandering: build candidates from current
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
                if (IsCandidateSquare(sq))
                    candidates.Add(p);
            }

            if (candidates.Count == 0)
            {
                moveRoutine = StartCoroutine(WaitThenRetry());
                return;
            }

            // Forward bias
            Vector2Int forwardPos = current + prevDirection;
            bool canGoForward = prevDirection != Vector2Int.zero && candidates.Contains(forwardPos);

            if (canGoForward && UnityEngine.Random.value < keepGoingChance)
                next = forwardPos;
            else
                next = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        // Now we have a valid 'next'
        prevDirection = next - current;

        previousPosition = current;

        SquareController squareController = allSquares[next.x, next.y].GetComponent<SquareController>();
        if (squareController != null)
        {

        


            moveRoutine = StartCoroutine(PottardMovement(next, squareController));






        }
    }

    static bool IsCandidateSquare(SquareController sq)
    {
        if (sq is null) return false;

        bool isMoveableType =
            sq.IsEmptySquare
            || sq.IsGoalSquare;
        bool squareValid =
            isMoveableType
            && !sq.ThisSquareHoldsPlayer
            && !sq.IsWater
            && !sq.IsTrapSquare;

        return squareValid;
    }

    IEnumerator WaitThenRetry()
    {
        yield return new WaitForSeconds(1f);
        StartNewMoveRoutine(Vector2Int.zero);
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

    IEnumerator PottardMovement(Vector2Int newPosition, SquareController squareToLand)
    {

        Vector2Int moveDelta = newPosition - currentPosition;
        SetFacing(moveDelta.x, moveDelta.y);

        if (turnRoutine != null)
            yield return turnRoutine;

        Vector3 start = transform.position;
        Vector3 end = new Vector3(
                 squareToLand.SquareXPosition,
                 transform.position.y,
                 squareToLand.SquareZPosition
                 );

        float duration = 0.35f;
        float t = 0f;

        if (currentSquareController != null)
        {
            currentSquareController.MakePottardSquare(false);
        }





        while (t < duration)
        {

            if (squareToLand.ThisSquareHoldsPlayer)
            {
                MoveBackOneSquare();
                yield break;
            }

            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);

            // Smoothstep-ish curve (feels nicer than linear)
            u = u * u * (3f - 2f * u);

            transform.position = Vector3.Lerp(start, end, u);
            yield return null;
        }

        this.transform.position = new Vector3(
            squareToLand.SquareXPosition,
            transform.position.y,
            squareToLand.SquareZPosition
            );

        yield return null;



        currentPosition = new Vector2Int(newPosition.x, newPosition.y);

        if (squareToLand != null)
        {

            if (squareToLand.IsTreasureSquare)
            {
                squareToLand.MakeEmptySquare();
            }




            squareToLand.MakePottardSquare(true);
            currentSquareController = squareToLand;
        }

  

        else
        {
            yield return new WaitForSeconds(1);

            StartNewMoveRoutine(Vector2Int.zero);
        }

    }

    void MoveBackOneSquare()
    {
        Vector2Int current = Vector2Int.RoundToInt(transform.position);
        Vector2Int delta = previousPosition - current;
        StartNewMoveRoutine(delta);
    }
}
