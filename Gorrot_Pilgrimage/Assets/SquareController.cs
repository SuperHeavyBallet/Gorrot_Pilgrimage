using UnityEngine;
using TMPro;

using System.Collections.Generic;


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
    public bool isItemSquare;

    public GameObject goalSquareSprite;
    public GameObject treasureSquareSprite;
    public GameObject enemySquareSprite;
    [SerializeField]SpriteRenderer enemySquareSpriteRenderer;
    public GameObject terrainSquareSprite;
    public GameObject emptySquareSprite;
    public GameObject healthSquareSprite;
    public GameObject itemSquareSprite;
    [SerializeField] GameObject waterAdjacentSprite;


    public int squareX = 0;
    public int squareY = 0;

    public enum squareQuantity { small, medium, large };
    public squareQuantity square = squareQuantity.medium;
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

    public enum SquareType
    {
        Empty, 
        Goal, 
        Treasure, 
        Enemy, 
        Terrain,
        Health, 
        Item, 
        Trap, 
        Merchant, 
        Water
    }

    [SerializeField] private SquareType type;
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
            MakeCornerBorderAtPosition(transform.position + new Vector3(-thisSquareSize, thisSquareSize, 0f));

        if (rightEmpty && upEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(thisSquareSize, thisSquareSize, 0f));

        if (leftEmpty && downEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(-thisSquareSize, -thisSquareSize, 0f));

        if (rightEmpty && downEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(thisSquareSize, -thisSquareSize, 0f));


        if (leftEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.left * thisSquareSize, "right");
        if (rightEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.right * thisSquareSize, "left");
        if (upEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.up * thisSquareSize, "bottom");
        if (downEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.down * thisSquareSize, "top");
    }

    void MakeCornerBorderAtPosition(Vector3 position)
    {
        Instantiate(
            SquareSpriteLibrary.Instance.getBorderSquare(),
            position,
            Quaternion.identity,
            transform.parent
        );
    }

    void MakeBorderSquareAtPosition(Vector3 position, string shadowSide)
    {
        GameObject newBorderSquare = UnityEngine.Object.Instantiate(
            SquareSpriteLibrary.Instance.getBorderSquare(),
            position,
            Quaternion.identity,
            transform.parent
            );

        BorderSquareController borderSquareController = newBorderSquare.GetComponent<BorderSquareController>();
        if (borderSquareController != null)
        {
            borderSquareController.PositionBorderShadow(shadowSide);
        }
    }

    private void Awake()
    {
        squareValue.gameObject.SetActive(false);
        sacredMarker.gameObject.SetActive(false);
        waterMarker.gameObject.SetActive(false);
        ActivateStepInWaterSprite(false);
        groundSprite.SetActive(true);

       

        
    }

    private void Start()
    {
        AssignWaterBorderSprites();
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
        Sprite chosenSprite = SquareSpriteLibrary.Instance.GetRandomSprite(squareType);
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
                SetSquare(squareQuantity.small, 0.25f, "small");
                break;

            case 1:
                SetSquare(squareQuantity.medium, 0.5f, "medium");
                break;
            case 2:
                SetSquare(squareQuantity.large, 1f, "large");
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
        isWater = value;
        waterMarker.SetActive(value);   
        SpriteRenderer waterSR = waterMarker.GetComponent<SpriteRenderer>();
        waterSR.material = thisSquareMapData.WaterShader;

        if(value == true)
        {
            groundSprite.SetActive(false);
            
        }
    }


    [SerializeField] GameObject stepInWaterSprite;
    Transform stepInWaterOriginalScale;

    public void ActivateStepInWaterSprite(bool value)
    {
        stepInWaterSprite.SetActive(value);



    }
    public bool GetIsSacred()
    {
        return isSacred;
    }

    void SetSquare(squareQuantity sq, float scale, string name)
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
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = false;
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
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = false;
        isHealthSquare = false;
        isItemSquare = true;

        squareType = "item";
        ChooseSquareSprite();

        ItemCatalogue itemCatalogue = ItemCatalogue.Instance;
        Sprite itemSprite = null;

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

        if(itemSprite != null)
        {
            squareItemSpriteRenderer.sprite = itemSprite;
        }

        

        ActivateGameObject(itemSquareSprite);
    }

    public void MakeFlowerSquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = false;
        isHealthSquare = false;
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

    public bool CheckIsEnemy()
    {
        return isEnemySquare;
    }

    public void MakeGoalSquare()
    {
        isGoalSquare = true;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = false;
        isHealthSquare = false;

        squareType = "goal";
        ChooseSquareSprite();

        ActivateGameObject(goalSquareSprite);
    }

    public void MakeTreasureSquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = true;
        isEmptySquare = false;

        squareType = "treasure";
        ChooseSquareSprite();

        string treasureSize = "medium";
       
        switch (square)
        {
            case squareQuantity.small:
                treasureSize = "small";
                break;
            case squareQuantity.medium:
                treasureSize = "medium";
                break;
            case squareQuantity.large:
                treasureSize = "large";
                break;
            default:
                treasureSize = "medium";
                break;

        }

        squareValue.gameObject.SetActive(true);
        squareValueText.text = treasureSize;

        Sprite treasureSprite = SquareSpriteLibrary.Instance.GetTreasureSprite(treasureSize);

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
        isGoalSquare= false;
        isEnemySquare = true;
        isTreasureSquare = false;
        isEmptySquare = false;

        

        squareType = "enemy";
        ChooseSquareSprite();

        squareValue.gameObject.SetActive(true );
        moodIndicator.SetActive(true);


        switch (square)
        {
            case squareQuantity.small:
                squareValueText.text = "3+";
                enemySquareSpriteRenderer.sprite = thisMap.GetSmallEnemySprite();
                break;
            case squareQuantity.medium:
                squareValueText.text = "4+";
                enemySquareSpriteRenderer.sprite = thisMap.GetMediumEnemySprite();
                break;
            case squareQuantity.large:
                squareValueText.text = "5+";
                enemySquareSpriteRenderer.sprite = thisMap.GetLargeEnemySprite();
                break;
            default:
                break;

        }

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
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isTerrainSquare = true;
        isEmptySquare = false;

        squareValue.gameObject.SetActive(false);

        squareType = "terrain";
        ChooseSquareSprite();
        ActivateGameObject(terrainSquareSprite);
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
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isItemSquare = false;

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
            squareQuantity.small => 0,
            squareQuantity.medium => 1,
            squareQuantity.large => 2,
            _ => 4
        };
    }

}
