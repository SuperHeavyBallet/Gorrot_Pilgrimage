using TMPro;
using UnityEngine;

public class DeathPhaseResolution : MonoBehaviour
{
    [SerializeField] GameObject deathText;
    [SerializeField] GameObject deathScreen;

    [SerializeField] TurnOrganiser turnOrganiser;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deathScreen.SetActive(false);
        deathText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterDeathPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.death);

        battlefieldBuilder.StartFadeToBlack();

        deathScreen.SetActive(true);
        deathText.SetActive(true);

    }


}
