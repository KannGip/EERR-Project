using UnityEngine;

public class GridNode
{
    public bool isWalkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    
    public GridNode parent;
    
    public GridNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        isWalkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
    
    public void Reset()
    {
        gCost = 0;
        hCost = 0;
        parent = null;
    }
}