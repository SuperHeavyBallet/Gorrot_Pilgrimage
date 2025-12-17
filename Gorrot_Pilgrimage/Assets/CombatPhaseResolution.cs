using UnityEngine;
using TMPro;
using System.Collections;

public class CombatPhaseResolution : MonoBehaviour
{

    TurnOrganiser turnOrganiser;

    public DiceController diceController;
    public TextMeshProUGUI DiceRollFormulaText;

    
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
    [SerializeField] PlayerMovementController playerMovementController;

    int requiredToWin;


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

        SquareController sq = turnOrganiser.landedSquare;

        currentEnemyDamage = sq.square switch
        {
            SquareController.squareQuantity.small => 1,
            SquareController.squareQuantity.medium => 2,
            SquareController.squareQuantity.large => 3,
            _ => 2
        };

        int baseRequiredToWin = sq.GetEnemyBaseRequiredToWin();
        requiredToWin = Mathf.Clamp(baseRequiredToWin - playerCurrentAttackBoost, 2, 6);

        UpdateDiceRollFormulaText();
    }


    IEnumerator CombatRollScreen()
    {
        CalculateDiceStats();
        Debug.Log($"EnemySize: {currentEnemyDamage}, AttackBoost: {playerCurrentAttackBoost}, RequiredToWin: {requiredToWin}");

        yield return new WaitForSeconds(0.5f);

        combatScreen.SetActive(true);
        diceDisplay.SetActive(true);



        hasPressedRoll = false;
        waitingForPressRoll = true;

        // Wait for UI button
        yield return new WaitUntil(() => hasPressedRoll);

        waitingForPressRoll = false;
        diceController.RollDice();

        // Wait for dice finish
        yield return new WaitUntil(() => !diceController.isRolling);
        yield return new WaitForSeconds(0.25f);


        result = diceController.getDiceResult();

        if (result < requiredToWin)
        {
            // Lose Results
            playerStatsController.alterHealth(currentEnemyDamage * -1);
            playerStatsController.resetSuffering();
            playerMovementController.MovePlayerBackOneSquare();
            
        }
        else
        {
            // Win Results
            playerStatsController.resetSuffering();
            audioManager.playCombatWinSoundEffect();
            turnOrganiser.landedSquare.MakeEmptySquare();
        }

        currentEnemyDamage = 0;


        CloseCombatScene();


    }



    public void PlayerPressedRoll()
    {

        if(waitingForPressRoll)
        {
            hasPressedRoll = true;
            waitingForPressRoll = false;
        }
    }


    void UpdateDiceRollFormulaText()
    {
        int requiredRoll = requiredToWin;

        string displayText;

        if (requiredRoll <= 1)
            displayText = "Auto Success";
        else if (requiredRoll >= 7)
            displayText = "Impossible Roll";
        else
            displayText = "Need " + requiredRoll + "+ to Win";

        DiceRollFormulaText.text = displayText;
    }



    void CloseCombatScene()
    {
        battlefieldBuilder.StartFadeFromBlack();
        combatScreen.SetActive(false);
        diceDisplay.SetActive(false);
        turnOrganiser.SetLandedOnEnemySquare(false, null);
        turnOrganiser.BuildNextTurn();
    }



}
