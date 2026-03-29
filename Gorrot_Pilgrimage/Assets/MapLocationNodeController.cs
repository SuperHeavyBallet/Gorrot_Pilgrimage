using UnityEngine;
using GorrotGame;
using TMPro;

public class MapLocationNodeController : MonoBehaviour
{

    [SerializeField] MapNames thisMapName;

    public MapNames ThisMapName => thisMapName;

    [SerializeField] TextMeshProUGUI nameDisplay;
    
    void SetNameDisplay()
    {
        if(nameDisplay != null)
        {
            nameDisplay.text = ThisMapName.ToString();
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetNameDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
