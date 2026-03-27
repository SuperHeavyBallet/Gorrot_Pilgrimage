using UnityEngine;

public class SmallTerrainController : MonoBehaviour
{
    [SerializeField] GameObject smallTerrainContainer;

    private void Awake()
    {
        smallTerrainContainer.SetActive(false);
        
    }
    public void SetSmallTerrain(MapData thisSquareMapData)
    {
        DeleteAllChildren(smallTerrainContainer.transform);

        GameObject newObject = MapAssetsController.Instance.GetSmallTerrainPiece(thisSquareMapData.GetMapLocation());

        GameObject smallTerrainPrefab = Instantiate(newObject, smallTerrainContainer.transform);
        smallTerrainContainer.SetActive(true);
    }
    void DeleteAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
