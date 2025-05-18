using System.Collections.Generic;
using UnityEngine;

public static class CellToGridNodeMapper
{
    public static GridNode[,] GenerateGridFromCells(List<Cell> cells, int width, int height)
    {
        GridNode[,] grid = new GridNode[width, height];

        foreach (var cell in cells)
        {
            int x = (int)cell.coords.x;
            int y = (int)cell.coords.y;

            bool walkable = cell.possiblePrototypes.Count > 0 && cell.possiblePrototypes[0].isWalkable;
            Vector3 worldPos = cell.transform.position;

            grid[x, y] = new GridNode(walkable, worldPos, x, y);
        }

        return grid;
    }
}
