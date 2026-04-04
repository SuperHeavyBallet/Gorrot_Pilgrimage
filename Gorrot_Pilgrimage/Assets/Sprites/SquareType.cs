using System.Collections.Generic;
using UnityEngine;

namespace GorrotGame
{
    public enum SquareType
    {
        Empty,
        Goal,
        Start,
        Treasure,
        Enemy,
        Terrain,
        Health,
        Item,
        Trap,
        Merchant,
        Water
    }

    public enum SquareSize
    {
        Small,
        Medium,
        Large,
    }

    public enum SquareMood
    {
        Positive,
        Negative,
    }

    public enum NamedNPCS
    {
        Pottard,
    }

    public enum ItemNames
    {
        Any,
        Flower,
    }

    public enum OrthogonalPositions
    {
        North,
        East,
        South,
        West
    }

    public enum CornerPositions
    {
        NorthEast,
        SouthEast,
        SouthWest,
        NorthWest
    }


    public enum BridgeOrientation
    {
        None,
        Horizontal,
        Vertical
    }

    public enum MapNames
    {
        Fetsmeld,
        Garthun,
        Farhnith,
        Semsun,
        Hindruhn,
        Katharn,
        Odrikloft,
        Molgeritch,
        Arx_Thronus,
        Dwindir,
        Halbegorn,
        Wyrsmet,
        Limmut,
        Kangiel,
        Ithen,
        Borgen,
        Ritten,
        Myr,
        Hingrel,
        Imthuhl,
        Gorrot_Town,
        Gorrot_Church,
        Gorrot_Waiting_Hall,
        Gorrot_Corridoor,
        Gorrot

    }

    public enum MapBiomeType
    {
        Forest,
        Swamp,
        Steppe,
        Mountain,

    }

    public enum MapSettlementType
    {
        None,
        Village,
        Town,
        City
    }

    public static class GridUtilities
    {
        public static bool IsInsideGrid(int x, int y, int gridWidth, int gridHeight)
        {
            return x >= 0 && x < gridWidth &&
             y >= 0 && y < gridHeight;
        }


        public static GameObject[] GetEightSurroundingSquares(Vector2Int currentPosition, int gridWidth, int gridHeight, GameObject[,] allSquares )
        {
            Vector2Int playerCurrentPosition = currentPosition;

            List<GameObject> neighbours = new List<GameObject>();

            int x = currentPosition.x;
            int y = currentPosition.y;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue; // skip the centre square

                    int nx = x + dx;
                    int ny = y + dy;

                    // boundary check (important!)
                    if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
                    {
                        neighbours.Add(allSquares[nx, ny]);
                    }
                }
            }

            // Maybe later
            Vector2Int[] offsets =
            {
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1)
        };



            return neighbours.ToArray();

        }

    }
}

    

