using System.Collections;
using TMPro;
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

    public void GetLostStatus(bool value)
    {
        if(value == true)
        {
            Debug.Log("PLAYER ESCAPED!");
            transitionScreenLostText.text = "You Escaped...";
        }
        else
        {
            transitionScreenLostText.text = "You Are Lost...";
        }
    }

    public void EnterGoalPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.goalReach);

        goalText.SetActive(true);
        goalScreen.SetActive(true);
        transitionScreen.SetActive(true);
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
