using UnityEngine;
using TMPro;
using System.Collections;

public class CombatPhaseResolution : MonoBehaviour
{

    TurnOrganiser turnOrganiser;

    public DiceController diceController;
    public TextMeshProUGUI DiceRollFormulaText;

    int enemyRollToBeat;
    int playerCurrentAttackBoost;

    public PlayerStatsController playerStatsController;

    int currentEnemyDamage;

    public AudioManager audioManager;

    int result;

    bool waitingForPressRoll;
    bool hasPressedRoll;

    public GameObject combatScreen;
    public GameObject diceDisplay;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        turnOrganiser = GetComponent<TurnOrganiser>();
        combatScreen.SetActive(false);
        diceDisplay.SetActive(false);
    }

    public void EnterCombatPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.combat);

        battlefieldBuilder.StartFadeToBlack();

        

        StartCoroutine(CombatRollScreen());
    }



    void CalculateDiceStats()
    {
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
    }


    IEnumerator CombatRollScreen()
    {
        CalculateDiceStats();

        yield return new WaitForSeconds(0.5f);

        combatScreen.SetActive(true);
        diceDisplay.SetActive(true);



        hasPressedRoll = false;
        waitingForPressRoll = true;

        // Wait for UI button
        yield return new WaitUntil(() => hasPressedRoll);

        waitingForPressRoll = false;
        diceController.RollDice();
        

        yield return new WaitForSeconds(3f);

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

        //DiceRollFormulaText.text = "1D6 + " + playerCurrentAttackBoost + " vs " + enemyRollToBeat;

        int requiredRoll = enemyRollToBeat - playerCurrentAttackBoost + 1;

        string displayText;

        if (requiredRoll <= 1)
        {
            displayText = "Auto Success";
        }
        else if (requiredRoll >= 7)
        {
            displayText = "Impossible Roll";
        }
        else
        {
            displayText = "Need " + requiredRoll + "+ to Win";
        }

        DiceRollFormulaText.text = displayText;
    }


    void CloseCombatScene()
    {
        battlefieldBuilder.StartFadeFromBlack();
        combatScreen.SetActive(false);
        diceDisplay.SetActive(false);
        turnOrganiser.SetLandedOnEnemySquare(false);
        turnOrganiser.BuildNextTurn();
    }



}
