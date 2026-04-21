using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FateCounter : MonoBehaviour
{

    int fateCounter = 0;
    int maxFateCounter = 20;

    public FateOutcomes fateOutcomes;

   public TurnOrganiser turnOrganiser;

    [SerializeField] Image fateBarFill;

    Vector3 fateFillBaseScale;

    float fillAmount = 0;

    void Awake()
    {
        fateFillBaseScale = fateBarFill.transform.localScale;
        UpdateFateFillBar();
    }

    public void alterFateCounter(int alterAmount)
    {
        fateCounter = Mathf.Clamp(fateCounter + alterAmount, 0, maxFateCounter);

        UpdateFateFillBar();

        if(fateCounter >= maxFateCounter)
        {
            resetFateCounter();

           
            SelectFateOutcome();
            
            
        }

    }

    void UpdateFateFillBar()
    {

        Debug.Log("UPDATE FATE FILL BAR");

        fillAmount = (float)fateCounter / maxFateCounter;

        fateBarFill.transform.localScale =
         new Vector3(
             fateFillBaseScale.x,
             fateFillBaseScale.y * fillAmount,
             fateFillBaseScale.z
         );
    }

    public void resetFateCounter()
    {
        fateCounter = 0;
        UpdateFateFillBar();
    }

    void SelectFateOutcome()
    {
        // fateOutcomes.SelectFateOutcome();
        turnOrganiser.SetWaitingForFate(true);

    }
}
