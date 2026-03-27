using UnityEngine;

public class LargeTerrainController : MonoBehaviour
{

    [SerializeField] GameObject largeTerrainContainer;

    public void SetFourBlockTerrainSprite(MapData thisSquareMapData)
    {
        DeleteAllChildren(largeTerrainContainer.transform);
        GameObject largeTerrainPrefab = MapAssetsController.Instance.GetLargeTerrainPiece(thisSquareMapData.GetMapLocation());
        GameObject fourSquarePrefab = Instantiate(largeTerrainPrefab, largeTerrainContainer.transform);

    }

    void DeleteAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}

