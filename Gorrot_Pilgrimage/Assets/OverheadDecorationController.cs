using UnityEngine;

public class OverheadDecorationController : MonoBehaviour
{

    [SerializeField] Transform spawnNextPosition;

    public Transform GetSpawnNextPosition => spawnNextPosition;
    [SerializeField] GameObject underhangObject;
    [SerializeField] GameObject spanSupportMesh;

    MapData thisMap;

    bool isFirstOrLast;

    private void Awake()
    {
        spanSupportMesh.SetActive(false);
        underhangObject.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       

      
       
        
    }

    public void SetOverheadDecoration()
    {
        Debug.Log("HANGING OBJECT: " + thisMap.GetMapName());
        if (isFirstOrLast == true)
        {
            DisplaySpanSupportMesh();
        }
        else
        {
            int rng = UnityEngine.Random.Range(0, 2);
            if (rng == 0)
            {
                DisplayUnderHangObject();
            }
        }
    }


    public void SetIsFirstOrLast(bool value) => isFirstOrLast = value;

    public void SetThisMapData(MapData newMap) => thisMap = newMap;

    void DisplayUnderHangObject()
    {
        underhangObject.SetActive (true);
    }

    void DisplaySpanSupportMesh()
    {
        spanSupportMesh.SetActive (true);
    }
}
