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

    public TextMeshProUGUI DiceRollFormulaText;

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
        int playerCurrentAttackBost = playerStatsController.getCurrentAttackBuff();

        switch(squareQuantity)
        {
            case 1:
                enemyRollToBeat = 2 - playerCurrentAttackBost;
                break;

            case 2:
                enemyRollToBeat = 3 - playerCurrentAttackBost;
                break;

            case 3:
                enemyRollToBeat = 4 - playerCurrentAttackBost;
                break;
            default:
                enemyRollToBeat = 3 - playerCurrentAttackBost;
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

        UpdateDiceRollFormulaText();

        waitForDiceRoll = StartCoroutine(RollDiceContestRoutine());
    }

    void UpdateDiceRollFormulaText()
    {
        int playerAttackBuff = playerStatsController.getCurrentAttackBuff();

        DiceRollFormulaText.text = "1D6 + " + playerAttackBuff + " vs " + enemyRollToBeat; 
    }

    private IEnumerator RollDiceContestRoutine()
    {
        diceController.RollDice();

        // Wait until the dice has settled
        yield return new WaitUntil(() => !diceController.isRolling);

        int result = diceController.getDiceResult();

        if(result <= enemyRollToBeat)
        {
            playerStatsController.alterHealth(currentEnemyDamage * -1);
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
