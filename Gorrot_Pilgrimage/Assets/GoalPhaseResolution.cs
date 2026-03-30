using GorrotGame;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GoalPhaseResolution : MonoBehaviour
{

    [SerializeField] GameObject goalText;
    [SerializeField] GameObject goalScreen;

    [SerializeField] TurnOrganiser turnOrganiser;

    [SerializeField] PlayerStatsController playerStatsController;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    [SerializeField] GameObject transitionScreen;
    [SerializeField] TextMeshProUGUI transitionScreenLostText;

   

    [SerializeField] GameObject succesfulTransitionGO;
    [SerializeField] GameObject lostTransitionGO;
    

    [SerializeField] TransitionMapScreenController transitionMapScreenController;

   

    [SerializeField] LevelTransitionPhaseResolution levelTransitionPhaseResolution;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goalText.SetActive(false);
        goalScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EnterGoalPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.goalReach);


      //  goalScreen.SetActive(true);
        
        StartCoroutine(ArriveAtGoal());

    }

    IEnumerator ArriveAtGoal()
    {
        

        battlefieldBuilder.StartFadeToBlack();

        
        yield return new WaitForSeconds(1);
        
        playerStatsController.resetSuffering();
        //battlefieldBuilder.BuildNewBattlefield();

        levelTransitionPhaseResolution.EnterLevelTransitionPhase();

       // transitionMapScreenController.StartMapTransition(currentMap, nextMap);
        


       // yield return new WaitForSeconds(2);

       // goalScreen.SetActive(false);
    
    }
}
