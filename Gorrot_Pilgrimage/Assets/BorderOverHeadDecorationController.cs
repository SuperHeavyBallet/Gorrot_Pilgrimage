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
                    Debug.LogError("Default Span Fallback");
                }
                    
                OverheadDecorationController controller = newOverhead.GetComponent<OverheadDecorationController>();

                if(controller != null )
                {
                    controller.SetThisMapData(thisMap);
                    controller.SetIsFirstOrLast(isFirstOrLast);
                    controller.SetOverheadDecoration();
                   // controller.SetSagRotation(i, spanAmount);
                    spawnNextPosition = controller.GetSpawnNextPosition;
                }
            }
        }
    }
}
