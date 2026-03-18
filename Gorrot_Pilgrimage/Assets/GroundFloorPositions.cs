using System.Collections.Generic;
using UnityEngine;

public class GroundFloorPositions : MonoBehaviour
{
    [SerializeField] Transform[] groundFloorPositions;
    [SerializeField] GameObject[] groundFloorDecorations;

    [SerializeField, Range(0f, 1f)] float fillChance = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildGroundFloorDecorations();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildGroundFloorDecorations()
    {
        foreach (Transform chosenDecorationPosition in groundFloorPositions)
        {
            if (UnityEngine.Random.value > fillChance)
                continue;

            int randomDecoration = UnityEngine.Random.Range(0, groundFloorDecorations.Length);

            GameObject chosenDecoration = Instantiate(
                groundFloorDecorations[randomDecoration],
                chosenDecorationPosition,
                false
            );

            chosenDecoration.transform.localPosition = Vector3.zero;
            chosenDecoration.transform.localRotation = Quaternion.identity;
        }
    }


}
