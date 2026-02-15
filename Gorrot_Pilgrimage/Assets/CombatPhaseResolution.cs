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
    int enemyCurrentAttackBoost;

    public PlayerStatsController playerStatsController;

    int currentEnemyDamage;

    public AudioManager audioManager;

    int currentPlayerRoll;
    int enemyResult;

    [SerializeField] TextMeshProUGUI playerTotal;
    [SerializeField] TextMeshProUGUI enemyTotal;

    private int currentEnemyRoll = -1;
    private int currentEnemyBuff = -1;

    bool waitingForPressRoll;

    [SerializeField] GameObject diceDisplay;
    [SerializeField]  GameObject enemyDiceDisplay;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;
    [SerializeField] PlayerMovementController playerMovementController;


    [SerializeField] TextMeshProUGUI payButtonText;

    public enum CombatChoice { None, Roll, Pay, Talk, Flee }

    CombatChoice choice = CombatChoice.None;
    Coroutine combatRoll;
    Coroutine enemyCombatRoll;

    MapData currentMap;
    int bribeMultiplier;
    int thisCombatBribeMultiplerMax = 1;
    int totalBribeAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        diceDisplay.SetActive(false);
        enemyDiceDisplay.SetActive(false);
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

        diceDisplay.SetActive(true);
        enemyDiceDisplay.SetActive(true);

        SetupEnemyStart();
    }

    void SetupEnemyStart()
    {
        CalculateThisBribe();

        SquareController sq = turnOrganiser.GetLandedSquare();
        currentEnemyBuff = sq.GetEnemyBaseBuff();

        UpdateDiceRollFormulaText(currentEnemyBuff, 0, -1);
    }

    IEnumerator EnemyRollRoutine()
    {
        

        


        enemyDiceController.RollDice();

        // Wait for dice finish
        yield return new WaitUntil(() => !enemyDiceController.isRolling);
        yield return new WaitForSeconds(0.25f);


        currentEnemyRoll = enemyDiceController.getDiceResult();

        UpdateDiceRollFormulaText(currentEnemyBuff, currentEnemyRoll, -1);


        // Now Hand Over To Player
        StartCombatRoutine();
    }


    /*
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
    }*/

    
    public void UpdateCombatRoll()
    {
        if (!waitingForPressRoll) return;
        UpdateDiceRollFormulaText(currentEnemyBuff, currentEnemyRoll, -1);
    }

    void StartCombatRoutine()
    {


        if (combatRoll != null)
        {
            StopCoroutine(combatRoll);
        }
        combatRoll = StartCoroutine(CombatRollScreen());
    }

    void CalculateThisBribe()
    {
        if (currentMap.GetCanBeBribed() == true)
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

    }


    IEnumerator CombatRollScreen()
    {

       yield return new WaitForSeconds(0.5f);

        waitingForPressRoll = true;
        choice = CombatChoice.None;

        // Wait for UI button Press
        yield return new WaitUntil(() => choice != CombatChoice.None);
        waitingForPressRoll = false;



        if (choice == CombatChoice.Pay)
        {
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
        else if (choice == CombatChoice.Talk)
        {

            Debug.Log("Pressed Talk");

            audioManager.PlayPayOffChuckle();

            CloseCombatScene();
            combatRoll = null;

            yield break;
        }
        else
        {
            diceController.RollDice();

            yield return new WaitUntil(() => !diceController.isRolling);
            yield return new WaitForSeconds(0.25f);

            ResolveCombat();

        }
            



    }

    void ResolveCombat()
    {
        currentPlayerRoll = diceController.getDiceResult();

        UpdateDiceRollFormulaText(currentEnemyBuff, currentEnemyRoll, currentPlayerRoll);

        int playerTotalScore = currentPlayerRoll + playerCurrentAttackBoost;
        int enemyTotalScore = currentEnemyRoll + currentEnemyBuff; // use cached buff

        bool playerWins = playerTotalScore >= enemyTotalScore; // ties win

        if (!playerWins)
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

    public void PlayerPressedPay()
    {
     //   if (!waitingForPressRoll) return;
        if (playerStatsController.GetPlayerCurrentMoney() >= totalBribeAmount)
        { 
            choice = CombatChoice.Pay;
        }
           
    }

    public void PlayerPressedFlee()
    {
      //  if (!waitingForPressRoll) return;
        choice = CombatChoice.Flee;
    }

    public void PlayerPressedRoll()
    {

       // if (!waitingForPressRoll) return;

       
            choice = CombatChoice.Roll;

        
    }

    public void PlayerPressedTalk()
    {
       // if (!waitingForPressRoll) return;


        choice = CombatChoice.Talk;
    }

    public void PlayerPressedFight()
    {
        Debug.Log("Player Pressed Fight");

        if (enemyCombatRoll != null)
        {
            StopCoroutine(enemyCombatRoll);
        }

        currentEnemyRoll = -1;
        currentEnemyBuff = -1;

        enemyCombatRoll = StartCoroutine(EnemyRollRoutine());

    }


    void UpdateDiceRollFormulaText(int enemyBuff, int enemyRoll, int playerRoll)
    {
        playerCurrentAttackBoost = playerStatsController.getCurrentAttackBuff();

        // Enemy info / required roll text (only if we have enemy values)
        if (enemyRoll != -1 && enemyBuff != -1)
        {
            int enemyTotalScore = enemyRoll + enemyBuff;
            int requiredPlayerRoll = enemyTotalScore - playerCurrentAttackBoost; // roll needed so that roll+boost >= enemyTotal

            // Clamp / readability
            if (requiredPlayerRoll <= 1)
            {
                DiceRollFormulaText.text = "1+ to Win"; // or "Any roll wins" if you allow auto-win vibes
            }
            else if (requiredPlayerRoll > 6)
            {
                DiceRollFormulaText.text = "Impossible"; // or "7+ to Win" to keep numeric style
            }
            else
            {
                DiceRollFormulaText.text = requiredPlayerRoll.ToString() + "+ to Win";
            }

            enemyTotal.text = enemyRoll.ToString() + " + " + enemyBuff.ToString();
        }

        // Player breakdown text
        string preparedPlayerString = "0 + " + playerCurrentAttackBoost;

        if (playerRoll != -1)
        {
            preparedPlayerString = playerRoll.ToString() + " + " + playerCurrentAttackBoost;
        }

        playerTotal.text = preparedPlayerString;
    }

    



    void CloseCombatScene()
    {

        battlefieldBuilder.StartFadeFromBlack();
        //combatScreen.SetActive(false);
        diceDisplay.SetActive(false);
        enemyDiceDisplay.SetActive(false);
        currentEnemyRoll = -1;
        currentEnemyBuff = -1;
        turnOrganiser.SetLandedOnEnemySquare(false, null);
        turnOrganiser.BuildNextTurn();

    }



}
