using UnityEngine;

public class MovementPhaseResolution : MonoBehaviour
{
    TurnOrganiser turnOrganiser;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnOrganiser = GetComponent<TurnOrganiser>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterMovementPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.movement);
        turnOrganiser.enablePlayerTurn();
    }

    
}
