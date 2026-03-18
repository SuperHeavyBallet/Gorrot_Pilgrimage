using UnityEngine;

public class BuildingFloorPositions : MonoBehaviour
{

    [SerializeField] Transform floorBottomPosition;

    public Transform FloorBottomPosition => floorBottomPosition;
    [SerializeField] Transform floorTopPosition;
    public Transform FloorTopPosition => floorTopPosition;  


}
