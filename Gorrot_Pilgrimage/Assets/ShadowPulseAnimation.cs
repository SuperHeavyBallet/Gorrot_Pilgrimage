using UnityEngine;

public class ShadowPulseAnimation : MonoBehaviour
{

    public GameObject shadow;
    float startingSize;
    float maxSizeDelta = 0.03f;
    float minSizeDelta = 0.01f;
    float maxSize;
    float minSize;
    float changeRate = 0.0001f;

    bool shouldShrink = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingSize = shadow.transform.localScale.x;
        maxSize = startingSize + maxSizeDelta;
        minSize = startingSize - minSizeDelta;
    }

    // Update is called once per frame
    void Update()
    {

        if(shadow.transform.localScale.x >= maxSize)
        {
            shouldShrink = true;
        }

        if(shadow.transform.localScale.x <= minSize)
        {
            shouldShrink = false;
        }

        if(shouldShrink)
        {
            shadow.transform.localScale = new Vector3(shadow.transform.localScale.x - changeRate, shadow.transform.localScale.y - changeRate, 1);
        }
        else
        {
            shadow.transform.localScale = new Vector3(shadow.transform.localScale.x + changeRate, shadow.transform.localScale.y + changeRate, 1);
        }
    }
}
