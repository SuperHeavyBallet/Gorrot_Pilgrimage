using UnityEngine;

public class FatePhaseResolution : MonoBehaviour
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

    public void EnterFatePhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.fate);
    }

    public void ExitFatePhase()
    {
        CloseFateScene();   
    }


    void OpenFateScene()
    {

    }


    void CloseFateScene()
    {

    }
}
