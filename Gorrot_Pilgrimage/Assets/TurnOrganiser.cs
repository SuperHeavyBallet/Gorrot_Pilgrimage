using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections;

public class TurnOrganiser : MonoBehaviour
{

    public bool isPlayerTurn;
    int playerTurnCooldownTime = 1;

    public AudioManager audioManager;

    public TextMeshProUGUI turnDisplay;

    public bool hasLandedOnEnemy = false;


    public DiceController diceController;

    Coroutine waitForDiceRoll;

    public PlayerStatsController playerStatsController;
    int currentEnemyDamage;
    int enemyRollToBeat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enablePlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildContestPhase()
    {
        Debug.Log("Build Contest Phase");

        if (hasLandedOnEnemy)
        {
            RollDiceContest();
        }
        else
        {
            Invoke("enablePlayerTurn", playerTurnCooldownTime);
        }
    }

    public void disablePlayerTurn()
    {
        audioManager.changeTurnSound("enemy");
        isPlayerTurn = false;
        turnDisplay.text = "Turn: Contest";

        BuildContestPhase();


        
    }

    public void landedOnEnemySquare(int squareQuantity)
    {
        currentEnemyDamage = squareQuantity;
        switch(squareQuantity)
        {
            case 1:
                enemyRollToBeat = 2;
                break;

            case 2:
                enemyRollToBeat = 3;
                break;

            case 3:
                enemyRollToBeat = 4;
                break;
            default:
                enemyRollToBeat = 3;
                break;
        }
        hasLandedOnEnemy = true;
    }

    public void enablePlayerTurn()
    {
        audioManager.changeTurnSound("player");
        isPlayerTurn = true;
        hasLandedOnEnemy = false;
        turnDisplay.text = "Turn: Player";
        
    }

    public bool GetPlayerTurn()
    {
        return isPlayerTurn;
    }

    public void RollDiceContest()
    {
        if (waitForDiceRoll != null)
            StopCoroutine(waitForDiceRoll);

        waitForDiceRoll = StartCoroutine(RollDiceContestRoutine());
    }

    private IEnumerator RollDiceContestRoutine()
    {
        diceController.RollDice();

        // Wait until the dice has settled
        yield return new WaitUntil(() => !diceController.isRolling);

        int result = diceController.getDiceResult();
        Debug.Log("Final contest result = " + result);

        if(result <= enemyRollToBeat)
        {
            playerStatsController.subtractHealth(currentEnemyDamage);
            playerStatsController.resetSuffering();
        }
        else
        {
            playerStatsController.resetSuffering();
        }

        currentEnemyDamage = 0;
        enemyRollToBeat = 2;
        StopCoroutine(waitForDiceRoll);

            // Now continue the flow of your turn system here
            enablePlayerTurn();   // or whatever the next step is
    }
}
