using System.Collections.Generic;
using UnityEngine;

public class OverHeadSpansBuilder : MonoBehaviour
{
    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    

    public void CreateOverheadSpanDecorations()
    {
        /*
        HashSet<int> chosenPositions = new HashSet<int>();

        // Generate a random number of total spans within the total amount of available borders per map
        // Example: total possible = 10, chosen amount for this map = 5 => 5 used, 5 free, but unspecified which yet
        int numberOfSpans = UnityEngine.Random.Range(0, battlefieldBuilder.ThisMap.GetMapSize());


        // For that total amount selected, choose random positions within the available borders, then add that to the hashset to prevent duplication
        for (int i = 0; i < numberOfSpans; i++)
        {
            int randomBorderPosition = UnityEngine.Random.Range(0, battlefieldBuilder.BorderSquares.Count);

            if (!chosenPositions.Contains(randomBorderPosition))
            {
                chosenPositions.Add(randomBorderPosition);

                var square = battlefieldBuilder.BorderSquares[randomBorderPosition];

                BorderOverHeadDecorationController borderOHController = square.obj.GetComponent<BorderOverHeadDecorationController>();

                if (borderOHController != null && battlefieldBuilder.RowFreeForOverhead(randomBorderPosition))
                {
                    // Send the signal for this starting border to begin a span, use +2 to account for starting and ending border as well as actual map width
                    borderOHController.SpawnChainOverhead(
                        battlefieldBuilder.ThisMap.GetMapSize() + 2, 
                       // square.edge, 
                        battlefieldBuilder.ThisMap);
                }

            }
        }*/
    }
}
