using UnityEngine;
using GorrotGame;

public class ContentSquareBuilder : MonoBehaviour
{
    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    int terrainSquareCount = 5;
    int healthSquareCount = 5;
    int potionSquareCount = 5;

    public void PlaceContentSquares()
    {
        MapData thisMap = battlefieldBuilder.ThisMap;

        PlaceTypeSquares(terrainSquareCount, sq => sq.MakeSquare(SquareType.Terrain, thisMap), disallowReservedWalkway: true);

        if (thisMap.GetHasEnemies == true)
        {
            int size = thisMap.GetMapSize();

            int enemyCount = Mathf.RoundToInt(size * thisMap.EnemyDensity); // EnemyDensity in 0..1
            PlaceTypeSquares(enemyCount, sq => sq.MakeSquare(SquareType.Enemy, thisMap), disallowReservedWalkway: false);

        }

        if(thisMap.HasHealthBoosts)
        {
            PlaceTypeSquares(healthSquareCount, sq => sq.MakeSquare(SquareType.Health, thisMap), disallowReservedWalkway: false);
        }
        
        PlaceTypeSquares(potionSquareCount, sq => sq.MakeSquare(SquareType.Item, thisMap), disallowReservedWalkway: false);
        PlaceTypeSquares(potionSquareCount, sq => sq.MakeSquare(SquareType.Treasure, thisMap), disallowReservedWalkway: false);

        if (thisMap.GetHasHiddenTraps == true)
        {
            PlaceTypeSquares(
                Mathf.CeilToInt(thisMap.GetMapSize() * thisMap.GetHiddenTrapDensity),
                sq => sq.MakeSquare(SquareType.Trap, thisMap), disallowReservedWalkway: true
            );
        }


        if (thisMap.GetWaterAmount() > 0)
        {
            int waterLevel = Mathf.RoundToInt(thisMap.GetWaterAmount());
            PlaceWaterFlowerSquares(waterLevel * thisMap.GetMapSize(), sq => sq.MakeFlowerSquare());
        }

    }

    void PlaceTypeSquares(int count, System.Action<SquareController> applyType, bool disallowReservedWalkway)
    {


        int placed = 0;
        int guard = 0;


        while (placed < count && battlefieldBuilder.FreeSquares.Count > 0 && guard < 100000)
        {
            guard++;

            int index = UnityEngine.Random.Range(0, battlefieldBuilder.FreeSquares.Count);
            Vector2Int coord = battlefieldBuilder.FreeSquares[index];

            SquareController sq = battlefieldBuilder.AllSquares[coord.x, coord.y].GetComponent<SquareController>();
            if (sq == null) { battlefieldBuilder.FreeSquares.RemoveAt(index); continue; }

            if (sq.IsSacred) continue;
            if (disallowReservedWalkway && sq.IsReservedWalkway) continue;

            battlefieldBuilder.FreeSquares.RemoveAt(index);
            applyType(sq);
            placed++;
        }

        if (placed < count)
            Debug.LogWarning($"Could not place full quota ({count}) for type; only placed {placed}.");

    }


    void PlaceWaterFlowerSquares(int count, System.Action<SquareController> applyType)
    {
        int placed = 0;
        int guard = 0;

        while (placed < count && battlefieldBuilder.FreeSquares.Count > 0 && guard < 100000)
        {
            guard++;

            int index = UnityEngine.Random.Range(0, battlefieldBuilder.FreeSquares.Count);
            Vector2Int coord = battlefieldBuilder.FreeSquares[index];

            SquareController sq = battlefieldBuilder.AllSquares[coord.x, coord.y].GetComponent<SquareController>();
            if (sq == null) { battlefieldBuilder.FreeSquares.RemoveAt(index); continue; }

            if (!sq.IsWaterAdjacent)
            {
                continue;
            }

            battlefieldBuilder.FreeSquares.RemoveAt(index);
            applyType(sq);
            placed++;
        }
    }

}
