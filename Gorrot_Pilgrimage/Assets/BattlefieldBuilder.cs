using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using TMPro;

public class BattlefieldBuilder : MonoBehaviour
{

    //public int battleFieldSize;
    //BattlefieldSquare[,] grid;
    public GameObject battleFieldSquare;

    public GameObject[,] allSquares;

    public GameObject player;

    public int goalSquareCount = 1;
    public int enemySquareCount = 5;
    public int treasureSquareCount = 5;
   public int terrainSquareCount = 5;
    public int healthSquareCount = 5;
    public int potionSquareCount = 5;

    int playerStartingPosition = 0;

    public bool isFinalMap = false;

    private List<Vector2Int> freeSquares = new List<Vector2Int>();

    public TextMeshProUGUI finalMapText;
    public int currentMapCount = 0;
    public TextMeshProUGUI currentMapCountText;


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

        BuildNewBattlefield();
        
    }


    void FinalMapDecider()
    {
        int chance = Mathf.Clamp(10 - currentMapCount, 2, 8);

        // lower chance higher probability of final
        int roll = UnityEngine.Random.Range(0, chance);

        isFinalMap = (roll == 0);

        finalMapText.text = isFinalMap ? "Final Map" : "Keep Going";
    }

    int CalculateMapSize()
    {
        int mapSize = 10 * currentMapCount;
        return mapSize;
    }

    public void BuildNewBattlefield()
    {
        if(!isFinalMap)
        {
            currentMapCount += 1;
            currentMapCountText.text = "Map: " + currentMapCount.ToString();
            int currentMapSize = CalculateMapSize();

            ClearOldBattlefield();   // <- add this
            FinalMapDecider();
            setPlayerStartSquare(currentMapSize);
            setContentAmount(currentMapSize);


            buildBattleFieldGrid(currentMapSize);
            placePlayer(currentMapSize);
        }
        else
        {
            QuitGame(); 
        }
       
    }

    public void QuitGame()
    {
        // This code only runs in the Unity Editor
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #else
                Application.Quit(); // This runs in standalone builds
        #endif
    }


    void ClearOldBattlefield()
    {
        // Loop backwards to avoid issues when destroying children while iterating
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }


    void setContentAmount(int currentMapSize)
    {
        int area = currentMapSize * currentMapSize;

        // --- 1. Define easy / hard ratios ---

        float easyEnemyRatio = 1f / 40f;
        float easyTreasureRatio = 1f / 20f;
        float easyTerrainRatio = 1f / 20f;
        float easyHealthRatio = 1f / 20f;
        float easyPotionRatio = 1f / 20f;

        float hardEnemyRatio = 1f / 20f;
        float hardTreasureRatio = 1f / 40f;
        float hardTerrainRatio = 1f / 10f;
        float hardHealthRatio = 1f / 40f;
        float hardPotionRatio = 1f / 40f;

        // --- 2. Compute difficulty [0..1] ---

        int maxDifficultyMap = 3;
        float difficulty01;

        if (isFinalMap)
        {
            difficulty01 = 1f;
        }
        else
        {
            difficulty01 = Mathf.Clamp01(
                (currentMapCount - 1f) / (maxDifficultyMap - 1f)
            );
        }

        // --- 3. Lerp ratios based on difficulty ---

        float enemyRatio = Mathf.Lerp(easyEnemyRatio, hardEnemyRatio, difficulty01);
        float treasureRatio = Mathf.Lerp(easyTreasureRatio, hardTreasureRatio, difficulty01);
        float terrainRatio = Mathf.Lerp(easyTerrainRatio, hardTerrainRatio, difficulty01);
        float healthRatio = Mathf.Lerp(easyHealthRatio, hardHealthRatio, difficulty01);
        float potionRatio = Mathf.Lerp(easyPotionRatio, hardPotionRatio, difficulty01);

        // --- 4. Convert ratios to tile counts ---

        enemySquareCount = Mathf.Max(1, Mathf.RoundToInt(area * enemyRatio));
        treasureSquareCount = Mathf.Max(1, Mathf.RoundToInt(area * treasureRatio));
        terrainSquareCount = Mathf.Max(1, Mathf.RoundToInt(area * terrainRatio));
        healthSquareCount = Mathf.Max(1, Mathf.RoundToInt(area * healthRatio));
        potionSquareCount = Mathf.Max(1, Mathf.RoundToInt(area * potionRatio));
    }



    void setPlayerStartSquare(int currentMapSize)
    {
        // ranndom Horizontal Index on the first row
        playerStartingPosition = UnityEngine.Random.Range(0, currentMapSize);
    }

    int setRandomGoalSquare(int currentMapSize)
    {
        return UnityEngine.Random.Range(0, currentMapSize);
    }




   

    void buildBattleFieldGrid(int size)
    {

        allSquares = new GameObject[size, size];
        freeSquares.Clear();

        int randomGoalSquare = setRandomGoalSquare(size);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                GameObject newSquare = Instantiate(battleFieldSquare, transform);
                newSquare.transform.position = new Vector3(x, y, 0);
                allSquares[x, y] = newSquare;

                SquareController newSquareController = newSquare.GetComponent<SquareController>();
                newSquareController.SetSquarePosition(x, y);
                newSquareController.MakeEmptySquare();

                bool isPlayerStart = (x == playerStartingPosition && y == 0);
                bool isGoalSpot = (y == size - 1 && x == randomGoalSquare);

                // Goal placement
                if (isGoalSpot)
                {
                    if (newSquareController != null)
                    {
                        newSquareController.MakeGoalSquare();
                        PlayerCompassController compassController = player.GetComponent<PlayerCompassController>();
                        compassController.SetGoalLocation(newSquare);
                        PlayerDistanceController distanceController = player.GetComponent<PlayerDistanceController>();
                        distanceController.SetGoalLocation(newSquare);

                    }
                    continue; // don't add goal to free squares
                }

                // Don't add the player start tile to free list either
                if (!isPlayerStart)
                {
                    freeSquares.Add(new Vector2Int(x, y));
                }
            }
        }

        AssignContentSquares();


        /*
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool thisSquareIsOccupied = false;

               // grid[x, y] = new BattlefieldSquare(x, y);


                GameObject newSquare = Instantiate(battleFieldSquare, transform);
                newSquare.transform.position = new Vector3(x, y, 0);
                allSquares[x,y] = newSquare;

                SquareController newSquareController = newSquare.GetComponent<SquareController>();
                newSquareController.SetSquarePosition(x, y);
                
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
*/
    }

    void AssignContentSquares()
    {
        // Shuffle-style selection: random index, remove, repeat.

        // Terrain
        PlaceTypeSquares(terrainSquareCount, sq => sq.MakeTerrainSquare());

        // Enemies
        PlaceTypeSquares(enemySquareCount, sq => sq.MakeEnemySquare());

        // Health
        PlaceTypeSquares(healthSquareCount, sq => sq.MakeHealthSquare());

        // Potions
        PlaceTypeSquares(potionSquareCount, sq => sq.MakeItemSquare());

        // Treasure (also hook compass on first one if you want)
        bool treasureTargetSet = false;
        PlaceTypeSquares(treasureSquareCount, sq =>
        {
            sq.MakeTreasureSquare();
            if (!treasureTargetSet)
            {
                PlayerCompassController compassController = player.GetComponent<PlayerCompassController>();
                compassController.SetTreasureLocation(sq.gameObject);
                treasureTargetSet = true;
            }
        });
    }

    void PlaceTypeSquares(int count, System.Action<SquareController> applyType)
    {
        int placed = 0;

        while (placed < count && freeSquares.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, freeSquares.Count);
            Vector2Int coord = freeSquares[index];
            freeSquares.RemoveAt(index);

            SquareController sq = allSquares[coord.x, coord.y].GetComponent<SquareController>();
            if (sq != null)
            {
                applyType(sq);
                placed++;
            }
        }

        if (placed < count)
        {
            Debug.LogWarning($"Could not place full quota ({count}) for type; only placed {placed}.");
        }
    }

    void placePlayer(int size)
    {
        SquareController newSquareController = allSquares[playerStartingPosition,0].GetComponent<SquareController>();

        int testX = newSquareController.GetSquareXPosition();
        int testY = newSquareController.GetSquareYPosition();

        

        PlayerMovementController playerMovementController = player.GetComponent<PlayerMovementController>();
        if (playerMovementController != null)
        {
            playerMovementController.ReceiveBattlefieldSize(size, allSquares);
            playerMovementController.SetPlayerStartSquare(testX, testY);
        }
        else { Debug.LogError("No Player Controller"); }


    }
}
