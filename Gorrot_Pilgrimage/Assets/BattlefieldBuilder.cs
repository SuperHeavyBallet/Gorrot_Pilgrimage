using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BattlefieldBuilder : MonoBehaviour
{

    public int battleFieldSize;
    BattlefieldSquare[,] grid;
    public GameObject battleFieldSquare;

    public GameObject[,] allSquares;

    public GameObject player;

    public int goalSquareCount = 1;
    public int enemySquareCount = 5;
    public int treasureSquareCount = 5;
   public int terrainSquareCount = 5;

    int playerStartingPosition = 0;

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
        playerStartingPosition = UnityEngine.Random.Range(0,battleFieldSize);
        
        enemySquareCount = battleFieldSize / 10;
        treasureSquareCount = battleFieldSize / 10;
        terrainSquareCount = battleFieldSize / 10;

        buildBattleFieldGrid(battleFieldSize);
        placePlayer(battleFieldSize);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    void buildBattleFieldGrid(int size)
    {

        grid = new BattlefieldSquare[size, size];
        allSquares = new GameObject[size, size];

        int placedGoalSquares = 0;
        int placedEnemySquares = 0;
        int placedTreasureSquares = 0;
        int placedTerrainSquares = 0;

      

        int randomGoalSquare = UnityEngine.Random.Range(0, size);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool thisSquareIsOccupied = false;

                grid[x, y] = new BattlefieldSquare(x, y);


                GameObject newSquare = Instantiate(battleFieldSquare, transform);
                newSquare.transform.position = new Vector3(x, y, 0);
                allSquares[x,y] = newSquare;

                SquareController newSquareController = newSquare.GetComponent<SquareController>();
                
                newSquareController.MakeEmptySquare();

                // SKIP the player’s starting tile:
                if (x == 0 && y == playerStartingPosition)
                {
                    continue;
                }

                if (y == size - 1 && x == randomGoalSquare && placedGoalSquares == 0)
                    {
                        if (newSquareController != null)
                        {
                            placedGoalSquares += 1;
                            newSquareController.MakeGoalSquare();
                            PlayerCompassController compassController = player.GetComponent<PlayerCompassController>();
                            compassController.SetGoalLocation(newSquare);
                            thisSquareIsOccupied = true;
                        }
                    }
                    else
                    {
                        if(!thisSquareIsOccupied && placedTerrainSquares < terrainSquareCount)
                    {
                        int randomTerrainSquareChance = UnityEngine.Random.Range(0, size);

                        if(randomTerrainSquareChance < 1)
                        {
                            placedTerrainSquares += 1;
                            newSquareController.MakeTerrainSquare();
                            thisSquareIsOccupied=true;
                        }
                    }

                        if (!thisSquareIsOccupied && placedEnemySquares < enemySquareCount)
                        {

                            int randomEnemySquareChance = UnityEngine.Random.Range(0, size);

                            if (randomEnemySquareChance < 1)
                            {
                                placedEnemySquares += 1;
                                newSquareController.MakeEnemySquare();
                                thisSquareIsOccupied = true;
                            }

                        }

                        if (!thisSquareIsOccupied && placedTreasureSquares < treasureSquareCount)
                        {
                            int randomTreasureSquareChance = UnityEngine.Random.Range(0, size);

                            if (randomTreasureSquareChance < 1)
                            {
                                placedTreasureSquares += 1;
                                newSquareController.MakeTreasureSquare();
                                
                                thisSquareIsOccupied = true;
                            PlayerCompassController compassController = player.GetComponent<PlayerCompassController>();
                            compassController.SetTreasureLocation(newSquare);

                        }
                        }

                    }
                

               
                
                


            }
        }
    }

    void placePlayer(int size)
    {
        Vector2 startPosition = new Vector2(
             grid[playerStartingPosition, 0].x,
            grid[playerStartingPosition, 0].y
            );

        player.transform.position = new Vector3( 
            startPosition.x, 
            startPosition.y, 
            0);


        PlayerMovementController playerMovementController = player.GetComponent<PlayerMovementController>();
        if (playerMovementController != null)
        {
            playerMovementController.ReceiveBattlefieldSize(size, allSquares);
            playerMovementController.SetStartCurrentPosition(startPosition);
        }


    }
}
