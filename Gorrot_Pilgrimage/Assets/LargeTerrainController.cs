using UnityEngine;

public class LargeTerrainController : MonoBehaviour
{
    [SerializeField] GameObject fourSquareTerrainContainer;

    public void SetFourBlockTerrainSprite(MapData thisSquareMapData)
    {
        DeleteAllChildren(fourSquareTerrainContainer.transform);
        GameObject fourSquarePrefab = Instantiate(thisSquareMapData.GetFourSquareMesh(), fourSquareTerrainContainer.transform);

    }

    void DeleteAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}

