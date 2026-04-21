using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TurnOrganiser : MonoBehaviour
{

    bool isPlayerTurn;



    bool hasLandedOnEnemy = false;
    bool hasLandedOnGoal = false;


    [SerializeField] DiceController diceController;


    [SerializeField] PlayerStatsController playerStatsController;


    [SerializeField] TextMeshProUGUI DiceRollFormulaText;

    bool readyToReturnToPlayer;
    Coroutine inPhaseBuild;

    public bool waitingForFate;


    public enum ActivePhase {
        movement,
        combat,
        fate,
        goalReach,
        death,
        merchant,
        win,
        levelTransition
    }

    public ActivePhase currentPhase = ActivePhase.movement;

    [SerializeField] CombatPhaseResolution combatPhaseResolution;
    [SerializeField] FatePhaseResolution fatePhaseResolution;
    [SerializeField] MovementPhaseResolution movementPhaseResolution;
    [SerializeField] DeathPhaseResolution deathPhaseResolution;
    [SerializeField] GoalPhaseResolution goalPhaseResolution;
    [SerializeField] WinPhaseResolution winPhaseResolution;
    [SerializeField] MerchantPhaseResolution merchantPhaseResolution;

    bool isInMerchant;

    int currentEnemySize = 0;

    bool playerIsAlive;

    GameObject currentFly;
    FlyMovementController currentFlyMovementController;
    List<GameObject> currentFlies = new List<GameObject>();

    [SerializeField] BattlefieldBuilder battlefieldBuilder;



    SquareController landedSquare;
    public void SetLandedSquare(SquareController sq)
    {
        landedSquare = sq;
    }

    public SquareController GetLandedSquare()
    {
        return landedSquare;
    }

    public void ClearFlies()
    {
        currentFlies.Clear();
    }

    public void ReceiveFly(GameObject newFly)
    {
        currentFlies.Add(newFly);
    }

    void MoveFly()
    {
        foreach(GameObject newFly in currentFlies)
        {
            FlyMovementController newFlyMovement = newFly.GetComponent<FlyMovementController>();

            if (newFlyMovement != null)
            {
                newFlyMovement.RollNewDirection();
            }
            
        }
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
    }

    public void UpdateCurrentEnemySize(int newEnemySize)
    {
        currentEnemySize = newEnemySize;
    }

    public int GetEnemySize()
    {
        return currentEnemySize;
    }
 

    public void disablePlayerTurn()
    {
        AudioManager.Instance.changeTurnSound("enemy");
        isPlayerTurn = false;
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

            
            MoveFly();
            //SpawnNewEnemy();
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
        AudioManager.Instance.changeTurnSound("player");
        isPlayerTurn = true;
        hasLandedOnEnemy = false;
        
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

    public void StartWinPhase()
    {
        winPhaseResolution.EnterWinPhase();
    }

}


