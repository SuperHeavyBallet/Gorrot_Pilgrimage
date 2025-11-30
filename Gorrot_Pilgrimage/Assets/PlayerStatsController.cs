using UnityEngine;
using TMPro;

public class PlayerStatsController : MonoBehaviour
{

    public int playerCurrentHealth = 10;
    int playerMaxHealth = 10;
    int playerMinHealth = 0;

    public int playerCurrentSuffering = 0;
    int playerMaxSuffering = 10;
    int playerMinSuffering = 0;

    public TextMeshProUGUI healthDisplay;
    public TextMeshProUGUI sufferingDisplay;

    public bool playerIsAlive;

    public AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerIsAlive = true;
        UpdateNumbersDisplay();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void subtractHealth(int subtractAmount)
    {

        audioManager.playTakeDamageSoundEffect();

        if (playerCurrentHealth > playerMinHealth)
        {
            playerCurrentHealth -= subtractAmount;
            
        }

        UpdateNumbersDisplay();


    }

    public void UseItem(string itemName)
    {

        Debug.Log("USE ITEM: " + itemName);
        switch (itemName)
        {
            case "Sma Pot":
                addHealth(1);
                break;
            case "Med Pot":
                addHealth(3);
                break;
            case "Big Pot":
                addHealth(5);
                break;
            default:
                addHealth(3);

                break;
        }
    }

    public void addHealth(int addAmount)
    {
        playerCurrentHealth += addAmount;
        UpdateNumbersDisplay();
    }

    public void subtractSuffering(int subtractAmount)
    {
        if(playerCurrentSuffering > playerMinSuffering)
        {
            playerCurrentSuffering -= subtractAmount;
        }
        
        UpdateNumbersDisplay();
    }

    public void addSuffering(int addAmount)
    {
        if (playerCurrentSuffering >= playerMaxSuffering)
        {
            subtractHealth(1);

        }
        else
        {
            playerCurrentSuffering += addAmount;
        }

        UpdateNumbersDisplay();
    }

    public void resetSuffering()
    {
        playerCurrentSuffering = 0;
        UpdateNumbersDisplay();
    }

    void UpdateNumbersDisplay()
    {
        if (playerCurrentHealth > playerMinHealth)
        {
            playerIsAlive = true;
            healthDisplay.text = "Health: " + playerCurrentHealth.ToString();
            sufferingDisplay.text = "Suffering: " + playerCurrentSuffering.ToString();
        }
        else
        {
            playerIsAlive = false ;
            healthDisplay.text = "Health: " + "Dead";
            sufferingDisplay.text = "Suffering: " + "None";

        }

        

    }

}