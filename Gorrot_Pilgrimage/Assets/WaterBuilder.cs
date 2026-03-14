using System.Collections.Generic;
using UnityEngine;
using GorrotGame;

public class WaterBuilder : MonoBehaviour
{

    [SerializeField] BattlefieldBuilder battlefieldBuilder;

   public void SetWater(float waterAmount)
    {
        int width = battlefieldBuilder.AllSquares.GetLength(0);
        int height = battlefieldBuilder.AllSquares.GetLength(1);
        int area = width * height;

        waterAmount = Mathf.Clamp01(waterAmount);
        if (waterAmount <= 0f) return;



        // Decide how much water total this map should have.
        // Tune these numbers to taste.
        int waterBudget = Mathf.RoundToInt(area * Mathf.Lerp(0.03f, 0.18f, waterAmount));
        waterBudget = Mathf.Max(1, waterBudget);

        // Decide number of rivers (streams) from waterAmount
        int riverCount = Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(1f, 4f, waterAmount)), 1, 6);

        // Rivers in wetter maps can be slightly thicker
        int baseHalfWidth = (waterAmount < 0.35f) ? 0 : 1; // 0 = 1-tile wide, 1 = up to 3 tiles wide
        int maxHalfWidth = Mathf.Clamp(baseHalfWidth + Mathf.RoundToInt(waterAmount * 2f), 0, 3);

        // Safety: don’t let rivers obliterate your sacred path if that matters.
        // If sacred path must always remain dry, we’ll skip sacred tiles when painting.
        bool avoidSacred = true;

        // Divide budget across rivers
        int perRiverBudget = Mathf.Max(1, waterBudget / riverCount);

        for (int i = 0; i < riverCount; i++)
        {
            // Randomize direction a bit so rivers aren’t all parallel
            // 0 = left->right, 1 = bottom->top, 2 = right->left, 3 = top->bottom
            int dir = UnityEngine.Random.Range(0, 4);

            int halfWidth = UnityEngine.Random.Range(baseHalfWidth, maxHalfWidth + 1);

            GenerateRiver(width, height, dir, perRiverBudget, halfWidth, avoidSacred);
        }

    }


    void GenerateRiver(int width, int height, int direction, int maxTiles, int halfWidth, bool avoidSacred)
    {
        Vector2Int start, end;

        switch (direction)
        {
            case 0: // left -> right
                start = new Vector2Int(0, UnityEngine.Random.Range(0, height));
                end = new Vector2Int(width - 1, UnityEngine.Random.Range(0, height));
                break;
            case 1: // bottom -> top
                start = new Vector2Int(UnityEngine.Random.Range(0, width), 0);
                end = new Vector2Int(UnityEngine.Random.Range(0, width), height - 1);
                break;
            case 2: // right -> left
                start = new Vector2Int(width - 1, UnityEngine.Random.Range(0, height));
                end = new Vector2Int(0, UnityEngine.Random.Range(0, height));
                break;
            default: // top -> bottom
                start = new Vector2Int(UnityEngine.Random.Range(0, width), height - 1);
                end = new Vector2Int(UnityEngine.Random.Range(0, width), 0);
                break;
        }

        Vector2Int current = start;

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(current);

        int placed = 0;
        int maxSteps = width * height; // safety

        for (int step = 0; step < maxSteps; step++)
        {
            PaintWaterBlob(current, width, height, halfWidth, avoidSacred);
            placed++;

            if (current == end) break;
            if (placed >= maxTiles) break;

            Vector2Int next = battlefieldBuilder.GetDrunkNeighborTowardsGoal(current, end, width, height, visited);

            if (next == current) break;
            if (visited.Contains(next)) break;

            visited.Add(next);
            current = next;
        }

        // Ensure the end gets painted too
        PaintWaterBlob(current, width, height, halfWidth, avoidSacred);
    }

    void PaintWaterBlob(Vector2Int center, int width, int height, int halfWidth, bool avoidSacred)
    {
        // halfWidth 0 => just the center tile
        // halfWidth 1 => up to a 3x3 blob, etc.
        for (int dx = -halfWidth; dx <= halfWidth; dx++)
        {
            for (int dy = -halfWidth; dy <= halfWidth; dy++)
            {
                int x = center.x + dx;
                int y = center.y + dy;

                if (x < 0 || x >= width || y < 0 || y >= height) continue;

                // Optional: make edges of the blob less solid, more organic
                // e.g. only paint corners sometimes
                if (halfWidth > 0 && Mathf.Abs(dx) == halfWidth && Mathf.Abs(dy) == halfWidth)
                {
                    if (UnityEngine.Random.value < 0.5f) continue;
                }

                var sc = battlefieldBuilder.AllSquares[x, y].GetComponent<SquareController>();
                if (sc == null) continue;

                if (avoidSacred && sc.IsSacred) continue;

                sc.MakeSquare(SquareType.Water, battlefieldBuilder.ThisMap);
                battlefieldBuilder.FreeSquares.Remove(new Vector2Int(x, y));
            }
        }
    }

}
