using System.Collections;
using UnityEngine;

public class GoalPhaseResolution : MonoBehaviour
{

    [SerializeField] GameObject goalText;
    [SerializeField] GameObject goalScreen;

    [SerializeField] TurnOrganiser turnOrganiser;

    [SerializeField] PlayerStatsController playerStatsController;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;


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

        goalText.SetActive(true);
        goalScreen.SetActive(true);

        StartCoroutine(ArriveAtGoal());

    }

    IEnumerator ArriveAtGoal()
    {
        playerStatsController.resetSuffering();

        battlefieldBuilder.StartFadeToBlack();
        yield return new WaitForSeconds(2);

        battlefieldBuilder.BuildNewBattlefield();
        goalText.SetActive(false);
        goalScreen.SetActive(false);
    }
}
