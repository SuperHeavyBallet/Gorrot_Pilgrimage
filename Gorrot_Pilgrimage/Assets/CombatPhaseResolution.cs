using GorrotGame;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
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

   // [SerializeField] AudioManager audioManager;

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

    public enum CombatState { None, AwaitingStartChoice, AwaitingEnemyRolledChoice, AwaitingPlayerRoll, Resolving, InDialogue }
    CombatState state;

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

    bool canReroll;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textBox.SetActive(false);
        ActivateDiceDisplays(false);
    }

    private void Awake()
    {
        diceController.OnSettledSfx += PlayDiceRollCompleteSoundEffect;
        enemyDiceController.OnSettledSfx += PlayDiceRollCompleteSoundEffect;
    }


    void ActivateDiceDisplays(bool value)
    {
        diceDisplay.SetActive(value);
        enemyDiceDisplay.SetActive(value);
       
    }

    void PlayDiceRollCompleteSoundEffect()
    {
       AudioManager.Instance.PlayDiceRollCompleteSoundEffect();
    }
    public void EnterCombatPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.combat);
        state = CombatState.AwaitingStartChoice;
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
       
        SquareController sq = turnOrganiser.GetLandedSquare();

        currentEnemyDamage = sq.EnemyDamage;
        currentEnemyBuff = sq.GetEnemyBaseBuff();

        CalculateThisBribe(1);
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
        yield return new WaitUntil(() =>
         choice == CombatChoice.Fight ||
         choice == CombatChoice.Pay ||
         choice == CombatChoice.Roll ||
         choice == CombatChoice.Talk ||
         choice == CombatChoice.Flee);

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
            state = CombatState.InDialogue;

            AudioManager.Instance.PlayPayOffChuckle();

            SquareController sq = turnOrganiser.GetLandedSquare();
            SquareSize enemyWeight = sq.ThisSquareSize;

            SquareMood enemyMood = sq.EnemyMood;  

            string newDialogue = currentMap.GetRandomLine(enemyWeight.ToString(), enemyMood.ToString());

            textBoxText.text = newDialogue;

            textBox.SetActive(true);
            textBoxOpen = true;

            yield return new WaitUntil(() => textBoxOpen == false);

            if (waitForInput != null)
            {
                waitForInput = null;
            }
            state = CombatState.AwaitingStartChoice;
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
        canReroll = false;

       

        state = CombatState.Resolving;
        rollDiceButton.SetActive(false); // don't allow player roll yet

        enemyDiceController.RollDice();
        fightButtonText.text = "Rolling...";
        yield return new WaitUntil(() => !enemyDiceController.isRolling);
      
        yield return new WaitForSeconds(0.25f);
       

        currentEnemyRoll = enemyDiceController.getDiceResult();

        UpdateDiceRollFormulaText(currentEnemyBuff, currentEnemyRoll, -1);

        // Now player can choose what to do
        state = CombatState.AwaitingEnemyRolledChoice;
        rollDiceButton.SetActive(true);
        

        int rerollCost = (totalBribeAmount / 2) + enemyRerolls; // compute BEFORE increment or after, but consistently
        canReroll = playerStatsController.GetPlayerCurrentMoney() >= rerollCost;

        fightButtonText.text = canReroll ? $"Reroll: {rerollCost}" : $"Need: {rerollCost}";

        yield return new WaitUntil(() => choice != CombatChoice.None);



        // Now Hand Over To Player

        if (choice == CombatChoice.Fight)
        {
           

            AudioManager.Instance.PlayPayOffChuckle();

            playerStatsController.AlterMoney(-rerollCost);

            enemyRerolls++;
            CalculateThisBribe(2 + enemyRerolls);

            // Loop again
            enemyCombatRoll = StartCoroutine(EnemyRollRoutine());
            yield break;

        }
        else if(choice == CombatChoice.Pay)
        {
            AudioManager.Instance.PlayPayOffChuckle();

            playerStatsController.AlterMoney(-totalBribeAmount);
            playerStatsController.alterSuffering(currentEnemyDamage * -1);

            CloseCombatScene();
            yield break;

        }
        
        else if (choice == CombatChoice.Roll)
        {
            state = CombatState.AwaitingPlayerRoll;
            StartCombatRoutine();
            yield break;

        }
        else if (choice == CombatChoice.Flee)
        {
            // “Flee after committing” penalty
            // Example: take damage + suffering + move back, then close.
            playerMovementController.MovePlayerBackOneSquare();

            // Your choice: either direct health damage, or “suffering”, or both.
            playerStatsController.alterHealth(-1);         // or -currentEnemyDamage, tuned to taste
            playerStatsController.alterSuffering(2);                // e.g. panic/trauma tax

            CloseCombatScene();
            yield break;
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

        canReroll = false;

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

        if(currentMap != null)
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
            //turnOrganiser.GetLandedSquare().MakeEmptySquare();
            turnOrganiser.GetLandedSquare().MakeSquare(GorrotGame.SquareType.Empty, currentMap);
        }

        currentEnemyDamage = 0;

        combatRoll = null;
        CloseCombatScene();
    }

    public void PlayerPressedPay()
    {
        if (state == CombatState.AwaitingStartChoice || state == CombatState.AwaitingEnemyRolledChoice)
        {
            if (playerStatsController.GetPlayerCurrentMoney() >= totalBribeAmount)
                choice = CombatChoice.Pay;
        }

    }

    public void PlayerPressedFlee()
    {
        if (state == CombatState.AwaitingStartChoice ||
         state == CombatState.AwaitingEnemyRolledChoice)
        {
            choice = CombatChoice.Flee;
        }
    }

    public void PlayerPressedRoll()
    {

        if (state == CombatState.AwaitingEnemyRolledChoice)
            choice = CombatChoice.Roll;


    }

    public void PlayerPressedTalk()
    {
        if (state == CombatState.AwaitingStartChoice)
            choice = CombatChoice.Talk;
    }

    public void PlayerPressedFight()
    {
        // "Fight" on the first screen means "start combat / make enemy roll"
        if (state == CombatState.AwaitingStartChoice)
        {
            choice = CombatChoice.Fight;
            return;
        }

        // "Fight" after enemy rolled means "reroll enemy" (only if allowed)
        if (state == CombatState.AwaitingEnemyRolledChoice && canReroll)
        {
            choice = CombatChoice.Fight;
            return;
        }

        // Otherwise ignore (or play a denied click sound)

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
        if (waitForInput != null) StopCoroutine(waitForInput);
        if (combatRoll != null) StopCoroutine(combatRoll);
        if (enemyCombatRoll != null) StopCoroutine(enemyCombatRoll);
        if (processInput != null) StopCoroutine(processInput);

        waitForInput = combatRoll = enemyCombatRoll = processInput = null;

        state = CombatState.None;
        choice = CombatChoice.None;

        canReroll = false;
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
