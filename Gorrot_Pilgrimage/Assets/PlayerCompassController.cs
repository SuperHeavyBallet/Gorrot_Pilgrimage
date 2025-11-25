using System.Collections.Generic;
using UnityEngine;

public class PlayerCompassController : MonoBehaviour
{
    public GameObject goalCompassContainer;
    public GameObject treasureCompassContainer;

    GameObject goalSquare;
    Transform goalPosition;

    public bool goalLocated = false;
    public bool treasurePresent = false;
    List<GameObject> treasureLocationList = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (goalLocated)
        {

            // Direction from compass to goal, in XY plane
            Vector3 dir = goalPosition.position - goalCompassContainer.transform.position;
            dir.z = 0f; // ignore depth so we don't tilt out of plane

            if (dir.sqrMagnitude < 0.0001f)
                return;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // If your arrow sprite/model points "up" by default (along +Y), subtract 90
            float angleOffset = -90f;

            // Only change Z, keep X/Y as they are
            Vector3 euler = goalCompassContainer.transform.eulerAngles;
            euler.z = angle + angleOffset; // + angleOffset if needed
            goalCompassContainer.transform.eulerAngles = euler;
        }

        if(treasureLocationList.Count > 0)
        {
            float shortestDistance = 1000;
            GameObject closestTreasure = null;

            foreach(GameObject go in treasureLocationList)
            {
                float distanceToThisTreasure = Vector2.Distance(treasureCompassContainer.transform.position , go.transform.position);

                if(distanceToThisTreasure < shortestDistance)
                {
                    shortestDistance = distanceToThisTreasure;
                    closestTreasure = go;
                }    

            }

            if(closestTreasure != null)
            {
                Vector3 dir = closestTreasure.transform.position - treasureCompassContainer.transform.position;
                dir.z = 0f;

                if (dir.sqrMagnitude < 0.0001f)
                    return;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                float angleOffset = -90f;

                Vector3 euler = treasureCompassContainer.transform.eulerAngles;
                euler.z = angle + angleOffset;
                treasureCompassContainer.transform.eulerAngles = euler;
            }
            
        }

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
