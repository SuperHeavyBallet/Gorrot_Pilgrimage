using System.Collections;
using UnityEngine;

public class FateOutcomes : MonoBehaviour
{

    public FateOutcome[] allFateOutcomes;

    public PlayerStatsController playerStatsController;
    public PlayerMovementController playerMovementController;

    public TurnOrganiser turnOrganiser;

    int fateTimeout = 2;

    int playerCurrentHealth;
    int playerMinHealth;
    int playerMaxHealth;

    int playerCurrentSuffering;
    int playerMinSuffering;
    int playerMaxSuffering;

    Coroutine fateWait;
    FateOutcome chosenFateOutcome;
    string chosenFateStatEffected;
    int chosenFateEffectDelta;
    Vector2Int chosenFateEffectDirection;
    int randomNumber;

    private void Start()
    {
        playerMinHealth = playerStatsController.GetPlayerMinHealth();
        playerMaxHealth = playerStatsController.GetPlayerMaxHealth();

        playerMinSuffering = playerStatsController.GetPlayerMinSuffering();
        playerMaxSuffering = playerStatsController.GetPlayerMaxSuffering();
    }

    public void SelectFateOutcome()
    {
      
        turnOrganiser.WaitForFate();
        if( fateWait != null )
        {
            StopCoroutine(waitForFate());
        }

        fateWait = StartCoroutine(waitForFate());
 

        

      
    }

    public IEnumerator waitForFate()
    {
        yield return new WaitForSeconds(fateTimeout);
        ConcludeFateOutcome();
    }

    void PickFateOutcomeAtIndex(int index)
    {
        chosenFateOutcome = allFateOutcomes[index];
        chosenFateStatEffected = chosenFateOutcome.GetStatEffected();
        chosenFateEffectDelta = chosenFateOutcome.GetEffectDelta();
        chosenFateEffectDirection = chosenFateOutcome.GetEffectDirection();
    }

    void ConcludeFateOutcome()
    {
        playerCurrentHealth = playerStatsController.GetPlayerCurrentHealth();
        playerCurrentSuffering = playerStatsController.GetPlayerCurrentSuffering();

        randomNumber = Random.Range(0, allFateOutcomes.Length);
        PickFateOutcomeAtIndex(randomNumber);

        // Reroll  to reduce damage when hurt
        if (playerCurrentHealth < (playerMaxHealth / 2))
        {
            if(chosenFateStatEffected == "health" && chosenFateEffectDelta < 0)
            {
                randomNumber = Random.Range(0, allFateOutcomes.Length);
                PickFateOutcomeAtIndex(randomNumber);

            }
        }

        // Reroll to reduce Excess Health
        if (playerCurrentHealth > playerMaxHealth)
        {
            if (chosenFateStatEffected == "health" && chosenFateEffectDelta > 0)
            {
                randomNumber = Random.Range(0, allFateOutcomes.Length);
                PickFateOutcomeAtIndex(randomNumber);

            }
        }

        if (playerCurrentSuffering > (playerMaxSuffering / 2))
        {
            if(chosenFateStatEffected == "suffering" && chosenFateEffectDelta > 0)
            {
                randomNumber = Random.Range(0, allFateOutcomes.Length);
                PickFateOutcomeAtIndex(randomNumber);
            }
        }

        


        if (chosenFateStatEffected == "health")
        {
            playerStatsController.alterHealth(chosenFateEffectDelta);

            
        }
        else if (chosenFateStatEffected == "suffering")
        {
            playerStatsController.alterSuffering(chosenFateEffectDelta);

           
            
        }
        else if (chosenFateStatEffected == "attack")
        {
            playerStatsController.alterAttack(chosenFateEffectDelta);

         
        }


        fateWait = null;
        turnOrganiser.FinishFate();

    }

   

    


}
