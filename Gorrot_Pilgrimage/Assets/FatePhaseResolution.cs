using UnityEngine;
using System.Collections;
using TMPro;

public class FatePhaseResolution : MonoBehaviour
{
    TurnOrganiser turnOrganiser;

    public GameObject fateScreen;
    public GameObject fateDisplay;

    public TextMeshProUGUI fateOutcomeText;
    public TextMeshProUGUI fateOutcomeStatText;
    public TextMeshProUGUI fateOutcomeStatDelta;

    public FateOutcomes fateOutcomes;

    FateOutcome fateOutcome;

    [SerializeField] FateCounter fateCounter;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnOrganiser = GetComponent<TurnOrganiser>();
        fateScreen.SetActive(false);
        fateDisplay.SetActive(false); 
    }


    public void EnterFatePhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.fate);
        UpdateFateOutComeText("...");
        //fateScreen.SetActive(true);
        fateDisplay.SetActive(true);

        StartCoroutine(FateRollScreen());
    }

    void UpdateFateOutcome()
    {
        fateOutcome = fateOutcomes.GetFateOutcome();
        UpdateFateOutcomeText();

    }

    void UpdateFateOutcomeText()
    {
        fateOutcomeText.text = fateOutcome.fateName.ToString();
        fateOutcomeStatText.text = fateOutcome.GetStatEffectedString();

        int fateDelta = fateOutcome.GetEffectDelta();
        string deltaSign = "+";

        if(fateDelta < 0)
        {
            deltaSign = "-";
        }

        fateOutcomeStatDelta.text = deltaSign + " " + fateDelta;
    }
    IEnumerator FateRollScreen()
    {
        fateOutcomes.PickFate();

        UpdateFateOutcome();
        fateOutcomes.ApplyFate();
        turnOrganiser.FinishFate();
        turnOrganiser.BuildNextTurn();

        
       

        yield return new WaitForSeconds(1f);



        CloseFateScene();
    }

    void UpdateFateOutComeText(string newText)
    {
        fateOutcomeText.text = newText;
    }

    void CloseFateScene()
    {
       fateScreen.SetActive(false);
        fateDisplay.SetActive(false);
        UpdateFateOutComeText("...");
        
    }


}
