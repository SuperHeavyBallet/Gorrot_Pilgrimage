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



}
