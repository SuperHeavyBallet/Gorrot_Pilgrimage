using UnityEngine;
using GorrotGame;

public class BorderOverHeadDecorationController : MonoBehaviour
{
    [SerializeField] GameObject overHeadDecoration;
    [SerializeField] Transform overHeadStartPos;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SpawnChainOverhead(int spanAmount, MapData thisMap)
    {
        

        Vector2 spanDirection = Vector2.zero;
        Transform spawnNextPosition = overHeadStartPos;

        /*
        switch (startEdge)
        {
            case OrthogonalPositions.North:
                spanDirection = Vector2.down;
                spawnNextPosition.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;

            case OrthogonalPositions.South:
                spanDirection = Vector2.up;
                spawnNextPosition.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;

            case OrthogonalPositions.East:
                spanDirection = Vector2.left;
                spawnNextPosition.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;

            case OrthogonalPositions.West:
                spanDirection = Vector2.right;
                spawnNextPosition.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;

            default:
                spanDirection = Vector2.zero;
                break;
        }*/






        if (overHeadDecoration != null)
        {
            for(int i = 0; i < spanAmount; i++)
            {
                bool isFirstOrLast = false;

                if (i == 0 || i == spanAmount-1)
                {
                    isFirstOrLast = true;
                }

                GameObject thisMapOverheadSpanObject = thisMap.OverheadSpanObject;
                GameObject newOverhead = null;

                if (thisMapOverheadSpanObject != null)
                {
                    newOverhead = Instantiate(thisMapOverheadSpanObject, spawnNextPosition);
                }
                else
                {
                    newOverhead = Instantiate(overHeadDecoration, spawnNextPosition);
                }
                    
                OverheadDecorationController controller = newOverhead.GetComponent<OverheadDecorationController>();

                if(controller != null )
                {
                    controller.SetThisMapData(thisMap);
                    controller.SetIsFirstOrLast(isFirstOrLast);
                    controller.SetOverheadDecoration();
                    spawnNextPosition = controller.GetSpawnNextPosition;
                }
            }
        }
    }
}
