using UnityEngine;

public class PickRandomSpriteFromCollection : MonoBehaviour
{
    [SerializeField] Sprite[] spriteCollection;

    [SerializeField] SpriteRenderer spriteRenderer;

    Sprite chosenSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spriteCollection == null) Debug.LogError("Sprite Collection not assigned. ", this);
        if(spriteRenderer == null) Debug.LogError("Sprite Renderer not assigned. ", this);

        if(spriteCollection.Length > 0 || spriteRenderer != null)
        {
            PickSpriteFromCollection();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PickSpriteFromCollection()
    {
        if (spriteCollection == null) return;

        int chosenIndex = UnityEngine.Random.Range(0, spriteCollection.Length);
        chosenSprite = spriteCollection[chosenIndex];
        spriteRenderer.sprite = chosenSprite;

        
    }
}
