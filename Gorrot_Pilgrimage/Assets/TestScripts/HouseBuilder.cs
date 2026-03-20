using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class HouseBuilder : MonoBehaviour
{
    [SerializeField] GameObject groundFloor;
    [SerializeField] GameObject[] fullFloors;
    [SerializeField] GameObject halfFloor;
    [SerializeField] GameObject topFloor;
    [SerializeField] GameObject[] topFloors;

    [SerializeField] Transform houseBasePosition;
    [SerializeField] Transform houseParent;

    [SerializeField] int maxMidFloorCount;

    [SerializeField] TextAsset namesFile;

    Coroutine buildNewHouse;

 

    void Start()
    {
        BuildHouse();
       // StartCoroutine(RebuildLoop());
    }

    IEnumerator RebuildLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 10f));
            DestroyOldHouse();
            BuildHouse();
        }
    }

    void DestroyOldHouse()
    {

        for (int i = houseParent.childCount - 1; i >= 0; i--)
        {
            Destroy(houseParent.GetChild(i).gameObject);
        }
    }
    class House
    {
        GameObject groundFloor;
 
        List<GameObject> midFloors = new List<GameObject>();
        GameObject topFloor;

        string[] names;
        string houseName;
        TextAsset namesFile;

        public void SetGroundFloor(GameObject newGroundFloor) => groundFloor = newGroundFloor;
        public void AddMidFloor(GameObject newMidFloor) => midFloors.Add(newMidFloor);
        public void SetTopFloor(GameObject newTopFloor) => topFloor = newTopFloor;

       public string GetFloorNames() => "Ground: " + groundFloor.name + ", Mid: " + midFloors.Count + "Top: " + topFloor.name;

        public void SetNamesFile(TextAsset newNamesFile)
        {
            namesFile = newNamesFile;
        }
        public void SetHouseName()
        {
           
            TopFloorHouseNameBuilder nameBuilder = topFloor.GetComponent<TopFloorHouseNameBuilder>();

            if (nameBuilder != null)
            {
                GenerateName();
               nameBuilder.SetHouseName(houseName);
            }
            else
            {
                Debug.LogError("No Name Builder Found");
            }
              
        }

        public void GenerateName()
        {
            names = ParseLines(namesFile);
            houseName = names[UnityEngine.Random.Range(0, names.Length)];
        }

        string[] ParseLines(TextAsset file)
        {
            if (file == null) return Array.Empty<string>();

            return file.text
               .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
               .Select(s => s.Trim())
               .Where(s => s.Length > 0 && !s.StartsWith("#")) // allow comments
               .ToArray();
        }

    }

   

    void BuildHouse()
    {
        House newHouse = new House();


        houseParent.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 4) * 90f, 0);

        int middleFloorCount = UnityEngine.Random.Range(0, maxMidFloorCount);

        GameObject currentFloor = SpawnFloorAtPoint(groundFloor, houseBasePosition.position, houseParent);

        newHouse.SetGroundFloor(currentFloor);

        BuildingFloorPositions currentFloorPos = currentFloor.GetComponent<BuildingFloorPositions>();

        for (int i = 0; i < middleFloorCount; i++)
        {
            int randomFloor = UnityEngine.Random.Range(0, fullFloors.Length);
            GameObject nextFloor = UnityEngine.Random.value < 0.7f ? fullFloors[randomFloor] : halfFloor;

            GameObject spawnedMidFloor = SpawnFloorAligned(nextFloor, currentFloorPos.FloorTopPosition, houseParent);
            newHouse.AddMidFloor(spawnedMidFloor);

            currentFloor = spawnedMidFloor;
            currentFloorPos = currentFloor.GetComponent<BuildingFloorPositions>();
        }

        int roofIndex = UnityEngine.Random.Range(0, topFloors.Length);

        GameObject spawnedRoof = SpawnFloorAligned(topFloors[roofIndex], currentFloorPos.FloorTopPosition, houseParent);

        newHouse.SetTopFloor(spawnedRoof);
        //newHouse.SetNamesFile(namesFile);
        //newHouse.SetHouseName();

        //Debug.Log(newHouse.GetFloorNames());

       
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