using System.Collections;
using UnityEngine;

public class MerchantPhaseResolution : MonoBehaviour
{
    [SerializeField] TurnOrganiser turnOrganiser;

    [SerializeField] GameObject merchantUI;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    bool hasClickedLeaveMerchant = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterMerchantPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.merchant);
        turnOrganiser.SetIsInMerchant(true);
        battlefieldBuilder.StartFadeToBlack();
       SetHasClickedLeaveMerchant(false);
        merchantUI.SetActive(true);
        StartCoroutine(StayInMerchant());
    }

    public void SetHasClickedLeaveMerchant(bool value)
    {
        hasClickedLeaveMerchant=value;
    }

    public void ClickLeaveButton()
    {
        SetHasClickedLeaveMerchant(true);
    }

    IEnumerator StayInMerchant()
    {
        yield return new WaitUntil(() => hasClickedLeaveMerchant);

        ExitMerchantPhase();


    }

    void ExitMerchantPhase()
    {
        turnOrganiser.SetIsInMerchant(false);
        merchantUI.SetActive(false);
        battlefieldBuilder.StartFadeFromBlack();
        SetHasClickedLeaveMerchant(false);
        turnOrganiser.BuildNextTurn();
    }
}
