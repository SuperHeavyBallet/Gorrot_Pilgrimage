using TMPro;
using UnityEngine;

public class FateCounter : MonoBehaviour
{
    public TextMeshProUGUI fateCounterText;
    int fateCounter = 0;
    int maxFateCounter = 10;

    public FateOutcomes fateOutcomes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void alterFateCounter(int alterAmount)
    {
        Debug.Log("ADD FATE: " + alterAmount);
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
    }

    void SelectFateOutcome()
    {
        fateOutcomes.SelectFateOutcome();
    }
}
