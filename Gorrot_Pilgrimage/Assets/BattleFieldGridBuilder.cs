using UnityEngine;

public class BattleFieldGridBuilder : MonoBehaviour
{
    MapData chosenMap = null;
    MapData mapToBuild = null;
    MapData previousMap;
    MapData thisMap;

    bool isLost;

    [SerializeField] MapCatalogue mapCatalogue;
    [SerializeField] PlayerStatReceiver playerStatReceiver;
    [SerializeField] GoalPhaseResolution goalPhaseResolution;

    [SerializeField] PlayerMovementController playerMovementController;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void BuildNewGrid()
    {
        playerMovementController.PrepareForMapRebuild();
        chosenMap = GetMapToBuild();

        battlefieldBuilder.SetThisMap(chosenMap);
        thisMap.ParseDialogue();
        battlefieldBuilder.UpdateMapDataUI();
        battlefieldBuilder.CheckMapisWild();
        
    }

    MapData GetMapToBuild()
    {
        mapToBuild = null;
        isLost = false;

        if(previousMap == null)
        {
            mapToBuild = GetFirstMap();
        }
        else if (previousMap.GetIsWildMap())
        {
            mapToBuild = CalculateLostOrProgress();
        }
        else if (previousMap.GetIsFirstMap())
        {
            mapToBuild = previousMap.GetStartingMap(playerStatReceiver.GetPlayerStartingLocation());
        }
        else //Otherwise, proceed as standard, the mapToBuild is the previousMaps > NextMap
        {
            //canAdvanceDifficulty = true;
            mapToBuild = previousMap.RollNextMap();
        }

        goalPhaseResolution.SetTransitionData(isLost, previousMap, mapToBuild);

        return mapToBuild;

    }

    MapData GetFirstMap()
    {
        return mapCatalogue.GetFirstMap();
    }

    MapData CalculateLostOrProgress()
    {

        MapData chosenMap = null;

        float escapeChance = previousMap.GetEscapeChance();
        bool escaped = UnityEngine.Random.value < escapeChance;

        if (!escaped)
        {
            isLost = true;
            //  canAdvanceDifficulty = false;
            chosenMap = previousMap; // repeat
        }
        else
        {
            chosenMap = previousMap.RollNextMap();
        }

        return chosenMap;
    }
}
