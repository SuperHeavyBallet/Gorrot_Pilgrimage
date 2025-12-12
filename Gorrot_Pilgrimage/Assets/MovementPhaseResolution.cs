using UnityEngine;

public class MovementPhaseResolution : MonoBehaviour
{
    [SerializeField] TurnOrganiser turnOrganiser;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
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
