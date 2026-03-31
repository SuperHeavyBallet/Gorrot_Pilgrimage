using UnityEngine;
using TMPro;
using System.Collections;
using GorrotGame;

public class PlayerStatsController : MonoBehaviour
{

    int playerCurrentHealth = 10;
    int playerMaxHealth = 10; // Not actual Max, but a threshold for 'Stop giving free health via fate'
    int playerMinHealth = 0;

    int playerCurrentSuffering = 0;
    [SerializeField] int playerMaxSuffering = 10;
    int playerMinSuffering = 0;

    int playerCurrentAttack = 0;
    [SerializeField] int playerMaxAttack = 4;
    int playerMinAttack = 0;

    int playerCurrentMoney = 0;
    int playerMinMoney = 0;
    int playerMaxMoney = 9999;

    public TextMeshProUGUI healthDisplay;
    public TextMeshProUGUI sufferingDisplay;
    public TextMeshProUGUI attackDisplay;
    public TextMeshProUGUI moneyDisplay;

    [SerializeField] StatBoxAnimationController statBoxAnimationController;

    public bool playerIsAlive;
    bool playerHasDied;

    //public AudioManager audioManager;

    public GameObject healthPlus;
    public GameObject healthNeg;
    public GameObject attackPlus;
    public GameObject attackNeg;
    public GameObject sufferingPlus;
    public GameObject sufferingNeg;
    public GameObject moneyPlus;
    public GameObject moneyNeg;


    bool playerHasCarrionRose = false;
    public bool GetPlayerHasCarrionRose => playerHasCarrionRose;
    public void SetPlayerHasCarrionRose(bool value) { playerHasCarrionRose = value; }

    Coroutine activateSign;

    [SerializeField] TurnOrganiser turnOrganiser;
    [SerializeField] CombatPhaseResolution combatPhaseController;
    [SerializeField] PlayerStatReceiver playerStatReceiver;

    [SerializeField] PlayerMovementController playerMovementController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerIsAlive = true;
        

        healthPlus.SetActive(false);
        healthNeg.SetActive(false);
        attackPlus.SetActive(false);
        attackNeg.SetActive(false);
        sufferingPlus.SetActive(false);
        sufferingNeg.SetActive(false);
        moneyPlus.SetActive(false);
        moneyNeg.SetActive(false);

        SetStartingStats();
        UpdateNumbersDisplay();
    }


    public void SetStartingStats()
    {
        if(playerStatReceiver != null)
        {
            int startingHealth = playerStatReceiver.GetStartingHealth();
            if (startingHealth != 0) { playerCurrentHealth = startingHealth; }

            int startingMoney = playerStatReceiver.GetStartingMoney();
            if (startingMoney != 0) {playerCurrentMoney = startingMoney; }

            int startSuffering = playerStatReceiver.GetStartingSuffering();
            if( startSuffering != 0) { playerCurrentSuffering = startSuffering; }

        }
        else { Debug.Log("Null Reference: PlayerStatReceiver , assigning Default Stat Values");  }

        UpdateNumbersDisplay();
    }

    public void UseItem(string itemID)
    {
        
        string statEffected = ItemCatalogue.Instance.GetItemStatEffected(itemID);

        if(statEffected != "special")
        {
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
                    if (turnOrganiser.currentPhase == TurnOrganiser.ActivePhase.combat)
                    {
                        combatPhaseController.UpdateCombatRoll();
                    }
                    break;
                default:
                    alterHealth(3);

                    break;
            }
         
        }
        else
        {
            if (itemID == "bag_of_stones")
            {
                Debug.Log("Throw BAG OF STONES");


                // Get 8 Surrounding Squares
                // If any square = trap, Activate Trap
                // Stone throw is cosmetic on top

                GameObject[] surroundingSquares = playerMovementController.GetEightSurroundingSquares();

                bool playedSoundEffect = false;

                foreach (GameObject surroundingSquare in surroundingSquares)
                {
                    SquareController sq = surroundingSquare.GetComponent<SquareController>();

                    if(sq != null)
                    {
                        if(sq.IsTrapSquare && !sq.TrapActivated)
                        {
                            if(playedSoundEffect == false)
                            {
                                sq.ActivateTrap(playSoundEffect: true);
                                playedSoundEffect = true;
                            }
                            else sq.ActivateTrap(playSoundEffect : false);
                                

                        }
                    }
                }
            }
        }

    }


    public void alterAttack(int alterAmount)
    {
       // statBoxAnimationController.ShakeAttackAnimator();

        int before = playerCurrentAttack;
        int raw = before + alterAmount;

        playerCurrentAttack = Mathf.Clamp(raw, playerMinAttack, playerMaxAttack);

        if (alterAmount > 0)
        {
            ActivateSignForTime(attackPlus);
        }
        else if (alterAmount < 0)
        {
            ActivateSignForTime(attackNeg);
        }

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

            if(alterAmount > 0)
            {
                AudioManager.Instance.playHealthBoostSoundEffect();
                ActivateSignForTime(healthPlus);
            }
            else if (alterAmount < 0)
            {
                AudioManager.Instance.playTakeDamageSoundEffect();
                ActivateSignForTime(healthNeg);
            }


        }


       

        UpdateNumbersDisplay();

    }

    public void alterSuffering(int alterAmount)
    {
        int before = playerCurrentSuffering;
        int raw = before + alterAmount;

        playerCurrentSuffering = Mathf.Clamp(raw, playerMinSuffering, playerMaxSuffering);

        if (playerCurrentSuffering >= playerMaxSuffering)
        { 
                alterHealth(-1);
        }
        else
        {
            if (alterAmount > 0)
            {
                AudioManager.Instance.playAddSufferingSoundEffect();
                ActivateSignForTime(sufferingPlus);
            }
            else if (alterAmount < 0)
            {
                ActivateSignForTime(sufferingNeg);
            }
        }



            UpdateNumbersDisplay();
    }


    public void resetSuffering()
    {
        playerCurrentSuffering = 0;
        UpdateNumbersDisplay();
    }

    void SendPlayerDeath()
    {
        if(!playerHasDied)
        {
            playerHasDied = true;

            turnOrganiser.OnPlayerDeath();
        }
    }


    void UpdateNumbersDisplay()
    {
        if (playerCurrentHealth > playerMinHealth)
        {
            playerIsAlive = true;
            healthDisplay.text = playerCurrentHealth.ToString();

            if(playerCurrentHealth <= 10)
            {
                healthDisplay.color = Color.red;
            }
            else
            {
                healthDisplay.color = Color.white; 
            }
                sufferingDisplay.text = playerCurrentSuffering.ToString();
            if(playerCurrentSuffering >= 8)
            {
                sufferingDisplay.color = Color.red;
            }
            else
            {
                sufferingDisplay.color= Color.white;
            }

                attackDisplay.text = playerCurrentAttack.ToString();
            moneyDisplay.text = playerCurrentMoney.ToString();
            
        }
        else if(!playerHasCarrionRose)
        {
            playerIsAlive = false ;
            healthDisplay.text = "00";
            healthDisplay.color = Color.red;
            sufferingDisplay.text = "00";
            sufferingDisplay.color = Color.white;

            if (!playerHasDied )
            {
                SendPlayerDeath();
            }

        }
        else if(playerHasCarrionRose)
        {
            healthDisplay.text = "00";
            healthDisplay.color = Color.red;
        }

        

    }

    void ActivateSignForTime(GameObject sign)
    {
        // Turn all signs off first so we don't leave a stale one on
        healthPlus.SetActive(false);
        healthNeg.SetActive(false);
        attackPlus.SetActive(false);
        attackNeg.SetActive(false);
        sufferingPlus.SetActive(false);
        sufferingNeg.SetActive(false);
        moneyPlus.SetActive(false);
        moneyNeg.SetActive(false);

        sign.SetActive(true);
        if (activateSign != null)
        {
            StopCoroutine(activateSign);
        }

        activateSign = StartCoroutine(DeActivateSignAfterTime(sign));

    }

    IEnumerator DeActivateSignAfterTime(GameObject sign)
    {
        yield return new WaitForSeconds(1);
        sign.SetActive(false);
        activateSign = null;
    }

    public int GetPlayerCurrentHealth()
    {
        return playerCurrentHealth;
        
    }
    public int GetPlayerMinHealth()
    {
        return playerMinHealth;
    }

    public int GetPlayerMaxHealth()
    {
        return playerMaxHealth;
    }

    public int GetPlayerCurrentSuffering()
    {
        return playerCurrentSuffering;
    }

    public int GetPlayerMaxSuffering()
    {
        return playerMaxSuffering;
    }

    public int GetPlayerMinSuffering()
    {
        return playerMinSuffering;
    }

    public void AlterMoney(int alterAmount)
    {
        int before = playerCurrentMoney;
        int raw = before + alterAmount;

        playerCurrentMoney = Mathf.Clamp(raw, playerMinMoney, playerMaxMoney);

        if (alterAmount > 0)
        {
            AudioManager.Instance.playAddMoneySoundEffect();
            ActivateSignForTime(moneyPlus);
        }
        else if (alterAmount < 0)
        {
            ActivateSignForTime(moneyNeg);
        }


        UpdateNumbersDisplay();
    }

    public int GetPlayerCurrentMoney() => playerCurrentMoney;

    public int GetPlayerMinMoney() => playerMinMoney;

    public int GetPlayerMaxMoney() => playerMaxMoney;

}