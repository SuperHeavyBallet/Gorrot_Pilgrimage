using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections;

public class TurnOrganiser : MonoBehaviour
{

    public bool isPlayerTurn;
    int nextTurnBuildingTime = 1;

    public AudioManager audioManager;

    public TextMeshProUGUI turnDisplay;

    public bool hasLandedOnEnemy = false;


    public DiceController diceController;

    //Coroutine waitForDiceRoll;

    public PlayerStatsController playerStatsController;
    int currentEnemyDamage;
    int enemyRollToBeat;

    public TextMeshProUGUI DiceRollFormulaText;

    public bool readyToReturnToPlayer;
    Coroutine inPhaseBuild;

    public bool waitingForFate;


    public GameObject combatScene;
    public GameObject diceDisplay;

    Coroutine waitForFateRoll;

    public TextMeshProUGUI currentPhaseText;

    public enum ActivePhase {
        movement,
        combat,
        fate,
        goalReach,
        death
    }

    public ActivePhase currentPhase = ActivePhase.movement;

    CombatPhaseResolution combatPhaseResolution;
    FatePhaseResolution fatePhaseResolution;
    MovementPhaseResolution movementPhaseResolution;

    int currentEnemySize = 0;

    bool readyToRoll;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatPhaseResolution = GetComponent<CombatPhaseResolution>();
        fatePhaseResolution = GetComponent<FatePhaseResolution>();
        movementPhaseResolution = GetComponent<MovementPhaseResolution>();

        combatScene.SetActive(false);
        diceDisplay.SetActive(false);


       movementPhaseResolution.EnterMovementPhase();
    }

    public void UpdateCurrentPhase(ActivePhase newPhase)
    {
        currentPhase = newPhase;
        UpdatePhaseText();
    }

    public void UpdateCurrentEnemySize(int newEnemySize)
    {
        currentEnemySize = newEnemySize;
    }

    public int GetEnemySize()
    {
        return currentEnemySize;
    }
    void UpdatePhaseText()
    {
        switch(currentPhase)
        {
            case ActivePhase.movement:
                currentPhaseText.text = "Movement";
                break;
                case ActivePhase.combat:
                currentPhaseText.text = "Combat";
                break;
                case ActivePhase.fate:
                currentPhaseText.text = "Fate";
                break;
                default:
                currentPhaseText.text = "Broken";
                break;
        }
    }
 

    public void disablePlayerTurn()
    {
        audioManager.changeTurnSound("enemy");
        isPlayerTurn = false;
        turnDisplay.text = "Turn: Building Next";   
    }

    public void WaitForFate()
    {
        waitingForFate = true;
    }
    public void FinishFate()
    {
        waitingForFate = false;
        ReturnToMovementPhase();
    }

    public void SetLandedOnEnemySquare(bool value)
    {
        hasLandedOnEnemy = value;
    }

    public void SetReadyToReturnToPlayer(bool value)
    {
        readyToReturnToPlayer = value;
    }

    public void SetReadyToRoll(bool value)
    {
        readyToRoll = value;
    }


    public void BuildNextTurn()
    {
        disablePlayerTurn();
        BuildNextPhase();

    }

    void BuildNextPhase()
    {
        
        StartBuildPhase();

        if (hasLandedOnEnemy)
        {
            combatPhaseResolution.EnterCombatPhase();
        }
        else
        {
            if(waitingForFate)
            {
                fatePhaseResolution.EnterFatePhase();
            }
            else
            {     
               ReturnToMovementPhase();
            }
        }
    }

    void StartBuildPhase()
    {
        if (inPhaseBuild != null)
        {
            StopCoroutine(inPhaseBuild);
        }
        SetReadyToReturnToPlayer(false);

        inPhaseBuild = StartCoroutine(waitForPhaseBuildToFinish());
    }

    void ReturnToMovementPhase()
    {
        movementPhaseResolution.EnterMovementPhase();
        SetReadyToReturnToPlayer(true);
    }

    private IEnumerator waitForPhaseBuildToFinish()
    {
        yield return new WaitUntil(() => readyToReturnToPlayer);
        inPhaseBuild = null;
       
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

}


