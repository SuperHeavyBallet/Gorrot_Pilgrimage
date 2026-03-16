using UnityEngine;

public class LargeTerrainController : MonoBehaviour
{

    [SerializeField] GameObject largeTerrainContainer;

    public void SetFourBlockTerrainSprite(MapData thisSquareMapData)
    {
        DeleteAllChildren(largeTerrainContainer.transform);
        GameObject fourSquarePrefab = Instantiate(thisSquareMapData.GetFourSquareMesh(), largeTerrainContainer.transform);

    }

    void DeleteAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}

