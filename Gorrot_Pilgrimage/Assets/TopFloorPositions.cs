using System.Collections.Generic;
using UnityEngine;

public class TopFloorPositions : MonoBehaviour
{

    [SerializeField] Transform[] chimneyPositions;

    [SerializeField] GameObject[] chimneys;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildChimneys();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildChimneys()
    {
        int chimneyCount = UnityEngine.Random.Range(1, 4);

        // 2 Chimneys, each must be at different positions, remove each position from the list if selected

        HashSet<int> chimneyPositionsReserved = new HashSet<int>();

        for (int i = 0; i < chimneyCount; i++)
        {
            int randomChimneyPos = UnityEngine.Random.Range(0, chimneyPositions.Length);
            if (chimneyPositionsReserved.Contains(randomChimneyPos))
            {
                randomChimneyPos = UnityEngine.Random.Range(0, chimneyPositions.Length);

                if (chimneyPositionsReserved.Contains(randomChimneyPos)) continue;
            }
            
            chimneyPositionsReserved.Add(randomChimneyPos);

            Transform chosenChimneyPosition = chimneyPositions[randomChimneyPos];

            int randomChimney = UnityEngine.Random.Range(0, chimneys.Length);

            GameObject chosenChimney = Instantiate(chimneys[randomChimney], chosenChimneyPosition, transform);
            chosenChimney.transform.localPosition = Vector3.zero;
            chosenChimney.transform.rotation = chosenChimneyPosition.transform.rotation;
        }

     



    }
}
