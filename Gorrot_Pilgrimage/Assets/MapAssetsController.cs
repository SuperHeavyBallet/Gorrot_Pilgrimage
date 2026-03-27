using UnityEngine;

public class MapAssetsController : MonoBehaviour
{
    public static MapAssetsController Instance { get; private set; }

    [Header("Floor Squares")]
    [SerializeField] GameObject[] outmostTerritoriesFloorSquares;
    [SerializeField] GameObject[] outerRingFloorSquares;
    [SerializeField] GameObject[] innerRingFloorSquares;
    [SerializeField] GameObject[] swampBorderFloorSquares;
    [SerializeField] GameObject[] outerSwampFloorSquares;
    [SerializeField] GameObject[] innerSwampFloorSquares;
    [SerializeField] GameObject[] gorrotTownFloorSquares;
    [SerializeField] GameObject[] gorrotChurchFloorSquares;

    [Header("Wall Top Dressings")]
    [SerializeField] GameObject outmostTerritoriesWallTopDressing;
    [SerializeField] GameObject outerRingWallTopDressing;
    [SerializeField] GameObject innerRingWallTopDressing;
    [SerializeField] GameObject swampBorderWallTopDressing;
    [SerializeField] GameObject outerSwampWallTopDressing;
    [SerializeField] GameObject innerSwampWallTopDressing;
    [SerializeField] GameObject gorrotTownWallTopDressing;
    [SerializeField] GameObject gorrotChurchWallTopDressing;

    [Header("Small Terrain Pieces")]

    [SerializeField] GameObject[] outmostTerritoriesSmallTerrainPieces;
    [SerializeField] GameObject[] outerRingTerrainPieces;
    [SerializeField] GameObject[] innerRingTerrainPieces;
    [SerializeField] GameObject[] swampBorderTerrainPieces;
    [SerializeField] GameObject[] outerSwampTerrainPieces;
    [SerializeField] GameObject[] innerSwampTerrainPieces;
    [SerializeField] GameObject[] gorrotTownTerrainPieces;
    [SerializeField] GameObject[] gorrotChurchTerrainPieces;

    [Header("Large (Four Square) Terrain Pieces")]

    [SerializeField] GameObject[] outmostTerritoriesLargeTerrainPieces;
    [SerializeField] GameObject[] outerRingLargeTerrainPieces;
    [SerializeField] GameObject[] innerRingLargeTerrainPieces;
    [SerializeField] GameObject[] swampBorderLargeTerrainPieces;
    [SerializeField] GameObject[] outerSwampLargeTerrainPieces;
    [SerializeField] GameObject[] innerSwampLargeTerrainPieces;
    [SerializeField] GameObject[] gorrotTownLargeTerrainPieces;
    [SerializeField] GameObject[] gorrotChurchLargeTerrainPieces;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optional: persist between scenes
        DontDestroyOnLoad(gameObject);
    }

    public GameObject GetFloorSquare(string mapLocation)
    {

        GameObject result = mapLocation switch
        {
            "Outmost_Territories" => randomFromArray(outmostTerritoriesFloorSquares),
            "Outer_Ring" => randomFromArray(outerRingFloorSquares),
            "Inner_Ring" => randomFromArray(innerRingFloorSquares),
            "Swamp_Border" => randomFromArray(swampBorderFloorSquares),
            "Outer_Swamp" => randomFromArray(outerSwampFloorSquares),
            "Inner_Swamp" => randomFromArray(innerSwampFloorSquares),
            "Gorrot_Town" => randomFromArray(innerSwampFloorSquares),
            "Gorrot_Church" => randomFromArray(gorrotChurchFloorSquares),
            _ => randomFromArray(innerRingFloorSquares),
        };

        return result;
    }

    public GameObject GetWallDresings(string mapLocation)
    {
        GameObject result = mapLocation switch
        {
            "Outmost_Territories" => outmostTerritoriesWallTopDressing,
            "Outer_Ring" => outerRingWallTopDressing,
            "Inner_Ring" => innerRingWallTopDressing,
            "Swamp_Border" => swampBorderWallTopDressing,
            "Outer_Swamp" => outerSwampWallTopDressing,
            "Inner_Swamp" => innerSwampWallTopDressing,
            "Gorrot_Town" => gorrotTownWallTopDressing,
            "Gorrot_Church" => gorrotChurchWallTopDressing,
            _ => outerRingWallTopDressing,
        };

        return result;
    }

    public GameObject GetSmallTerrainPiece(string mapLocation)
    {

        GameObject result = mapLocation switch
        {
            "Outmost_Territories" => randomFromArray(outmostTerritoriesSmallTerrainPieces),
            "Outer_Ring" => randomFromArray(outerRingTerrainPieces),
            "Inner_Ring" => randomFromArray(innerRingTerrainPieces),
            "Swamp_Border" => randomFromArray(swampBorderTerrainPieces),
            "Outer_Swamp" => randomFromArray(outerSwampTerrainPieces),
            "Inner_Swamp" => randomFromArray(innerSwampTerrainPieces),
            "Gorrot_Town" => randomFromArray(gorrotChurchTerrainPieces),
            "Gorrot_Church" => randomFromArray(gorrotChurchTerrainPieces),
            _ => randomFromArray(innerRingTerrainPieces),
        };

        return result;
    }

    public GameObject GetLargeTerrainPiece(string mapLocation)
    {

        GameObject result = mapLocation switch
        {
            "Outmost_Territories" => randomFromArray(outmostTerritoriesLargeTerrainPieces),
            "Outer_Ring" => randomFromArray(outerRingLargeTerrainPieces),
            "Inner_Ring" => randomFromArray(innerRingLargeTerrainPieces),
            "Swamp_Border" => randomFromArray(swampBorderLargeTerrainPieces),
            "Outer_Swamp" => randomFromArray(outerSwampLargeTerrainPieces),
            "Inner_Swamp" => randomFromArray(innerSwampLargeTerrainPieces),
            "Gorrot_Town" => randomFromArray(gorrotTownLargeTerrainPieces),
            "Gorrot_Church" => randomFromArray(gorrotChurchLargeTerrainPieces),
            _ => randomFromArray(innerRingLargeTerrainPieces),
        };

        return result;
    }

    GameObject randomFromArray(GameObject[] array)
    {
        if (array == null || array.Length == 0)
        {
            Debug.LogWarning("Empty terrain array!");
            return null;
        }

        int randomInt = UnityEngine.Random.Range(0, array.Length);
        return array[randomInt];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
