using UnityEngine;
using GorrotGame;

public class SquareTypeController : MonoBehaviour
{
    [SerializeField] SquareType thisSquareType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConstructSquare(SquareType sqType, MapData thisMap)
    {
        Debug.Log("Received Make: " + sqType.ToString() + " Square, for Map: " + thisMap.GetMapName());

        thisSquareType = sqType;
    }
}
