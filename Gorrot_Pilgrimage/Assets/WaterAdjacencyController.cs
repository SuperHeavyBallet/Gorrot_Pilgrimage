using GorrotGame;
using UnityEngine;

public class WaterAdjacencyController : MonoBehaviour
{
    bool isWaterAdjacent;


    [SerializeField] GameObject waterBorderNorth;
    [SerializeField] GameObject waterBorderEast;
    [SerializeField] GameObject waterBorderSouth;
    [SerializeField] GameObject waterBorderWest;

    [SerializeField] SpriteRenderer waterBorderNorthSpriteRenderer;
    [SerializeField] SpriteRenderer waterBorderEastSpriteRenderer;
    [SerializeField] SpriteRenderer waterBorderSouthSpriteRenderer;
    [SerializeField] SpriteRenderer waterBorderWestSpriteRenderer;

    [SerializeField] SpriteRenderer waterFoamNorthSpriteRenderer;
    [SerializeField] SpriteRenderer waterFoamEastSpriteRenderer;
    [SerializeField] SpriteRenderer waterFoamSouthSpriteRenderer;
    [SerializeField] SpriteRenderer waterFoamWestSpriteRenderer;

    [SerializeField] SpriteRenderer[] foamObjects;

    [SerializeField] Sprite defaultWaterBorderSprite;

    int waterAdjacencyMask;

    bool waterNorth;
    bool waterEast;
    bool waterSouth;
    bool waterWest;

    const int N = 1;
    const int E = 2;
    const int S = 4;
    const int W = 8;

    [SerializeField] GameObject waterFoamCorner_NE;
    [SerializeField] GameObject waterFoamCorner_SE;
    [SerializeField] GameObject waterFoamCorner_SW;
    [SerializeField] GameObject waterFoamCorner_NW;

    [SerializeField] SpriteRenderer waterSpriteRenderer;

    static readonly int EdgeN = Shader.PropertyToID("_EdgeN");
    static readonly int EdgeE = Shader.PropertyToID("_EdgeE");
    static readonly int EdgeS = Shader.PropertyToID("_EdgeS");
    static readonly int EdgeW = Shader.PropertyToID("_EdgeW");

    static readonly int DiagNE = Shader.PropertyToID("_DiagNE");
    static readonly int DiagNW = Shader.PropertyToID("_DiagNW");
    static readonly int DiagSE = Shader.PropertyToID("_DiagSE");
    static readonly int DiagSW = Shader.PropertyToID("_DiagSW");

    static MaterialPropertyBlock mpb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        waterBorderNorth.SetActive(false);
        waterBorderEast.SetActive(false);
        waterBorderSouth.SetActive(false);
        waterBorderWest.SetActive(false);

        waterFoamCorner_NE.SetActive(false );
        waterFoamCorner_NW.SetActive(false );
        waterFoamCorner_SE.SetActive(false );
        waterFoamCorner_SW.SetActive(false );
    }


    public void AssignWaterBorderSprites(MapData thisSquareMapData)
    {
        /*
        if (thisSquareMapData.GetWaterBorderSpritesArrayLength > 0)
        {


            if (waterBorderNorthSpriteRenderer == null)
            {
                Debug.LogError("No Water Border North Sprite Renderer Assigned", this);
                return;
            }


            if (waterBorderEastSpriteRenderer == null)
            {
                Debug.LogError("No Water Border East Sprite Renderer Assigned", this);
                return;
            }
            if (waterBorderSouthSpriteRenderer == null)
            {
                Debug.LogError("No Water Border South Sprite Renderer Assigned", this);
                return;
            }

            if (waterBorderWestSpriteRenderer == null)
            {
                Debug.LogError("No Water Border West Sprite Renderer Assigned", this);
                return;
            }

            if (waterFoamNorthSpriteRenderer == null)
            {
                Debug.LogError("No Water Foam North Sprite Renderer Assigned", this);
                return;
            }


            if (waterFoamEastSpriteRenderer == null)
            {
                Debug.LogError("No Water Foam East Sprite Renderer Assigned", this);
                return;
            }
            if (waterFoamSouthSpriteRenderer == null)
            {
                Debug.LogError("No Water Foam South Sprite Renderer Assigned", this);
                return;
            }

            if (waterFoamWestSpriteRenderer == null)
            {
                Debug.LogError("No Water Foam West Sprite Renderer Assigned", this);
                return;
            }



            waterBorderNorthSpriteRenderer.sprite = thisSquareMapData.GetRandomWaterBorderSprite();
            waterBorderEastSpriteRenderer.sprite = thisSquareMapData.GetRandomWaterBorderSprite();
            waterBorderSouthSpriteRenderer.sprite = thisSquareMapData.GetRandomWaterBorderSprite();
            waterBorderWestSpriteRenderer.sprite = thisSquareMapData.GetRandomWaterBorderSprite();

            foreach (SpriteRenderer sr in foamObjects)
            {
                sr.material = thisSquareMapData.WaterFoamShader;
            }
        }
        else
        {
            if (defaultWaterBorderSprite == null)
            {
                Debug.LogError("Default Water Border Sprite Not Assigned", this);
                return;
            }

            waterBorderNorthSpriteRenderer.sprite = defaultWaterBorderSprite;
            waterBorderEastSpriteRenderer.sprite = defaultWaterBorderSprite;
            waterBorderSouthSpriteRenderer.sprite = defaultWaterBorderSprite;
            waterBorderWestSpriteRenderer.sprite = defaultWaterBorderSprite;
        }
        */
    }

    public void SetWaterAdjacencyMask(int mask)
    {
        waterAdjacencyMask = mask;
        RebuildWaterVisuals();

    }

    void ApplyWaterEdgesToRenderer(int mask)
    {
        if (waterSpriteRenderer == null) return;

        mpb ??= new MaterialPropertyBlock();
        waterSpriteRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat(EdgeN, (mask & 1) != 0 ? 1f : 0f);
        mpb.SetFloat(EdgeE, (mask & 2) != 0 ? 1f : 0f);
        mpb.SetFloat(EdgeS, (mask & 4) != 0 ? 1f : 0f);
        mpb.SetFloat(EdgeW, (mask & 8) != 0 ? 1f : 0f);



        waterSpriteRenderer.SetPropertyBlock(mpb);
    }

    public void SetWaterDiagonalMask(int diagMask)
    {
        // diagMask bits: 1=NE, 2=NW, 4=SE, 8=SW

        if (waterSpriteRenderer == null) return;

        mpb ??= new MaterialPropertyBlock();
        waterSpriteRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat(DiagNE, (diagMask & 1) != 0 ? 1f : 0f);
        mpb.SetFloat(DiagNW, (diagMask & 2) != 0 ? 1f : 0f);
        mpb.SetFloat(DiagSE, (diagMask & 4) != 0 ? 1f : 0f);
        mpb.SetFloat(DiagSW, (diagMask & 8) != 0 ? 1f : 0f);

        waterSpriteRenderer.SetPropertyBlock(mpb);
    }

    public void SetIsWaterAdjacent(bool value)
    {
        isWaterAdjacent = value;
    }

    public bool IsWaterAdjacent => isWaterAdjacent;

    public void DisableWaterBorder(OrthogonalPositions borderSide)
    {
        int bit = 0;

        switch (borderSide)
        {
            case OrthogonalPositions.North: bit = N; break;
            case OrthogonalPositions.East: bit = E; break;
            case OrthogonalPositions.South: bit = S; break;
            case OrthogonalPositions.West: bit = W; break;
            default: return;
        }

        waterAdjacencyMask &= ~bit;

        RebuildWaterVisuals();
    }

    public void EnableWaterBorder(OrthogonalPositions borderSide)
    {
         int bit = 0;

        switch (borderSide)
        {
            case OrthogonalPositions.North: bit = N; break;
            case OrthogonalPositions.East: bit = E; break;
            case OrthogonalPositions.South: bit = S; break;
            case OrthogonalPositions.West: bit = W; break;
            default: return;
        }

        waterAdjacencyMask &= ~bit;

        RebuildWaterVisuals();
    }

    void RebuildWaterVisuals()
    {
        waterNorth = (waterAdjacencyMask & N) != 0;
        waterEast = (waterAdjacencyMask & E) != 0;
        waterSouth = (waterAdjacencyMask & S) != 0;
        waterWest = (waterAdjacencyMask & W) != 0;

        waterBorderNorth.SetActive(waterNorth);
        waterBorderEast.SetActive(waterEast);
        waterBorderSouth.SetActive(waterSouth);
        waterBorderWest.SetActive(waterWest);

        waterFoamCorner_NE.SetActive(waterNorth && waterEast);
        waterFoamCorner_SE.SetActive(waterEast && waterSouth);
        waterFoamCorner_SW.SetActive(waterSouth && waterWest);
        waterFoamCorner_NW.SetActive(waterWest && waterNorth);

        ApplyWaterEdgesToRenderer(waterAdjacencyMask);
    }
}
