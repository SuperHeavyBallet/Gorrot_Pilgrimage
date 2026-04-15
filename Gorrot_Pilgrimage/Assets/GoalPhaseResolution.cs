using GorrotGame;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GoalPhaseResolution : MonoBehaviour
{

    //[SerializeField] GameObject goalText;
    //[SerializeField] GameObject goalScreen;
    [SerializeField] TurnOrganiser turnOrganiser;
    [SerializeField] PlayerStatsController playerStatsController;
    [SerializeField] BattlefieldBuilder battlefieldBuilder;
    [SerializeField] GameObject transitionScreen;
    [SerializeField] GameObject lostTransitionGO;
    [SerializeField] TransitionMapScreenController transitionMapScreenController;
    [SerializeField] LevelTransitionPhaseResolution levelTransitionPhaseResolution;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //goalText.SetActive(false);
      //  goalScreen.SetActive(false);
    }


    public void EnterGoalPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.goalReach);
        StartCoroutine(ArriveAtGoal());

    }

    IEnumerator ArriveAtGoal()
    {
        battlefieldBuilder.StartFadeToBlack();
        yield return new WaitForSeconds(1);       
        playerStatsController.resetSuffering();
        levelTransitionPhaseResolution.EnterLevelTransitionPhase();
    }
}
