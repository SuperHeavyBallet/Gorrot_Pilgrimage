using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class BattlefieldBuilder : MonoBehaviour
{
    [SerializeField] GameObject battleFieldSquare;

    [SerializeField] GameObject[,] allSquares;

    [SerializeField] GameObject player;

    [SerializeField] UIController uiController;

    [SerializeField] DifficultyTuning difficultyTuning;

    int enemySquareCount = 5;
    int treasureSquareCount = 5;
    int terrainSquareCount = 5;
    int healthSquareCount = 5;
    int potionSquareCount = 5;

    int playerStartingPosition = 0;

    bool isFinalMap = false;

    private List<Vector2Int> freeSquares = new List<Vector2Int>();

    int currentMapCount = 0;

    List<GameObject> enemySquares = new List<GameObject>();

    [SerializeField] MapCatalogue mapCatalogue;
    MapData currentMap;
    MapData mapToBuild;

    MapData previousMap;
    MapData thisMap;
    MapData nextMap;

    bool canAdvanceDifficulty;

    [SerializeField] TextMeshProUGUI hasMerchantText;
    [SerializeField] MerchantShopController merchantShopController;
    StartLocations startLocation;

    PlayerCompassController playerCompassController;
    PlayerDistanceController playerDistanceController;
    PlayerMovementController playerMovementController;
    PlayerStatsController playerStatsController;
    PlayerStatReceiver playerStatReceiver;
    [SerializeField] GoalPhaseResolution goalPhaseResolution;
    [SerializeField] GameObject transitionScreen;

    Vector2 goalSquareLocation;

    bool isLost = false;

    [Header("Sacred Path Drunkenness")]
    [Tooltip("0 = always best, 1 = very random")]
    [SerializeField, Range(0f, 1f)] float drunkenness;
    [Tooltip("higher = more greedy, lower = more meandery")]
    [SerializeField, Range(0.1f, 10f)] float weightSharpness;



    void Awake()
    {

        if (battleFieldSquare == null) Debug.LogError("Battlefield square prefab not set", this);
        if (player == null) Debug.LogError("Player not set", this);
        if (uiController == null) Debug.LogError("UIController not set", this);
        if (mapCatalogue == null) Debug.LogError("MapCatalogue not set", this);
        if (hasMerchantText == null) Debug.LogError("Merchant text not set", this);
        if (merchantShopController == null) Debug.LogError("Merchant Shop Controller not set", this);

        if (player != null)
        {
            playerMovementController = player.GetComponent<PlayerMovementController>();
            playerCompassController = player.GetComponent<PlayerCompassController>();
            playerDistanceController = player.GetComponent<PlayerDistanceController>();
            playerStatsController = player.GetComponent<PlayerStatsController>();
            playerStatReceiver = player.GetComponent<PlayerStatReceiver>();

            if (playerMovementController == null) Debug.LogError("PlayerMovementController missing", player);
            if (playerCompassController == null) Debug.LogError("PlayerCompassController missing", player);
            if (playerDistanceController == null) Debug.LogError("PlayerDistanceController missing", player);
            if (playerStatsController == null) Debug.LogError("PlayerStatsController missing", player);
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

    void DecideStartingLocation()
    {

        if(playerStatReceiver != null)
        {
            startLocation = playerStatReceiver.GetPlayerStartingLocation();
        }
        else { startLocation = StartLocations.Fetsmeld; }
    }

    public void StartFadeToBlack() { uiController.StartFadeToBlack(); }
    
    public void StartFadeFromBlack() { uiController.StartFadeFromBlack(); }

    void CheckIfFinalMap() { isFinalMap = currentMap.GetIsFinalMap(); }

    MapData DecideMapToBuild()
    {
        if (previousMap == null) 
        { 
            Debug.LogError("previous Map is null");
            isLost = false;
            canAdvanceDifficulty = false;
            return mapCatalogue.GetFirstMap();
        }

        canAdvanceDifficulty = false;
        isLost = false; // default unless proven otherwise

        if (previousMap.GetIsWildMap())
        {
            int wild = previousMap.GetWildLevel();

            float escapeChance = previousMap.GetEscapeChance();
            bool escaped = UnityEngine.Random.value < escapeChance;

            isLost = !escaped;
            mapToBuild = escaped ? previousMap.GetNextMap() : previousMap;
            canAdvanceDifficulty = escaped;

        }
        else
        {
            if (previousMap.GetIsFirstMap())
            {
                DecideStartingLocation();
                playerStatsController.SetStartingStats();
                mapToBuild = previousMap.GetStartingMap(startLocation);
            }
            else
            {
                mapToBuild = previousMap.GetNextMap();
                canAdvanceDifficulty = true;
            }
        }

        MapData nextMap = mapToBuild.GetNextMap();
        MapData nextNextMap = nextMap.GetNextMap();

        DeclareThisMap(mapToBuild, nextMap, nextNextMap);

        //goalPhaseResolution.SetLostStatus(isLost, nextMap, nextNextMap);
        return mapToBuild;
    }

    void DeclareThisMap(MapData thisMap, MapData nextMap, MapData nextNextMap)
    {
        Debug.Log("Current and Next Maps:");
        Debug.Log(thisMap.GetMapName());
        Debug.Log(nextMap.GetMapName());
        Debug.Log(nextNextMap.GetMapName());
    }

    void UpdateMapDataUI() { uiController.UpdateMapDataText(currentMap.GetMapName(), currentMap.GetMapLocation()); }

    void ClearEnemySquares() { enemySquares.Clear(); }

    void SetContent(int mapSize)
    {
        
        
        BuildBattleFieldGrid(mapSize);
        SetPlayerStartSquare(mapSize);
        
        SetSacredPath(mapSize);
        CheckMerchantNeeded();
        SetContentAmounts(mapSize);
        AssignContentSquares();
        CollectInitialEnemySquares();

        PlacePlayer(mapSize);
        
    }

    void CheckMerchantNeeded()
    {
        if (currentMap.GetHasMerchant())
        {
            hasMerchantText.text = "MERCHANT";
            PlaceMerchant();
            merchantShopController.SetCurrentMap(currentMap);
        }
        else { hasMerchantText.text = "..."; }
    }

    void BuildNewMap()
    {
        IncrementMapCount();
        ClearOldBattlefield();
        SetContent(currentMap.GetMapSize()); 
    }

    void IncrementMapCount() { if (canAdvanceDifficulty) { currentMapCount++; } }
    public void BuildNewBattlefield()
    {
        MapData chosen = DecideMapToBuild();
        thisMap = chosen;

        uiController.UpdateMapDataText(chosen.GetMapName(), chosen.GetMapLocation());
        Debug.Log($"[BattlefieldBuilder] UI set to: {currentMap.GetMapName()}  (isLost={isLost}, canAdvance={canAdvanceDifficulty})");


        CheckIfFinalMap();

        ClearEnemySquares();

        if(!isFinalMap) { BuildNewMap(); }
        else { QuitGame(); }

        StartFadeFromBlack();
        transitionScreen.SetActive(false);

    }

   

    void ClearOldBattlefield()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) { Destroy(transform.GetChild(i).gameObject); }
    }

    void SetContentAmounts(int currentMapSize)
    {
        if (difficultyTuning == null)
        {
            Debug.LogError("Difficulty Tuning not assigned.", this);
            return;
        }

        var counts = difficultyTuning.ComputeCounts(currentMapSize, currentMapCount, isFinalMap);

        enemySquareCount = counts.enemy;
        treasureSquareCount = counts.treasure;
        //terrainSquareCount = counts.terrain;
        healthSquareCount = counts.health;
        potionSquareCount = counts.potion;

        int area = currentMapSize * currentMapSize;

        float terrainRatio = currentMap.GetTerrainDensity();

        terrainSquareCount = Mathf.Max(1, Mathf.RoundToInt( terrainRatio * area));
    }

    void SetPlayerStartSquare(int currentMapSize) { playerStartingPosition = UnityEngine.Random.Range(0, currentMapSize); }

    void BuildBattleFieldGrid(int size)
    {
        allSquares = new GameObject[size, size];
        freeSquares.Clear();

        int randomGoalSquare = UnityEngine.Random.Range(0, size);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                GameObject newSquare = Instantiate(battleFieldSquare, transform);
                if(newSquare != null)
                {
                    newSquare.transform.position = new Vector3(x, y, 0);
                    allSquares[x, y] = newSquare;

                    SquareController newSquareController = newSquare.GetComponent<SquareController>();
                    if (newSquareController != null) { 
                        newSquareController.SetSquareMapData(currentMap);
                        newSquareController.SetupNewSquare(x, y, currentMap.GetMapLocation()); 
                    }

                    // Border Placement
                    if (x == 0 || x == size - 1 || y == 0 || y == size - 1) { MakeBorderSquare(x, y, size, newSquareController); }

                    // Goal placement
                    bool isGoalSpot = (y == size - 1 && x == randomGoalSquare);
                    if (isGoalSpot) {
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

    void MakeGoalSquare(SquareController newSquareController, GameObject newSquare)
    {
        if (newSquareController != null)
        {
            newSquareController.MakeGoalSquare();

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

    void MakeBorderSquare(int x, int y, int size, SquareController newSquareController)
    {
        newSquareController.MakeEdgeSquare();

        int[] sidesEmpty =
        {
            x == 0        ? 1 : 0,
            y == size-1  ? 1 : 0,
            x == size-1  ? 1 : 0,
            y == 0       ? 1 : 0
        };

        newSquareController.AddBorderSquare(sidesEmpty);
    }

    void CollectInitialEnemySquares()
    {
        foreach (var item in allSquares)
        {
            if(item != null)
            {
                SquareController squareController = item.GetComponent<SquareController>();
                if (squareController != null && squareController.CheckIsEnemy()) { enemySquares.Add(item); }

            }
        }
    }

    void AssignContentSquares()
    {
        PlaceTypeSquares(terrainSquareCount, sq => sq.MakeTerrainSquare());
        PlaceTypeSquares(enemySquareCount, sq => sq.MakeEnemySquare());
        PlaceTypeSquares(healthSquareCount, sq => sq.MakeHealthSquare());
        PlaceTypeSquares(potionSquareCount, sq => sq.MakeItemSquare());
        PlaceTypeSquares(treasureSquareCount, sq => sq.MakeTreasureSquare());
    }

    void PlaceTypeSquares(int count, System.Action<SquareController> applyType)
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

            if (sq.GetIsSacred())
            {
                // Don't remove it; just try another.
                continue;
            }

            // Now we commit to using it
            freeSquares.RemoveAt(index);
            applyType(sq);
            placed++;
        }

        if (placed < count)
            Debug.LogWarning($"Could not place full quota ({count}) for type; only placed {placed}.");

    }

    void PlacePlayer(int size)
    {
        SquareController newSquareController = allSquares[playerStartingPosition, 0].GetComponent<SquareController>();
        if (newSquareController != null)
        {
            int testX = newSquareController.GetSquareXPosition();
            int testY = newSquareController.GetSquareYPosition();


            if (playerMovementController != null)
            {
                playerMovementController.ReceiveBattlefieldSize(size, allSquares);
                playerMovementController.SetPlayerStartSquare(testX, testY);
            }
            else { Debug.LogError("No Player Controller"); }
        }
        else { Debug.LogError("No Player Start Square Controller"); }

     


    }

    void PlaceMerchant()
    {
        if (freeSquares.Count == 0)
        {
            Debug.LogWarning("No free squares left to place Merchant.");
            return;
        }

        int index = Random.Range(0, freeSquares.Count);
        Vector2Int merchantPosition = freeSquares[index];
        freeSquares.RemoveAt(index); // good idea to reserve it

        SquareController merchantSquareController = allSquares[merchantPosition.x, merchantPosition.y].GetComponent<SquareController>();

        if (merchantSquareController != null) { merchantSquareController.MakeMerchantSquare(); }
        else { Debug.LogError("No Merchant Square Controller"); }


    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    Vector2 CheckNextStep(Vector2 currentSquarePosition)
    {
       Vector2 nextClosestSquarePosition = Vector2.zero;



        return nextClosestSquarePosition;
    }


    void SetSacredPath(int size)
    {

        // Start from the player start tile
        Vector2Int current = new Vector2Int(playerStartingPosition, 0);

        // Find the goal tile coordinate (cheaper would be to store it when you place it)
        Vector2Int goal = FindGoalCoord(size);

        // Safety: prevent infinite loops
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(current);

        int maxSteps = size * size;

        for (int step = 0; step < maxSteps; step++)
        {
            if (current == goal) break;

            // Mark current as sacred if you want the path to include the start too
            var currentSq = allSquares[current.x, current.y].GetComponent<SquareController>();
            if (currentSq != null) currentSq.SetIsSacred(true);

            // Vector2Int next = GetBestNeighborTowardsGoal(current, goal, size);

            Vector2Int next = GetDrunkNeighborTowardsGoal(current, goal, size, visited);


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




    }

    Vector2Int FindGoalCoord(int size)
    {
        // You already store goalSquareLocation as a Vector2 world position.
        // Convert it back to grid coords since your squares are placed at (x,y,0).
        int gx = Mathf.RoundToInt(goalSquareLocation.x);
        int gy = Mathf.RoundToInt(goalSquareLocation.y);
        return new Vector2Int(gx, gy);
    }

    Vector2Int GetDrunkNeighborTowardsGoal(Vector2Int current, Vector2Int goal, int size, HashSet<Vector2Int> visited)
    {
        // 4-neighbors
        Vector2Int[] dirs =
        {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
    };

        List<Vector2Int> candidates = new List<Vector2Int>();
        List<float> weights = new List<float>();

        float currentDist = Vector2.Distance(current, goal);

        for (int i = 0; i < dirs.Length; i++)
        {
            Vector2Int n = current + dirs[i];

            if (n.x < 0 || n.x >= size || n.y < 0 || n.y >= size) continue;

            // If you want: avoid borders (since you mark them as edges)
            // var sc = allSquares[n.x, n.y].GetComponent<SquareController>();
            // if (sc != null && sc.IsEdgeSquare()) continue;

            // Optional: strongly avoid revisiting
            bool wasVisited = visited.Contains(n);

            float dist = Vector2.Distance(n, goal);

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

        return candidates[candidates.Count - 1];
    }


    Vector2Int GetBestNeighborTowardsGoal(Vector2Int current, Vector2Int goal, int size)
    {
        Vector2Int best = current;
        float bestDist = Vector2.Distance(current, goal); // start with current distance

        // 4-neighbors
        Vector2Int[] dirs =
        {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
    };

        foreach (var d in dirs)
        {
            Vector2Int n = current + d;

            if (n.x < 0 || n.x >= size || n.y < 0 || n.y >= size) continue;

            // Optional: avoid border squares if they're blocked/unwalkable
            // var sq = allSquares[n.x, n.y].GetComponent<SquareController>();
            // if (sq != null && sq.IsEdgeSquare()) continue;

            float dist = Vector2.Distance(n, goal);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = n;
            }
        }

        return best;
    }
}
