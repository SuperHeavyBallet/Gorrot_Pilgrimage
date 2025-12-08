using System.Collections;
using UnityEngine;

public class FateOutcomes : MonoBehaviour
{

    public FateOutcome[] allFateOutcomes;

    public PlayerStatsController playerStatsController;
    public PlayerMovementController playerMovementController;

    public TurnOrganiser turnOrganiser;

    int fateTimeout = 2;

    Coroutine fateWait;

   

    private void Start()
    {
        
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

    void ConcludeFateOutcome()
    {
        int randomNumber = UnityEngine.Random.Range(0, allFateOutcomes.Length);

        FateOutcome chosenFateOutcome = allFateOutcomes[randomNumber];
        string chosenFateStatEffected = chosenFateOutcome.GetStatEffected();
        int chosenFateEffectDelta = chosenFateOutcome.GetEffectDelta();
        Vector2Int chosenFateEffectDirection = chosenFateOutcome.GetEffectDirection();


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
