using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{

    [SerializeField] string mapID;

    [SerializeField] enum MapLocations
    {
        InnerGorrot,
        OuterGorrot,
        Midworld,
        Outworld
    }

    [SerializeField] MapLocations mapLocation = MapLocations.InnerGorrot;

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

    [Tooltip("Reserved for THE final map")]
    [SerializeField] bool isFinalMap = false;



   

    public string GetMapLocation()
    {
        return mapLocation.ToString();
    }

    public int GetMapSize()
    {
        return mapSize;
    }

    public MapData GetNextMap()
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

    
}
