using GorrotGame;
using UnityEngine;

public class SquareSpriteLibrary : MonoBehaviour
{
   public static SquareSpriteLibrary Instance { get; private set; }


    [SerializeField] GameObject borderSquare;
    [SerializeField] GameObject borderCornerSquare;


    [SerializeField] Sprite[] healthSprites;

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
    
    public Sprite GetHealthSprite(SquareSize squareSize)
    {

        switch (squareSize)
        {
            case SquareSize.Small:
                return healthSprites[0];
                
            case SquareSize.Medium:
                return healthSprites[1];
              
            case SquareSize.Large:
                return healthSprites[2];
              
            default:
                return healthSprites[1];
              
        }
    }

    public GameObject BorderSquare => borderSquare;


    public GameObject BorderCornerSquare => borderCornerSquare;
    
}
