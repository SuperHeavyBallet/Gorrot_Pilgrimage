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
    public GameObject terrainSquareSprite;
    public GameObject emptySquareSprite;
    public GameObject healthSquareSprite;
    public GameObject itemSquareSprite;


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

    float spriteScale = 1;

    string squareContentsID = "";

   public TextMeshProUGUI squareValue;

    List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();

    InventoryItemTemplate[] allItems;

    SquareSpriteLibrary squareSpriteLibrary;

    public SpriteRenderer squareTerrainSpriteRenderer;
    public SpriteRenderer squareItemSpriteRenderer;
    [SerializeField] SpriteRenderer groundSpriteRenderer;

    BattlefieldBuilder battlefieldBuilder;

  

    public string squareType = "empty";

    public bool isEdgeSquare;

    public bool leftEmpty;
   public bool upEmpty;
   public bool rightEmpty;
    public bool downEmpty;
    public bool needsCorner;

    string mapLocation;

    [SerializeField] SpriteRenderer treasureSpriteRenderer;

    bool isMerchantSquare;
    [SerializeField] GameObject merchantSprite;

    public void MakeEdgeSquare()
    {
        isEdgeSquare = true;
  
    }

    public void SetMapLocation(string newMapLocation)
    {
        mapLocation = newMapLocation;

    }

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

        if(leftEmpty && upEmpty || rightEmpty && upEmpty || leftEmpty && downEmpty || rightEmpty && downEmpty)
        {
            needsCorner = true;

            if(leftEmpty && upEmpty)
            {
                Vector3 position = new Vector3(this.transform.position.x - thisSquareSize, this.transform.position.y + thisSquareSize, this.transform.position.z);
                GameObject newBorderSquare = UnityEngine.Object.Instantiate(SquareSpriteLibrary.Instance.getBorderSquare(), position, Quaternion.identity);
                newBorderSquare.transform.SetParent(this.transform.parent);
            }

            if(rightEmpty && upEmpty)
            {
                Vector3 position = new Vector3(this.transform.position.x + thisSquareSize, this.transform.position.y + thisSquareSize, this.transform.position.z);
                GameObject newBorderSquare = UnityEngine.Object.Instantiate(SquareSpriteLibrary.Instance.getBorderSquare(), position, Quaternion.identity);
                newBorderSquare.transform.SetParent(this.transform.parent);
            }

            if(leftEmpty && downEmpty)
            {
                Vector3 position = new Vector3(this.transform.position.x - thisSquareSize, this.transform.position.y - thisSquareSize, this.transform.position.z);
                GameObject newBorderSquare = UnityEngine.Object.Instantiate(SquareSpriteLibrary.Instance.getBorderSquare(), position, Quaternion.identity);
                newBorderSquare.transform.SetParent(this.transform.parent);
            }

            if (rightEmpty && downEmpty)
            {
                Vector3 position = new Vector3(this.transform.position.x + thisSquareSize, this.transform.position.y - thisSquareSize, this.transform.position.z);
                GameObject newBorderSquare = UnityEngine.Object.Instantiate(SquareSpriteLibrary.Instance.getBorderSquare(), position, Quaternion.identity);
                newBorderSquare.transform.SetParent(this.transform.parent);
            }
        }


  

        if (leftEmpty)
        {
            Vector3 position = new Vector3(this.transform.position.x - thisSquareSize, this.transform.position.y, this.transform.position.z);
           

            GameObject newBorderSquare = UnityEngine.Object.Instantiate(SquareSpriteLibrary.Instance.getBorderSquare(), position, Quaternion.identity, transform.parent);
        }
        if(rightEmpty)
        {
            Vector3 position = new Vector3(this.transform.position.x + thisSquareSize, this.transform.position.y, this.transform.position.z);
            GameObject newBorderSquare = UnityEngine.Object.Instantiate(SquareSpriteLibrary.Instance.getBorderSquare(), position, Quaternion.identity, transform.parent);
        }
        if (upEmpty)
        {
            Vector3 position = new Vector3(this.transform.position.x, this.transform.position.y + thisSquareSize ,this.transform.position.z);
            GameObject newBorderSquare = UnityEngine.Object.Instantiate(SquareSpriteLibrary.Instance.getBorderSquare(), position, Quaternion.identity, transform.parent);
        }
        if (downEmpty)
        {
            Vector3 position = new Vector3(this.transform.position.x, this.transform.position.y - thisSquareSize, this.transform.position.z);
            GameObject newBorderSquare = UnityEngine.Object.Instantiate(SquareSpriteLibrary.Instance.getBorderSquare(), position, Quaternion.identity, transform.parent);
        }




    }

    private void OnEnable()
    {
        decideSquareQuantity();
        squareValue.gameObject.SetActive(false);

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

    public void ChooseSquareGroundSprite()
    {
        

        Sprite chosenGroundSprite = SquareSpriteLibrary.Instance.GetRandomGroundSprite(mapLocation);
        groundSpriteRenderer.sprite = chosenGroundSprite; 
    }
    public void SetEntryDirection(Vector2Int currentPosition, Vector2Int thisPosition)
    {
        Vector2Int delta = thisPosition - currentPosition;

        if (dirLookup.TryGetValue(delta, out directions d))
            enterDirection = d;
        else
            Debug.LogWarning("Invalid movement delta: " + delta);

        int angle = 0;

        switch(enterDirection)
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

       // squareValue.gameObject.SetActive(true);
        squareType = "health";
        ChooseSquareSprite();
        /*
        switch(square)
        {
            case squareQuantity.small:
                squareValue.text = "+1H";
                break;
            case squareQuantity.medium:
                squareValue.text = "+3H, -1S";
                break;
            case squareQuantity.large:
                squareValue.text = "5H, -3S";
                break;
            default:
                break;

        }*/

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

       // squareValue.gameObject.SetActive(true);
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

            for(int i = 0; i < itemCatalogueArray.Length; i++)
            {
                if(i == randomInt)
                {
                    randomID = itemCatalogueArray[i].itemID;
                    itemSprite = itemCatalogueArray[i].itemImage;
                }
            }

            squareContentsID = randomID;
            


        }

        if(itemSprite != null)
        {
            squareItemSpriteRenderer.sprite = itemSprite;
        }
        

        //squareValue.text = squareContentsID;


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
        isHealthSquare = true;

        //squareValue.gameObject.SetActive(false);
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

        //squareValue.gameObject.SetActive(true);
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

    public void MakeEnemySquare()
    {
        isGoalSquare= false;
        isEnemySquare = true;
        isTreasureSquare = false;
        isEmptySquare = false;

        //squareValue.gameObject.SetActive(true);
        squareType = "enemy";
        ChooseSquareSprite();

        /*
        switch (square)
        {
            case squareQuantity.small:
                squareValue.text = "-1H, >2";
                break;
            case squareQuantity.medium:
                squareValue.text = "-3H, >3";
                break;
            case squareQuantity.large:
                squareValue.text = "-5H, >4";
                break;
            default:
                break;

        }*/

        ActivateGameObject(enemySquareSprite);

        

    }

    public void MakeTerrainSquare()
    {
        isGoalSquare= false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isTerrainSquare = true;
        isEmptySquare = false;

        //squareValue.gameObject.SetActive(false);

        squareType = "terrain";
        ChooseSquareSprite();
        ActivateGameObject(terrainSquareSprite);
    }


    public void MakeEmptySquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = true;

        //squareValue.gameObject.SetActive(false);
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
        visitedSprite.gameObject.SetActive(true);
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
        ChooseSquareGroundSprite();
        MakeEmptySquare();
    }
}
