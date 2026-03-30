using System;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using GorrotGame;

public class BattlefieldBuilder : MonoBehaviour
{
    [Header("Builder Scripts")]
    [SerializeField] BattleFieldGridBuilder gridBuilder;
    [SerializeField] NPCBuilder npcBuilder;
    [SerializeField] BridgesBuilder bridgesBuilder;
    [SerializeField] ContentSquareBuilder contentSquareBuilder;
    [SerializeField] BattleFieldGridBuilder battleFieldGridBuilder;
    [SerializeField] WaterBuilder waterBuilder;
    [SerializeField] OverHeadSpansBuilder overHeadSpansBuilder;
    [SerializeField] LargeTerrainBuilder largeTerrainBuilder;
    [SerializeField] SacredPathBuilder sacredPathBuilder;
    [SerializeField] DrunkenPathController drunkenPathController;

    [Space(10)]
    [Header("Controller Scripts")]
    [SerializeField] UIController uiController;
    PlayerCompassController playerCompassController;
    PlayerDistanceController playerDistanceController;
    PlayerMovementController playerMovementController;

    [Space(10)]
    [Header("Game Object References")]
    [SerializeField] GameObject wildMapIcon;
    [SerializeField] GameObject fly;
    [SerializeField] GameObject pottardReference;
    [SerializeField] GameObject player;

    [Space(10)]
    [Header("Scripts")]
    [SerializeField] GoalPhaseResolution goalPhaseResolution;
    [SerializeField] GameObject transitionScreen;
    [SerializeField] MapCatalogue mapCatalogue;
    [SerializeField] TurnOrganiser turnOrganiser;
    PlayerStatReceiver playerStatReceiver;
    [SerializeField] TransitionMapScreenController transitionMapScreenController;
    [SerializeField] LevelTransitionPhaseResolution levelTransitionPhaseResolution;

    // Lists and Arrays
    List<Vector2Int> freeSquares = new List<Vector2Int>();
    List<GameObject> enemySquares = new List<GameObject>();
    List<(GameObject obj, OrthogonalPositions edge)> borderSquares = new List<(GameObject obj, OrthogonalPositions edge)>();
    List<(GameObject obj, CornerPositions corner)> cornerBorderSquares = new List<(GameObject obj, CornerPositions corner)>();
    GameObject[,] allSquares;
    HashSet<int> rowsCannotContainOverheadSpans = new HashSet<int>();

    public void AddRowToForbiddenForOverhead(int newRow)
    {
        rowsCannotContainOverheadSpans.Add(newRow);
    }

    public bool RowFreeForOverhead(int row)
    {
        if (rowsCannotContainOverheadSpans.Contains(row)) return false;

        else return true;
    }
    // Assorted
    Vector2Int goalSquareCoord;
    bool isLost;
    int playerStartingPosition = 0;

    // Map Data
    MapData previousMap;
    MapData thisMap;

    void Awake()
    {
        if (player == null) Debug.LogError("Player not set", this);
        if (uiController == null) Debug.LogError("UIController not set", this);
        if (mapCatalogue == null) Debug.LogError("MapCatalogue not set", this);

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

    public void PrepareNextMap()
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

            mapToBuild = previousMap.RollNextMap();
        }

       levelTransitionPhaseResolution.SetTransitionData(isLost, previousMap, mapToBuild);



        thisMap = mapToBuild;
    }

    public void BuildNewBattlefield()
    {

        playerMovementController.PrepareForMapRebuild();
        MapData chosen = GetMapToBuild();

        thisMap = chosen;
        thisMap.ParseDialogue();
        UpdateMapDataUI();
        CheckMapisWild();

        ClearEnemySquares();
        ClearBorderSquareList();
        ClearCornerBorderSquareList();

        if (!thisMap.GetIsFinalMap())
        {
            ClearOldBattlefield();
            SetContent();
            previousMap = thisMap;
        }
        else
        {

            QuitGame();
        }

        
        StartFadeFromBlack();
        /*transitionScreen.SetActive(false);*/
        playerMovementController.SetReachedGoalSquare(false);
    }

    void SetContent()
    {
        battleFieldGridBuilder.BuildBattleFieldGrid(ThisMap.GetMapSize(), player);
        SetPlayerStartSquare(ThisMap.GetMapSize());
        sacredPathBuilder.SetSacredPath(allSquares, playerStartingPosition);

        if (!thisMap.IsFinalCorridoor)
        {
            waterBuilder.SetWater(thisMap.GetWaterAmount());

            if (thisMap.GetWaterAmount() > 0) bridgesBuilder.DetectBridges();

            bridgesBuilder.RebuildBridgeAwareVisuals();

            npcBuilder.CheckNPCRequired();

            if (thisMap.HasFourBlockTerrain) largeTerrainBuilder.TestSpaceForBigPieces();

            contentSquareBuilder.PlaceContentSquares();

            //if (thisMap.GetHasEnemies == true) CollectInitialEnemySquares();


            // Check if overhead is needed and double check we actually have border squares to start from
            if(thisMap.HasOverheadSpanObjects && borderSquares.Count > 0)
            {
                overHeadSpansBuilder.CreateOverheadSpanDecorations();
            }

        }

        PlacePlayer(ThisMap.GetMapSize());

    }

    // Helper Functions
    public static readonly Vector2Int[] NeighDiag = new[]
    {
        new Vector2Int(1, 1),   // NE  bit 1
        new Vector2Int(-1, 1),  // NW  bit 2
        new Vector2Int(1, -1),  // SE  bit 4
        new Vector2Int(-1, -1), // SW  bit 8
    };
    public static readonly Vector2Int[] Neigh4 = new[]
    {
        new Vector2Int(0, 1),  // N
        new Vector2Int(1, 0),  // E
        new Vector2Int(0, -1), // S
        new Vector2Int(-1, 0), // W
    };
    public Vector2Int GetDrunkNeighborTowardsGoal(Vector2Int current, Vector2Int goal, int width, int height, HashSet<Vector2Int> visited)
    {
        return drunkenPathController.GetDrunkNeighborTowardsGoal(current, goal, width, height, visited);
    }
    public Vector2Int FindGoalCoord() => goalSquareCoord;
    public bool IsWaterAt(int x, int y)
    {
        if (!InBounds(x, y)) return false;

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
    public void RemoveFreeSquare(Vector2Int removeFreeSquare) => freeSquares.Remove(removeFreeSquare);
    public void QuitGame() => turnOrganiser.StartWinPhase();
    public GameObject[,] AllSquares => allSquares;
    public List<Vector2Int> FreeSquares => freeSquares;
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
    void CollectInitialEnemySquares()
    {
        foreach (var item in allSquares)
        {
            if (item != null)
            {
                SquareController squareController = item.GetComponent<SquareController>();
                if (squareController != null && squareController.IsEnemy) { enemySquares.Add(item); }

            }
        }
    }
    public void AddCornerBorderSquareToList(GameObject newCornerBorderSquare, CornerPositions cornerPos)
    {
        if (newCornerBorderSquare == null) return;
        cornerBorderSquares.Add((newCornerBorderSquare, cornerPos));
    }
    public List<(GameObject obj, OrthogonalPositions edge)> BorderSquares => borderSquares;
    public void AddBorderSquareToList(GameObject newBorderSquare, OrthogonalPositions newEdge)
    {
        if (newBorderSquare == null) return;
        if (newEdge != OrthogonalPositions.West) return;

        borderSquares.Add((newBorderSquare, newEdge));
    }
    public int PlayerStartingPosition => playerStartingPosition;
    public void SetGoalSquareCoord(Vector2Int coord) => goalSquareCoord = coord;
    public PlayerCompassController PlayerCompassController => playerCompassController;
    public PlayerDistanceController PlayerDistanceController => playerDistanceController;
    void SetPlayerStartSquare(int currentMapSize)
    {
        playerStartingPosition = UnityEngine.Random.Range(0, allSquares.GetLength(0));

        SquareController startSqController = allSquares[playerStartingPosition, 0].GetComponent<SquareController>();

        if (startSqController != null)
        {
            startSqController.MakeSquare(SquareType.Start, thisMap);
        }
    }
    void ClearBorderSquareList() => borderSquares.Clear();
    void ClearCornerBorderSquareList() => cornerBorderSquares.Clear();
    void ClearOldBattlefield()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) { Destroy(transform.GetChild(i).gameObject); }
    }
    public void SetThisMap(MapData newMap) => thisMap = newMap;
    public void SetAllSquares(GameObject[,] newSet) => allSquares = newSet;
    public GameObject Player => player;
    public bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0
            && x < allSquares.GetLength(0)
            && y < allSquares.GetLength(1);
    }
    void ClearEnemySquares() { enemySquares.Clear(); }
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
    MapData CalculateLostOrProgress()
    {

        MapData chosenMap = null; 

        float escapeChance = previousMap.GetEscapeChance();
        bool escaped = UnityEngine.Random.value < escapeChance;

        if (!escaped)
        {
            isLost = true;
           chosenMap = previousMap; // repeat
        }
        else
        {
            chosenMap = previousMap.RollNextMap();
        }

        return chosenMap;
    }


    // Map Data Related Functions
    public MapData ThisMap => thisMap;
    void SetFirstMap() { previousMap = mapCatalogue.GetFirstMap(); }
    MapData GetFirstMap()
    {
        return mapCatalogue.GetFirstMap();
    }

    public void PrepareNextMapToBuild()
    {

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

            mapToBuild = previousMap.RollNextMap();
        }

        levelTransitionPhaseResolution.SetTransitionData(isLost, previousMap, mapToBuild);

      
       

        return mapToBuild;
    }
    public MapData GetThisMap() => thisMap;

    public MapNames CurrentMapNames => previousMap.GetMapNames();
    public MapNames NextMapNames => thisMap.GetMapNames();

    // UI Functions
    public void UpdateMapDataUI() 
    { 
        string mapLocation = thisMap.GetMapLocation();
        mapLocation = mapLocation.Replace("_", " ");

        uiController.UpdateMapDataText(thisMap.GetMapName(), mapLocation); 
    }
    public void StartFadeToBlack() { 
        uiController.StartFadeToBlack(); 
    }
    public void StartFadeFromBlack() { 
        uiController.StartFadeFromBlack();
        }


}
