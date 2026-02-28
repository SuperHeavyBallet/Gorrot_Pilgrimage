using UnityEngine;

public class StandeeController : MonoBehaviour
{

    [SerializeField] SpriteRenderer frontSprite;
    [SerializeField] SpriteRenderer backSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSprites(Sprite fSpr, Sprite bSpr)
    {
        if (fSpr != null) frontSprite.sprite = fSpr;

        if(bSpr != null) backSprite.sprite = bSpr;
    }
}
