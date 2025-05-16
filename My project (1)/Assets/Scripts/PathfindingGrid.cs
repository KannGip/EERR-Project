using UnityEngine;
using System.Collections.Generic;

public class PathfindingGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public GridNode[,] grid;

    [HideInInspector]
    public MapData mapData; // <- externally set from MapDataToPathfindingGrid

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    void Awake()
    {
        // Disabled because we now load the grid from WFC via MapData
    }

    public GridNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x);
        int y = Mathf.RoundToInt(worldPosition.z);

        x = Mathf.Clamp(x, 0, grid.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, grid.GetLength(1) - 1);

        return grid[x, y];
    }

    public List<GridNode> GetNeighbours(GridNode node)
    {
        List<GridNode> neighbours = new List<GridNode>();

        if (mapData == null || mapData.walkableDirections == null)
        {
            Debug.LogError("⚠️ PathfindingGrid: MapData or walkableDirections is null.");
            return neighbours;
        }

        int x = node.gridX;
        int z = node.gridY;
        var directions = mapData.walkableDirections;

        DirectionFlags currentDirs = directions[x, z];

        TryAddNeighbour(neighbours, x, z, x + 1, z, currentDirs.HasFlag(DirectionFlags.PosX), DirectionFlags.NegX); // Right
        TryAddNeighbour(neighbours, x, z, x - 1, z, currentDirs.HasFlag(DirectionFlags.NegX), DirectionFlags.PosX); // Left
        TryAddNeighbour(neighbours, x, z, x, z + 1, currentDirs.HasFlag(DirectionFlags.PosZ), DirectionFlags.NegZ); // Up
        TryAddNeighbour(neighbours, x, z, x, z - 1, currentDirs.HasFlag(DirectionFlags.NegZ), DirectionFlags.PosZ); // Down


        return neighbours;
    }


    private void TryAddNeighbour(List<GridNode> list, int x, int z, int nx, int nz, bool canMoveOut, DirectionFlags requiredEntry)
    {
        Debug.Log($"🔍 Checking neighbor from ({x}, {z}) → ({nx}, {nz})");

        if (!canMoveOut)
        {
            Debug.Log($"❌ Blocked exit from ({x}, {z}) to ({nx}, {nz}) — no direction flag to move out.");
            return;
        }

        if (nx >= 0 && nz >= 0 && nx < grid.GetLength(0) && nz < grid.GetLength(1))
        {
            var targetDirs = mapData.walkableDirections[nx, nz];
            var targetNode = grid[nx, nz];

            Debug.Log($"📍 From ({x},{z}) dirs: {mapData.walkableDirections[x, z]} → ({nx},{nz}) needs: {requiredEntry}, target dirs: {targetDirs}");

            if (!targetNode.isWalkable)
            {
                Debug.Log($"⛔ Target tile at ({nx},{nz}) is not walkable.");
                return;
            }

            if (targetDirs.HasFlag(requiredEntry))
            {
                Debug.Log($"✅ Valid connection: from ({x},{z}) to ({nx},{nz})");
                list.Add(targetNode);
            }
            else
            {
                Debug.Log($"⛔ Target tile at ({nx},{nz}) is missing required entry direction: {requiredEntry}");
            }
        }
    }



    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (GridNode n in grid)
            {
                Gizmos.color = (n.isWalkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
