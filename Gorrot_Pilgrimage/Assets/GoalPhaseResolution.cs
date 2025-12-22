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

    public void SetLostStatus(bool value, MapData currentMap, MapData nextMap)
    {

        isLost = value;

        string currentMapName = currentMap.GetMapName();
        string nextMapName = nextMap.GetMapName();

        if(isLost )
        {

            transitionText.text = "You lost your way and remain in " + currentMapName + ".";
        }
        else
        {
            transitionText.text = "You Move from " + currentMapName + " to " + nextMapName + ".";
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
