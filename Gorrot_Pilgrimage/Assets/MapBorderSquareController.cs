using UnityEngine;
using GorrotGame;

public class MapBorderSquareController : MonoBehaviour
{
    bool leftEmpty;
    bool upEmpty;
    bool rightEmpty;
    bool downEmpty;

    BattlefieldBuilder builder;
    MapData thisMap;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBorderSquare(int[] sides, BattlefieldBuilder battlefieldBuilder)
    {
        builder = battlefieldBuilder;
        thisMap = builder.GetThisMap();

        int squareLeft = sides[0];
        int squareUp = sides[1];
        int squareRight = sides[2];
        int squareBottom = sides[3];

        leftEmpty = (sides[0] == 1);
        upEmpty = (sides[1] == 1);
        rightEmpty = (sides[2] == 1);
        downEmpty = (sides[3] == 1);

        float thisSquareSize = this.transform.localScale.x;

        Vector3 baseLocalPos = transform.localPosition;

        if (leftEmpty && upEmpty)
            MakeCornerBorderAtPosition(baseLocalPos + new Vector3(-thisSquareSize,  0f, thisSquareSize), CornerPositions.NorthWest);

        if (rightEmpty && upEmpty)
            MakeCornerBorderAtPosition(baseLocalPos + new Vector3(thisSquareSize,  0f, thisSquareSize), CornerPositions.NorthEast);

        if (leftEmpty && downEmpty)
            MakeCornerBorderAtPosition(baseLocalPos + new Vector3(-thisSquareSize, 0f, -thisSquareSize), CornerPositions.SouthWest);

        if (rightEmpty && downEmpty)
            MakeCornerBorderAtPosition(baseLocalPos + new Vector3(thisSquareSize, 0f, -thisSquareSize), CornerPositions.SouthEast);

        if (leftEmpty) MakeBorderSquareAtPosition(baseLocalPos + Vector3.left * thisSquareSize, OrthogonalPositions.West);
        if (rightEmpty) MakeBorderSquareAtPosition(baseLocalPos + Vector3.right * thisSquareSize, OrthogonalPositions.East);
        if (upEmpty) MakeBorderSquareAtPosition(baseLocalPos + Vector3.forward * thisSquareSize, OrthogonalPositions.North);
        if (downEmpty) MakeBorderSquareAtPosition(baseLocalPos + Vector3.back * thisSquareSize, OrthogonalPositions.South);
    }

    public void MakeCornerBorderAtPosition(Vector3 localPosition, CornerPositions cornerPos)
    {
        GameObject newCornerBorderSquare = Instantiate(
            SquareSpriteLibrary.Instance.BorderCornerSquare,
            transform.parent
        );

        newCornerBorderSquare.transform.localPosition = localPosition;
        

       

        Vector3 rotationEuler = new Vector3(0, 0, 0);

        switch (cornerPos)
        {
            case CornerPositions.NorthEast:
                rotationEuler = new Vector3(0f, 90f, 0f);
                break;
            case CornerPositions.SouthEast:
                rotationEuler = new Vector3(0f, 180f, 0f);
                break;
            case CornerPositions.SouthWest:
                rotationEuler = new Vector3(0f, -90f, 0f);
                break;
                default:
                rotationEuler = new Vector3(0f, 0f, 0f);
                break;
        }

        newCornerBorderSquare.transform.localRotation = Quaternion.Euler(rotationEuler);

        BorderSquareController borderSquareController = newCornerBorderSquare.GetComponent<BorderSquareController>();

        if (borderSquareController != null)
        {
            borderSquareController.SetWallTopDressing(MapAssetsController.Instance.GetWallDresings(thisMap.GetMapLocation()));
        }

        builder.AddCornerBorderSquareToList(newCornerBorderSquare, cornerPos);
    }

    public void MakeBorderSquareAtPosition(Vector3 localPosition, OrthogonalPositions borderPos)
    {
        GameObject newBorderSquare = Instantiate(
            SquareSpriteLibrary.Instance.BorderSquare,
            transform.parent
        );

        newBorderSquare.transform.localPosition = localPosition;
        newBorderSquare.transform.localRotation = Quaternion.identity;

        BorderSquareController borderSquareController = newBorderSquare.GetComponent<BorderSquareController>();

        if( borderSquareController != null )
        {
            borderSquareController.SetWallTopDressing(MapAssetsController.Instance.GetWallDresings(thisMap.GetMapLocation()));
        }

        builder.AddBorderSquareToList(newBorderSquare, borderPos);
    }
}
