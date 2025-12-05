using UnityEngine;

public class SquareSpriteLibrary : MonoBehaviour
{
    public static SquareSpriteLibrary Instance { get; private set; }

    public Sprite[] terrainSprites;

    private void Awake()
    {

        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Sprite GetRandomSprite(string spriteType)
    {
        if(spriteType == "terrain")
        {
            int randomNumber = UnityEngine.Random.Range(0, terrainSprites.Length);
            return terrainSprites[randomNumber];
        }

        return terrainSprites[1];


    }
}
