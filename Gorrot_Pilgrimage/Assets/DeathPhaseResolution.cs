using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPhaseResolution : MonoBehaviour
{
    [SerializeField] GameObject deathUI;
    [SerializeField] TextMeshProUGUI deathNameText;
    [SerializeField] GameObject deathScreen;

    [SerializeField] TurnOrganiser turnOrganiser;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;
    bool playerIsDead;

    string playerName;
    string playerHome;

    [SerializeField] UIController uiController;

    [SerializeField] DeathPushNameToLedger deathPushNameToLedger;

    [SerializeField] Animator standeeAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerIsDead = false;
        standeeAnimator.SetBool("isDead",false);
        deathScreen.SetActive(false);
        deathUI.SetActive(false);
    }

    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterDeathPhase()
    {
        CharacterStatSheet sheet = CharacterStatSheet.Instance;

        if (sheet != null)
        {
            playerName = CharacterStatSheet.Instance.GetCharacterName();
            playerHome = CharacterStatSheet.Instance.GetCharacterHome();
        }
        else
        {
            playerName = "Default Bob";
            playerHome = "Avbarnia";
        }

            deathNameText.text = playerName;   
        playerIsDead = false;
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.death);

        battlefieldBuilder.StartFadeToBlack();

        deathScreen.SetActive(true);
       deathUI.SetActive(true);
        playerIsDead=true;
        standeeAnimator.SetBool("isDead", true);


        PushDataToDeathLedger();

    }

    public void PushDataToDeathLedger()
    {
        CharacterStatSheet sheet = CharacterStatSheet.Instance;

        if (sheet != null)
        {
            playerName = CharacterStatSheet.Instance.GetCharacterName();
            playerHome = CharacterStatSheet.Instance.GetCharacterHome();
        }
        else
        {
            playerName = "Default Bob";
            playerHome = "Avbarnia";
        }

        string deathLocation = battlefieldBuilder.GetThisMap().name;
        if (deathLocation == null)
        {
            deathLocation = "Bibbety Bop";
        }

        deathPushNameToLedger.FormPlayerInfo(
            playerName,
            playerHome,
            deathLocation);
    }

    public void LoadCharacterCreation()
    {
        if(playerIsDead)
        {
            uiController.LoadCharacterCreationScene();
        }
       
    }


}
