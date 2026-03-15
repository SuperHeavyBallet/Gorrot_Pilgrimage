using GorrotGame;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{

    [SerializeField] string mapID;

    [SerializeField] enum MapLocations
    {
        Outmost_Territories,
        Outer_Ring,
        Inner_Ring,
        Swamp_Border,
        Outer_Swamp,
        Inner_Swamp,
        Gorrot
    }

    [SerializeField] MapLocations mapLocation = MapLocations.Outmost_Territories;

    [SerializeField] string mapName;
    public string GetMapName() => mapName;

    [SerializeField] int mapSize;

    [SerializeField] MapData[] nextMaps;

    [SerializeField] Sprite floorSprite;

    [SerializeField] bool isWildMap; // This decides if a map is single passing through or random, getting lost in a swamp or wilderness etc

    [Tooltip("The number represents the chances of becoming 'stuck' in this map. 0 = No chance, 1 slight chance, 2+ higher chance. Rolled against random number, range: 0 - this int, if not 0 - Reroll same map")]
    [SerializeField] int wildLevel;

    [SerializeField] float escapeChance = 0.7f;
    public float GetEscapeChance() => escapeChance;

    [Tooltip("Reserved for THE first map")]
    [SerializeField] bool isFirstMap = false;
    [Tooltip("Reserved for THE final map")]
    [SerializeField] bool isFinalMap = false;

    [SerializeField]  bool hasMerchant = false;

    [SerializeField] MerchantStock merchantStock;

    [Tooltip("The amount of terrain to fill the map. 0 = none, 1 = most")]
    [SerializeField, Range(0f, 1f)]
    float terrainDensity = 0.5f;

    [SerializeField] bool hasFlies;

    [SerializeField] bool isFinalCorridoor;
    int finalCorrWidth = 2;
    int finalCorrLength = 50;

    [SerializeField, Range(0f, 1f)]
    float waterAmount; // 0 = dry, 1 = very wet

    public float GetWaterAmount() => waterAmount;

    [SerializeField] Sprite[] smallEnemySprites;
    [SerializeField] Sprite[] mediumEnemySprites;
    [SerializeField] Sprite[] largeEnemySprites;

    [SerializeField] Sprite[] floorSprites;

    [SerializeField] bool hasEnemies;
    [Tooltip("Fraction of tiles that become enemies. 0.05 = 5% of map area.")]
    [SerializeField, Range(0f, 5f)]
    float enemyDensity = 0.5f;
    public float EnemyDensity => enemyDensity;

    [SerializeField] bool hasShadows;

    [SerializeField] bool canHavePottard = true;

    public bool CanHavePottard => canHavePottard;
    public bool GetHasEnemies => hasEnemies;

    public bool GetHasShadows() => hasShadows;

    [SerializeField] bool hasHiddenTraps;
    public bool GetHasHiddenTraps => hasHiddenTraps;

    [Tooltip("The amount of hidden traps in the map. 0 = none, 2 = most")]
    [SerializeField, Range(0f, 1f)]
    float hiddenTrapDensity; // 0 = min, 1 = max

    public float GetHiddenTrapDensity => hiddenTrapDensity;

    [SerializeField] int bribeMultiplier = 2;
    [SerializeField] bool canBeBribed = true;

    public bool GetCanBeBribed() => canBeBribed;

    public int GetBribeMultiplier() => bribeMultiplier;

    [SerializeField] Material waterShader;
    [SerializeField] Material waterFoamShader;
    public Material WaterShader => waterShader;
    public Material WaterFoamShader => waterFoamShader;

    [SerializeField] bool hasFourBlockTerrain = true;
    public bool HasFourBlockTerrain => hasFourBlockTerrain;

    [Tooltip("The amount of four block terrain in the map. 0 = none, 2 = most")]
    [SerializeField, Range(0f, 1f)]
    float fourBlockTerrainDensity = 0.5f; // 0 = min, 1 = max
    public float GetFourBlockTerrainDensity() => fourBlockTerrainDensity;

    [SerializeField]
    Sprite[] terrainSprites;
    Sprite blankTerrainSprite;

    [SerializeField] GameObject[] thisMap_SquareMeshes;
    [SerializeField]  GameObject default_SquareMesh;

    [SerializeField] GameObject[] thisMap_FourSquareTerrain;
    [SerializeField] GameObject default_FourSquareTerrain;
    public GameObject GetFourSquareMesh()
    {
        if (thisMap_FourSquareTerrain.Length > 0)
        {
            return thisMap_FourSquareTerrain[UnityEngine.Random.Range(0, thisMap_FourSquareTerrain.Length)];
        }

        return default_FourSquareTerrain;

    }
    public GameObject GetSquareMesh()
    {
        if(thisMap_SquareMeshes.Length > 0)
        {
            return thisMap_SquareMeshes[UnityEngine.Random.Range(0, thisMap_SquareMeshes.Length)];
        }

        return default_SquareMesh;

        
    }

    public Sprite GetRandomTerrainSprite() => RandomSpriteFromArray(terrainSprites);

    Sprite RandomSpriteFromArray(Sprite[] spriteArrray)
    {
        if(spriteArrray.Length > 0)
        {
            int ranInt = UnityEngine.Random.Range(0, spriteArrray.Length);
            return spriteArrray[ranInt];
        }
        else
        {
            return null;
        }
        

    }

    [SerializeField] Sprite[] fourBlockTerrainSprites;
    public Sprite GetRandomFourBlockTerrainSprite()
    {
        if(fourBlockTerrainSprites.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, fourBlockTerrainSprites.Length);
            return fourBlockTerrainSprites[randomIndex];
        }
        else
        {
            Debug.LogError("No Four Block Terrain Sprite Assigned", this);
            return null;
        }
       
    }


    [Tooltip("The amount of treasure in the map. 0 = none, 1 = most")]
    [SerializeField, Range(0f, 10f)]
    int treasureDensity; // 0 = none, 10 = rich


    [SerializeField] Sprite[] waterBorderSprites;

    [SerializeField] TextAsset enemyDialogue;
    EnemyDialogueData dialogueData;

    public void ParseDialogue()
    {
        if (enemyDialogue != null)
        {
            Debug.Log("Ennemy Dialogue Assigned");
            dialogueData = JsonUtility.FromJson<EnemyDialogueData>(enemyDialogue.text);
        }
        else
        {
            Debug.Log("Ennemy Dialogue Null");
        }
    }

    [System.Serializable]
    public class EnemyDialogueData
    {
        public string mapId;
        public DialogueCategories categories;
    }

    [System.Serializable]
    public class DialogueCategories
    {
        public DialogueSet small;
        public DialogueSet medium;
        public DialogueSet large;
    }

    [System.Serializable]
    public class DialogueSet
    {
        public string[] positive;
        public string[] negative;
    }


    public string GetRandomLine(SquareSize size, SquareMood mood)
    {
        Debug.Log(size + " " + mood);   
        if (dialogueData == null)
        {
            
            return "...";
        }



        if (mood == SquareMood.Positive)
        {
            string[] pool = size switch
            {
                SquareSize.Small => dialogueData.categories.small.positive,
               SquareSize.Medium => dialogueData.categories.medium.positive,
                SquareSize.Large  => dialogueData.categories.large.positive,
                _ => null
            };

            

            if (pool == null || pool.Length == 0)
            {
                return "...";
            }

            return pool[UnityEngine.Random.Range(0, pool.Length)];
        }
        else
        {
            string[] pool = size switch
            {
                SquareSize.Small => dialogueData.categories.small.negative,
                SquareSize.Medium => dialogueData.categories.medium.negative,
                SquareSize.Large => dialogueData.categories.large.negative,
                _ => null
            };

            if (pool == null || pool.Length == 0)
            {
                return "...";
            }

            return pool[UnityEngine.Random.Range(0, pool.Length)];
        }



    }


    public int GetWaterBorderSpritesArrayLength => waterBorderSprites.Length;    

    public Sprite GetRandomWaterBorderSprite()
    {
        int randomNumber = ReturnRandomNumberInRange(0, waterBorderSprites.Length);
        return waterBorderSprites[randomNumber];
    }

    int ReturnRandomNumberInRange(int bottomRange, int topRange)
    {
        return UnityEngine.Random.Range(bottomRange, topRange);
    }
    public int GetTreasureDensity() => treasureDensity;

    public MerchantStock GetMerchantStock()
    {
        if(hasMerchant)
        {
            return merchantStock;
        }
        else
        {
            return null;
        }
    }

    public Vector2Int GetFinalCorrDimensions()
    {
        return new Vector2Int(finalCorrWidth, finalCorrLength);

    }

    public bool IsFinalCorridoor => isFinalCorridoor;
    

    public Sprite GetSmallEnemySprite()
    { return smallEnemySprites[0]; }

    public Sprite GetMediumEnemySprite()
    {
        return mediumEnemySprites[0];
    }

    public Sprite GetLargeEnemySprite()
    {
        return largeEnemySprites[0];
    }

    public string GetMapLocation()
    {
        return mapLocation.ToString();
    }

    public int GetMapSize()
    {
        return mapSize;
    }

    public bool GetHasFlies()
    {
        return hasFlies;
    }

    public MapData RollNextMap()
    {

        int randomNumber = UnityEngine.Random.Range(0, nextMaps.Length);
        return nextMaps[randomNumber];
    }

    public MapData GetStartingMap(StartLocations startingLocation)
    {
        MapData startingMapName = nextMaps[0];

        for (int i = 0; i < nextMaps.Length; i++)
        {
            if (startingLocation.ToString() == nextMaps[i].mapName)
            {
                startingMapName = nextMaps[i];
                return startingMapName;
            }
        }



        return startingMapName;
    }

    public Sprite GetFloorSprite()
    {
        if (floorSprites != null && floorSprites.Length > 0)
        {
            return floorSprites[UnityEngine.Random.Range(0, floorSprites.Length)];
        }
        else
        {
            return floorSprite;
        }
            
    }

    public bool GetIsWildMap()
    {
        return isWildMap;
    }

    public int GetWildLevel()
    {
        return wildLevel;
    }

    public bool GetIsFinalMap()
    {
        return isFinalMap;
    }

    public bool GetIsFirstMap() => isFirstMap;

    public bool GetHasMerchant()
    {
        return hasMerchant;
    }

    /*
    public MapData GetStartingMap(StartLocations startingLocation)
    {
        Debug.Log("STARTING MAP: " + startingLocation.ToString());

        MapData startingMap = nextMaps[0];

        for (int i = 0; i < nextMaps.Length; i++)
        {

            if (nextMaps[i].GetMapName() == startingLocation.ToString())
            {
                startingMap = nextMaps[i];
                break;
            }
        }

        Debug.Log(startingMap.GetMapName());
        return startingMap;
    }*/

    public float GetTerrainDensity()
    {
        return terrainDensity;
    }

    [SerializeField] bool hasOverheadSpanObjects = false;
    public bool HasOverheadSpanObjects => hasOverheadSpanObjects;

    [SerializeField] GameObject overHeadSpanObject;
    public GameObject OverheadSpanObject => overHeadSpanObject;

    
}
