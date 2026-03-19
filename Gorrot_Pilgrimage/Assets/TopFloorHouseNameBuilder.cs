using UnityEngine;
using TMPro;

public class TopFloorHouseNameBuilder : MonoBehaviour
{

    [SerializeField]    TextMeshProUGUI houseNameDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHouseName(string newName)
    {
        houseNameDisplay.text = newName;
    }
}
