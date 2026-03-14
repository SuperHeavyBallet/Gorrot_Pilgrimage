using GorrotGame;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class SquareTypeController : MonoBehaviour
{

    [SerializeField] SquareController squareController;

    SquareType thisSquareType = SquareType.Empty;
    public SquareType ThisSquareType => thisSquareType;

    SquareSize thisSquareSize = SquareSize.Medium;

    SquareMood thisSquareMood = SquareMood.Positive;
    MapData thisMapData;

    [SerializeField] StandeeController standeeController;

    [SerializeField] GameObject squareValueObject;
    [SerializeField] TMP_Text squareValueText;

    [SerializeField] GameObject moodIndicatorObject;
    [SerializeField] TMP_Text moodIndicatorText;

    [SerializeField] GameObject enemySquareVisuals;
    int squareValue = 0;
    int squareBaseDamage = 0;
    public int SquareBaseDamage => squareBaseDamage;

    [SerializeField] GameObject healthSquareSpriteObject;
    [SerializeField] SpriteRenderer healthSquareSpriteRenderer;

    List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();

    [SerializeField] GameObject itemSquareSprite;
    [SerializeField] GameObject item_Greatsword;
    [SerializeField] Transform itemPosition;
    [SerializeField] SpriteRenderer squareItemSpriteRenderer;
    string squareContentsID = "";
    public string ContentsID => squareContentsID;

    [SerializeField] GameObject treasureSquareSprite;
    [SerializeField] GameObject treasure_Coins;
    [SerializeField] GameObject treasure_CoinSack;
    [SerializeField] GameObject treasure_Chest;
    [SerializeField] Transform treasurePosition;


    [SerializeField] GameObject trapSprite;
    [SerializeField] GameObject hiddenTrapSprite;
    bool trapActivated = false;
    public bool TrapActivated => trapActivated;

    [SerializeField] GameObject warningSign;

    [SerializeField] GameObject goalSpriteObject;
    [SerializeField] GameObject startSpriteObject;

    [SerializeField] GameObject merchantSpriteObject;

    [SerializeField] GameObject waterSpriteObject;
    [SerializeField] GameObject regularSquareMesh;
    [SerializeField] Transform squareMeshContainer;

    [SerializeField] GameObject terrainSpriteObject;

    [SerializeField] GameObject stoneThrowEffect;

    private void OnEnable()
    {
        DisableAllObjects();
    }


    public void ConstructSquare(SquareType sqType, SquareSize sqSize, MapData thisMap)
    {
        

        thisSquareType = sqType;
        thisSquareSize = sqSize;
        thisMapData = thisMap;

        SetSquareMesh(thisMap);

        regularSquareMesh.SetActive(true);

        if (thisSquareType == SquareType.Enemy)
        {
            SetEnemySprite();
            SetSquareValue();
            SetMood();
            squareController.LocatePlayer();

            enemySquareVisuals.SetActive(true);
        }
        else if (thisSquareType == SquareType.Health)
        {
            SetHealthSprite();
            SetSquareValue();
        }
        else if (thisSquareType == SquareType.Item)
        {
            SetItemType(ItemNames.Any);
        }
        else if (thisSquareType == SquareType.Empty)
        {
            SetEmptySquare();
        }
        else if (thisSquareType == SquareType.Treasure)
        {
            SetTreasureSprite();
        }
        else if (thisSquareType == SquareType.Trap)
        {
            SetTrap();
        }
        else if(thisSquareType == SquareType.Goal)
        {
            SetGoal();
        }
        else if(thisSquareType == SquareType.Start)
        {
            SetStart();
        }
        else if(thisSquareType == SquareType.Merchant)
        {
            SetMerchant();
        }
        else if(thisSquareType == SquareType.Water)
        {
            SetWater();
        }
        else if(thisSquareType == SquareType.Terrain)
        {
            SetSmallTerrain();
        }
        
      
       
    }

   

    // Basic Square Route

    void SetSquareMesh(MapData thisSquareMapData)
    {
        GameObject prefab = thisSquareMapData.GetSquareMesh();
        ClearChildren(squareMeshContainer);

        GameObject mesh = Instantiate(prefab, squareMeshContainer);
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


        void SetSquareValue()
    {
        switch (thisSquareSize)
        {
            case SquareSize.Small:
                squareValue = 3;
                break;
            case SquareSize.Medium:
                squareValue = 4;
                break;
            case SquareSize.Large:
                squareValue = 5;
                break;
            default:
                squareValue = 3;
                break;

        }

        squareValueObject.SetActive(true);
        squareValueText.text = squareValue.ToString() + " +";
        squareBaseDamage = squareValue;

    }

    // Terrain Squares Route

    void SetSmallTerrain()
    {
        terrainSpriteObject.SetActive(true);
    }

    public void ConstructEmptyTerrainSquare(SquareType sqType, SquareSize sqSize, MapData thisMap)
    {
        thisSquareType = sqType;
        thisSquareSize = sqSize;
        thisMapData = thisMap;
    }
    // Water Square Route

    void SetWater()
    {
        waterSpriteObject.SetActive(true);
        SpriteRenderer waterSR = waterSpriteObject.GetComponent<SpriteRenderer>();
        waterSR.material = thisMapData.WaterShader;
        regularSquareMesh.SetActive(false);

    }

    // Merchant Square Route

    void SetMerchant()
    {
        merchantSpriteObject.SetActive(true);
    }

    // Flower Specific Item Route
    public void ConstructFlowerSquare(SquareType sqType, SquareSize sqSize, MapData thisMap)
    {
        thisSquareType = sqType;
        thisSquareSize = sqSize;
        thisMapData = thisMap;

        SetItemType(ItemNames.Flower);

    }

    // Start Square Route

    void SetStart()
    {
        startSpriteObject.SetActive(true);
    }

    // Goal Square Route

    void SetGoal()
    {

        goalSpriteObject.SetActive(true);
    }


    // Trap Square Route

    void SetTrap()
    {
        hiddenTrapSprite.SetActive(true);
    }

    public void ActivateTrap(bool playSoundEffect)
    {
        hiddenTrapSprite.SetActive(false);
        trapSprite.SetActive(true);
        trapActivated = true;

        stoneThrowEffect.SetActive(true);
        StartCoroutine(DisableAfterTime(stoneThrowEffect, 0.5f));
        if(playSoundEffect)
        {
            AudioManager.Instance.PlayTrapTriggerSoundEffect();
        }
        
    }

    public IEnumerator DisableAfterTime(GameObject go, float time)
    {
        yield return new WaitForSeconds(time);
        go.SetActive(false);
    }

    // Treasure Square Route

    void SetTreasureSprite()
    {
        GameObject prefab = null;

        switch (thisSquareSize)
        {
            case SquareSize.Small:
                prefab = treasure_Coins;
                break;
            case SquareSize.Medium:
                prefab = treasure_CoinSack;
                break;
            case SquareSize.Large:
                prefab = treasure_Chest;
                break;
            default:
                prefab = treasure_CoinSack;
                break;

        }

        if (prefab != null)
        {
            GameObject treasurePrefab = Instantiate(prefab, treasurePosition);
        }

        treasureSquareSprite.SetActive(true);

    }

    // Empty Square Route

    void SetEmptySquare()
    {
        DisableAllObjects();
    }

    // Item Pickup Route
    void SetItemType(ItemNames itemName)
    {
        ItemCatalogue itemCatalogue = ItemCatalogue.Instance;
        Sprite itemSprite = null;
        GameObject prefab = null;
        squareContentsID = "";


        if (itemCatalogue != null)
        {
            if(itemName == ItemNames.Any)
            {
                allItemsList = itemCatalogue.GetAllItems();
                InventoryItemTemplate[] itemCatalogueArray = allItemsList.ToArray();

                int randomInt = UnityEngine.Random.Range(0, itemCatalogueArray.Length);
                string randomID = "";

                if (squareController.IsWaterAdjacent)
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
                            prefab = itemCatalogueArray[i].ItemMesh;
                        }
                    }
                }

                squareContentsID = randomID;
            }
            else if (itemName == ItemNames.Flower)
            {
                string flowerID = itemCatalogue.GetFlowerItem().itemID; ;
                Sprite flowerSprite = itemCatalogue.GetFlowerItem().itemImage;




                squareItemSpriteRenderer.sprite = flowerSprite;

                squareContentsID = flowerID;
            }
          
        }


        if (squareContentsID == "greatsword")
        {
            prefab = item_Greatsword;
        }

        if (prefab != null)
        {
            GameObject itemGameObject = Instantiate(prefab, itemPosition);
        }

        if (itemSprite != null)
        {
            squareItemSpriteRenderer.sprite = itemSprite;
        }

        itemSquareSprite.SetActive(true);

    }

    // Health Boost Route
    void SetHealthSprite()
    {
        Sprite chosenSprite = SquareSpriteLibrary.Instance.GetHealthSprite(thisSquareSize);

        healthSquareSpriteRenderer.sprite = chosenSprite;
        healthSquareSpriteObject.SetActive(true);
        
    }
  
    //  Enemy / NPC Routes

    void SetEnemySprite()
    {
        Sprite chosenSprite = null;

        switch (thisSquareSize)
        {
            case SquareSize.Small:
                chosenSprite = thisMapData.GetSmallEnemySprite();
                break;
            case SquareSize.Medium:
                chosenSprite = thisMapData.GetMediumEnemySprite();
                break;
            case SquareSize.Large:
                chosenSprite = thisMapData.GetLargeEnemySprite();
                break;
            default:
                chosenSprite = thisMapData.GetSmallEnemySprite();
                break;

        }

        standeeController.SetSprites(chosenSprite, chosenSprite);
    }

    void SetMood()
    {
        int roll = Random.Range(0, 2);

        if(roll < 1)
        {
            thisSquareMood = SquareMood.Negative;
            moodIndicatorText.text = "-";
        }
        else
        {
            thisSquareMood = SquareMood.Positive;
            moodIndicatorText.text = "+";
        }

        moodIndicatorObject.SetActive(true);
    }

    public SquareMood SquareMood => thisSquareMood;

    // Helper Functions

    void DisableAllObjects()
    {
        if (squareValueObject != null)
            squareValueObject.SetActive(false);
        else
            Debug.LogError($"Missing Square Value on {name}", this);

        if (moodIndicatorObject != null)
            moodIndicatorObject.SetActive(false);
        else
            Debug.LogError($"Missing Mood Indicator on {name}", this);

        if(healthSquareSpriteObject != null)
            healthSquareSpriteObject.SetActive(false);
        else
            Debug.LogError($"Missing Health Sprite on {name}", this);

        if(itemSquareSprite != null)
            itemSquareSprite.SetActive(false);
        else
            Debug.LogError($"Missing Item Sprite on {name}", this);

        if(enemySquareVisuals != null)
            enemySquareVisuals.SetActive(false);
        else
            Debug.LogError($"Missing Enemy Visuals on {name}", this);

        if(treasureSquareSprite != null)
            treasureSquareSprite.SetActive(false);
        else
            Debug.LogError($"Missing Treasure Sprite on {name}", this);

        if(hiddenTrapSprite != null)
            hiddenTrapSprite.SetActive(false);
        else
            Debug.LogError($"Missing Trap on {name}", this);

        if(trapSprite != null)
            trapSprite.SetActive(false);
        else
            Debug.LogError($"Missing Hidden Trap on {name}", this);

        if(goalSpriteObject != null) goalSpriteObject.SetActive(false);
        else
            Debug.LogError($"Missing Goal on {name}", this);

        if (startSpriteObject != null) startSpriteObject.SetActive(false);
        else
            Debug.LogError($"Missing Start on {name}", this);

        if(merchantSpriteObject != null) merchantSpriteObject.SetActive(false);
        else
            Debug.LogError($"Missing Merchant Sprite on {name}", this);

        if(waterSpriteObject != null) waterSpriteObject.SetActive(false);
        else
            Debug.LogError($"Missing Water Sprite on {name}", this);

        if(terrainSpriteObject != null) terrainSpriteObject.SetActive(false);
        else
            Debug.LogError($"Missing Terrain Sprite on {name}", this);

        if(stoneThrowEffect != null) stoneThrowEffect.SetActive(false);
        else
            Debug.LogError($"Missing Stone Throw Effect on {name}", this);
    }
    
}
