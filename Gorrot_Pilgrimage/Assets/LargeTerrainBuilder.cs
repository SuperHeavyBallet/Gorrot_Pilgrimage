using System.Collections.Generic;
using UnityEngine;

public class LargeTerrainBuilder : MonoBehaviour
{
    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    List<GameObject> candidateBaseSquaresforLargeItems = new List<GameObject>();

    public void TestSpaceForBigPieces()
    {
        candidateBaseSquaresforLargeItems.Clear();

        // 0..1 where 0 = none, 1 = "as many as possible"
        float density = Mathf.Clamp01(battlefieldBuilder.ThisMap.GetFourBlockTerrainDensity());

        List<Vector2Int> candidates = new List<Vector2Int>();
        Vector2Int[] freeSquaresArray = battlefieldBuilder.FreeSquares.ToArray();





        for (int i = 0; i < freeSquaresArray.Length; i++)
        {
            int x = freeSquaresArray[i].x;
            int y = freeSquaresArray[i].y;

            // quick bounds guard: since we need x+1 and y+1, base must not be on top/right edge
            if (y == 0) continue;
            if (x < 0 || y < 0 || x + 1 >= battlefieldBuilder.ThisMap.GetMapSize() || y + 1 >= battlefieldBuilder.ThisMap.GetMapSize())
                continue;



            SquareController baseSq = battlefieldBuilder.AllSquares[x, y].GetComponent<SquareController>();
            if (baseSq == null) continue;
            if (!baseSq.IsEmptySquare) continue;
            if (baseSq.IsSacred) continue;


            // Check the other 3 tiles of the 2x2 are empty & not sacred
            Vector2Int[] dirs =
            {
            new Vector2Int(0, 1),  // N
            new Vector2Int(1, 0),  // E
            new Vector2Int(1, 1),  // NE
        };

            bool allFree = true;
            for (int j = 0; j < dirs.Length; j++)
            {
                int nx = x + dirs[j].x;
                int ny = y + dirs[j].y;

                SquareController nSq = battlefieldBuilder.AllSquares[nx, ny].GetComponent<SquareController>();
                if (nSq == null || nSq.IsSacred || !nSq.IsEmptySquare)
                {
                    allFree = false;
                    break;
                }
            }

            if (!allFree) continue;

            // IMPORTANT: at this stage only require the halo to be clear based on CURRENT state
            if (!HaloIsClearForBigTerrain(x, y, 1)) continue;

            candidates.Add(new Vector2Int(x, y));

        }

        if (candidates.Count == 0 || density <= 0f)
            return;

        int targetCount = Mathf.RoundToInt(candidates.Count * density);

        // Optional: add a bit of RNG wobble so maps don't feel "samey"
        // e.g. density 0.5 gives ~0.4..0.6
        // targetCount = Mathf.Clamp(Mathf.RoundToInt(candidates.Count * Mathf.Clamp01(density + UnityEngine.Random.Range(-0.1f, 0.1f))), 0, candidates.Count);

        // 3) Place up to targetCount, re-checking halo each time because earlier placements changed the board
        Shuffle(candidates);

        int placed = 0;
        int guard = 0;

        for (int i = 0; i < candidates.Count && placed < targetCount && guard < 50000; i++)
        {
            guard++;

            int x = candidates[i].x;
            int y = candidates[i].y;

            // Re-check after previous placements
            if (!HaloIsClearForBigTerrain(x, y, 1)) continue;

            var baseSq = battlefieldBuilder.AllSquares[x, y].GetComponent<SquareController>();
            if (baseSq == null || !baseSq.IsEmptySquare || baseSq.IsSacred) continue;

            // Reserve halo so later ones can't steal it
            ReserveHaloAroundBigTerrain(x, y, 1);

            // Place 2x2
            baseSq.MakeEmptyTerrainSquare();
            battlefieldBuilder.AllSquares[x, y + 1].GetComponent<SquareController>().MakeEmptyTerrainSquare();
            battlefieldBuilder.AllSquares[x + 1, y].GetComponent<SquareController>().MakeEmptyTerrainSquare();
            battlefieldBuilder.AllSquares[x + 1, y + 1].GetComponent<SquareController>().MakeEmptyTerrainSquare();

            // Add Rows to prevent Overhead spans
            battlefieldBuilder.AddRowToForbiddenForOverhead(y);
            battlefieldBuilder.AddRowToForbiddenForOverhead(y+1);

            // Reserve base for sprite activation
            candidateBaseSquaresforLargeItems.Add(battlefieldBuilder.AllSquares[x, y]);

            // Remove these from freeSquares
            battlefieldBuilder.FreeSquares.Remove(new Vector2Int(x, y));
            battlefieldBuilder.FreeSquares.Remove(new Vector2Int(x, y + 1));
            battlefieldBuilder.FreeSquares.Remove(new Vector2Int(x + 1, y));
            battlefieldBuilder.FreeSquares.Remove(new Vector2Int(x + 1, y + 1));

            placed++;
        }

        // Activate sprites for bases placed
        for (int i = 0; i < candidateBaseSquaresforLargeItems.Count; i++)
        {
            var candSC = candidateBaseSquaresforLargeItems[i].GetComponent<SquareController>();
            candSC.ActivateLargeTerrainSprite();
        }

    }

    // Fisher–Yates shuffle
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    void ReserveHaloAroundBigTerrain(int baseX, int baseY, int haloRadius = 1)
    {
        // The 2x2 occupies: (baseX..baseX+1, baseY..baseY+1)
        // Halo area becomes: (baseX-1..baseX+2, baseY-1..baseY+2) for radius 1
        int minX = baseX - haloRadius;
        int maxX = baseX + 1 + haloRadius;
        int minY = baseY - haloRadius;
        int maxY = baseY + 1 + haloRadius;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (!battlefieldBuilder.InBounds(x, y)) continue;
                if (y == 0) continue;

                var sc = battlefieldBuilder.AllSquares[x, y].GetComponent<SquareController>();
                if (sc == null) continue;

                if (sc.IsSacred) continue;
                if (sc.IsWater) continue;

                // Mark as reserved walkway (halo)
                sc.SetReservedWalkway(true);

                // Reserve by removing from placement pool
                // freeSquares.Remove(new Vector2Int(x, y));
            }
        }
    }

    bool HaloIsClearForBigTerrain(int baseX, int baseY, int haloRadius = 1)
    {
        int minX = baseX - haloRadius;
        int maxX = baseX + 1 + haloRadius;
        int minY = baseY - haloRadius;
        int maxY = baseY + 1 + haloRadius;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (!battlefieldBuilder.InBounds(x, y)) continue;
                if (y == 0) continue;

                var sc = battlefieldBuilder.AllSquares[x, y].GetComponent<SquareController>();
                if (sc == null) return false;

                // halo must remain walkable
                if (sc.IsSacred) return false;
                if (sc.IsWater) return false;
                if (!sc.IsEmptySquare) return false; // strict version
            }
        }
        return true;
    }

}
