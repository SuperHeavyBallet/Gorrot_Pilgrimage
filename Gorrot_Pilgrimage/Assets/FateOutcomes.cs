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

    public GameObject healthPlus;
    public GameObject healthNeg;
    public GameObject attackPlus;
    public GameObject attackNeg;
    public GameObject sufferingPlus;
    public GameObject sufferingNeg;

    Coroutine activateSign;

    private void Start()
    {
        healthPlus.SetActive(false);
        healthNeg.SetActive(false);
        attackPlus.SetActive(false);
        attackNeg.SetActive(false);
        sufferingPlus.SetActive(false);
        sufferingNeg.SetActive(false);
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

            if(chosenFateEffectDelta > 0 )
            {
                ActivateSignForTime(healthPlus);
            }
            else if (chosenFateEffectDelta < 0 )
            {
                ActivateSignForTime(healthNeg);
            }
            
        }
        else if (chosenFateStatEffected == "suffering")
        {
            playerStatsController.alterSuffering(chosenFateEffectDelta);

            if (chosenFateEffectDelta > 0)
            {
                ActivateSignForTime(sufferingPlus);
            }
            else if (chosenFateEffectDelta < 0)
            {
                ActivateSignForTime(sufferingNeg);
            }
            
        }
        else if (chosenFateStatEffected == "attack")
        {
            playerStatsController.alterAttack(chosenFateEffectDelta);

            if (chosenFateEffectDelta > 0)
            {
                ActivateSignForTime(attackPlus);
            }
            else if (chosenFateEffectDelta < 0)
            {
                ActivateSignForTime(attackNeg);
            }
        }


        fateWait = null;
        turnOrganiser.FinishFate();

    }

    void ActivateSignForTime(GameObject sign)
    {
        sign.SetActive(true);
        if(activateSign != null)
        {
            StopCoroutine(activateSign);
        }

        activateSign = StartCoroutine(DeActivateSignAfterTime(sign));

    }

    IEnumerator DeActivateSignAfterTime(GameObject sign)
    {
        yield return new WaitForSeconds(1);
        sign.SetActive(false);
    }


}
