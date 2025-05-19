using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public static List<Vector3> FindPath(Vector2Int startCoords, Vector2Int endCoords, GridNode[,] grid)
    {
        GridNode startNode = grid[startCoords.x, startCoords.y];
        GridNode endNode = grid[endCoords.x, endCoords.y];

        List<GridNode> openSet = new List<GridNode> { startNode };
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, endNode);

        while (openSet.Count > 0)
        {
            GridNode current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost || (openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost))
                    current = openSet[i];
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == endNode)
                return RetracePath(startNode, endNode);

            foreach (GridNode neighbor in GetNeighbours(current, grid))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                int newCost = current.gCost + GetDistance(current, neighbor);
                if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector3>(); // No path found
    }

    private static List<Vector3> RetracePath(GridNode start, GridNode end)
    {
        List<Vector3> path = new List<Vector3>();
        GridNode current = end;

        while (current != start)
        {
            path.Add(current.worldPosition);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    private static int GetDistance(GridNode a, GridNode b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return dx > dy ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
    }

    private static List<GridNode> GetNeighbours(GridNode node, GridNode[,] grid)
    {
        List<GridNode> neighbors = new List<GridNode>();
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int( 0,  1), // up
        new Vector2Int( 0, -1), // down
        new Vector2Int( 1,  0), // right
        new Vector2Int(-1,  0), // left
        };

        foreach (var dir in directions)
        {
            int checkX = node.gridX + dir.x;
            int checkY = node.gridY + dir.y;

            if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
            {
                GridNode neighbor = grid[checkX, checkY];
                if (neighbor != null)
                    neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}
