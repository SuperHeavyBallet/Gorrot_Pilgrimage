using TMPro;
using UnityEngine;

public class FateCounter : MonoBehaviour
{
    public TextMeshProUGUI fateCounterText;
    int fateCounter = 0;
    int maxFateCounter = 10;

    public FateOutcomes fateOutcomes;

   public TurnOrganiser turnOrganiser;



    public void alterFateCounter(int alterAmount)
    {
        fateCounter = Mathf.Clamp(fateCounter + alterAmount, 0, maxFateCounter);

        UpdateFateCounterText();

        if(fateCounter >= maxFateCounter)
        {
            resetFateCounter();

           
            SelectFateOutcome();
            
            
        }

    }

    void UpdateFateCounterText()
    {
        fateCounterText.text = "Fate: " + fateCounter;
    }

    public void resetFateCounter()
    {
        fateCounter = 0;
        UpdateFateCounterText();
    }

    void SelectFateOutcome()
    {
        // fateOutcomes.SelectFateOutcome();
        turnOrganiser.SetWaitingForFate(true);

    }
}
