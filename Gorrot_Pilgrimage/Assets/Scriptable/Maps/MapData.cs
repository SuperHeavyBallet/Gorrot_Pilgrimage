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
    [SerializeField] float terrainDensity = 0.1f;

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
    [SerializeField] bool hasShadows;

    public bool GetHasEnemies() => hasEnemies;

    public bool GetHasShadows() => hasShadows;

    [SerializeField] int bribeMultiplier = 2;

    public int GetBribeMultiplier() => bribeMultiplier;

    [Tooltip("The amount of treasure in the map. 0 = none, 1 = most")]
    [SerializeField, Range(0f, 10f)]
    int treasureDensity; // 0 = none, 10 = rich

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

    public bool GetIsFinalCorridoor()
    {
        return isFinalCorridoor;
    }

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

    
}
