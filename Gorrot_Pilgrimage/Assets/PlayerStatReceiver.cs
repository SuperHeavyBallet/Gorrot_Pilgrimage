using TMPro;
using UnityEngine;

public class PlayerStatReceiver : MonoBehaviour
{

    string playerName;
    string playerHome;

    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI playerHomeText;

    int startingHealth;
    int startingMoney;
    int startingSuffering;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GetCharacterStats();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetCharacterStats()
    {
        CharacterStatSheet sheet = CharacterStatSheet.Instance;

        if(sheet != null )
        {
            playerName = sheet.GetCharacterName();
            playerHome = sheet.GetCharacterStartLocation().ToString();

            startingHealth = sheet.GetStartingHealth();
            startingMoney = sheet.GetStartingMoney();
            startingSuffering = sheet.GetStartingSuffering();
        }
        else
        {
            playerName = "Default";
            playerHome = "Default";
            startingHealth = 10;
            startingMoney = 5;
            startingSuffering = 0;
        }
        

    }

    void UpdateUI()
    {
        playerNameText.text = playerName;
        playerHomeText.text = playerHome;
    }

    public int GetStartingHealth() => startingHealth;
    public int GetStartingMoney() => startingMoney;
    public int GetStartingSuffering() => startingSuffering;

}
