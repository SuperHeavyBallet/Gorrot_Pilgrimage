using UnityEngine;
using GorrotGame;

public class MapBorderSquareController : MonoBehaviour
{
    bool leftEmpty;
    bool upEmpty;
    bool rightEmpty;
    bool downEmpty;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBorderSquare(int[] sides)
    {

        int squareLeft = sides[0];
        int squareUp = sides[1];
        int squareRight = sides[2];
        int squareBottom = sides[3];

        leftEmpty = (sides[0] == 1);
        upEmpty = (sides[1] == 1);
        rightEmpty = (sides[2] == 1);
        downEmpty = (sides[3] == 1);

        float thisSquareSize = this.transform.localScale.x;

        if (leftEmpty && upEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(-thisSquareSize, thisSquareSize, 0f), CornerPositions.NorthWest);

        if (rightEmpty && upEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(thisSquareSize, thisSquareSize, 0f), CornerPositions.NorthEast);

        if (leftEmpty && downEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(-thisSquareSize, -thisSquareSize, 0f), CornerPositions.SouthWest);

        if (rightEmpty && downEmpty)
            MakeCornerBorderAtPosition(transform.position + new Vector3(thisSquareSize, -thisSquareSize, 0f), CornerPositions.SouthEast);


        if (leftEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.left * thisSquareSize, OrthogonalPositions.West);
        if (rightEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.right * thisSquareSize, OrthogonalPositions.East);
        if (upEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.up * thisSquareSize, OrthogonalPositions.North);
        if (downEmpty) MakeBorderSquareAtPosition(transform.position + Vector3.down * thisSquareSize, OrthogonalPositions.South);
    }

    public void MakeCornerBorderAtPosition(Vector3 position, CornerPositions cornerPos)
    {
        GameObject newCornerBorderSquare = Instantiate(
            SquareSpriteLibrary.Instance.GetBorderCornerSquare(),
            position,
            Quaternion.identity,
            transform.parent
        );

        switch (cornerPos)
        {
            case CornerPositions.NorthWest:
                newCornerBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, 0f);
                break;
            case CornerPositions.NorthEast:
                newCornerBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, -90f);
                break;
            case CornerPositions.SouthEast:
                newCornerBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, -180f);
                break;
            case CornerPositions.SouthWest:
                newCornerBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, 90f);
                break;
            default:
                break;
        }
    }

   public void MakeBorderSquareAtPosition(Vector3 position, OrthogonalPositions borderPos)
    {
        GameObject newBorderSquare = UnityEngine.Object.Instantiate(
            SquareSpriteLibrary.Instance.getBorderSquare(),
            position,
            Quaternion.identity,
            transform.parent
            );


        if (borderPos == OrthogonalPositions.North)
        {
            newBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, 90f);
        }
        else if (borderPos == OrthogonalPositions.South)
        {
            newBorderSquare.transform.localRotation *= Quaternion.Euler(0f, 0f, -90f);
        }

    }
}
