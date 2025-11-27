using UnityEngine;

public class SquareController : MonoBehaviour
{
    public bool hasBeenVisited;

    public GameObject visitedSprite;
    public Transform squareCentre;

    public bool isGoalSquare;
    public bool isTreasureSquare;
    public bool isEnemySquare;
    public bool isTerrainSquare;
    public bool isEmptySquare;
    public bool isHealthSquare;

    public GameObject goalSquareSprite;
    public GameObject treasureSquareSprite;
    public GameObject enemySquareSprite;
    public GameObject terrainSquareSprite;
    public GameObject emptySquareSprite;
    public GameObject healthSquareSprite;

    public int squareX = 0;
    public int squareY = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

       
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeHealthSquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = false;
        isHealthSquare = true;

        enemySquareSprite.SetActive(false);
        treasureSquareSprite.SetActive(false);
        terrainSquareSprite.SetActive(false);
        emptySquareSprite.SetActive(false);
        healthSquareSprite.SetActive(true);
    }

    public void MakeGoalSquare()
    {
        isGoalSquare = true;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = false;
        isHealthSquare = true;

        goalSquareSprite.SetActive(true);

        enemySquareSprite.SetActive(false);
        treasureSquareSprite.SetActive(false);
        terrainSquareSprite.SetActive(false);
        emptySquareSprite.SetActive(false);
        healthSquareSprite.SetActive(false);
    }

    public void MakeTreasureSquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = true;
        isEmptySquare = false;

        goalSquareSprite.SetActive(false);
        enemySquareSprite.SetActive(false);
        terrainSquareSprite.SetActive(false);
        emptySquareSprite.SetActive(false);
        healthSquareSprite.SetActive(false);

        treasureSquareSprite.SetActive(true);
    }

    public void MakeEnemySquare()
    {
        isGoalSquare= false;
        isEnemySquare = true;
        isTreasureSquare = false;
        isEmptySquare = false;

        goalSquareSprite.SetActive(false);
        enemySquareSprite.SetActive(true);
        treasureSquareSprite.SetActive(false);
        terrainSquareSprite.SetActive(false);
        emptySquareSprite.SetActive(false);
        healthSquareSprite.SetActive(false);

    }

    public void MakeTerrainSquare()
    {
        isGoalSquare= false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isTerrainSquare = true;
        isEmptySquare = false;

        goalSquareSprite.SetActive(false);
        enemySquareSprite.SetActive(false);
        treasureSquareSprite.SetActive(false);
        terrainSquareSprite.SetActive(true);
        emptySquareSprite.SetActive(false);
        healthSquareSprite.SetActive(false);
    }


    public void MakeEmptySquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = true;


        goalSquareSprite.SetActive(false);
        enemySquareSprite.SetActive(false);
        treasureSquareSprite.SetActive(false);
        terrainSquareSprite.SetActive(false);
        emptySquareSprite.SetActive(true);
        healthSquareSprite.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasBeenVisited)
        {
            if (collision != null)
            {
                if(collision.gameObject.CompareTag("Player"))
                {
                    ActivateSquareVisited();
                    
                }
            }
        }
    }

    public Transform GetSquareCentre()
    {
        return squareCentre;
    }

    public void ActivateSquareVisited()
    {
        hasBeenVisited = true;
        visitedSprite.gameObject.SetActive(true);
    }

    public void SetSquarePosition(int x, int y)
    {
        squareX = x; squareY = y;

    }

    public int GetSquareXPosition()
    {
        return squareX;
    }

    public int GetSquareYPosition()
    {
        return squareY;
    }

    public bool isMoveableSquare()
    {
        return !isTerrainSquare;
    }
}
