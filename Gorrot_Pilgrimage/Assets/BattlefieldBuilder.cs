using System.Collections.Generic;
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

    bool canAdvanceDifficulty;

    [SerializeField] TextMeshProUGUI hasMerchantText;
    [SerializeField] MerchantShopController merchantShopController;
    StartLocations startLocation;

    PlayerCompassController playerCompassController;
    PlayerDistanceController playerDistanceController;
    PlayerMovementController playerMovementController;
    PlayerStatsController playerStatsController;

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

            if (playerMovementController == null) Debug.LogError("PlayerMovementController missing", player);
            if (playerCompassController == null) Debug.LogError("PlayerCompassController missing", player);
            if (playerDistanceController == null) Debug.LogError("PlayerDistanceController missing", player);
            if (playerStatsController == null) Debug.LogError("PlayerStatsController missing", player);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiController.ActivateBlackScreen(true);
        SetFirstMap();
        BuildNewBattlefield();
    }
    void SetFirstMap() { currentMap = mapCatalogue.GetFirstMap(); }

    void DecideStartingLocation()
    {
        CharacterStatSheet sheet = CharacterStatSheet.Instance;
        if(sheet != null ) { startLocation = CharacterStatSheet.Instance.GetCharacterStartLocation(); }
        else { startLocation = StartLocations.Fetsmeld; }
    }

    public void StartFadeToBlack() { uiController.StartFadeToBlack(); }
    
    public void StartFadeFromBlack() { uiController.StartFadeFromBlack(); }

    void CheckIfFinalMap() { isFinalMap = currentMap.GetIsFinalMap(); }

    MapData DecideMapToBuild()
    {
        if (currentMap == null) 
        { 
            Debug.LogError("currentMap is null"); 
            return mapCatalogue.GetFirstMap(); 
        }

        canAdvanceDifficulty = false;

        if (currentMap.GetIsWildMap())
        {
            int wild = currentMap.GetWildLevel();

            float escapeChance = currentMap.GetEscapeChance();
            bool escaped = UnityEngine.Random.value < escapeChance;

            mapToBuild = escaped ? currentMap.GetNextMap() : currentMap;
            canAdvanceDifficulty = escaped;
        }
        else
        {
            if (currentMap.GetIsFirstMap())
            {
                DecideStartingLocation();
                playerStatsController.SetStartingStats();
                mapToBuild = currentMap.GetStartingMap(startLocation);
            }
            else
            {
                mapToBuild = currentMap.GetNextMap();
                canAdvanceDifficulty = true;
            }
        }

        return mapToBuild;
    }

    void UpdateMapDataUI() { uiController.UpdateMapDataText(currentMap.GetMapName(), currentMap.GetMapLocation()); }

    void ClearEnemySquares() { enemySquares.Clear(); }

    void SetContent(int mapSize)
    {
        SetPlayerStartSquare(mapSize);
        SetContentAmounts(mapSize);
        BuildBattleFieldGrid(mapSize);
        PlacePlayer(mapSize);
        CheckMerchantNeeded();
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
        currentMap = DecideMapToBuild();

        UpdateMapDataUI();

        CheckIfFinalMap();

        ClearEnemySquares();

        if(!isFinalMap) { BuildNewMap(); }
        else { QuitGame(); }

        StartFadeFromBlack();

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
        terrainSquareCount = counts.terrain;
        healthSquareCount = counts.health;
        potionSquareCount = counts.potion;
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
                    if (newSquareController != null) { newSquareController.SetupNewSquare(x, y, currentMap.GetMapLocation()); }

                    // Border Placement
                    if (x == 0 || x == size - 1 || y == 0 || y == size - 1) { MakeBorderSquare(x, y, size, newSquareController); }

                    // Goal placement
                    bool isGoalSpot = (y == size - 1 && x == randomGoalSquare);
                    if (isGoalSpot) { MakeGoalSquare(newSquareController, newSquare); }

                    // Don't add the player start or goal tile to free list either
                    bool isPlayerStart = (x == playerStartingPosition && y == 0);
                    if (!isPlayerStart && !isGoalSpot) { freeSquares.Add(new Vector2Int(x, y)); }
                }
                else { Debug.LogError("Square prefab missing SquareController.", newSquare); return; }

               
            }
        }

        AssignContentSquares();
        CollectInitialEnemySquares();

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

        if (placed < count) { Debug.LogWarning($"Could not place full quota ({count}) for type; only placed {placed}."); }
        
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
        int index = Random.Range(0, freeSquares.Count);
        Vector2Int merchantPosition = freeSquares[index];

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
}
