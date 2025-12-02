using UnityEngine;
using TMPro;

public class PlayerStatsController : MonoBehaviour
{

    int playerCurrentHealth = 10;
    int playerMaxHealth = 10;
    int playerMinHealth = 0;

    int playerCurrentSuffering = 0;
    int playerMaxSuffering = 10;
    int playerMinSuffering = 0;

    int playerCurrentAttack = 0;
    int playerMaxAttack = 6;
    int playerMinAttack = 0;

    public TextMeshProUGUI healthDisplay;
    public TextMeshProUGUI sufferingDisplay;
    public TextMeshProUGUI attackDisplay;

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

    public void UseItem(string itemID)
    {
        string statEffected = ItemCatalogue.Instance.GetItemStatEffected(itemID);
        int effectDelta = ItemCatalogue.Instance.GetItemEffectDelta(itemID);


        switch (statEffected)
        {
            case "health":
                alterHealth(effectDelta);
                break;
            case "suffering":
                alterSuffering(effectDelta);
                break;
            case "attack":
                alterAttack(effectDelta);
                break;
            default:
                alterHealth(3);

                break;
        }
    }


    public void alterAttack(int alterAmount)
    {
        Debug.Log("ATTACK BOOST BY: " + alterAmount);

        int before = playerCurrentAttack;
        int raw = before + alterAmount;

        playerCurrentAttack = Mathf.Clamp(raw, playerMinAttack, playerMaxAttack);

        UpdateNumbersDisplay();
    }

    public int getCurrentAttackBuff()
    {
        return playerCurrentAttack;
    }

    public void alterHealth(int alterAmount)
    {
        

        if (playerCurrentHealth > playerMinHealth)
        {
            playerCurrentHealth += alterAmount;

        }

        if(alterAmount < 0)
        {
            audioManager.playTakeDamageSoundEffect();
        }

        UpdateNumbersDisplay();

    }

    public void alterSuffering(int alterAmount)
    {
        int before = playerCurrentSuffering;
        int raw = before + alterAmount;

        playerCurrentSuffering = Mathf.Clamp(raw, playerMinSuffering, playerMaxSuffering);

        if(playerCurrentSuffering >= playerMaxSuffering)
        {
            alterHealth(-1);
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
            attackDisplay.text = "Attack: +" + playerCurrentAttack.ToString();
        }
        else
        {
            playerIsAlive = false ;
            healthDisplay.text = "Health: " + "Dead";
            sufferingDisplay.text = "Suffering: " + "None";

        }

        

    }

}