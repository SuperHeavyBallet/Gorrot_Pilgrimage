using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class CombatPhaseResolution : MonoBehaviour
{

    [SerializeField] TurnOrganiser turnOrganiser;

    public DiceController diceController;
    public DiceController enemyDiceController;
    public TextMeshProUGUI DiceRollFormulaText;

    
    int playerCurrentAttackBoost;

    public PlayerStatsController playerStatsController;

    int currentEnemyDamage;

    public AudioManager audioManager;

    int result;

    bool waitingForPressRoll;
    bool hasPressedRoll;
    bool hasPressedPay;

    //public GameObject combatScreen;
    [SerializeField] GameObject diceDisplay;
    [SerializeField]  GameObject enemyDiceDisplay;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;
    [SerializeField] PlayerMovementController playerMovementController;

    int requiredToWin;

    [SerializeField] TextMeshProUGUI payButtonText;

    public enum CombatChoice { None, Roll, Pay, Flee }

    CombatChoice choice = CombatChoice.None;
    Coroutine combatRoll;

    MapData currentMap;
    int bribeMultiplier;
    int thisCombatBribeMultiplerMax = 1;
    int totalBribeAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
       // combatScreen.SetActive(false);
        diceDisplay.SetActive(false);
        enemyDiceDisplay.SetActive(false);
    }

    public void RollEnemyDice()
    {
        enemyDiceController.RollDice();

    }

    public void EnterCombatPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.combat);

        battlefieldBuilder.StartFadeToBlack();
        currentMap = battlefieldBuilder.GetThisMap();
        if(currentMap != null)
        {
            bribeMultiplier = currentMap.GetBribeMultiplier();
        }
        else
        {
            bribeMultiplier = 1;
        }

        RollEnemyDice();
            StartCombatRoutine();
    }



    void CalculateDiceStats()
    {
        playerCurrentAttackBoost = playerStatsController.getCurrentAttackBuff();

        SquareController sq = turnOrganiser.GetLandedSquare();

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

    public void UpdateCombatRoll()
    {
        if (!waitingForPressRoll) return;
        StartCombatRoutine();
    }

    void StartCombatRoutine()
    {
        if (combatRoll != null)
        {
            StopCoroutine(combatRoll);
        }
        combatRoll = StartCoroutine(CombatRollScreen());
    }


    IEnumerator CombatRollScreen()
    {
        CalculateDiceStats();

        if(currentMap.GetCanBeBribed() == true)
        {
            thisCombatBribeMultiplerMax = UnityEngine.Random.Range(1, currentEnemyDamage * 3);
            totalBribeAmount = thisCombatBribeMultiplerMax * bribeMultiplier;

            payButtonText.text = "Pay: " + (totalBribeAmount).ToString();
        }
        else
        {
            totalBribeAmount = 9999;
            payButtonText.text = "No Pay.";
        }


            yield return new WaitForSeconds(0.5f);

        //combatScreen.SetActive(true);
        diceDisplay.SetActive(true);
        enemyDiceDisplay.SetActive(true);




        waitingForPressRoll = true;
        choice = CombatChoice.None;

        // Wait for UI button
        yield return new WaitUntil(() => choice != CombatChoice.None);

        waitingForPressRoll = false;

        if (choice == CombatChoice.Pay)
        {
            // resolve pay route
            
            playerStatsController.AlterMoney((totalBribeAmount) * -1);
            playerStatsController.alterSuffering(currentEnemyDamage * -1);
            audioManager.PlayPayOffChuckle();

            CloseCombatScene();
            combatRoll = null;

            yield break;
        }
        else if(choice == CombatChoice.Flee)
        {
            playerMovementController.MovePlayerBackOneSquare();
            playerStatsController.alterSuffering(1);
            CloseCombatScene();
            combatRoll = null;

            yield break;
        }
        else
        {
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
                playerStatsController.AlterMoney(10);
                audioManager.playCombatWinSoundEffect();
                turnOrganiser.GetLandedSquare().MakeEmptySquare();
            }

            currentEnemyDamage = 0;

            combatRoll = null;
            CloseCombatScene();

        }
            



    }

    public void PlayerPressedPay()
    {
        if (!waitingForPressRoll) return;
        if (playerStatsController.GetPlayerCurrentMoney() >= totalBribeAmount)
        { 
            choice = CombatChoice.Pay;
        }
           
    }

    public void PlayerPressedFlee()
    {
        if (!waitingForPressRoll) return;
        choice = CombatChoice.Flee;
    }

    public void PlayerPressedRoll()
    {

        if (!waitingForPressRoll) return;

       
            choice = CombatChoice.Roll;

        
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
            displayText = requiredRoll + "+ to Win";

        DiceRollFormulaText.text = displayText;
    }



    void CloseCombatScene()
    {
        battlefieldBuilder.StartFadeFromBlack();
        //combatScreen.SetActive(false);
        diceDisplay.SetActive(false);
        enemyDiceDisplay.SetActive(false);
        turnOrganiser.SetLandedOnEnemySquare(false, null);
        turnOrganiser.BuildNextTurn();
    }



}
