using UnityEngine;

public class MapDataToPathfindingGrid : MonoBehaviour
{
    public PathfindingGrid pathfindingGrid;

    public void LoadMapData(MapData mapData)
    {
        Debug.Log("Converting MapData into pathfinding grid...");

        int width = mapData.isWalkable.GetLength(0);
        int height = mapData.isWalkable.GetLength(1);
        GridNode[,] nodes = new GridNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 worldPos = new Vector3(x, 0, z); // assume each tile is 1 unit apart
                bool walkable = mapData.isWalkable[x, z];
                nodes[x, z] = new GridNode(walkable, worldPos, x, z);
            }
        }

        pathfindingGrid.grid = nodes;
        pathfindingGrid.mapData = mapData;
        Debug.Log("Grid conversion complete. Grid size: " + width + "x" + height);
    }
}
