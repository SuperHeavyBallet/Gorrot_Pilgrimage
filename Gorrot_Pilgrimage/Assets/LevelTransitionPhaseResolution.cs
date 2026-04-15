using System.Collections;
using UnityEngine;
using GorrotGame;
using TMPro;

public class LevelTransitionPhaseResolution : MonoBehaviour
{
    [SerializeField] TurnOrganiser turnOrganiser;
    [SerializeField] BattlefieldBuilder battlefieldBuilder;
    [SerializeField] GameObject transitionMapScreen;
    [SerializeField] TransitionMapScreenController transitionMapScreenController;

    bool isLost = false;
    MapNames currentMap;
    MapNames nextMap;

    [SerializeField] TextMeshProUGUI transitionText;

    public bool canStartNextLevel;

    public void EnterLevelTransitionPhase()
    {
        canStartNextLevel = false;

        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.levelTransition);
        transitionMapScreen.SetActive(true);
        StartCoroutine(BuildNewBattleField()); 
    }


    public void SetTransitionData(bool lostValue, MapData leavingMap, MapData goingToMap)
    {

        isLost = lostValue;

        MapNames leavingMapName = leavingMap.GetMapNames();
        MapNames goingToMapName = goingToMap.GetMapNames();

        currentMap = leavingMapName;
        nextMap = goingToMapName;

        if (isLost)
        {

            transitionText.text = "You lost your way and remain in " + leavingMapName.ToString() + ".";
        }
        else
        {
            transitionText.text = "You Move from " + leavingMapName.ToString() + " to " + goingToMapName.ToString() + ".";
        }



    }

    IEnumerator BuildNewBattleField()
    {
        battlefieldBuilder.PrepareNextMapToBuild();
        transitionMapScreenController.StartMapTransition(battlefieldBuilder.CurrentMapNames, battlefieldBuilder.NextMapNames);
        battlefieldBuilder.BuildNewBattlefield();
        

       
        yield return new WaitForSeconds(1);

        canStartNextLevel = true;

       
    }

    void ExitLevelTransitionPhase()
    {
        transitionMapScreen.SetActive(false);
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.movement);
    }

    public void OnClickStartNextLevel()
    {
        if(canStartNextLevel)
        {
            ExitLevelTransitionPhase();
        }
    }
}
