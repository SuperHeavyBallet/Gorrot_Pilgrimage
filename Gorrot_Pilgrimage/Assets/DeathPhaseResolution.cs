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

    [SerializeField] UIController uiController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerIsDead = false;
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
        }
        else
        {
            playerName = "Default Bob";
        }

            deathNameText.text = playerName;   
        playerIsDead = false;
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.death);

        battlefieldBuilder.StartFadeToBlack();

        deathScreen.SetActive(true);
       deathUI.SetActive(true);
        playerIsDead=true;
       

    }

    public void LoadCharacterCreation()
    {
        if(playerIsDead)
        {
            uiController.LoadCharacterCreationScene();
        }
       
    }


}
