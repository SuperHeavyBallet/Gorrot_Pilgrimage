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
    [SerializeField] SpriteRenderer groundSpriteRenderer;

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

    [SerializeField] GameObject shadow;
    public bool isInShadow;

    [SerializeField] GameObject trapSprite;
    [SerializeField] GameObject hiddenTrapSprite;
    bool trapActivated = false;
    // bool isTrapSquare;

    public bool thisSquareHoldsPottard = false;

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
   
    }

    public bool GetIsWaterAdjacent() => isWaterAdjacent;

    public bool GetIsWater() => isWater;

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

        
    }

    private void OnEnable()
    {
        waterNorth = false;
        waterEast = false;
        waterSouth = false;
        waterWest = false;


        waterBorderNorth.SetActive(false);
        waterBorderEast.SetActive(false);
        waterBorderSouth.SetActive(false);
        waterBorderWest.SetActive(false);
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

    public void MakeEnemySquare(MapData thisMap)
    {
        isGoalSquare= false;
        isEnemySquare = true;
        isTreasureSquare = false;
        isEmptySquare = false;

        

        squareType = "enemy";
        ChooseSquareSprite();

        squareValue.gameObject.SetActive(true );

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

        ActivateGameObject(enemySquareSprite);

        

    }

    public void MakeTerrainSquare()
    {
        isGoalSquare= false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isTerrainSquare = true;
        isEmptySquare = false;

        squareValue.gameObject.SetActive(false);

        squareType = "terrain";
        ChooseSquareSprite();
        ActivateGameObject(terrainSquareSprite);
    }

    public void MakePottardSquare(bool value)
    {
        Debug.Log(this + "This Square Holds Pottard: " + value);
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

    public int GetEnemyBaseRequiredToWin()
    {
        return square switch
        {
            squareQuantity.small => 3,
            squareQuantity.medium => 4,
            squareQuantity.large => 5,
            _ => 4
        };
    }

}
