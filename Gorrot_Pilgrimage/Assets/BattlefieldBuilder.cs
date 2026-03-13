using System;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using GorrotGame;

public class BattlefieldBuilder : MonoBehaviour
{
    [SerializeField] BattleFieldGridBuilder gridBuilder;

    [SerializeField] NPCBuilder npcBuilder;

    [SerializeField] GameObject battleFieldSquare;

    [SerializeField] GameObject[,] allSquares;

    [SerializeField] GameObject player;

    [SerializeField] UIController uiController;

    int enemySquareCount = 5;
    int treasureSquareCount = 5;
    int terrainSquareCount = 5;
    int healthSquareCount = 5;
    int potionSquareCount = 5;

    int playerStartingPosition = 0;

    bool isFinalMap = false;

    private List<Vector2Int> freeSquares = new List<Vector2Int>();
    List<GameObject> candidateBaseSquaresforLargeItems = new List<GameObject>();

    List<GameObject> enemySquares = new List<GameObject>();

    [SerializeField] MapCatalogue mapCatalogue;


    MapData previousMap;
    MapData thisMap;


    //[SerializeField] GameObject hasMerchantIcon;
    //[SerializeField] MerchantShopController merchantShopController;

    [SerializeField] GameObject wildMapIcon;

    PlayerCompassController playerCompassController;
    PlayerDistanceController playerDistanceController;
    PlayerMovementController playerMovementController;
    PlayerStatReceiver playerStatReceiver;
    [SerializeField] GoalPhaseResolution goalPhaseResolution;
    [SerializeField] GameObject transitionScreen;

    Vector2 goalSquareLocation;
    bool isLost;

  //  [Header("Sacred Path Drunkenness")]
   // [Tooltip("0 = always best, 1 = very random")]
   // [SerializeField, Range(0f, 1f)] float drunkenness;
    //[Tooltip("higher = more greedy, lower = more meandery")]
    //[SerializeField, Range(0.1f, 10f)] float weightSharpness;

    [SerializeField] TurnOrganiser turnOrganiser;


    [SerializeField] GameObject fly;

    List<SquareController> waterAdjacentSquares = new List<SquareController>();
    List<SquareController> waterAdjacentSquaresForSprites = new List<SquareController>();

    [SerializeField] GameObject pottardReference;

    List<(GameObject obj, OrthogonalPositions edge)> borderSquares 
    = new List<(GameObject obj, OrthogonalPositions edge)>();


    List<(GameObject obj, CornerPositions corner)> cornerBorderSquares
    = new List<(GameObject obj, CornerPositions corner)>();

    [SerializeField] OverHeadSpansBuilder overHeadSpansBuilder;

    void Awake()
    {

        if (battleFieldSquare == null) Debug.LogError("Battlefield square prefab not set", this);
        if (player == null) Debug.LogError("Player not set", this);
        if (uiController == null) Debug.LogError("UIController not set", this);
        if (mapCatalogue == null) Debug.LogError("MapCatalogue not set", this);
       // if (hasMerchantIcon == null) Debug.LogError("Merchant icon not set", this);
       // if (merchantShopController == null) Debug.LogError("Merchant Shop Controller not set", this);

        if (player != null)
        {
            playerMovementController = player.GetComponent<PlayerMovementController>();
            playerCompassController = player.GetComponent<PlayerCompassController>();
            playerDistanceController = player.GetComponent<PlayerDistanceController>();
            playerStatReceiver = player.GetComponent<PlayerStatReceiver>();

            if (playerMovementController == null) Debug.LogError("PlayerMovementController missing", player);
            if (playerCompassController == null) Debug.LogError("PlayerCompassController missing", player);
            if (playerDistanceController == null) Debug.LogError("PlayerDistanceController missing", player);
            if (playerStatReceiver == null) Debug.LogError("PlayerStatReceiver missing", player);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiController.ActivateBlackScreen(true);
        SetFirstMap();
        BuildNewBattlefield();
    }
    void SetFirstMap() { previousMap = mapCatalogue.GetFirstMap(); }

    [SerializeField] LargeTerrainBuilder largeTerrainBuilder;
    
    /*
    void TestSpaceForBigPieces()
    {

        

        

        candidateBaseSquaresforLargeItems.Clear();

        // 0..1 where 0 = none, 1 = "as many as possible"
        float density = Mathf.Clamp01(thisMap.GetFourBlockTerrainDensity());

        List<Vector2Int> candidates = new List<Vector2Int>();
        Vector2Int[] freeSquaresArray = freeSquares.ToArray();





        for (int i = 0; i < freeSquaresArray.Length; i++)
        {
            int x = freeSquaresArray[i].x;
            int y = freeSquaresArray[i].y;

            // quick bounds guard: since we need x+1 and y+1, base must not be on top/right edge
            if (y == 0) continue;
            if (x < 0 || y < 0 || x + 1 >= thisMap.GetMapSize() || y + 1 >= thisMap.GetMapSize())
                continue;

           

            SquareController baseSq = allSquares[x, y].GetComponent<SquareController>();
            if (baseSq == null) continue;
            if (!baseSq.IsEmptySquare) continue;
            if (baseSq.IsSacred) continue;


            // Check the other 3 tiles of the 2x2 are empty & not sacred
            Vector2Int[] dirs =
            {
            new Vector2Int(0, 1),  // N
            new Vector2Int(1, 0),  // E
            new Vector2Int(1, 1),  // NE
        };

            bool allFree = true;
            for (int j = 0; j < dirs.Length; j++)
            {
                int nx = x + dirs[j].x;
                int ny = y + dirs[j].y;

                SquareController nSq = allSquares[nx, ny].GetComponent<SquareController>();
                if (nSq == null || nSq.IsSacred || !nSq.IsEmptySquare)
                {
                    allFree = false;
                    break;
                }
            }

            if (!allFree) continue;

            // IMPORTANT: at this stage only require the halo to be clear based on CURRENT state
            if (!HaloIsClearForBigTerrain(x, y, 1)) continue;

            candidates.Add(new Vector2Int(x, y));

        }

        if (candidates.Count == 0 || density <= 0f)
            return;

        int targetCount = Mathf.RoundToInt(candidates.Count * density);

        // Optional: add a bit of RNG wobble so maps don't feel "samey"
        // e.g. density 0.5 gives ~0.4..0.6
       // targetCount = Mathf.Clamp(Mathf.RoundToInt(candidates.Count * Mathf.Clamp01(density + UnityEngine.Random.Range(-0.1f, 0.1f))), 0, candidates.Count);

        // 3) Place up to targetCount, re-checking halo each time because earlier placements changed the board
        Shuffle(candidates);

        int placed = 0;
        int guard = 0;

        for (int i = 0; i < candidates.Count && placed < targetCount && guard < 50000; i++)
        {
            guard++;

            int x = candidates[i].x;
            int y = candidates[i].y;

            // Re-check after previous placements
            if (!HaloIsClearForBigTerrain(x, y, 1)) continue;

            var baseSq = allSquares[x, y].GetComponent<SquareController>();
            if (baseSq == null || !baseSq.IsEmptySquare || baseSq.IsSacred) continue;

            // Reserve halo so later ones can't steal it
            ReserveHaloAroundBigTerrain(x, y, 1);

            // Place 2x2
            baseSq.MakeEmptyTerrainSquare();
            allSquares[x, y + 1].GetComponent<SquareController>().MakeEmptyTerrainSquare();
            allSquares[x + 1, y].GetComponent<SquareController>().MakeEmptyTerrainSquare();
            allSquares[x + 1, y + 1].GetComponent<SquareController>().MakeEmptyTerrainSquare();

            // Reserve base for sprite activation
            candidateBaseSquaresforLargeItems.Add(allSquares[x, y]);

            // Remove these from freeSquares
            freeSquares.Remove(new Vector2Int(x, y));
            freeSquares.Remove(new Vector2Int(x, y + 1));
            freeSquares.Remove(new Vector2Int(x + 1, y));
            freeSquares.Remove(new Vector2Int(x + 1, y + 1));

            placed++;
        }

        // Activate sprites for bases placed
        for (int i = 0; i < candidateBaseSquaresforLargeItems.Count; i++)
        {
            var candSC = candidateBaseSquaresforLargeItems[i].GetComponent<SquareController>();
            candSC.ActivateLargeTerrainSprite();
        }

    }
*/
    /*
    // Fisher–Yates shuffle
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
    */

    public bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0
            && x < allSquares.GetLength(0)
            && y < allSquares.GetLength(1);
    }

    /*

    void ReserveHaloAroundBigTerrain(int baseX, int baseY, int haloRadius = 1)
    {
        // The 2x2 occupies: (baseX..baseX+1, baseY..baseY+1)
        // Halo area becomes: (baseX-1..baseX+2, baseY-1..baseY+2) for radius 1
        int minX = baseX - haloRadius;
        int maxX = baseX + 1 + haloRadius;
        int minY = baseY - haloRadius;
        int maxY = baseY + 1 + haloRadius;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (!InBounds(x, y)) continue;
                if (y == 0) continue;

                var sc = allSquares[x, y].GetComponent<SquareController>();
                if (sc == null) continue;

                if (sc.IsSacred) continue;
                if (sc.IsWater) continue;

                // Mark as reserved walkway (halo)
                sc.SetReservedWalkway(true);

                // Reserve by removing from placement pool
               // freeSquares.Remove(new Vector2Int(x, y));
            }
        }
    }*/

    /*
    bool HaloIsClearForBigTerrain(int baseX, int baseY, int haloRadius = 1)
    {
        int minX = baseX - haloRadius;
        int maxX = baseX + 1 + haloRadius;
        int minY = baseY - haloRadius;
        int maxY = baseY + 1 + haloRadius;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (!InBounds(x, y)) continue;
                if (y == 0) continue;

                var sc = allSquares[x, y].GetComponent<SquareController>();
                if (sc == null) return false;

                // halo must remain walkable
                if (sc.IsSacred) return false;
                if (sc.IsWater) return false;
                if (!sc.IsEmptySquare) return false; // strict version
            }
        }
        return true;
    }
    */


    /*
    void PlacePottard()
    {
        Vector2Int[] freeSqArray = freeSquares.ToArray();

        int randomInt = UnityEngine.Random.Range(0, freeSqArray.Length);

        GameObject newPottard = Instantiate(pottardReference, transform);

        PottardController pottardController = newPottard.GetComponent<PottardController>();
        Vector2 startPos = new Vector2(freeSqArray[randomInt].x, freeSqArray[randomInt].y);

        if (pottardController != null)
        {
            pottardController.GetCurrentBattlefield(freeSqArray, allSquares);
            pottardController.SetStartPosition(startPos);
        }


       
    }
*/
    /*
    void PlaceFly(int size)
    {
        turnOrganiser.ClearFlies();
        Vector2Int[] freeSqArray = freeSquares.ToArray();

        int max = (int)(size * 0.8f); // size 30 => 24
        float t = UnityEngine.Random.value;
        t = t * t; // bias toward 0
        int randomFlyCount = (size / 6) + Mathf.RoundToInt(t * (max - (size / 6)));

        for (int i = 0; i < randomFlyCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, freeSquares.Count);

            

            Vector3 newPos = new Vector3(freeSqArray[randomIndex].x, freeSqArray[randomIndex].y, 1);
            GameObject newFly = Instantiate(fly, transform);
            newFly.transform.position = newPos;

            FlyMovementController flyController = newFly.GetComponent<FlyMovementController>();

            if (flyController != null)
            {
                int testX = freeSqArray[randomIndex].x;
                int testY = freeSqArray[randomIndex].y;

                flyController.SetBattleFieldSize(size, allSquares);
                flyController.SetPlayerStartSquare(testX, testY);
            }
            else
            {
                Debug.LogError("No Controller on Fly Available", this);
            }

            turnOrganiser.ReceiveFly(newFly);

        }





    }*/

    public void CheckMapisWild()
    {
        if(thisMap.GetIsWildMap())
        {
            
            wildMapIcon.SetActive(true);
        }
        else
        {
            wildMapIcon.SetActive(false);
        }
    }
    MapData GetMapToBuild()
    {
        MapData mapToBuild = null;
        isLost = false;

        if (previousMap == null)
        {
            mapToBuild = GetFirstMap();
        }
        else if (previousMap.GetIsWildMap())
        {
            mapToBuild = CalculateLostOrProgress();
        }
        else if (previousMap.GetIsFirstMap())
        {             
            mapToBuild = previousMap.GetStartingMap(playerStatReceiver.GetPlayerStartingLocation());
        }
        else //Otherwise, proceed as standard, the mapToBuild is the previousMaps > NextMap
        {
           //canAdvanceDifficulty = true;
            mapToBuild = previousMap.RollNextMap();
        }

        goalPhaseResolution.SetTransitionData(isLost, previousMap, mapToBuild);

        return mapToBuild;
    }


    MapData CalculateLostOrProgress()
    {

        MapData chosenMap = null; 

        float escapeChance = previousMap.GetEscapeChance();
        bool escaped = UnityEngine.Random.value < escapeChance;

        if (!escaped)
        {
            isLost = true;
         //  canAdvanceDifficulty = false;
           chosenMap = previousMap; // repeat
        }
        else
        {
            chosenMap = previousMap.RollNextMap();
        }

        return chosenMap;
    }


   

    MapData GetFirstMap()
    {
        return mapCatalogue.GetFirstMap();
    }



    public void UpdateMapDataUI() 
    { 
        string mapLocation = thisMap.GetMapLocation();
        mapLocation = mapLocation.Replace("_", " ");

        uiController.UpdateMapDataText(thisMap.GetMapName(), mapLocation); 
    }


    void ClearEnemySquares() { enemySquares.Clear(); }

    [SerializeField] BridgesBuilder bridgesBuilder;

    void SetContent(int mapSize)
    {
        BuildBattleFieldGrid(mapSize);
        SetPlayerStartSquare(mapSize);
        SetSacredPath();

        if (!thisMap.IsFinalCorridoor)
        {
            SetWater(thisMap.GetWaterAmount());

            if (thisMap.GetWaterAmount() > 0)
            {
                bridgesBuilder.DetectBridges();

            }

            bridgesBuilder.RebuildBridgeAwareVisuals();

            npcBuilder.CheckNPCRequired();


           

            if(thisMap.HasFourBlockTerrain) largeTerrainBuilder.TestSpaceForBigPieces();

            AssignContentSquares();

            if (thisMap.GetHasEnemies == true)
            {
                CollectInitialEnemySquares();
            }

           

            // Add Overhead Decorations

            if(borderSquares.Count > 0)
            {
                overHeadSpansBuilder.CreateOverheadSpanDecorations();
            }
        }

            PlacePlayer(mapSize);
   
    }
    /*
    void SetNPCPresent()
    {
        //CheckMerchantNeeded();
       // CheckGateKeeperNeeded();

        //CheckFlies(thisMap.GetMapSize());
        //CheckPottardPlacement();
    }
    */

    /*
    void CreateOverHeadSpanDecoration()
    {
        // This gives the square length of the side to start spans from
        int borderHeight = thisMap.GetMapSize();

        HashSet<int> chosenPositions = new HashSet<int>();

        int numberOfSpans = UnityEngine.Random.Range(0, borderHeight);



        for(int i = 0; i < numberOfSpans; i++)
        {
            int randomBorderPosition = UnityEngine.Random.Range(0, borderSquares.Count);

            if (!chosenPositions.Contains(randomBorderPosition))
            {
                chosenPositions.Add(randomBorderPosition);

                var square = borderSquares[randomBorderPosition];

                BorderOverHeadDecorationController bc =
                    square.obj.GetComponent<BorderOverHeadDecorationController>();

                if (bc != null)
                {
                    bc.SpawnChainOverhead(thisMap.GetMapSize() + 2, square.edge);
                }

                Debug.Log("SQUARE AT " + randomBorderPosition + " SHOULD FIRE " + square.edge.ToString());
            }
        }

      
    }

    */
    /*
    void CheckPottardPlacement()
    {

        if(thisMap.CanHavePottard && RollPottardPresentRNG())
        {
            PlacePottard();
        }
        

    }*/
    /*
    bool RollPottardPresentRNG()
    {
        bool finalChoice = false;

        int randomNumber = UnityEngine.Random.Range(0, 2);

        if(randomNumber == 0)
        {
            finalChoice = true;
        }


        return finalChoice;
    }*/
    
    public GameObject Player => player;

    /*
    void CheckGateKeeperNeeded()
    {
        PlayerStatsController playerStatController = player.GetComponent<PlayerStatsController>();

        if(playerStatController != null)
        {
           int currentMoney = playerStatController.GetPlayerCurrentMoney();
            int currentHealth = playerStatController.GetPlayerCurrentHealth();

            if(currentMoney > 20 || currentHealth > 20)
            {
                Debug.Log("GateKeeper Should Appear...");
            }
        }
    }*/

    /*
    void CheckFlies(int mapSize)
    {
        turnOrganiser.ClearFlies();
        if (thisMap.GetHasFlies())
        {
            PlaceFly(mapSize);
        }
    }
    */

    public MapData ThisMap => thisMap;

    void CheckMerchantNeeded()
    {
        if (thisMap.GetHasMerchant())
        {
           // hasMerchantIcon.SetActive(true);
          //  PlaceMerchant();
          //  merchantShopController.SetCurrentMap(thisMap);
        }
        else 
        { 
    
           // hasMerchantIcon.SetActive(false);
        }
    }

    public void SetThisMap(MapData newMap)
    {
        thisMap = newMap;
    }

    public void BuildNewBattlefield()
    {

        //gridBuilder.BuildNewGrid();

       playerMovementController.PrepareForMapRebuild();
       MapData chosen = GetMapToBuild();

       thisMap = chosen;
       thisMap.ParseDialogue();
       UpdateMapDataUI();
       CheckMapisWild();
        
        ClearEnemySquares();
        ClearEnemySquares();
        ClearBorderSquareLists();

        if(!thisMap.GetIsFinalMap())
        { 
            BuildNewMap();
            previousMap = thisMap;
        }
        else 
        { 
            
            QuitGame(); 
        }

        StartFadeFromBlack();
        transitionScreen.SetActive(false);
        playerMovementController.SetReachedGoalSquare(false);
        Debug.Log("BORDER SQUARES: " + borderSquares.Count);
        Debug.Log("CORNER BORDER SQUARES: " + cornerBorderSquares.Count);
    }

    void BuildNewMap()
    {
        ClearOldBattlefield();
        SetContent(thisMap.GetMapSize());
        
    }

    void ClearBorderSquareLists()
    {
        borderSquares.Clear();
        cornerBorderSquares.Clear();
    }
    void ClearOldBattlefield()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) { Destroy(transform.GetChild(i).gameObject); }
    }

  
    void SetPlayerStartSquare(int currentMapSize) {
        playerStartingPosition = UnityEngine.Random.Range(0, allSquares.GetLength(0));

        SquareController startSqController = allSquares[playerStartingPosition, 0].GetComponent<SquareController>();

        if (startSqController != null)
        {
            startSqController.MakeSquare(SquareType.Start, thisMap);
        }
    }

    void BuildBattleFieldGrid(int size)
    {
        if(!thisMap.IsFinalCorridoor)
        {
            allSquares = new GameObject[size, size];
            freeSquares.Clear();

            int randomGoalSquare = UnityEngine.Random.Range(0, size);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    GameObject newSquare = Instantiate(battleFieldSquare, transform);
                    if (newSquare != null)
                    {
                        newSquare.transform.position = new Vector3(x, y, 0);
                        allSquares[x, y] = newSquare;

                        SquareController newSquareController = newSquare.GetComponent<SquareController>();
                        if (newSquareController != null)
                        {
                            newSquareController.SetSquareMapData(thisMap);
                            newSquareController.SetupNewSquare(x, y);
                            newSquareController.AssignPlayer(player);
                        }

                            newSquareController.SetIsInShadow(thisMap.GetHasShadows());

                        // Border Placement
                        if (x == 0 || x == size - 1 || y == 0 || y == size - 1) { MakeBorderSquare(x, y, size, size, newSquareController); }

                        // Goal placement
                        bool isGoalSpot = (y == size - 1 && x == randomGoalSquare);
                        if (isGoalSpot)
                        {
                            MakeGoalSquare(newSquareController, newSquare);
                            goalSquareLocation = newSquare.transform.position;
                        }

                        // Don't add the player start or goal tile to free list either
                        bool isPlayerStart = (x == playerStartingPosition && y == 0);

                        if (!isPlayerStart && !isGoalSpot) { freeSquares.Add(new Vector2Int(x, y)); }


                    }
                    else { Debug.LogError("Square prefab missing SquareController.", newSquare); return; }


                }
            }
        }
        else
        {
            Vector2Int corridoorSize = thisMap.GetFinalCorrDimensions();

            int width = corridoorSize.x;
            int height = corridoorSize.y;

            allSquares = new GameObject[width, height];
            freeSquares.Clear();

            // pick a goal column within corridor width
            int goalX = UnityEngine.Random.Range(0, width);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject newSquare = Instantiate(battleFieldSquare, transform);
                    newSquare.transform.position = new Vector3(x, y, 0);
                    allSquares[x, y] = newSquare;

                    var sc = newSquare.GetComponent<SquareController>();
                    sc.SetSquareMapData(thisMap);
                    sc.SetupNewSquare(x, y);

                    sc.SetIsInShadow(thisMap.GetHasShadows());

                    // Border placement using corridor bounds
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        MakeBorderSquare(x, y, width, height, sc); // see note below

                    // Goal placement: top row of corridor
                    bool isGoalSpot = (y == height - 1 && x == goalX);
                    if (isGoalSpot)
                    {
                        MakeGoalSquare(sc, newSquare);
                        goalSquareLocation = newSquare.transform.position;
                    }

                    bool isPlayerStart = (x == playerStartingPosition && y == 0);
                    if (!isPlayerStart && !isGoalSpot)
                        freeSquares.Add(new Vector2Int(x, y));
                }
            }
        }

       

        

    }

    public void AddBorderSquareToList(GameObject newBorderSquare, OrthogonalPositions newEdge)
    {
        if (newBorderSquare == null) return;

        // Only add West Side borders, spans are horizontal so should not start from any other, this also gives a defined limit/count/position from that
        if (newEdge != OrthogonalPositions.West) return;

        borderSquares.Add((newBorderSquare, newEdge));

    }

    public List<(GameObject obj, OrthogonalPositions edge)> BorderSquares => borderSquares;

    public void AddCornerBorderSquareToList(GameObject newCornerBorderSquare, CornerPositions cornerPos)
    {
        if(newCornerBorderSquare == null) return;

        cornerBorderSquares.Add((newCornerBorderSquare, cornerPos));
    }


    void MakeGoalSquare(SquareController newSquareController, GameObject newSquare)
    {
        if (newSquareController != null)
        {
            newSquareController.MakeSquare(SquareType.Goal, thisMap);

            if(player != null)
            {
                if (playerCompassController != null) { playerCompassController.SetGoalLocation(newSquare); }
                else { Debug.LogError("No Compass Controller Component Found on Player"); }

                if (playerDistanceController != null) { playerDistanceController.SetGoalLocation(newSquare); }
                else { Debug.LogError("No Distance Controller Component Found on Player"); }
            }
            else { Debug.Log("No Player Object Found"); }
            
        }
    }

    void MakeBorderSquare(int x, int y, int width, int height, SquareController sc)
    {
       // sc.MakeEdgeSquare();

        int[] sidesEmpty =
        {
        x == 0          ? 1 : 0, // left
        y == height - 1 ? 1 : 0, // top
        x == width - 1  ? 1 : 0, // right
        y == 0          ? 1 : 0, // bottom
    };

        sc.AddBorderSquare(sidesEmpty, this);
    }

    void CollectInitialEnemySquares()
    {
        foreach (var item in allSquares)
        {
            if(item != null)
            {
                SquareController squareController = item.GetComponent<SquareController>();
                if (squareController != null && squareController.IsEnemy) { enemySquares.Add(item); }

            }
        }
    }

    void AssignContentSquares()
    {
       // PlaceTypeSquares(terrainSquareCount, sq => sq.MakeTerrainSquare(), disallowReservedWalkway: true);
        PlaceTypeSquares(terrainSquareCount, sq => sq.MakeSquare(SquareType.Terrain, thisMap), disallowReservedWalkway: true);

        if(thisMap.GetHasEnemies == true)
        {
            int size = thisMap.GetMapSize();

            int enemyCount = Mathf.RoundToInt(size * thisMap.EnemyDensity); // EnemyDensity in 0..1
            //PlaceTypeSquares(enemyCount, sq => sq.MakeEnemySquare(thisMap), disallowReservedWalkway: false);
            PlaceTypeSquares(enemyCount, sq => sq.MakeSquare(SquareType.Enemy, thisMap), disallowReservedWalkway: false);

        }
        
        //PlaceTypeSquares(healthSquareCount, sq => sq.MakeHealthSquare(), disallowReservedWalkway: false);
        PlaceTypeSquares(healthSquareCount, sq => sq.MakeSquare(SquareType.Health, thisMap), disallowReservedWalkway: false);
        //PlaceTypeSquares(potionSquareCount, sq => sq.MakeItemSquare(), disallowReservedWalkway: false);
        PlaceTypeSquares(potionSquareCount, sq => sq.MakeSquare(SquareType.Item, thisMap), disallowReservedWalkway: false);
        PlaceTypeSquares(potionSquareCount, sq => sq.MakeSquare(SquareType.Treasure, thisMap), disallowReservedWalkway: false);
       // PlaceTypeSquares(treasureSquareCount, sq => sq.MakeTreasureSquare(), disallowReservedWalkway: false);

        if(thisMap.GetHasHiddenTraps == true)
        {
            PlaceTypeSquares(
                Mathf.CeilToInt(thisMap.GetMapSize() * thisMap.GetHiddenTrapDensity),
                sq => sq.MakeSquare(SquareType.Trap, thisMap), disallowReservedWalkway: true
            );
        }
        

        if (thisMap.GetWaterAmount() > 0)
        {

            int waterLevel = Mathf.RoundToInt(thisMap.GetWaterAmount());
            PlaceWaterFlowerSquares(waterLevel * thisMap.GetMapSize(), sq => sq.MakeFlowerSquare());
        }
        
    }

    void PlaceTypeSquares(int count, System.Action<SquareController> applyType, bool disallowReservedWalkway)
    {


        int placed = 0;
        int guard = 0;


        while (placed < count && freeSquares.Count > 0 && guard < 100000)
        {
            guard++;

            int index = UnityEngine.Random.Range(0, freeSquares.Count);
            Vector2Int coord = freeSquares[index];

            SquareController sq = allSquares[coord.x, coord.y].GetComponent<SquareController>();
            if (sq == null) { freeSquares.RemoveAt(index); continue; }

            if (sq.IsSacred) continue;
            if (disallowReservedWalkway && sq.IsReservedWalkway) continue;

            // Now we commit to using it
            freeSquares.RemoveAt(index);
            applyType(sq);
            placed++;
        }

        if (placed < count)
            Debug.LogWarning($"Could not place full quota ({count}) for type; only placed {placed}.");

    }

    void PlaceWaterFlowerSquares(int count, System.Action<SquareController> applyType)
    {
        int placed = 0;
        int guard = 0;

        while(placed < count && freeSquares.Count > 0 && guard < 100000)
        {
            guard++;

            int index = UnityEngine.Random.Range(0, freeSquares.Count);
            Vector2Int coord = freeSquares[index];

            SquareController sq = allSquares[coord.x, coord.y].GetComponent<SquareController>();
            if (sq == null) { freeSquares.RemoveAt(index); continue; }

            if (!sq.IsWaterAdjacent)
            {
                // Don't remove it; just try another.
                continue;
            }

            // Now we commit to using it
            freeSquares.RemoveAt(index);
            applyType(sq);
            placed++;
        }
    }

    void PlacePlayer(int size)
    {
        SquareController newSquareController = allSquares[playerStartingPosition, 0].GetComponent<SquareController>();
        if (newSquareController != null)
        {
            int testX = newSquareController.SquareXPosition;
            int testY = newSquareController.SquareYPosition;


            if (playerMovementController != null)
            {
                playerMovementController.ReceiveBattlefieldSize(allSquares);
                playerMovementController.SetPlayerStartSquare(testX, testY);
            }
            else { Debug.LogError("No Player Controller"); }
        }
        else { Debug.LogError("No Player Start Square Controller"); }

     


    }

    public List<Vector2Int> FreeSquares => freeSquares;

    /*
    void PlaceMerchant()
    {
        if (freeSquares.Count == 0)
        {
            Debug.LogWarning("No free squares left to place Merchant.");
            return;
        }

        int index = UnityEngine.Random.Range(0, freeSquares.Count);
        Vector2Int merchantPosition = freeSquares[index];
        freeSquares.RemoveAt(index); // good idea to reserve it

        SquareController merchantSquareController = allSquares[merchantPosition.x, merchantPosition.y].GetComponent<SquareController>();

        if (merchantSquareController != null) { merchantSquareController.MakeSquare(SquareType.Merchant, thisMap); }
        else { Debug.LogError("No Merchant Square Controller"); }


    }*/

    public void QuitGame()
    {
        turnOrganiser.StartWinPhase();

    }

    void SetWater(float waterAmount)
    {
        int width = allSquares.GetLength(0);
        int height = allSquares.GetLength(1);
        int area = width * height;

        waterAmount = Mathf.Clamp01(waterAmount);
        if (waterAmount <= 0f) return;

        

        // Decide how much water total this map should have.
        // Tune these numbers to taste.
        int waterBudget = Mathf.RoundToInt(area * Mathf.Lerp(0.03f, 0.18f, waterAmount));
        waterBudget = Mathf.Max(1, waterBudget);

        // Decide number of rivers (streams) from waterAmount
        int riverCount = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(1f, 4f, waterAmount)), 1, 6);

        // Rivers in wetter maps can be slightly thicker
        int baseHalfWidth = (waterAmount < 0.35f) ? 0 : 1; // 0 = 1-tile wide, 1 = up to 3 tiles wide
        int maxHalfWidth = Mathf.Clamp(baseHalfWidth + Mathf.RoundToInt(waterAmount * 2f), 0, 3);

        // Safety: don’t let rivers obliterate your sacred path if that matters.
        // If sacred path must always remain dry, we’ll skip sacred tiles when painting.
        bool avoidSacred = true;

        // Divide budget across rivers
        int perRiverBudget = Mathf.Max(1, waterBudget / riverCount);

        for (int i = 0; i < riverCount; i++)
        {
            // Randomize direction a bit so rivers aren’t all parallel
            // 0 = left->right, 1 = bottom->top, 2 = right->left, 3 = top->bottom
            int dir = UnityEngine.Random.Range(0, 4);

            int halfWidth = UnityEngine.Random.Range(baseHalfWidth, maxHalfWidth + 1);

            GenerateRiver(width, height, dir, perRiverBudget, halfWidth, avoidSacred);
        }

    }

    void GenerateRiver(int width, int height, int direction, int maxTiles, int halfWidth, bool avoidSacred)
    {
        Vector2Int start, end;

        switch (direction)
        {
            case 0: // left -> right
                start = new Vector2Int(0, UnityEngine.Random.Range(0, height));
                end = new Vector2Int(width - 1, UnityEngine.Random.Range(0, height));
                break;
            case 1: // bottom -> top
                start = new Vector2Int(UnityEngine.Random.Range(0, width), 0);
                end = new Vector2Int(UnityEngine.Random.Range(0, width), height - 1);
                break;
            case 2: // right -> left
                start = new Vector2Int(width - 1, UnityEngine.Random.Range(0, height));
                end = new Vector2Int(0, UnityEngine.Random.Range(0, height));
                break;
            default: // top -> bottom
                start = new Vector2Int(UnityEngine.Random.Range(0, width), height - 1);
                end = new Vector2Int(UnityEngine.Random.Range(0, width), 0);
                break;
        }

        Vector2Int current = start;

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(current);

        int placed = 0;
        int maxSteps = width * height; // safety

        for (int step = 0; step < maxSteps; step++)
        {
            PaintWaterBlob(current, width, height, halfWidth, avoidSacred);
            placed++;

            if (current == end) break;
            if (placed >= maxTiles) break;

            Vector2Int next = GetDrunkNeighborTowardsGoal(current, end, width, height, visited);

            if (next == current) break;
            if (visited.Contains(next)) break;

            visited.Add(next);
            current = next;
        }

        // Ensure the end gets painted too
        PaintWaterBlob(current, width, height, halfWidth, avoidSacred);
    }

    void PaintWaterBlob(Vector2Int center, int width, int height, int halfWidth, bool avoidSacred)
    {
        // halfWidth 0 => just the center tile
        // halfWidth 1 => up to a 3x3 blob, etc.
        for (int dx = -halfWidth; dx <= halfWidth; dx++)
        {
            for (int dy = -halfWidth; dy <= halfWidth; dy++)
            {
                int x = center.x + dx;
                int y = center.y + dy;

                if (x < 0 || x >= width || y < 0 || y >= height) continue;

                // Optional: make edges of the blob less solid, more organic
                // e.g. only paint corners sometimes
                if (halfWidth > 0 && Mathf.Abs(dx) == halfWidth && Mathf.Abs(dy) == halfWidth)
                {
                    if (UnityEngine.Random.value < 0.5f) continue;
                }

                var sc = allSquares[x, y].GetComponent<SquareController>();
                if (sc == null) continue;

                if (avoidSacred && sc.IsSacred) continue;

                sc.MakeSquare(SquareType.Water, thisMap);
                freeSquares.Remove(new Vector2Int(x, y));
            }
        }
    }


    public GameObject[,] AllSquares => allSquares;

    /*
    void DetectBridges()
    {
        int width = allSquares.GetLength(0);
        int height = allSquares.GetLength(1);

        bool[,] processedVertical = new bool[width, height];
        bool[,] processedHorizontal = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsVerticalBridgeCandidate(x, y) && !processedVertical[x, y])
                {
                    List<Vector2Int> run = GetVerticalBridgeRun(x, y);

                    foreach (var pos in run)
                        processedVertical[pos.x, pos.y] = true;

                    if (RunConnectsLandAtBothEnds(run, BridgeOrientation.Vertical))
                    {
                        MarkBridgeRun(run, BridgeOrientation.Vertical);
                    }
                }

                if (IsHorizontalBridgeCandidate(x, y) && !processedHorizontal[x, y])
                {
                    List<Vector2Int> run = GetHorizontalBridgeRun(x, y);

                    foreach (var pos in run)
                        processedHorizontal[pos.x, pos.y] = true;

                    if (RunConnectsLandAtBothEnds(run, BridgeOrientation.Horizontal))
                    {
                        MarkBridgeRun(run, BridgeOrientation.Horizontal);
                    }
                }
            }
        }
    }*/


    /*
    List<Vector2Int> GetVerticalBridgeRun(int x, int y)
    {
        List<Vector2Int> run = new List<Vector2Int>();

        int startY = y;
        while (IsVerticalBridgeCandidate(x, startY - 1))
            startY--;

        int endY = y;
        while (IsVerticalBridgeCandidate(x, endY + 1))
            endY++;

        for (int yy = startY; yy <= endY; yy++)
            run.Add(new Vector2Int(x, yy));

        return run;
    }*/

    /*
    List<Vector2Int> GetHorizontalBridgeRun(int x, int y)
    {
        List<Vector2Int> run = new List<Vector2Int>();

        int startX = x;
        while (IsHorizontalBridgeCandidate(startX - 1, y))
            startX--;

        int endX = x;
        while (IsHorizontalBridgeCandidate(endX + 1, y))
            endX++;

        for (int xx = startX; xx <= endX; xx++)
            run.Add(new Vector2Int(xx, y));

        return run;
    }*/

    /*

    bool RunConnectsLandAtBothEnds(List<Vector2Int> run, BridgeOrientation orientation)
    {
        if (run == null || run.Count == 0)
            return false;

        if (orientation == BridgeOrientation.Vertical)
        {
            Vector2Int first = run[0];
            Vector2Int last = run[run.Count - 1];

            bool hasLandBefore = IsLandAt(first.x, first.y - 1) && !IsVerticalBridgeCandidate(first.x, first.y - 1);
            bool hasLandAfter = IsLandAt(last.x, last.y + 1) && !IsVerticalBridgeCandidate(last.x, last.y + 1);

            return hasLandBefore && hasLandAfter;
        }
        else if (orientation == BridgeOrientation.Horizontal)
        {
            Vector2Int first = run[0];
            Vector2Int last = run[run.Count - 1];

            bool hasLandBefore = IsLandAt(first.x - 1, first.y) && !IsHorizontalBridgeCandidate(first.x - 1, first.y);
            bool hasLandAfter = IsLandAt(last.x + 1, last.y) && !IsHorizontalBridgeCandidate(last.x + 1, last.y);

            return hasLandBefore && hasLandAfter;
        }

        return false;
    }*/

    void RebuildBridgeAwareVisuals()
    {
        int width = allSquares.GetLength(0);
        int height = allSquares.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SquareController sc = allSquares[x, y]?.GetComponent<SquareController>();
                if (sc == null) continue;

                if (sc.IsWater)
                {
                    RebuildWaterTileVisuals(x, y, sc);
                }
                else if (sc.IsBridge)
                {
                    RebuildBridgeTileVisuals(x, y, sc);
                }
                else
                {
                    RebuildLandTileVisuals(x, y, sc);
                }
            }
        }
    }

    void RebuildWaterTileVisuals(int x, int y, SquareController sc)
    {
        int waterEdgeMask = 0;
        int diagWaterMask = 0;

        // N E S W
        for (int i = 0; i < Neigh4.Length; i++)
        {
            Vector2Int n = new Vector2Int(x, y) + Neigh4[i];

            if (!InBounds(n.x, n.y))
            {
                // map edge still counts as open water edge
                waterEdgeMask |= (1 << i);
                continue;
            }

            bool neighborIsWater = IsWaterAt(n.x, n.y);
            bool neighborIsBridge = IsBridgeAt(n.x, n.y);

            // Only show normal water border against SOLID LAND, not bridge.
            if (!neighborIsWater && !neighborIsBridge)
            {
                waterEdgeMask |= (1 << i);
            }
        }

        // Diagonals still based on water-water relationship
        for (int i = 0; i < NeighDiag.Length; i++)
        {
            Vector2Int d = new Vector2Int(x, y) + NeighDiag[i];

            if (!InBounds(d.x, d.y)) continue;

            if (IsWaterAt(d.x, d.y))
            {
                diagWaterMask |= (1 << i);
            }
        }

        sc.SetIsWaterAdjacent(false);
        sc.SetWaterAdjacencyMask(waterEdgeMask);
        sc.SetWaterDiagonalMask(diagWaterMask);
    }

    void RebuildBridgeTileVisuals(int x, int y, SquareController sc)
    {
        sc.SetIsWaterAdjacent(false);

        // clear old caps first
        sc.SetBridgeEndCaps(false, false, false, false);

        BridgeOrientation orientation = sc.GetBridgeOrientation();

        bool northCap = false;
        bool eastCap = false;
        bool southCap = false;
        bool westCap = false;

        if (orientation == BridgeOrientation.Horizontal)
        {
            westCap = IsSolidLandAt(x - 1, y);
            eastCap = IsSolidLandAt(x + 1, y);
        }
        else if (orientation == BridgeOrientation.Vertical)
        {
            southCap = IsSolidLandAt(x, y - 1);
            northCap = IsSolidLandAt(x, y + 1);
        }

        sc.SetBridgeEndCaps(northCap, eastCap, southCap, westCap);
    }

    void RebuildLandTileVisuals(int x, int y, SquareController sc)
    {
        int waterNeighborMask = 0;

        for (int i = 0; i < Neigh4.Length; i++)
        {
            Vector2Int n = new Vector2Int(x, y) + Neigh4[i];

            if (IsWaterAt(n.x, n.y))
            {
                waterNeighborMask |= (1 << i);
            }
        }

        bool adjacentToWater = (waterNeighborMask != 0);
        sc.SetIsWaterAdjacent(adjacentToWater);

        // Optional later:
        // sc.SetLandWaterNeighborMask(waterNeighborMask);
    }




    /*
    void MarkBridgeRun(List<Vector2Int> run, BridgeOrientation orientation)
    {
        foreach (var pos in run)
        {
            var sc = allSquares[pos.x, pos.y]?.GetComponent<SquareController>();
            if (sc == null) continue;

            sc.SetIsBridge(true);
            sc.SetBridgeOrientation(orientation);
        }
    }

    bool IsVerticalBridgeCandidate(int x, int y)
    {
        return IsLandAt(x, y)
            && IsWaterAt(x - 1, y)
            && IsWaterAt(x + 1, y);
    }

    bool IsHorizontalBridgeCandidate(int x, int y)
    {
        return IsLandAt(x, y)
            && IsWaterAt(x, y - 1)
            && IsWaterAt(x, y + 1);
    }*/

    public bool IsWaterAt(int x, int y)
    {
        if(!InBounds(x, y)) return false;

        SquareController sc = allSquares[x, y].GetComponent<SquareController>();
        return sc != null && sc.IsWater;
    }

   public bool IsLandAt(int x, int y)
    {
        if (!InBounds(x, y)) return false;
        SquareController sc = allSquares[x, y].GetComponent<SquareController>();
        return sc != null && !sc.IsWater;

    }
    
    public bool IsBridgeAt(int x, int y)
    {
        if (!InBounds(x, y)) return false;

        SquareController sc = allSquares[x, y]?.GetComponent<SquareController>();
        return sc != null && sc.IsBridge;
    }
    

   public bool IsSolidLandAt(int x, int y)
    {
        if (!InBounds(x, y)) return false;

        SquareController sc = allSquares[x, y]?.GetComponent<SquareController>();
        return sc != null && !sc.IsWater && !sc.IsBridge;
    }

    public void RemoveFreeSquare(Vector2Int removeFreeSquare)
    {
        freeSquares.Remove(removeFreeSquare);
    }

    [SerializeField] SacredPathBuilder sacredPathBuilder;

    void SetSacredPath()
    {
        sacredPathBuilder.SetSacredPath(allSquares, playerStartingPosition);

        /*
        int width = allSquares.GetLength(0);
        int height = allSquares.GetLength(1);

        // Start from the player start tile
        Vector2Int current = new Vector2Int(playerStartingPosition, 0);

        // Find the goal tile coordinate (cheaper would be to store it when you place it)
        Vector2Int goal = FindGoalCoord();

        // Safety: prevent infinite loops
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(current);

        int maxSteps = width * height;

        for (int step = 0; step < maxSteps; step++)
        {
            if (current == goal) break;

            // Mark current as sacred if you want the path to include the start too
            var currentSq = allSquares[current.x, current.y].GetComponent<SquareController>();
            if (currentSq != null) currentSq.SetIsSacred(true);
            freeSquares.Remove(current);

            // Vector2Int next = GetBestNeighborTowardsGoal(current, goal, size);

            Vector2Int next = GetDrunkNeighborTowardsGoal(current, goal, width, height, visited);


            // If we can't progress, give up (or you could fall back to a real pathfinding algorithm)
            if (next == current) break;

            // If we're looping, break
            if (visited.Contains(next)) break;
            visited.Add(next);

            current = next;
        }

        // Also mark the goal as sacred (optional)
        var goalSq = allSquares[goal.x, goal.y].GetComponent<SquareController>();
        if (goalSq != null) goalSq.SetIsSacred(true);
        freeSquares.Remove(goal); // harmless if not present*/



    }

   public Vector2Int FindGoalCoord()
    {
        int gx = Mathf.RoundToInt(goalSquareLocation.x);
        int gy = Mathf.RoundToInt(goalSquareLocation.y);
        return new Vector2Int(gx, gy);
    }


    [SerializeField] DrunkenPathController drunkenPathController;

   public  Vector2Int GetDrunkNeighborTowardsGoal(Vector2Int current, Vector2Int goal, int width, int height, HashSet<Vector2Int> visited)
    {
    return drunkenPathController.GetDrunkNeighborTowardsGoal(current, goal, width, height, visited);

        /*
        List<Vector2Int> candidates = new List<Vector2Int>();
        List<float> weights = new List<float>();

        int currentDist = Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);

        foreach (var dir in Neigh4)
        {
            Vector2Int n = current + dir;

            if (n.x < 0 || n.x >= width || n.y < 0 || n.y >= height) continue;

            // If you want: avoid borders (since you mark them as edges)
            // var sc = allSquares[n.x, n.y].GetComponent<SquareController>();
            // if (sc != null && sc.IsEdgeSquare()) continue;

            // Optional: strongly avoid revisiting
            bool wasVisited = visited.Contains(n);

            int dist = Mathf.Abs(n.x - goal.x) + Mathf.Abs(n.y - goal.y);

            // Improvement: positive if this step gets closer.
            float improvement = currentDist - dist;

            // Base desirability:
            // - prefer getting closer (improvement > 0)
            // - allow sideways/backwards a bit when drunk
            float desirability = improvement;

            // Penalize revisits heavily to avoid loops
            if (wasVisited) desirability -= 999f;

            // Convert desirability into a weight.
            // We want weights > 0 even for "not great" moves.
            // Use an exponential-ish curve controlled by weightSharpness.
            float w = Mathf.Exp(desirability * weightSharpness);

            candidates.Add(n);
            weights.Add(w);
        }

        if (candidates.Count == 0) return current;

        // Mix between greedy and random:
        // - drunkenness 0 => almost always pick max weight
        // - drunkenness 1 => pick by weights (still biased, but much wobblier)
        if (UnityEngine.Random.value > drunkenness)
        {
            // Greedy pick
            int bestIndex = 0;
            float bestW = weights[0];
            for (int i = 1; i < weights.Count; i++)
            {
                if (weights[i] > bestW)
                {
                    bestW = weights[i];
                    bestIndex = i;
                }
            }
            return candidates[bestIndex];
        }

        // Weighted random pick
        float total = 0f;
        for (int i = 0; i < weights.Count; i++) total += weights[i];

        float roll = UnityEngine.Random.value * total;
        float accum = 0f;

        for (int i = 0; i < weights.Count; i++)
        {
            accum += weights[i];
            if (roll <= accum) return candidates[i];
        }

        return candidates[candidates.Count - 1];*/
    }



    public static readonly Vector2Int[] Neigh4 = new[]
    {
        new Vector2Int(0, 1),  // N
        new Vector2Int(1, 0),  // E
        new Vector2Int(0, -1), // S
        new Vector2Int(-1, 0), // W
    };


    /*
    void MarkWaterAndShore()
    {
        int width = allSquares.GetLength(0);
        int height = allSquares.GetLength(1);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var sc = allSquares[x, y]?.GetComponent<SquareController>();
                if (sc == null) continue;

                int waterNeighborMask = 0; // land: which cardinals are water
                int waterEdgeMask = 0;     // water: which sides touch NON-water (open edges)
                int diagWaterMask = 0;     // water: which diagonals ARE water

                // ---- Cardial neighbors (N/E/S/W) ----
                for (int i = 0; i < Neigh4.Length; i++)
                {
                    Vector2Int n = new Vector2Int(x, y) + Neigh4[i];

                    bool inBounds = (n.x >= 0 && n.x < width && n.y >= 0 && n.y < height);
                    bool neighborIsWater = false;

                    if (inBounds)
                    {
                        var nsc = allSquares[n.x, n.y]?.GetComponent<SquareController>();
                        neighborIsWater = (nsc != null && nsc.IsWater);
                    }

                    // Land: record water adjacency
                    if (neighborIsWater)
                        waterNeighborMask |= (1 << i);

                    // Water: record OPEN edges where neighbor is not water (or out of bounds)
                    if (sc.IsWater && !neighborIsWater)
                        waterEdgeMask |= (1 << i);
                }

                // ---- Diagonal neighbors (NE/NW/SE/SW) ----
                if (sc.IsWater)
                {
                    for (int i = 0; i < NeighDiag.Length; i++)
                    {
                        Vector2Int d = new Vector2Int(x, y) + NeighDiag[i];
                        if (d.x < 0 || d.x >= width || d.y < 0 || d.y >= height)
                            continue;

                        var dsc = allSquares[d.x, d.y]?.GetComponent<SquareController>();
                        if (dsc != null && dsc.IsWater)
                            diagWaterMask |= (1 << i); // bit 1,2,4,8 for NE,NW,SE,SW
                    }
                }

                if (sc.IsWater)
                {
                    sc.SetIsWaterAdjacent(false);
                    sc.SetWaterAdjacencyMask(waterEdgeMask);
                    sc.SetWaterDiagonalMask(diagWaterMask); // <-- NEW
                }
                else
                {
                    bool adjacentToWater = (waterNeighborMask != 0);
                    sc.SetIsWaterAdjacent(adjacentToWater);

                    // optional: if you want land visuals to know which side water is on:
                    // sc.SetLandWaterNeighborMask(waterNeighborMask);
                }
            }
    }
     */

    public static readonly Vector2Int[] NeighDiag = new[]
    {
        new Vector2Int(1, 1),   // NE  bit 1
        new Vector2Int(-1, 1),  // NW  bit 2
        new Vector2Int(1, -1),  // SE  bit 4
        new Vector2Int(-1, -1), // SW  bit 8
    };
   
 



    public void StartFadeToBlack() { uiController.StartFadeToBlack(); }

    public void StartFadeFromBlack() { uiController.StartFadeFromBlack(); }

    public MapData GetThisMap() => thisMap;
}
