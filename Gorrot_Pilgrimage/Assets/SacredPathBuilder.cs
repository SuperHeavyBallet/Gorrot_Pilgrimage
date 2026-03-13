using System.Collections.Generic;
using UnityEngine;

public class SacredPathBuilder : MonoBehaviour
{
    [SerializeField] BattlefieldBuilder battlefieldBuilder;
   public void SetSacredPath(GameObject[,] allSquares, int playerStartingPosition)
    {
        int width = allSquares.GetLength(0);
        int height = allSquares.GetLength(1);

        // Start from the player start tile
        Vector2Int current = new Vector2Int(playerStartingPosition, 0);

        // Find the goal tile coordinate (cheaper would be to store it when you place it)
        Vector2Int goal = battlefieldBuilder.FindGoalCoord();

        // Safety: prevent infinite loops
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(current);

        int maxSteps = width * height;

        for (int step = 0; step < maxSteps; step++)
        {
            if (current == goal) break;

            // Mark current as sacred if you want the path to include the start too
            var currentSq = allSquares[current.x, current.y].GetComponent<SquareController>();
            if (currentSq != null) currentSq.SetIsSacred(true);
            battlefieldBuilder.RemoveFreeSquare(current);

            // Vector2Int next = GetBestNeighborTowardsGoal(current, goal, size);

            Vector2Int next = battlefieldBuilder.GetDrunkNeighborTowardsGoal(current, goal, width, height, visited);


            // If we can't progress, give up (or you could fall back to a real pathfinding algorithm)
            if (next == current) break;

            // If we're looping, break
            if (visited.Contains(next)) break;
            visited.Add(next);

            current = next;
        }

        // Also mark the goal as sacred (optional)
        var goalSq = allSquares[goal.x, goal.y].GetComponent<SquareController>();
        if (goalSq != null) goalSq.SetIsSacred(true);
        battlefieldBuilder.RemoveFreeSquare(goal);



    }

}
