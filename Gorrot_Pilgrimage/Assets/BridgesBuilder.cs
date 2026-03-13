using System.Collections.Generic;
using UnityEngine;
using GorrotGame;

public class BridgesBuilder : MonoBehaviour
{
    [SerializeField] BattlefieldBuilder battlefieldBuilder;
    public void DetectBridges()
    {
        int width = battlefieldBuilder.AllSquares.GetLength(0);
        int height = battlefieldBuilder.AllSquares.GetLength(1);

        bool[,] processedVertical = new bool[width, height];
        bool[,] processedHorizontal = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsVerticalBridgeCandidate(x, y) && !processedVertical[x, y])
                {
                    List<Vector2Int> run = GetVerticalBridgeRun(x, y);

                    foreach (var pos in run)
                        processedVertical[pos.x, pos.y] = true;

                    if (RunConnectsLandAtBothEnds(run, BridgeOrientation.Vertical))
                    {
                        MarkBridgeRun(run, BridgeOrientation.Vertical);
                    }
                }

                if (IsHorizontalBridgeCandidate(x, y) && !processedHorizontal[x, y])
                {
                    List<Vector2Int> run = GetHorizontalBridgeRun(x, y);

                    foreach (var pos in run)
                        processedHorizontal[pos.x, pos.y] = true;

                    if (RunConnectsLandAtBothEnds(run, BridgeOrientation.Horizontal))
                    {
                        MarkBridgeRun(run, BridgeOrientation.Horizontal);
                    }
                }
            }
        }
    }

    List<Vector2Int> GetVerticalBridgeRun(int x, int y)
    {
        List<Vector2Int> run = new List<Vector2Int>();

        int startY = y;
        while (IsVerticalBridgeCandidate(x, startY - 1))
            startY--;

        int endY = y;
        while (IsVerticalBridgeCandidate(x, endY + 1))
            endY++;

        for (int yy = startY; yy <= endY; yy++)
            run.Add(new Vector2Int(x, yy));

        return run;
    }

    List<Vector2Int> GetHorizontalBridgeRun(int x, int y)
    {
        List<Vector2Int> run = new List<Vector2Int>();

        int startX = x;
        while (IsHorizontalBridgeCandidate(startX - 1, y))
            startX--;

        int endX = x;
        while (IsHorizontalBridgeCandidate(endX + 1, y))
            endX++;

        for (int xx = startX; xx <= endX; xx++)
            run.Add(new Vector2Int(xx, y));

        return run;
    }


    bool RunConnectsLandAtBothEnds(List<Vector2Int> run, BridgeOrientation orientation)
    {
        if (run == null || run.Count == 0)
            return false;

        if (orientation == BridgeOrientation.Vertical)
        {
            Vector2Int first = run[0];
            Vector2Int last = run[run.Count - 1];

            bool hasLandBefore = battlefieldBuilder.IsLandAt(first.x, first.y - 1) && !IsVerticalBridgeCandidate(first.x, first.y - 1);
            bool hasLandAfter = battlefieldBuilder.IsLandAt(last.x, last.y + 1) && !IsVerticalBridgeCandidate(last.x, last.y + 1);

            return hasLandBefore && hasLandAfter;
        }
        else if (orientation == BridgeOrientation.Horizontal)
        {
            Vector2Int first = run[0];
            Vector2Int last = run[run.Count - 1];

            bool hasLandBefore = battlefieldBuilder.IsLandAt(first.x - 1, first.y) && !IsHorizontalBridgeCandidate(first.x - 1, first.y);
            bool hasLandAfter = battlefieldBuilder.IsLandAt(last.x + 1, last.y) && !IsHorizontalBridgeCandidate(last.x + 1, last.y);

            return hasLandBefore && hasLandAfter;
        }

        return false;
    }

    bool IsVerticalBridgeCandidate(int x, int y)
    {
        return battlefieldBuilder.IsLandAt(x, y)
            && battlefieldBuilder.IsWaterAt(x - 1, y)
            && battlefieldBuilder.IsWaterAt(x + 1, y);
    }

    bool IsHorizontalBridgeCandidate(int x, int y)
    {
        return battlefieldBuilder.IsLandAt(x, y)
            && battlefieldBuilder.IsWaterAt(x, y - 1)
            && battlefieldBuilder.IsWaterAt(x, y + 1);
    }

    void MarkBridgeRun(List<Vector2Int> run, BridgeOrientation orientation)
    {
        foreach (var pos in run)
        {
            var sc = battlefieldBuilder.AllSquares[pos.x, pos.y]?.GetComponent<SquareController>();
            if (sc == null) continue;

            sc.SetIsBridge(true);
            sc.SetBridgeOrientation(orientation);
        }
    }

    public void RebuildBridgeAwareVisuals()
    {
        int width = battlefieldBuilder.AllSquares.GetLength(0);
        int height = battlefieldBuilder.AllSquares.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SquareController sc = battlefieldBuilder.AllSquares[x, y]?.GetComponent<SquareController>();
                if (sc == null) continue;

                if (sc.IsWater)
                {
                    RebuildWaterTileVisuals(x, y, sc);
                }
                else if (sc.IsBridge)
                {
                    RebuildBridgeTileVisuals(x, y, sc);
                }
                else
                {
                    RebuildLandTileVisuals(x, y, sc);
                }
            }
        }
    }

    void RebuildWaterTileVisuals(int x, int y, SquareController sc)
    {
        int waterEdgeMask = 0;
        int diagWaterMask = 0;

        // N E S W
        for (int i = 0; i < BattlefieldBuilder.Neigh4.Length; i++)
        {
            Vector2Int n = new Vector2Int(x, y) + BattlefieldBuilder.Neigh4[i];

            if (!battlefieldBuilder.InBounds(n.x, n.y))
            {
                // map edge still counts as open water edge
                waterEdgeMask |= (1 << i);
                continue;
            }

            bool neighborIsWater = battlefieldBuilder.IsWaterAt(n.x, n.y);
            bool neighborIsBridge =  battlefieldBuilder.IsBridgeAt(n.x, n.y);

            // Only show normal water border against SOLID LAND, not bridge.
            if (!neighborIsWater && !neighborIsBridge)
            {
                waterEdgeMask |= (1 << i);
            }
        }

        // Diagonals still based on water-water relationship
        for (int i = 0; i < BattlefieldBuilder.NeighDiag.Length; i++)
        {
            Vector2Int d = new Vector2Int(x, y) + BattlefieldBuilder.NeighDiag[i];

            if (!battlefieldBuilder.InBounds(d.x, d.y)) continue;

            if (battlefieldBuilder.IsWaterAt(d.x, d.y))
            {
                diagWaterMask |= (1 << i);
            }
        }

        sc.SetIsWaterAdjacent(false);
        sc.SetWaterAdjacencyMask(waterEdgeMask);
        sc.SetWaterDiagonalMask(diagWaterMask);
    }

    void RebuildBridgeTileVisuals(int x, int y, SquareController sc)
    {
        sc.SetIsWaterAdjacent(false);

        // clear old caps first
        sc.SetBridgeEndCaps(false, false, false, false);

        BridgeOrientation orientation = sc.GetBridgeOrientation();

        bool northCap = false;
        bool eastCap = false;
        bool southCap = false;
        bool westCap = false;

        if (orientation == BridgeOrientation.Horizontal)
        {
            westCap = battlefieldBuilder.IsSolidLandAt(x - 1, y);
            eastCap = battlefieldBuilder.IsSolidLandAt(x + 1, y);
        }
        else if (orientation == BridgeOrientation.Vertical)
        {
            southCap = battlefieldBuilder.IsSolidLandAt(x, y - 1);
            northCap = battlefieldBuilder.IsSolidLandAt(x, y + 1);
        }

        sc.SetBridgeEndCaps(northCap, eastCap, southCap, westCap);
    }


    void RebuildLandTileVisuals(int x, int y, SquareController sc)
    {
        int waterNeighborMask = 0;

        for (int i = 0; i < BattlefieldBuilder.Neigh4.Length; i++)
        {
            Vector2Int n = new Vector2Int(x, y) + BattlefieldBuilder.Neigh4[i];

            if (battlefieldBuilder.IsWaterAt(n.x, n.y))
            {
                waterNeighborMask |= (1 << i);
            }
        }

        bool adjacentToWater = (waterNeighborMask != 0);
        sc.SetIsWaterAdjacent(adjacentToWater);

        // Optional later:
        // sc.SetLandWaterNeighborMask(waterNeighborMask);
    }


}
