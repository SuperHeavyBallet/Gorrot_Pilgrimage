using UnityEngine;
using UnityEngine.UIElements.Experimental;

[CreateAssetMenu(fileName = "DifficultyTuning", menuName = "Scriptable Objects/DifficultyTuning")]
public class DifficultyTuning : ScriptableObject
{
        [System.Serializable]
        public struct Ratios
        {
            [Min(0f)] public float enemy;
            [Min(0f)] public float treasure;
            [Min(0f)] public float terrain;
            [Min(0f)] public float health;
            [Min(0f)] public float potion;
        }

        [Header("Ratios (per-tile probability-ish)")]
        public Ratios easy = new Ratios
        {
            enemy = 1f / 40f,
            treasure = 1f / 30f,
            terrain = 1f / 20f,
            health = 1f / 40f,
            potion = 1f / 40f
        };

        public Ratios hard = new Ratios
        {
            enemy = 1f / 30f,
            treasure = 1f / 80f,
            terrain = 1f / 10f,
            health = 1f / 90f,
            potion = 1f / 90f
        };

        [Header("Difficulty Progression")]
        [Min(2)] public int maxDifficultyMap = 3;

        [Header("Minimum Counts")]
        [Min(0)] public int minEnemy = 1;
        [Min(0)] public int minTreasure = 1;
        [Min(0)] public int minTerrain = 1;
        [Min(0)] public int minHealth = 1;
        [Min(0)] public int minPotion = 1;

        public float ComputeDifficulty01(int currentMapCount, bool isFinalMap)
        {
            if (isFinalMap) return 1f;

            // If maxDifficultyMap == 2, denominator becomes 1 -> fine.
            float denom = (maxDifficultyMap - 1f);
            return Mathf.Clamp01((currentMapCount - 1f) / denom);
        }

        public Counts ComputeCounts(int mapSize, int currentMapCount, bool isFinalMap)
        {
            int area = mapSize * mapSize;
            float t = ComputeDifficulty01(currentMapCount, isFinalMap);

            float enemyRatio = Mathf.Lerp(easy.enemy, hard.enemy, t);
            float treasureRatio = Mathf.Lerp(easy.treasure, hard.treasure, t);
            float terrainRatio = Mathf.Lerp(easy.terrain, hard.terrain, t);
            float healthRatio = Mathf.Lerp(easy.health, hard.health, t);
            float potionRatio = Mathf.Lerp(easy.potion, hard.potion, t);

            return new Counts
            {
                enemy = Mathf.Max(minEnemy, Mathf.RoundToInt(area * enemyRatio)),
                treasure = Mathf.Max(minTreasure, Mathf.RoundToInt(area * treasureRatio)),
                terrain = Mathf.Max(minTerrain, Mathf.RoundToInt(area * terrainRatio)),
                health = Mathf.Max(minHealth, Mathf.RoundToInt(area * healthRatio)),
                potion = Mathf.Max(minPotion, Mathf.RoundToInt(area * potionRatio)),
            };
        }

        [System.Serializable]
        public struct Counts
        {
            public int enemy, treasure, terrain, health, potion;
        }
    

}
