using UnityEngine;

public class MapCatalogue : MonoBehaviour
{

    [SerializeField] MapData[] allMaps;

    [SerializeField] MapData[] OutworldMaps;

    [SerializeField] MapData[] InworldMaps;

    [SerializeField] MapData[] OuterGorrotMaps;

    [SerializeField] MapData[] InnterGorrotMaps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public MapData GetMap(int stage)
    {
        return OutworldMaps[0];
    }
}
