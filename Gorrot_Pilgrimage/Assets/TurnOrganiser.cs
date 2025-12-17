using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections;

public class TurnOrganiser : MonoBehaviour
{

    bool isPlayerTurn;

    [SerializeField] AudioManager audioManager;

    [SerializeField] TextMeshProUGUI turnDisplay;

    bool hasLandedOnEnemy = false;
    bool hasLandedOnGoal = false;


    [SerializeField] DiceController diceController;


    [SerializeField] PlayerStatsController playerStatsController;


    [SerializeField] TextMeshProUGUI DiceRollFormulaText;

    bool readyToReturnToPlayer;
    Coroutine inPhaseBuild;

    public bool waitingForFate;

    public TextMeshProUGUI currentPhaseText;

    public enum ActivePhase {
        movement,
        combat,
        fate,
        goalReach,
        death,
        merchant
    }

    public ActivePhase currentPhase = ActivePhase.movement;

    [SerializeField] CombatPhaseResolution combatPhaseResolution;
    [SerializeField] FatePhaseResolution fatePhaseResolution;
    [SerializeField] MovementPhaseResolution movementPhaseResolution;
    [SerializeField] DeathPhaseResolution deathPhaseResolution;
    [SerializeField] GoalPhaseResolution goalPhaseResolution;
    [SerializeField] MerchantPhaseResolution merchantPhaseResolution;

    bool isInMerchant;

    int currentEnemySize = 0;

    bool playerIsAlive;

    public SquareController landedSquare;
    public void SetLandedSquare(SquareController sq)
    {
        landedSquare = sq;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {




        playerIsAlive = true;
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


    public void SetWaitingForFate(bool value)
    {
        waitingForFate = value;
    }

    public void FinishFate()
    {
        waitingForFate = false;
    }

    public void SetLandedOnEnemySquare(bool value, SquareController landedSquare)
    {
        hasLandedOnEnemy = value;
        if(landedSquare != null)
        {
             SetLandedSquare(landedSquare);
        }
       
    }

    public void LandedOnMerchantSquare()
    {
        
        merchantPhaseResolution.EnterMerchantPhase();
    }

    public void SetIsInMerchant(bool value)
    {
        isInMerchant = value;
    }

    public bool GetIsInMerchant()
    {
        return isInMerchant;
    }

    public void SetReadyToReturnToPlayer(bool value)
    {
        readyToReturnToPlayer = value;
    }



    public void BuildNextTurn()
    {
        if(!isInMerchant)
        {
            disablePlayerTurn();
            BuildNextPhase();
        }

        

    }

    public bool GetLandedOnGoal()
    {
        return hasLandedOnGoal;
    }

    void SetLandedOnGoal(bool value)
    {
        hasLandedOnGoal = value;
    }
    public void LandedOnGoal()
    {
        SetLandedOnGoal(true);
       goalPhaseResolution.EnterGoalPhase();

    }
    void BuildNextPhase()
    {
        if(playerIsAlive)
        {
            SetReadyToReturnToPlayer(false);
            StartBuildPhase();

            if (hasLandedOnEnemy)
            {
                combatPhaseResolution.EnterCombatPhase();
            }
            else
            {
                if (waitingForFate)
                {
                    fatePhaseResolution.EnterFatePhase();
                }
                else
                {
                    ReturnToMovementPhase();
                }
            }
        }
       
    }

    void StartBuildPhase()
    {
        if (inPhaseBuild != null)
        {
            StopCoroutine(inPhaseBuild);
        }
        

        inPhaseBuild = StartCoroutine(waitForPhaseBuildToFinish());
    }

    void ReturnToMovementPhase()
    {
        movementPhaseResolution.EnterMovementPhase();
        if(GetLandedOnGoal())
        {
            SetLandedOnGoal(false);
        }
        
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

    public void OnPlayerDeath()
    {
        playerIsAlive = false;
        deathPhaseResolution.EnterDeathPhase();
    }

}


