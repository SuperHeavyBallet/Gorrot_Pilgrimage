using System.Collections;
using UnityEngine;

public class FateOutcomes : MonoBehaviour
{

    public FateOutcome[] allFateOutcomes;

    public PlayerStatsController playerStatsController;
    public PlayerMovementController playerMovementController;

    public TurnOrganiser turnOrganiser;



    int playerCurrentHealth;

    int playerMaxHealth;

    int playerCurrentSuffering;

    int playerMaxSuffering;

    FateOutcome chosenFateOutcome;
    string chosenFateStatEffected;
    int chosenFateEffectDelta;
    int randomNumber;

    private void Start()
    {

        playerMaxHealth = playerStatsController.GetPlayerMaxHealth();

        playerMaxSuffering = playerStatsController.GetPlayerMaxSuffering();
    }

    public void PickFate()
    {
        int currentHealth = playerStatsController.GetPlayerCurrentHealth();
        int currentSuffering = playerStatsController.GetPlayerCurrentSuffering();

        const int maxAttempts = 25; // safety cap
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int index = Random.Range(0, allFateOutcomes.Length);
            PickFateOutcomeAtIndex(index);

            if (!CheckIfShouldReroll(currentHealth, currentSuffering))
                return;
        }

        // If we got here, we kept rerolling into "bad" outcomes.
        // Pick last roll as-is, or fall back to a neutral outcome.
        Debug.LogWarning("PickFate hit max reroll attempts; keeping last outcome.");

    }

    bool CheckIfShouldReroll(int currentHealth, int currentSuffering)
    {

        // Prefer enums over strings long-term, but keeping your current approach.
        if (chosenFateStatEffected == "health")
        {
            bool lowHealth = currentHealth * 2 < playerMaxHealth;
            bool atMaxHealth = currentHealth == playerMaxHealth;

            if (lowHealth && chosenFateEffectDelta < 0) return true;   // avoid more damage
            if (atMaxHealth && chosenFateEffectDelta > 0) return true; // avoid wasted healing
        }
        else if (chosenFateStatEffected == "suffering")
        {
            bool highSuffering = currentSuffering > (playerMaxSuffering / 2);
            if (highSuffering && chosenFateEffectDelta > 0) return true; // avoid more suffering
        }

        return false;
    }

    public void ApplyFate()
    {
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

    }

    public FateOutcome GetFateOutcome()
    {
        return chosenFateOutcome;
    }



    void PickFateOutcomeAtIndex(int index)
    {
        chosenFateOutcome = allFateOutcomes[index];
        chosenFateStatEffected = chosenFateOutcome.GetStatEffected();
        chosenFateEffectDelta = chosenFateOutcome.GetEffectDelta();

    }


   

    


}
