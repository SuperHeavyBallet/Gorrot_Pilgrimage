using UnityEngine;
using System.Collections;
public class PottardController : MonoBehaviour
{
    Coroutine pottardMoveRoutine;

    Vector2Int freeSquares;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartNewMoveRoutine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCurrentBattlefield(Vector2Int recFreeSquares)
    {
        freeSquares = recFreeSquares;
    }

    

    void StartNewMoveRoutine()
    {
        if(pottardMoveRoutine != null)
        {
            pottardMoveRoutine = null;
        }

        pottardMoveRoutine = StartCoroutine(PottardMovement());
    }

    IEnumerator PottardMovement()
    {
        yield return new WaitForSeconds(3);

        Vector2 currentPosition = this.transform.position;

        this.transform.position = new Vector2(currentPosition.x, currentPosition.y + 1);

        yield return new WaitForSeconds(3);

        StartNewMoveRoutine();
    }
}
