using GorrotGame;
using UnityEngine;

public class BridgeController : MonoBehaviour
{

    BridgeOrientation bridgeOrientation;
    [SerializeField] GameObject bridgeMarker;

    bool isBridge;

    public bool GetIsBridge => isBridge;

    [SerializeField] GameObject bridgeEndNorth;
    [SerializeField] GameObject bridgeEndEast;
    [SerializeField] GameObject bridgeEndSouth;
    [SerializeField] GameObject bridgeEndWest;
    [SerializeField] GameObject waterSpriteObject;
    [SerializeField] GameObject regularSquareMesh;

    [SerializeField] SquareController squareController;




    public void SetIsBridge(bool newIsBridge)
    {
        isBridge = newIsBridge;

        if (isBridge)
        {
            bridgeMarker.SetActive(true);
            MakeWaterUnderBridge();
        }
    }

    public BridgeOrientation GetBridgeOrientation()
    {
        return bridgeOrientation;
    }

    public void SetBridgeOrientation(BridgeOrientation orientation)
    {
        bridgeOrientation = orientation;

        if (orientation == BridgeOrientation.Horizontal)
        {
            bridgeMarker.transform.Rotate(0, 0, 90);
        }
        else
        {
            bridgeMarker.transform.Rotate(0, 0, 0);
        }
    }

    public void SetBridgeEndCaps(bool north, bool east, bool south, bool west)
    {

        if (bridgeEndNorth != null) bridgeEndNorth.SetActive(north);
        if (bridgeEndEast != null) bridgeEndEast.SetActive(east);
        if (bridgeEndSouth != null) bridgeEndSouth.SetActive(south);
        if (bridgeEndWest != null) bridgeEndWest.SetActive(west);
    }

    public void MakeWaterUnderBridge()
    {
        waterSpriteObject.SetActive(true);
        SpriteRenderer waterSR = waterSpriteObject.GetComponent<SpriteRenderer>();
        waterSR.material = squareController.ThisMap.WaterShader;
        regularSquareMesh.SetActive(false);
    }


}
