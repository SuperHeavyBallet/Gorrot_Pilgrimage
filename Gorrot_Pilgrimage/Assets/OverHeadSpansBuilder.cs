using System.Collections.Generic;
using UnityEngine;

public class OverHeadSpansBuilder : MonoBehaviour
{
    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    

    public void CreateOverheadSpanDecorations()
    {
        // This gives the square length of the side to start spans from
        int borderHeight = battlefieldBuilder.ThisMap.GetMapSize();

        HashSet<int> chosenPositions = new HashSet<int>();

        int numberOfSpans = UnityEngine.Random.Range(0, borderHeight);



        for (int i = 0; i < numberOfSpans; i++)
        {
            int randomBorderPosition = UnityEngine.Random.Range(0, battlefieldBuilder.BorderSquares.Count);

            if (!chosenPositions.Contains(randomBorderPosition))
            {
                chosenPositions.Add(randomBorderPosition);

                var square = battlefieldBuilder.BorderSquares[randomBorderPosition];

                BorderOverHeadDecorationController bc =
                    square.obj.GetComponent<BorderOverHeadDecorationController>();

                if (bc != null)
                {
                 
                    bc.SpawnChainOverhead(battlefieldBuilder.ThisMap.GetMapSize() + 2, square.edge, battlefieldBuilder.ThisMap);
                }

            }
        }
    }
}
