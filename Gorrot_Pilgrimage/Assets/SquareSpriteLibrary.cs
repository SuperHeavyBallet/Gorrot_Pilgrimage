using UnityEngine;

public class SquareSpriteLibrary : MonoBehaviour
{
    public static SquareSpriteLibrary Instance { get; private set; }

    public Sprite[] terrainSprites;

    public GameObject borderSquare;

    public Sprite[] midworldGround;
    public Sprite[] outworldGround;

    public Sprite[] outswampGround;
    public Sprite[] inswampGround;

    public Sprite[] genericGround;

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

    public Sprite GetRandomGroundSprite(string mapLocation)
    {
        int randomNumber = 0;

        if (mapLocation == "OuterGorrot")
        {
            randomNumber = UnityEngine.Random.Range(0, outswampGround.Length);
            return outswampGround[randomNumber];
        }
        else if (mapLocation == "InnerGorrot")
        {
            randomNumber = UnityEngine.Random.Range(0, inswampGround.Length);
            return inswampGround[randomNumber];
        }
        else if (mapLocation == "Midworld")
        {
            randomNumber = UnityEngine.Random.Range(0, midworldGround.Length);
            return midworldGround[randomNumber];
        }
        else if(mapLocation == "Outworld")
        {
            randomNumber = UnityEngine.Random.Range(0, outworldGround.Length);
            return outworldGround[randomNumber];
        }
        else
        {
            randomNumber = UnityEngine.Random.Range(0, genericGround.Length);
            return genericGround[randomNumber];
        }
    }

    public GameObject getBorderSquare()
    {
        return borderSquare;
    }
}
