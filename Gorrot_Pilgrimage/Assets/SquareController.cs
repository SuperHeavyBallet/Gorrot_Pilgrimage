using UnityEngine;
using TMPro;
using GorrotGame;
using System.Collections.Generic;


public class SquareController : MonoBehaviour
{
    public bool hasBeenVisited;

    public GameObject visitedSprite;
    public Transform squareCentre;

    public bool isGoalSquare;
    public bool isTreasureSquare;
   // public bool isEnemySquare;
    public bool isTerrainSquare;
    public bool isEmptySquare;
    public bool IsEmptySquare => type == SquareType.Empty;
    public bool isHealthSquare;
    public bool isItemSquare;

    public GameObject goalSquareSprite;
    public GameObject treasureSquareSprite;
    public GameObject enemySquareSprite;
    //[SerializeField] SpriteRenderer enemySquareSpriteRenderer;
    public GameObject terrainSquareSprite;
    public GameObject emptySquareSprite;
    public GameObject healthSquareSprite;
    public GameObject itemSquareSprite;
    [SerializeField] GameObject waterAdjacentSprite;

    public GameObject startSquareSprite;
    public GameObject goalSquareSprite_Pressed;

    [SerializeField] Transform treasurePositon;
    [SerializeField] GameObject treasure_CoinSack;
    [SerializeField] GameObject treasure_Coins;
    [SerializeField] GameObject treasure_Chest;
    
    [SerializeField] Transform itemPosition;
    [SerializeField] GameObject item_Greatsword;

    GameObject player;
    Vector3 playerPosition;

    int enemyDamage;
    public int EnemyDamage => enemyDamage;
    [SerializeField] Animator enemyAnimator;

    PlayerMovementController playerMovementController;

    [SerializeField] StandeeController standeeController;

    [SerializeField]    SquareTypeController squareTypeController;


    //[SerializeField] SpriteRenderer fourBlockTerrainSpriteRenderer;
    void SetFourBlockTerrainSprite()
    {
        /*
        Sprite randomSprite = thisSquareMapData.GetRandomFourBlockTerrainSprite();
        if(randomSprite != null )
        {
            fourBlockTerrainSpriteRenderer.sprite = randomSprite;
        }
        else
        {
            Debug.LogError("Random Four Block Terrain Sprite is null", this);
        }*/
        
    }

    void LocatePlayer()
    {
        player = GameObject.Find("Player");

        if(player != null )
        {
            Debug.Log("Found Player");
            playerPosition = player.transform.position;
            Debug.Log("At: " + playerPosition.x + " , " + playerPosition.y + " , " + playerPosition.z);

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
        Debug.Log("Update Player Position: " + +playerPosition.x + " , " + playerPosition.y + " , " + playerPosition.z);
        float dist = Vector3.Distance(this.transform.position, playerPosition);
        Debug.Log("This Square Distance to Player: " +  dist);
        
        if(dist <= 4)
        {
            Debug.Log("Player IN RANGE");
            enemyAnimator.SetBool("playerInRange", true);
        }
        else
        {
            enemyAnimator.SetBool("playerInRange", false);
        }
    }

    public int squareX = 0;
    public int squareY = 0;

    //public enum squareQuantity { small, medium, large };
    public SquareSize square = SquareSize.Medium;
    public string squareQuantityString;

    public enum directions
    {
        up, down, left, right
    }

    public directions enterDirection = directions.up;

    string squareContentsID = "";

    List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();

    //InventoryItemTemplate[] allItems;

    //SquareSpriteLibrary squareSpriteLibrary;

    public SpriteRenderer squareTerrainSpriteRenderer;
    public SpriteRenderer squareItemSpriteRenderer;
    [SerializeField] GameObject groundSprite;
    [SerializeField] SpriteRenderer groundSpriteRenderer;
    [SerializeField] SpriteRenderer waterSpriteRenderer;

    //BattlefieldBuilder battlefieldBuilder;

    [SerializeField] GameObject largeTerrainSprite;
    public void ActivateLargeTerrainSprite()
    {
        SetFourBlockTerrainSprite();
        largeTerrainSprite.SetActive(true);
        
    }

    [SerializeField] GameObject freeMarker;
    public void MarkAsFree() => freeMarker.SetActive(true);

    public string squareType = "empty";
    bool isEdgeSquare;
    bool leftEmpty;
    bool upEmpty;
    bool rightEmpty;
    bool downEmpty;

    string mapLocation;

    [SerializeField] SpriteRenderer treasureSpriteRenderer;

    bool isMerchantSquare;
    [SerializeField] GameObject merchantSprite;

    float spriteScale = 1;

    [SerializeField] GameObject squareValue;
    [SerializeField] TextMeshProUGUI squareValueText;

    MapData thisSquareMapData;

    int waterAdjacencyMask;

    public bool waterNorth;
    public bool waterEast;
    public bool waterSouth;
    public bool waterWest;

    const int N = 1;
    const int E = 2;
    const int S = 4;
    const int W = 8;

    public GameObject waterBorderNorth;
    public GameObject waterBorderEast;
    public GameObject waterBorderSouth;
    public GameObject waterBorderWest;

    [SerializeField] SpriteRenderer waterBorderNorthSpriteRenderer;
    [SerializeField] SpriteRenderer waterBorderEastSpriteRenderer;
    [SerializeField] SpriteRenderer waterBorderSouthSpriteRenderer;
    [SerializeField] SpriteRenderer waterBorderWestSpriteRenderer;

    [SerializeField] SpriteRenderer waterFoamNorthSpriteRenderer;
    [SerializeField] SpriteRenderer waterFoamEastSpriteRenderer;
    [SerializeField] SpriteRenderer waterFoamSouthSpriteRenderer;
    [SerializeField] SpriteRenderer waterFoamWestSpriteRenderer;

    [SerializeField] SpriteRenderer[] foamObjects;

    [SerializeField] Sprite defaultWaterBorderSprite;

    [SerializeField] Transform squareMeshHolder;

    void SetSquareMesh()
    {
        GameObject prefab = thisSquareMapData.GetSquareMesh();
        ClearChildren(squareMeshHolder);

        GameObject mesh = Instantiate(prefab, squareMeshHolder);
    }

    void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
                Destroy(parent.GetChild(i).gameObject);
            else
                DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

    void AssignWaterBorderSprites()
    {
        if (thisSquareMapData.GetWaterBorderSpritesArrayLength > 0)
        {
            

            if (waterBorderNorthSpriteRenderer == null)
            {
                Debug.LogError("No Water Border North Sprite Renderer Assigned", this);
                return;
            }


            if (waterBorderEastSpriteRenderer == null)
            {
                Debug.LogError("No Water Border East Sprite Renderer Assigned", this);
                return;
            }
            if (waterBorderSouthSpriteRenderer == null)
            {
                Debug.LogError("No Water Border South Sprite Renderer Assigned", this);
                return;
            }

            if (waterBorderWestSpriteRenderer == null)
            {
                Debug.LogError("No Water Border West Sprite Renderer Assigned", this);
                return;
            }

            if (waterFoamNorthSpriteRenderer == null)
            {
                Debug.LogError("No Water Foam North Sprite Renderer Assigned", this);
                return;
            }


            if (waterFoamEastSpriteRenderer == null)
            {
                Debug.LogError("No Water Foam East Sprite Renderer Assigned", this);
                return;
            }
            if (waterFoamSouthSpriteRenderer == null)
            {
                Debug.LogError("No Water Foam South Sprite Renderer Assigned", this);
                return;
            }

            if (waterFoamWestSpriteRenderer == null)
            {
                Debug.LogError("No Water Foam West Sprite Renderer Assigned", this);
                return;
            }



            waterBorderNorthSpriteRenderer.sprite = thisSquareMapData.GetRandomWaterBorderSprite();
            waterBorderEastSpriteRenderer.sprite = thisSquareMapData.GetRandomWaterBorderSprite();
            waterBorderSouthSpriteRenderer.sprite = thisSquareMapData.GetRandomWaterBorderSprite();
            waterBorderWestSpriteRenderer.sprite = thisSquareMapData.GetRandomWaterBorderSprite();

            foreach(SpriteRenderer sr in foamObjects)
            {
                sr.material = thisSquareMapData.WaterFoamShader;
            }
        }
        else
        {
            if(defaultWaterBorderSprite == null)
            {
                Debug.LogError("Default Water Border Sprite Not Assigned", this);
                return;
            }

            waterBorderNorthSpriteRenderer.sprite = defaultWaterBorderSprite;
            waterBorderEastSpriteRenderer.sprite = defaultWaterBorderSprite;
            waterBorderSouthSpriteRenderer.sprite = defaultWaterBorderSprite;
            waterBorderWestSpriteRenderer.sprite = defaultWaterBorderSprite;
        }
        
    }

    [SerializeField] GameObject shadow;
    public bool isInShadow;

    [SerializeField] GameObject trapSprite;
    [SerializeField] GameObject hiddenTrapSprite;
    bool trapActivated = false;
    // bool isTrapSquare;

    public bool thisSquareHoldsPottard = false;

    [SerializeField] GameObject waterFoamCorner_NE;
    [SerializeField] GameObject waterFoamCorner_SE;
    [SerializeField] GameObject waterFoamCorner_SW;
    [SerializeField] GameObject waterFoamCorner_NW;

    public void MakeTrapSquare(MapData thisMap)
    {
        
        type = SquareType.Trap;
        hiddenTrapSprite.SetActive(true);
        
    }
    public void ActivateTrap()
    {
        
        trapActivated = true;
        hiddenTrapSprite.SetActive(false);
        trapSprite.SetActive(true);
    }

    public bool GetTrapActivated => trapActivated;

   // public bool GetIsTrapSquare => isTrapSquare;

    public void SetIsInShadow(bool value)
    {
        isInShadow = value;
        shadow.SetActive(value);
    }

    public bool GetIsInShadow()
    {
        return isInShadow;
    }

    public void MakeEdgeSquare() { isEdgeSquare = true; }

    public void SetMapLocation(string newMapLocation) { mapLocation = newMapLocation; }

    [SerializeField] private SquareType type = SquareType.Empty;

    public SquareType Type => type;
    public bool isTrapSquare => type == SquareType.Trap;


    public bool isSacred;
    [SerializeField] GameObject sacredMarker;

    public bool isWater;
    [SerializeField] GameObject waterMarker;

    [SerializeField] Sprite waterEdgeSprite;

    public bool isWaterAdjacent;
    public void SetIsWaterAdjacent(bool value)
    { 
        isWaterAdjacent = value; 
        waterAdjacentSprite.SetActive(value);

      
            
     
    }

    public void MakeSquare(SquareType sqType, MapData thisMap)
    {
        squareTypeController.ConstructSquare(sqType, thisMap);
    }

    static readonly int TimeOffset = Shader.PropertyToID("_TimeOffset");

    void ApplyTimeOffset()
    {
        if (waterSpriteRenderer == null) return;

        mpb ??= new MaterialPropertyBlock();
        waterSpriteRenderer.GetPropertyBlock(mpb);

        // Stable-ish random per tile: based on grid coords, or transform position
        float seed = (squareX * 73856093) ^ (squareY * 19349663);
        float offset = Mathf.Abs(seed % 1000) * 0.01f; // 0..10-ish seconds

        mpb.SetFloat(TimeOffset, offset);
        waterSpriteRenderer.SetPropertyBlock(mpb);
        waterSpriteRenderer.SetPropertyBlock(mpb);
    }

    static readonly int EdgeN = Shader.PropertyToID("_EdgeN");
    static readonly int EdgeE = Shader.PropertyToID("_EdgeE");
    static readonly int EdgeS = Shader.PropertyToID("_EdgeS");
    static readonly int EdgeW = Shader.PropertyToID("_EdgeW");
    static MaterialPropertyBlock mpb;

    void ApplyWaterEdgesToRenderer(int mask)
    {
        if (waterSpriteRenderer == null) return;

        mpb ??= new MaterialPropertyBlock();
        waterSpriteRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat(EdgeN, (mask & 1) != 0 ? 1f : 0f);
        mpb.SetFloat(EdgeE, (mask & 2) != 0 ? 1f : 0f);
        mpb.SetFloat(EdgeS, (mask & 4) != 0 ? 1f : 0f);
        mpb.SetFloat(EdgeW, (mask & 8) != 0 ? 1f : 0f);
        


        waterSpriteRenderer.SetPropertyBlock(mpb);
    }


    static readonly int DiagNE = Shader.PropertyToID("_DiagNE");
    static readonly int DiagNW = Shader.PropertyToID("_DiagNW");
    static readonly int DiagSE = Shader.PropertyToID("_DiagSE");
    static readonly int DiagSW = Shader.PropertyToID("_DiagSW");

    public void SetWaterDiagonalMask(int diagMask)
    {
        // diagMask bits: 1=NE, 2=NW, 4=SE, 8=SW

        if (waterSpriteRenderer == null) return;

        mpb ??= new MaterialPropertyBlock();
        waterSpriteRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat(DiagNE, (diagMask & 1) != 0 ? 1f : 0f);
        mpb.SetFloat(DiagNW, (diagMask & 2) != 0 ? 1f : 0f);
        mpb.SetFloat(DiagSE, (diagMask & 4) != 0 ? 1f : 0f);
        mpb.SetFloat(DiagSW, (diagMask & 8) != 0 ? 1f : 0f);

        waterSpriteRenderer.SetPropertyBlock(mpb);
    }


    public void SetWaterAdjacencyMask(int mask)
    {
        waterAdjacencyMask = mask;


        waterNorth = (mask & N) != 0;
        waterEast = (mask & E) != 0;
        waterSouth = (mask & S) != 0;
        waterWest = (mask & W) != 0;

       


        waterBorderNorth.SetActive(waterNorth);
        waterBorderEast.SetActive(waterEast);
        waterBorderSouth.SetActive(waterSouth);
        waterBorderWest.SetActive(waterWest);

        if(waterNorth && waterEast)
        {
            waterFoamCorner_NE.SetActive(true);
        }

        if(waterEast && waterSouth)
        {
            waterFoamCorner_SE.SetActive(true);
        }

        if(waterSouth && waterWest)
        {
            waterFoamCorner_SW.SetActive(true);
        }

        if(waterWest && waterNorth)
        {
            waterFoamCorner_NW.SetActive(true);
        }

        ApplyWaterEdgesToRenderer(mask);

    }

    public bool GetIsWaterAdjacent() => isWaterAdjacent;

    public bool GetIsWater() => isWater;

    public bool IsWater => isWater;

    public void AddBorderSquare(int[] sides)
    {

        int squareLeft = sides[0];
        int squareUp = sides[1];
        int squareRight = sides[2];
        int squareBottom = sides[3];

        leftEmpty = (sides[0] == 1);
        upEmpty = (sides[1] == 1);
        rightEmpty = (sides[2] == 1);
        downEmpty = (sides[3] == 1);

        float thisSquareSize = this.transform.localScale.x;

        if (leftEmpty && upEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(-thisSquareSize, thisSquareSize, 0f), CornerPositions.NorthWest);

        if (rightEmpty && upEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(thisSquareSize, thisSquareSize, 0f), CornerPositions.NorthEast);

        if (leftEmpty && downEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(-thisSquareSize, -thisSquareSize, 0f), CornerPositions.SouthWest);

        if (rightEmpty && downEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(thisSquareSize, -thisSquareSize, 0f), CornerPositions.SouthEast);


        if (leftEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.left * thisSquareSize,  OrthogonalPositions.West);
        if (rightEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.right * thisSquareSize,  OrthogonalPositions.East);
        if (upEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.up * thisSquareSize,  OrthogonalPositions.North);
        if (downEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.down * thisSquareSize,  OrthogonalPositions.South);
    }

    enum OrthogonalPositions { North, East, South, West };
    enum CornerPositions { NorthEast, SouthEast, SouthWest, NorthWest};


    void MakeCornerBorderAtPosition(Vector3 position, CornerPositions cornerPos)
    {
        GameObject newCornerBorderSquare = Instantiate(
            SquareSpriteLibrary.Instance.GetBorderCornerSquare(),
            position,
            Quaternion.identity,
            transform.parent
        );

        switch(cornerPos )
        {
            case CornerPositions.NorthWest:
                newCornerBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, 0f);
                    break;
            case CornerPositions.NorthEast:
                newCornerBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, -90f);
                break;
            case CornerPositions.SouthEast:
                newCornerBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, -180f);
                break;
            case CornerPositions.SouthWest:
                newCornerBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, 90f);
                break;
            default:
                break;
        }
    }

    void MakeBorderSquareAtPosition(Vector3 position,  OrthogonalPositions borderPos)
    {
        GameObject newBorderSquare = UnityEngine.Object.Instantiate(
            SquareSpriteLibrary.Instance.getBorderSquare(),
            position,
            Quaternion.identity,
            transform.parent
            );


        if(borderPos == OrthogonalPositions.North)
        {
            newBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, 90f);
        }
        else if (borderPos == OrthogonalPositions.South)
        {
            newBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, -90f);
        }
       
    }

    private void Awake()
    {
        squareValue.gameObject.SetActive(false);
        sacredMarker.gameObject.SetActive(false);
        waterMarker.gameObject.SetActive(false);
        ActivateStepInWaterSprite(false);
        groundSprite.SetActive(true);
        largeTerrainSprite.SetActive(false);
       SetReservedWalkway(false);
        goalSquareSprite_Pressed.SetActive(false);
        
        

       

        
    }

    private void Start()
    {
        AssignWaterBorderSprites();
        SetSquareMesh();

        if(type == SquareType.Enemy)
        {
            LocatePlayer();
        }
        
    }

    private void OnEnable()
    {
        waterNorth = false;
        waterEast = false;
        waterSouth = false;
        waterWest = false;

        groundSprite.SetActive(true);

        waterBorderNorth.SetActive(false);
        waterBorderEast.SetActive(false);
        waterBorderSouth.SetActive(false);
        waterBorderWest.SetActive(false);


        waterFoamCorner_NE.SetActive(false);
        waterFoamCorner_SE.SetActive(false);
        waterFoamCorner_SW.SetActive(false);
        waterFoamCorner_NW.SetActive(false);

        moodIndicator.SetActive(false);
    }

    void ChooseSquareSprite()
    {
        //Sprite chosenSprite = SquareSpriteLibrary.Instance.GetRandomSprite(squareType);
        //squareTerrainSpriteRenderer.sprite = chosenSprite;

        Sprite chosenSprite = thisSquareMapData.GetRandomTerrainSprite();

        if(chosenSprite == null)
        {
            chosenSprite = SquareSpriteLibrary.Instance.GetRandomSprite(squareType);
        }

       squareTerrainSpriteRenderer.sprite = chosenSprite;
    }

    private static readonly Dictionary<Vector2Int, directions> dirLookup =
    new Dictionary<Vector2Int, directions>
    {
        { new Vector2Int(0, 1), directions.up },
        { new Vector2Int(0, -1), directions.down },
        { new Vector2Int(1, 0), directions.right },
        { new Vector2Int(-1, 0), directions.left }
    };

    public void SetSquareMapData(MapData mapData)
    {
        thisSquareMapData = mapData;
    }

    public void ChooseSquareGroundSprite()
    {
        if (thisSquareMapData != null)
        {
            groundSpriteRenderer.sprite = thisSquareMapData.GetFloorSprite();
        }
        else
        {
            Debug.LogError("Error Getting Ground Sprite via Map Data, No Map Data Found, resort to backup.", this);
            Sprite chosenGroundSprite = SquareSpriteLibrary.Instance.GetRandomGroundSprite(mapLocation);
            groundSpriteRenderer.sprite = chosenGroundSprite;
        }



    }
    public void SetEntryDirection(Vector2Int currentPosition, Vector2Int thisPosition)
    {
        Vector2Int delta = thisPosition - currentPosition;

        if (dirLookup.TryGetValue(delta, out directions d))
            enterDirection = d;
        else
            Debug.LogWarning("Invalid movement delta: " + delta);

        int angle = 0;

        switch (enterDirection)
        {
            case directions.up:
                angle = 0;
                break;
            case directions.down:
                angle = 180;
                break;
            case directions.left:
                angle = 90;
                break;
            case directions.right:
                angle = 270;
                break;
            default:
                angle = 0;
                break;
        }
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        visitedSprite.transform.rotation = rot;
    }


    void ActivateGameObject(GameObject targetGO)
    {
        goalSquareSprite.SetActive(false);
        enemySquareSprite.SetActive(false);
        treasureSquareSprite.SetActive(false);
        terrainSquareSprite.SetActive(false);
        emptySquareSprite.SetActive(false);
        healthSquareSprite.SetActive(false);
        itemSquareSprite.SetActive(false);
        targetGO.SetActive(true);
    }

    void decideSquareQuantity()
    {
        int randomChance = UnityEngine.Random.Range(0, 3);
        switch (randomChance)
        {
            case 0:
                SetSquare(SquareSize.Small, 0.25f, "small");
                break;

            case 1:
                SetSquare(SquareSize.Medium, 0.5f, "medium");
                break;
            case 2:
                SetSquare(SquareSize.Large, 1f, "large");
                break;
        }
    }

    public void SetIsSacred(bool value)
    {
        isSacred = value;
        sacredMarker.SetActive(value);
    }

    public void SetIsWater(bool value)
    {
        ClearLegacyFlags();
        isWater = value;
        waterMarker.SetActive(value);   
        SpriteRenderer waterSR = waterMarker.GetComponent<SpriteRenderer>();
        waterSR.material = thisSquareMapData.WaterShader;

        if(value == true)
        {
            groundSprite.SetActive(false);
            
        }

        type = SquareType.Water;
    }

    void ClearLegacyFlags()
    {
        isGoalSquare = false;
        isTreasureSquare = false;
        //isEnemySquare = false;
        isTerrainSquare = false;
        isEmptySquare = false;
        isHealthSquare = false;
        isItemSquare = false;

        isMerchantSquare = false;

        // water & trap visuals / state
        isWater = false;
        trapActivated = false;
        if (trapSprite) trapSprite.SetActive(false);
        if (hiddenTrapSprite) hiddenTrapSprite.SetActive(false);
        if (merchantSprite) merchantSprite.SetActive(false);
    }


    [SerializeField] GameObject stepInWaterSprite;
    Transform stepInWaterOriginalScale;

    public void ActivateStepInWaterSprite(bool value)
    {
        stepInWaterSprite.SetActive(value);



    }
    public bool IsSacred => isSacred;

    bool isReservedWalkway = false;
    public bool IsReservedWalkway => isReservedWalkway;
    
    public void SetReservedWalkway(bool value)
    {
        isReservedWalkway = value;
    }

    void SetSquare(SquareSize sq, float scale, string name)
    {
        square = sq;
        spriteScale = scale;
        squareQuantityString = name;
    }

    public string getSquareQuantity()
    {
        return squareQuantityString;
    }

    public enum enemyMoods { positive, negative };
    enemyMoods thisEnemyMood = enemyMoods.positive;

    public enemyMoods EnemyMood => thisEnemyMood;
    public void SetEnemyMood(enemyMoods newMood)
    {
        thisEnemyMood = newMood;

        if(newMood == enemyMoods.positive)
        {
            moodIndicatorText.text = "+";
        }
        else
        {
            moodIndicatorText.text = "-";
        }
    }

    public void MakeHealthSquare()
    {
        ClearLegacyFlags();
        type = SquareType.Health;
        isHealthSquare = true;
        squareType = "health";
        ChooseSquareSprite();

        ActivateGameObject(healthSquareSprite);
    }

    public string GetContentsID()
    {
        return squareContentsID;
    }
    public void MakeItemSquare()
    {
        ClearLegacyFlags();
        type = SquareType.Item;
        isItemSquare = true;
        squareType = "item";
        ChooseSquareSprite();

        ItemCatalogue itemCatalogue = ItemCatalogue.Instance;
        Sprite itemSprite = null;

        GameObject prefab = null;

        if (itemCatalogue != null)
        {
            allItemsList = itemCatalogue.GetAllItems();

            InventoryItemTemplate[] itemCatalogueArray = allItemsList.ToArray();

            int randomInt = UnityEngine.Random.Range(0, itemCatalogueArray.Length);
            string randomID = "";

            if (isWaterAdjacent)
            {
                randomID = itemCatalogue.GetFlowerItem().itemID;
                itemSprite = itemCatalogue.GetFlowerItem().itemImage;
            }

            else
            {
                for (int i = 0; i < itemCatalogueArray.Length; i++)
                {
                    if (i == randomInt)
                    {
                        randomID = itemCatalogueArray[i].itemID;
                        itemSprite = itemCatalogueArray[i].itemImage;
                    }
                }
            }

                
            squareContentsID = randomID;

        }

        if(squareContentsID == "greatsword")
        {
            prefab = item_Greatsword;
        }

        if(prefab != null)
        {
            GameObject itemGameObject = Instantiate(prefab, itemPosition);
        }

        if(itemSprite != null)
        {
            squareItemSpriteRenderer.sprite = itemSprite;
        }

        

        ActivateGameObject(itemSquareSprite);
    }

    public void MakeFlowerSquare()
    {
        ClearLegacyFlags();
        type = SquareType.Item;
        isItemSquare = true;
        squareType = "item";
        ChooseSquareSprite();

        ItemCatalogue itemCatalogue = ItemCatalogue.Instance;


        if (itemCatalogue != null)
        {
  


            string flowerID = itemCatalogue.GetFlowerItem().itemID; ;
            Sprite flowerSprite = itemCatalogue.GetFlowerItem().itemImage;




            squareItemSpriteRenderer.sprite = flowerSprite;

            squareContentsID = flowerID;

        }

       



        ActivateGameObject(itemSquareSprite);


    }

    public bool IsEnemy => type == SquareType.Enemy;

    public void MakeStartSquare()
    {
        ActivateGameObject(startSquareSprite);
    }

    public void MakeGoalSquarePressed()
    {
        ActivateGameObject(goalSquareSprite_Pressed);
    }
    public void MakeGoalSquare()
    {
        ClearLegacyFlags();
        type = SquareType.Goal;
        isGoalSquare = true;
        squareType = "goal";
        ChooseSquareSprite();

        ActivateGameObject(goalSquareSprite);
    }

    public void MakeTreasureSquare()
    {
        ClearLegacyFlags();
        type = SquareType.Treasure;
        isTreasureSquare = true;
        squareType = "treasure";
        ChooseSquareSprite();

        string treasureSize = "medium";

        GameObject prefab = null;
       
        switch (square)
        {
            case SquareSize.Small:
                treasureSize = "small";
                prefab = treasure_Coins;
                break;
            case SquareSize.Medium:
                treasureSize = "medium";
                prefab = treasure_CoinSack;
                break;
            case SquareSize.Large:
                treasureSize = "large";
                prefab = treasure_Chest;
                break;
            default:
                treasureSize = "medium";
                prefab = treasure_CoinSack;
                break;

        }

        squareValue.gameObject.SetActive(true);
        squareValueText.text = treasureSize;

        Sprite treasureSprite = SquareSpriteLibrary.Instance.GetTreasureSprite(treasureSize);

        if(prefab != null)
        {
            GameObject treasurePrefab = Instantiate(prefab, treasurePositon);
        }
        

        if(treasureSprite != null)
        {
            treasureSpriteRenderer.sprite = treasureSprite;
        }
        else
        {
            Debug.Log("No Sprite Gotten");
        }

       ActivateGameObject(treasureSquareSprite);

    }

    public void MakeMerchantSquare()
    {
        ClearLegacyFlags();
        type = SquareType.Merchant;

        merchantSprite.SetActive(true);
        isMerchantSquare = true;
    }

   public bool GetIsMerchantSquare()
    {
        return isMerchantSquare;
    }

    [SerializeField] TextMeshProUGUI moodIndicatorText;
    [SerializeField] GameObject moodIndicator;
    public void MakeEnemySquare(MapData thisMap)
    {
        ClearLegacyFlags();
        type = SquareType.Enemy;
        //isEnemySquare = true;

        squareType = "enemy";
       // ChooseSquareSprite();

        squareValue.gameObject.SetActive(true );
        moodIndicator.SetActive(true);

        Sprite chosenSprite = null;

        switch (square)
        {
            case SquareSize.Small:
                squareValueText.text = "3+";
                chosenSprite = thisMap.GetSmallEnemySprite();
                enemyDamage = 1;
                break;
            case SquareSize.Medium:
                squareValueText.text = "4+";
                chosenSprite = thisMap.GetMediumEnemySprite();
                enemyDamage = 2;
                break;
            case SquareSize.Large:
                squareValueText.text = "5+";
                chosenSprite = thisMap.GetLargeEnemySprite();
                enemyDamage = 3;
                break;
            default:
                squareValueText.text = "3+";
                chosenSprite = thisMap.GetSmallEnemySprite();
                enemyDamage = 1;
                break;

        }

        //enemySquareSpriteRenderer.sprite = chosenSprite;
        standeeController.SetSprites(chosenSprite, chosenSprite);

        int roll = Random.Range(0, 2);

        if(roll < 1)
        {
            SetEnemyMood(enemyMoods.negative);
        }
        else
        {
            SetEnemyMood(enemyMoods.positive);
        }

            ActivateGameObject(enemySquareSprite);

        

    }


    public void MakeTerrainSquare()
    {
        ClearLegacyFlags();
        type = SquareType.Terrain;
        isTerrainSquare = true;
        squareType = "terrain";
        //ChooseSquareSprite();
        ActivateGameObject(terrainSquareSprite);
    }

    public void MakeEmptyTerrainSquare()
    {
        ClearLegacyFlags();
        type = SquareType.Terrain;
        isTerrainSquare = true;
        squareType = "terrain";
    
    }

    public bool thisSquareHoldsPlayer;

    public void MakeThisSquareHoldPlayer(bool value)
    {
    
        thisSquareHoldsPlayer = value;
    }

    public bool ThisSquareHoldsPlayer => thisSquareHoldsPlayer;

    public void MakePottardSquare(bool value)
    {
        thisSquareHoldsPottard = value;
    }

    public bool ThisSquareHoldsPottard => thisSquareHoldsPottard;
    public void MakeEmptySquare()
    {
        ClearLegacyFlags();
        type = SquareType.Empty;
        isEmptySquare = true;
        squareValue.gameObject.SetActive(false);
        squareType = "empty";
        ChooseSquareSprite();

        ActivateGameObject(emptySquareSprite);
    }

  

    public Transform GetSquareCentre()
    {
        return squareCentre;
    }

    public void ActivateSquareVisited()
    {
        hasBeenVisited = true;

        if (!isWater)
        {
            visitedSprite.gameObject.SetActive(true);
        }
        
        
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

    public void SetupNewSquare(
        int x, int y,
        string newMapLocation
        )
    {
        SetSquarePosition(x, y);
        SetMapLocation(newMapLocation);
        decideSquareQuantity();
        ChooseSquareGroundSprite();
        MakeEmptySquare();
    }

    public int GetEnemyBaseBuff()
    {
        return square switch
        {
            SquareSize.Small => 0,
            SquareSize.Medium => 1,
            SquareSize.Large => 2,
            _ => 4
        };
    }

}
