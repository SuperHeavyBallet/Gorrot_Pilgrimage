using UnityEngine;
using TMPro;
using GorrotGame;
using System.Collections.Generic;


public class SquareController : MonoBehaviour
{
    GameObject player;
    Vector3 playerPosition;

    [SerializeField] Animator enemyAnimator;

    PlayerMovementController playerMovementController;

    [SerializeField] StandeeController standeeController;
    [SerializeField] SquareTypeController squareTypeController;
    [SerializeField] MapBorderSquareController mapBorderSquareController;
    [SerializeField] WaterAdjacencyController waterAdjacencyController;
    [SerializeField] EnemyStandeeController enemyStandeeController;
    [SerializeField] LargeTerrainController largeTerrainController;

    [SerializeField] GameObject shadow;


    // Bool Checks
    public bool IsGoalSquare => squareTypeController.ThisSquareType == SquareType.Goal;
    public bool IsTreasureSquare => squareTypeController.ThisSquareType == SquareType.Treasure;
    public bool IsEmptySquare => squareTypeController.ThisSquareType == SquareType.Empty;
    public bool IsHealthSquare => squareTypeController.ThisSquareType == SquareType.Health;
    public bool IsItemSquare => squareTypeController.ThisSquareType == SquareType.Item;
    public bool IsTrapSquare => squareTypeController.ThisSquareType == SquareType.Trap;
    public bool IsWater => squareTypeController.ThisSquareType == SquareType.Water;
    public bool IsEnemy => squareTypeController.ThisSquareType == SquareType.Enemy;
    public bool IsMerchantSquare => squareTypeController.ThisSquareType == SquareType.Merchant;
    public bool IsMoveableSquare => squareTypeController.ThisSquareType != SquareType.Terrain;


    // Value Retrieval
    public int EnemyDamage => squareTypeController.SquareBaseDamage;
    public int SquareXPosition => squareX;
    public int SquareYPosition => squareY;
    public SquareMood EnemyMood => squareTypeController.SquareMood;
    public string ContentsID => squareTypeController.ContentsID;
    public int GetEnemyBaseBuff()
    {
        return squareSize switch
        {
            SquareSize.Small => 0,
            SquareSize.Medium => 1,
            SquareSize.Large => 2,
            _ => 4
        };
    }


    // Square Setup
    private void Awake()
    {
        ActivateStepInWaterSprite(false);
        SetReservedWalkway(false);
    }

    private void Start()
    {
        //FIX Water Adjacency and Water 'decorations' are currently bundled, maybe seperate out water decorations from border logic
        waterAdjacencyController.AssignWaterBorderSprites(thisSquareMapData);

        if (squareTypeController.ThisSquareType == SquareType.Enemy)
        {
            LocatePlayer();
        }
    }

    public void SetSquareMapData(MapData mapData) => thisSquareMapData = mapData;
    public void SetupNewSquare(int x, int y)
    {
        SetSquarePosition(x, y);
        SetSquareStartSize();
        MakeSquare(SquareType.Empty, thisSquareMapData);
    }

    int squareX = 0;
    int squareY = 0;

    public void SetSquarePosition(int x, int y)
    {
        squareX = x; squareY = y;
    }

    SquareSize squareSize = SquareSize.Medium;
    public SquareSize ThisSquareSize => squareSize;

    void SetSquareStartSize()
    {
        int randomChance = UnityEngine.Random.Range(0, 3);
        switch (randomChance)
        {
            case 0:
                squareSize = SquareSize.Small;
                break;
            case 1:
                squareSize = SquareSize.Medium;
                break;
            case 2:
                squareSize = SquareSize.Large;
                break;
        }
    }

    // Tracking Player Position


    public void AssignPlayer(GameObject newPlayer)
    {
        player = newPlayer;
        Debug.Log("Got Player Reference, " +  player);

    }

    void LocatePlayer()
    {
       // player = GameObject.Find("Player");

        if(player != null )
        { 
            playerPosition = player.transform.position;

            playerMovementController = player.GetComponent<PlayerMovementController>();

            if(playerMovementController != null )
            {
              Bind();
            }
        }
    }

    public void Bind()
    {
        // If rebinding, unbind first
        if (playerMovementController != null)
            playerMovementController.OnPlayerMoved -= UpdatePlayerPosition;

        if (playerMovementController != null)
            playerMovementController.OnPlayerMoved += UpdatePlayerPosition;
    }



    private void OnDisable()
    {
        if (playerMovementController != null)
            playerMovementController.OnPlayerMoved -= UpdatePlayerPosition;
    }

    private void OnDestroy()
    {
        if (playerMovementController != null)
            playerMovementController.OnPlayerMoved -= UpdatePlayerPosition;
    }

    void UpdatePlayerPosition()
    {
        playerPosition = player.transform.position;
        float dist = Vector3.Distance(this.transform.position, playerPosition);
        
        if(dist <= 4)
        {
            enemyAnimator.SetBool("playerInRange", true);
        }
        else
        {
            enemyAnimator.SetBool("playerInRange", false);
        }
    }


    // Large Terrain
    public void ActivateLargeTerrainSprite() => largeTerrainController.SetFourBlockTerrainSprite(thisSquareMapData);

    // Map Data
    MapData thisSquareMapData;
    public MapData ThisMap => thisSquareMapData;


    // Traps
    public bool TrapActivated => squareTypeController.TrapActivated;
    public void ActivateTrap() => squareTypeController.ActivateTrap();
    

    // Shadows - Not yet really used
   
    public bool isInShadow;
    public void SetIsInShadow(bool value)
    {
        isInShadow = value;
        shadow.SetActive(value);
    }
    public bool IsInShadow => isInShadow;
    

    // Create Specific Square Types
    public void MakeSquare(SquareType sqType, MapData thisMap) => squareTypeController.ConstructSquare(sqType, squareSize, thisMap);


    // Water Adjacency for water side placement
    public void SetIsWaterAdjacent(bool value) => waterAdjacencyController.SetIsWaterAdjacent(value);
    public void SetWaterAdjacencyMask(int mask) => waterAdjacencyController.SetWaterAdjacencyMask(mask);
    public bool IsWaterAdjacent => waterAdjacencyController.IsWaterAdjacent;
    public void SetWaterDiagonalMask(int diagMask) => waterAdjacencyController.SetWaterDiagonalMask(diagMask);

    // Map Borders
    public void AddBorderSquare(int[] sides) => mapBorderSquareController.AddBorderSquare(sides);


    // Sacred Path
    bool isSacred;
    public bool IsSacred => isSacred;
    public void SetIsSacred(bool value) => isSacred = value;


    // Water Step Effect
    [SerializeField] GameObject stepInWaterSprite;
    public void ActivateStepInWaterSprite(bool value) => stepInWaterSprite.SetActive(value);
    
   
    // Reserved Walkway for Large Terrain Clear Space Halo
    bool isReservedWalkway = false;
    public bool IsReservedWalkway => isReservedWalkway;
    public void SetReservedWalkway(bool value) => isReservedWalkway = value;
  

    // Make Specific Square Types Directly
    public void MakeFlowerSquare() => squareTypeController.ConstructFlowerSquare(SquareType.Item, squareSize, thisSquareMapData);
    public void MakeEmptyTerrainSquare() => squareTypeController.ConstructEmptyTerrainSquare(SquareType.Terrain, squareSize, thisSquareMapData);
    public void MakeEmptySquare() => MakeSquare(SquareType.Empty, thisSquareMapData);

    // Player Moves In/Out Square
    bool thisSquareHoldsPlayer;
    public bool ThisSquareHoldsPlayer => thisSquareHoldsPlayer;
    public void MakeThisSquareHoldPlayer(bool value) => thisSquareHoldsPlayer = value;

    // Pottard Moves In/Out Square
    bool thisSquareHoldsPottard = false;
    public bool ThisSquareHoldsPottard => thisSquareHoldsPottard;
    public void MakePottardSquare(bool value) => thisSquareHoldsPottard = value;

}
