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

    [SerializeField] int mapSize;

    [SerializeField] MapData nextMap;

    [SerializeField] Sprite floorSprite;

    public string GetMapName()
    {
        return mapName;
    }

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
        return nextMap;
    }

    public Sprite GetFloorSprite()
    {
        return floorSprite;
    }


    
}
