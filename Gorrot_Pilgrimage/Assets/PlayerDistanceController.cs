using TMPro;
using UnityEngine;

public class PlayerDistanceController : MonoBehaviour
{
    Transform goalPosition;

    private int distanceToGoal = 0;
    private string currentDistanceText = "Very Far";

    public TextMeshProUGUI distanceDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        calculateDistanceToGoal();
    }

    public void SetGoalLocation(GameObject newGoal)
    {
        goalPosition = newGoal.transform;
    }

    void calculateDistanceToGoal()
    {
        float newDistanceToGoal = Vector2.Distance(this.transform.position, goalPosition.position);
        distanceToGoal = Mathf.RoundToInt(newDistanceToGoal);

        UpdateDistanceText();


    }

    public void UpdateDistanceText()
    {
     

        string currentText = "Somewhere";

        if(distanceToGoal >= 15)
        {
            currentText = "Very Far";
        }
        else if (distanceToGoal >= 10 && distanceToGoal < 15)
        {
            currentText = "Far";
        }
        else if (distanceToGoal >= 5 && distanceToGoal < 10)
        {
            currentText = "Nearby";
        }
        else if (distanceToGoal < 5)
        {
            currentText = "Very Close";
        }

        distanceDisplay.text = "Goal: " + currentText;
    }
}

