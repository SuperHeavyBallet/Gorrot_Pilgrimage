using System.Collections.Generic;
using UnityEngine;

public class PlayerCompassController : MonoBehaviour
{
    public GameObject goalCompassContainer;
    //public GameObject treasureCompassContainer;

    GameObject goalSquare;
    Transform goalPosition;

    public bool goalLocated = false;
    public bool treasurePresent = false;
    List<GameObject> treasureLocationList = new List<GameObject>();

    [SerializeField] float rotationSmoothTime = 0.15f;
    float angularVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!goalLocated || goalPosition == null)
            return;

        Vector3 dir = goalPosition.position - goalCompassContainer.transform.position;
        dir.z = 0f;

        if (dir.magnitude < 0.25f)
            return;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = goalCompassContainer.transform.eulerAngles.z;

        float smoothedAngle = Mathf.SmoothDampAngle(
            currentAngle,
            targetAngle,
            ref angularVelocity,
            rotationSmoothTime
        );

        Vector3 euler = goalCompassContainer.transform.eulerAngles;
        euler.z = smoothedAngle;
        goalCompassContainer.transform.eulerAngles = euler;



    }

    public void SetGoalLocation(GameObject goal)
    {
        goalSquare = goal;
        goalPosition = goalSquare.transform;
        goalLocated = true;
    }

    public void SetTreasureLocation(GameObject treasure)
    {
        
        treasureLocationList.Add(treasure);
    }
}
