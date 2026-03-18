using UnityEngine;

public class HouseBuilder : MonoBehaviour
{
    [SerializeField] GameObject groundFloor;
    [SerializeField] GameObject fullFloor;
    [SerializeField] GameObject halfFloor;
    [SerializeField] GameObject topFloor;
    [SerializeField] GameObject[] topFloors;

    [SerializeField] Transform houseBasePosition;
    [SerializeField] Transform houseParent;

    [SerializeField] int maxMidFloorCount;


    void Start()
    {
        BuildHouse();
    }

    void BuildHouse()
    {
        houseParent.rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90f, 0);

        int middleFloorCount = UnityEngine.Random.Range(0, maxMidFloorCount);

        GameObject currentFloor = SpawnFloorAtPoint(groundFloor, houseBasePosition.position, houseParent);
        BuildingFloorPositions currentFloorPos = currentFloor.GetComponent<BuildingFloorPositions>();

        for (int i = 0; i < middleFloorCount; i++)
        {
            GameObject nextFloor = Random.value < 0.7f ? fullFloor : halfFloor;

            currentFloor = SpawnFloorAligned(nextFloor, currentFloorPos.FloorTopPosition, houseParent);
            currentFloorPos = currentFloor.GetComponent<BuildingFloorPositions>();
        }

        int roofIndex = UnityEngine.Random.Range(0, topFloors.Length);

        SpawnFloorAligned(topFloors[roofIndex], currentFloorPos.FloorTopPosition, houseParent);
    }

    GameObject SpawnFloorAtPoint(GameObject floorPrefab, Vector3 targetBottomPoint, Transform parent)
    {
        GameObject newFloor = Instantiate(floorPrefab, parent);
        newFloor.transform.localPosition = Vector3.zero;

        BuildingFloorPositions floorPositions = newFloor.GetComponent<BuildingFloorPositions>();

        if (floorPositions == null)
        {
            Debug.LogError($"{floorPrefab.name} is missing BuildingFloorPositions component.");
            return newFloor;
        }

        if (floorPositions.FloorBottomPosition == null)
        {
            Debug.LogError($"{floorPrefab.name} is missing FloorBottomPosition reference.");
            return newFloor;
        }

        Vector3 offset = targetBottomPoint - floorPositions.FloorBottomPosition.position;
        newFloor.transform.position += offset;

        return newFloor;
    }

    GameObject SpawnFloorAligned(GameObject floorPrefab, Transform targetTopPosition, Transform parent)
    {
        return SpawnFloorAtPoint(floorPrefab, targetTopPosition.position, parent);
    }
}