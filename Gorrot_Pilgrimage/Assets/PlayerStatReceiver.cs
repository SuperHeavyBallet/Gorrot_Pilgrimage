using TMPro;
using UnityEngine;

public class PlayerStatReceiver : MonoBehaviour
{

    string playerName;
    StartLocations playerHome;

    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI playerHomeText;

    int startingHealth;
    int startingMoney;
    int startingSuffering;

    [SerializeField] string default_PlayerName;
    [SerializeField] StartLocations default_PlayerHome;
    [SerializeField] int default_StartingHealth;
    [SerializeField] int default_StartingMoney;
    [SerializeField] int default_StartingSuffering;



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
            playerHome = sheet.GetCharacterStartLocation();
            startingHealth = sheet.GetStartingHealth();
            startingMoney = sheet.GetStartingMoney();
            startingSuffering = sheet.GetStartingSuffering();
        }
        else
        {
            playerName = default_PlayerName;
            playerHome = default_PlayerHome;
            startingHealth = default_StartingHealth;
            startingMoney = default_StartingMoney;
            startingSuffering = default_StartingSuffering;
        }
        

    }

    void UpdateUI()
    {
        playerNameText.text = playerName;
        playerHomeText.text = playerHome.ToString();
    }

    public int GetStartingHealth() => startingHealth;
    public int GetStartingMoney() => startingMoney;
    public int GetStartingSuffering() => startingSuffering;

    public StartLocations GetPlayerStartingLocation()
    {
        return playerHome;
    }

}
