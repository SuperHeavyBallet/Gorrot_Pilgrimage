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

    //public AudioManager audioManager;

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

    [SerializeField] GameObject textBox;
    [SerializeField] TextMeshProUGUI textBoxText;

    public enum CombatChoice { None, Fight, Roll, Pay, Talk, Flee }

    CombatChoice choice = CombatChoice.None;
    Coroutine waitForInput;
    Coroutine combatRoll;
    Coroutine enemyCombatRoll;
    Coroutine processInput;

    MapData currentMap;
    int bribeMultiplier;
    int thisCombatBribeMultiplerMax = 1;
    int totalBribeAmount;
    int enemyRerolls = 0;

    public bool textBoxOpen = false;

    public GameObject rollDiceButton;

    public TextMeshProUGUI fightButtonText;
    public GameObject talkButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textBox.SetActive(false);
        ActivateDiceDisplays(false);
    }



    void ActivateDiceDisplays(bool value)
    {
        diceDisplay.SetActive(value);
        enemyDiceDisplay.SetActive(value);
       
    }


    public void EnterCombatPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.combat);
        fightButtonText.text = "Fight";
        talkButton.SetActive(true);
        battlefieldBuilder.StartFadeToBlack();
        currentMap = battlefieldBuilder.GetThisMap();
        rollDiceButton.SetActive(false);
        ActivateDiceDisplays(true);
        enemyRerolls = 0;
        SetupEnemyStart();
    }

    void SetupEnemyStart()
    {
        CalculateThisBribe(1);
        SquareController sq = turnOrganiser.GetLandedSquare();
        currentEnemyBuff = sq.GetEnemyBaseBuff();
        UpdateDiceRollFormulaText(currentEnemyBuff, 0, -1);

        if(waitForInput != null)
        {
            waitForInput = null;
        }

        waitForInput = StartCoroutine(WaitForInput());

    }

    IEnumerator WaitForInput()
    {
        waitingForPressRoll = true;
        choice = CombatChoice.None;

        // Wait for UI button Press
        yield return new WaitUntil(() => choice != CombatChoice.None);
        waitingForPressRoll = false;

        if(processInput != null)
        {
            processInput = null;
        }
        processInput = StartCoroutine(ProcessInput());
    }

    IEnumerator ProcessInput()
    {
        if (choice == CombatChoice.Pay)
        {
            playerStatsController.AlterMoney((totalBribeAmount) * -1);
            playerStatsController.alterSuffering(currentEnemyDamage * -1);
            AudioManager.Instance.PlayPayOffChuckle();

            CloseCombatScene();


            yield break;
        }
        else if (choice == CombatChoice.Flee)
        {
            playerMovementController.MovePlayerBackOneSquare();
            playerStatsController.alterSuffering(1);
            CloseCombatScene();

            yield break;
        }
        else if (choice == CombatChoice.Talk)
        {

            AudioManager.Instance.PlayPayOffChuckle();

            SquareController sq = turnOrganiser.GetLandedSquare();
            string enemyWeight = sq.getSquareQuantity();

            string enemyMood = sq.EnemyMood.ToString();  

            string newDialogue = currentMap.GetRandomLine(enemyWeight, enemyMood);

            textBoxText.text = newDialogue;

            textBox.SetActive(true);
            textBoxOpen = true;

            yield return new WaitUntil(() => textBoxOpen == false);

            if (waitForInput != null)
            {
                waitForInput = null;
            }

            waitForInput = StartCoroutine(WaitForInput());

       
        }
        else if(choice == CombatChoice.Fight)
        {
            AudioManager.Instance.PlayPayOffChuckle();

            if (enemyCombatRoll != null)
            {
                enemyCombatRoll = null;
            }
            CalculateThisBribe(2);
            talkButton.SetActive(false);
            enemyCombatRoll = StartCoroutine(EnemyRollRoutine());
        }
    
    }

    IEnumerator EnemyRollRoutine()
    {

        
        choice = CombatChoice.None;


        enemyDiceController.RollDice();

        // Wait for dice finish
        yield return new WaitUntil(() => !enemyDiceController.isRolling);
        yield return new WaitForSeconds(0.25f);
       

        currentEnemyRoll = enemyDiceController.getDiceResult();

        UpdateDiceRollFormulaText(currentEnemyBuff, currentEnemyRoll, -1);

        rollDiceButton.SetActive(true);
        fightButtonText.text = "Roll Again?";

        yield return new WaitUntil(() => choice != CombatChoice.None);



        // Now Hand Over To Player

        if (choice == CombatChoice.Fight)
        {
            AudioManager.Instance.PlayPayOffChuckle();

            if (enemyCombatRoll != null)
            {
                enemyCombatRoll = null;
            }
            enemyRerolls++;
            CalculateThisBribe(2 + enemyRerolls);
            enemyCombatRoll = StartCoroutine(EnemyRollRoutine());


        }
        else if(choice == CombatChoice.Pay)
        {
            AudioManager.Instance.PlayPayOffChuckle();

            playerStatsController.AlterMoney((totalBribeAmount) * -1);
            playerStatsController.alterSuffering(currentEnemyDamage * -1);
            AudioManager.Instance.PlayPayOffChuckle();

            CloseCombatScene();


            yield break;


        }
        
        else if (choice == CombatChoice.Roll)
        {
            StartCombatRoutine();
        }
            
    }




    
    public void UpdateCombatRoll()
    {
        if (!waitingForPressRoll) return;
        UpdateDiceRollFormulaText(currentEnemyBuff, currentEnemyRoll, -1);
    }

    public void CloseTextBox()
    {
        textBox.SetActive(false);
        textBoxOpen = false;
    }

    void StartCombatRoutine()
    {
       

        if (combatRoll != null)
        {
            StopCoroutine(combatRoll);
        }
        combatRoll = StartCoroutine(CombatRollScreen());
    }

    void CalculateThisBribe(int excessBribe)
    {
        if (currentMap != null)
        {
            bribeMultiplier = currentMap.GetBribeMultiplier() * excessBribe;
        }
        else
        {
            bribeMultiplier = 1 * excessBribe;
        }

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
        
            diceController.RollDice();
        
            yield return new WaitUntil(() => !diceController.isRolling);
            yield return new WaitForSeconds(0.25f);

            ResolveCombat();

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
            AudioManager.Instance.playTakeDamageSoundEffect();
            playerStatsController.resetSuffering();
            playerMovementController.MovePlayerBackOneSquare();

        }
        else
        {
            // Win Results
            playerStatsController.resetSuffering();
            playerStatsController.AlterMoney(10);
            AudioManager.Instance.playCombatWinSoundEffect();
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

        choice = CombatChoice.Fight;


        /*
        if (enemyCombatRoll != null)
        {
            StopCoroutine(enemyCombatRoll);
        }

        currentEnemyRoll = -1;
        currentEnemyBuff = -1;

        enemyCombatRoll = StartCoroutine(EnemyRollRoutine());*/

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
        StopAllCoroutines();
        battlefieldBuilder.StartFadeFromBlack();
        diceDisplay.SetActive(false);
        enemyDiceDisplay.SetActive(false);
        currentEnemyRoll = -1;
        enemyRerolls = 0;
        currentEnemyBuff = -1;
        turnOrganiser.SetLandedOnEnemySquare(false, null);
        turnOrganiser.BuildNextTurn();

    }



}
