using UnityEngine;
using TMPro;
using System.Collections;

public class CombatPhaseResolution : MonoBehaviour
{

    TurnOrganiser turnOrganiser;

    public DiceController diceController;
    public TextMeshProUGUI DiceRollFormulaText;
    Coroutine waitForDiceRoll;

    int enemyRollToBeat;
    int playerCurrentAttackBoost;

    public PlayerStatsController playerStatsController;

    int currentEnemyDamage;

    public AudioManager audioManager;

    bool currentlyRolling;

    int result;

    bool waitingForPressRoll;
    bool hasPressedRoll;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnOrganiser = GetComponent<TurnOrganiser>();
    }

    public void EnterCombatPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.combat);
        turnOrganiser.combatScene.SetActive(true);
        turnOrganiser.diceDisplay.SetActive(true);

        StartCoroutine(CombatRollScreen());
    }




    IEnumerator CombatRollScreen()
    {
        yield return new WaitForSeconds(0.5f);

        playerCurrentAttackBoost = playerStatsController.getCurrentAttackBuff();
        currentEnemyDamage = turnOrganiser.GetEnemySize();

        switch (currentEnemyDamage)
        {
            case 1: enemyRollToBeat = 2 - playerCurrentAttackBoost; break;
            case 2: enemyRollToBeat = 3 - playerCurrentAttackBoost; break;
            case 3: enemyRollToBeat = 4 - playerCurrentAttackBoost; break;
            default: enemyRollToBeat = 3 - playerCurrentAttackBoost; break;
        }

        UpdateDiceRollFormulaText();

        hasPressedRoll = false;
        waitingForPressRoll = true;

        // Wait for UI button
        yield return new WaitUntil(() => hasPressedRoll);

        waitingForPressRoll = false;

        diceController.RollDice();

        // Wait for dice finish
        yield return new WaitUntil(() => !diceController.isRolling);
        result = diceController.getDiceResult();

        if (result <= enemyRollToBeat)
        {
            playerStatsController.alterHealth(currentEnemyDamage * -1);
            playerStatsController.resetSuffering();
        }
        else
        {
            playerStatsController.resetSuffering();
            audioManager.playCombatWinSoundEffect();
        }

        currentEnemyDamage = 0;
        enemyRollToBeat = 2;

        CloseCombatScene();


    }



    public void PlayerPressedRoll()
    {
        Debug.Log("PLAYER PRESSED ROLL");

        if(waitingForPressRoll)
        {
            hasPressedRoll = true;
            waitingForPressRoll = false;
        }
    }


    void UpdateDiceRollFormulaText()
    {

        DiceRollFormulaText.text = "1D6 + " + playerCurrentAttackBoost + " vs " + enemyRollToBeat;
    }


    void CloseCombatScene()
    {

        turnOrganiser.combatScene.SetActive(false);
        turnOrganiser.diceDisplay.SetActive(false);
        turnOrganiser.SetLandedOnEnemySquare(false);
        turnOrganiser.BuildNextTurn();
    }



}
