using UnityEngine;

public class FateOutcomes : MonoBehaviour
{

    public FateOutcome[] allFateOutcomes;

    public PlayerStatsController playerStatsController;
    public PlayerMovementController playerMovementController;

    public void SelectFateOutcome()
    {
        Debug.Log("FATE OUTCOMES");

        int randomNumber = UnityEngine.Random.Range(0, allFateOutcomes.Length);

        FateOutcome chosenFateOutcome = allFateOutcomes[randomNumber];
        string chosenFateStatEffected = chosenFateOutcome.GetStatEffected();
        int chosenFateEffectDelta = chosenFateOutcome.GetEffectDelta();
        Vector2Int chosenFateEffectDirection = chosenFateOutcome.GetEffectDirection();


        if(chosenFateStatEffected == "health")
        {
            playerStatsController.alterHealth(chosenFateEffectDelta);
        }
        else if(chosenFateStatEffected == "suffering")
        {
            playerStatsController.alterSuffering(chosenFateEffectDelta);
        }
        else if(chosenFateStatEffected == "attack")
        {
            playerStatsController.alterAttack(chosenFateEffectDelta);
        }
        else if (chosenFateStatEffected == "movement")
        {
            playerMovementController.FateMovement(chosenFateOutcome.GetEffectDirection());
        }
    }
}
