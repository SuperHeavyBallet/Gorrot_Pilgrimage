using GorrotGame;
using UnityEngine;

public class SquarePositionsController : MonoBehaviour
{
    [SerializeField] Transform centrePosition;
    [SerializeField] Transform frontPosition;
    [SerializeField] Transform backPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 DecideSquarePlayerPosition(SquareType sqType)
    {

        return sqType switch
        {
            SquareType.Goal => frontPosition.position,
            _ => centrePosition.position
        };
        
    }
}
