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

    public string GetMapLocation()
    {
        return mapLocation.ToString();
    }

    public int GetMapSize()
    {
        return mapSize;
    }

    public MapData RollNextMap()
    {

        int randomNumber = UnityEngine.Random.Range(0, nextMaps.Length);
        return nextMaps[randomNumber];
    }

    public Sprite GetFloorSprite()
    {
        return floorSprite;
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
    }

    public float GetTerrainDensity()
    {
        return terrainDensity;
    }

    
}
