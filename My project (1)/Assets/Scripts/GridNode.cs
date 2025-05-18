using UnityEngine;

public class GridNode
{
    public bool isWalkable;
    public Vector3 worldPosition;
    public int gridX, gridY;

    public int gCost, hCost;
    public GridNode parent;

    public int fCost => gCost + hCost;

    public GridNode(bool walkable, Vector3 pos, int x, int y)
    {
        isWalkable = walkable;
        worldPosition = pos;
        gridX = x;
        gridY = y;
    }
}
