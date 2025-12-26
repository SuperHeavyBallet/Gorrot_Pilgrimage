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

    bool isLost = false;

    [SerializeField] GameObject succesfulTransitionGO;
    [SerializeField] GameObject lostTransitionGO;
    [SerializeField] TextMeshProUGUI transitionText;


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

    public void SetTransitionData(bool lostValue, MapData leavingMap, MapData goingToMap)
    {

        isLost = lostValue;

        string leavingMapName = leavingMap.GetMapName();
        string goingToMapName = goingToMap.GetMapName();

        if(isLost )
        {

            transitionText.text = "You lost your way and remain in " + leavingMapName + ".";
        }
        else
        {
            transitionText.text = "You Move from " + leavingMapName + " to " + goingToMapName + ".";
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
