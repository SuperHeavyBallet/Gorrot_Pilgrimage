using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class TurnOrganiser : MonoBehaviour
{

    public bool isPlayerTurn;
    public int playerTurnCooldownTime = 5;

    public AudioManager audioManager;

    public TextMeshProUGUI turnDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enablePlayerTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void disablePlayerTurn()
    {
        audioManager.changeTurnSound("enemy");
        isPlayerTurn = false;
        turnDisplay.text = "Turn: Enemy";

        Invoke("enablePlayerTurn", playerTurnCooldownTime);
    }

    public void enablePlayerTurn()
    {
        audioManager.changeTurnSound("player");
        isPlayerTurn = true; 
        turnDisplay.text = "Turn: Player";
        
    }

    public bool GetPlayerTurn()
    {
        return isPlayerTurn;
    }
}
