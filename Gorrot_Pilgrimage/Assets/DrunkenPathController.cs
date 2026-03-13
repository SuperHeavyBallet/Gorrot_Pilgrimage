using System.Collections.Generic;
using UnityEngine;

public class DrunkenPathController : MonoBehaviour
{
    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    [Header("Sacred Path Drunkenness")]
    [Tooltip("0 = always best, 1 = very random")]
    [SerializeField, Range(0f, 1f)] float drunkenness;
    [Tooltip("higher = more greedy, lower = more meandery")]
    [SerializeField, Range(0f, 1f)] float weightSharpness;

   
    public Vector2Int GetDrunkNeighborTowardsGoal(Vector2Int current, Vector2Int goal, int width, int height, HashSet<Vector2Int> visited)
    {

        List<Vector2Int> candidates = new List<Vector2Int>();
        List<float> weights = new List<float>();

        int currentDist = Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);

        foreach (var dir in BattlefieldBuilder.Neigh4)
        {
            Vector2Int n = current + dir;

            if (n.x < 0 || n.x >= width || n.y < 0 || n.y >= height) continue;

            bool wasVisited = visited.Contains(n);

            int dist = Mathf.Abs(n.x - goal.x) + Mathf.Abs(n.y - goal.y);

            // Improvement: positive if this step gets closer.
            float improvement = currentDist - dist;

            // Base desirability:
            // - prefer getting closer (improvement > 0)
            // - allow sideways/backwards a bit when drunk
            float desirability = improvement;

            // Penalize revisits heavily to avoid loops
            if (wasVisited) desirability -= 999f;

            // Convert desirability into a weight.
            // We want weights > 0 even for "not great" moves.
            // Use an exponential-ish curve controlled by weightSharpness.
            float w = Mathf.Exp(desirability * weightSharpness);

            candidates.Add(n);
            weights.Add(w);
        }

        if (candidates.Count == 0) return current;

        // Mix between greedy and random:
        // - drunkenness 0 => almost always pick max weight
        // - drunkenness 1 => pick by weights (still biased, but much wobblier)
        if (UnityEngine.Random.value > drunkenness)
        {
            // Greedy pick
            int bestIndex = 0;
            float bestW = weights[0];
            for (int i = 1; i < weights.Count; i++)
            {
                if (weights[i] > bestW)
                {
                    bestW = weights[i];
                    bestIndex = i;
                }
            }
            return candidates[bestIndex];
        }

        // Weighted random pick
        float total = 0f;
        for (int i = 0; i < weights.Count; i++) total += weights[i];

        float roll = UnityEngine.Random.value * total;
        float accum = 0f;

        for (int i = 0; i < weights.Count; i++)
        {
            accum += weights[i];
            if (roll <= accum) return candidates[i];
        }

        return candidates[candidates.Count - 1];
    }

}
