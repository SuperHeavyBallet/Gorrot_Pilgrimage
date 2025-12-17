using UnityEngine;

public class BorderSquareController : MonoBehaviour
{
    [SerializeField] GameObject borderShadow;

    public string shadowSide;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        borderShadow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PositionBorderShadow(string side)
    {
        borderShadow.SetActive(true);

        shadowSide = side;

        float z = side switch
        {
            "top" => 90f,
            "right" => 0,
            "bottom" => -90f,
            "left" => 180f,
            _ => 180f
        };

        borderShadow.transform.localEulerAngles = new Vector3(0f, 0f, z);
    }
}
