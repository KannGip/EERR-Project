using UnityEngine;
using System.Collections.Generic;

[System.Flags]
public enum DirectionFlags
{
    None = 0,
    PosX = 1 << 0,
    NegX = 1 << 1,
    PosZ = 1 << 2,
    NegZ = 1 << 3
}

public class MapData
{
    public bool[,] isWalkable;
    public DirectionFlags[,] walkableDirections;

    public Vector2Int startPoint;
    public Vector2Int endPoint;

    public void ChooseRandomStartAndEnd()
    {
        List<Vector2Int> walkableTiles = new List<Vector2Int>();

        for (int x = 0; x < isWalkable.GetLength(0); x++)
        {
            for (int z = 0; z < isWalkable.GetLength(1); z++)
            {
                if (!isWalkable[x, z]) continue;

                var dirs = walkableDirections[x, z];

                int count = 0;
                if (dirs.HasFlag(DirectionFlags.PosX)) count++;
                if (dirs.HasFlag(DirectionFlags.NegX)) count++;
                if (dirs.HasFlag(DirectionFlags.PosZ)) count++;
                if (dirs.HasFlag(DirectionFlags.NegZ)) count++;

                // En az 2 yön varsa bu hücreyi aday olarak al
                if (count >= 2)
                    walkableTiles.Add(new Vector2Int(x, z));
            }
        }

        if (walkableTiles.Count < 2)
        {
            Debug.LogWarning("❌ Not enough valid tiles with multiple directions to choose start and end points.");
            return;
        }

        startPoint = walkableTiles[Random.Range(0, walkableTiles.Count)];

        // Bitiş noktasını mümkün olduğunca uzak seç
        int attempts = 0;
        do
        {
            endPoint = walkableTiles[Random.Range(0, walkableTiles.Count)];
            attempts++;
        }
        while (Vector2Int.Distance(startPoint, endPoint) < 5 && attempts < 100);

        if (attempts >= 100)
            Debug.LogWarning("⚠️ Could not find a distant enough endpoint after 100 tries.");

        Debug.Log($"🧭 Start: {startPoint}, End: {endPoint}");
    }


    public void UpdateTile(int x, int z, bool walkable, DirectionFlags dirs)
    {
        isWalkable[x, z] = walkable;
        walkableDirections[x, z] = dirs;
    }

    public void PrintToConsole()
    {
        int width = isWalkable.GetLength(0);
        int height = isWalkable.GetLength(1);
        string mapPrint = "\n🗺️ MAP OVERVIEW (Top = High Z):\n";

        for (int z = height - 1; z >= 0; z--)
        {
            string line = "";
            for (int x = 0; x < width; x++)
            {
                string symbol;
                if (!isWalkable[x, z])
                {
                    symbol = "🟩"; // grass or unwalkable
                }
                else
                {
                    var dirs = walkableDirections[x, z];

                    if (dirs.HasFlag(DirectionFlags.PosX) && dirs.HasFlag(DirectionFlags.NegX) &&
                        !dirs.HasFlag(DirectionFlags.PosZ) && !dirs.HasFlag(DirectionFlags.NegZ))
                        symbol = "↕";
                    else if (dirs.HasFlag(DirectionFlags.PosZ) && dirs.HasFlag(DirectionFlags.NegZ) &&
                             !dirs.HasFlag(DirectionFlags.PosX) && !dirs.HasFlag(DirectionFlags.NegX))
                        symbol = "↔";
                    else if (dirs.HasFlag(DirectionFlags.PosX) && dirs.HasFlag(DirectionFlags.PosZ))
                        symbol = "╮";
                    else if (dirs.HasFlag(DirectionFlags.NegX) && dirs.HasFlag(DirectionFlags.PosZ))
                        symbol = "╭";
                    else if (dirs.HasFlag(DirectionFlags.NegX) && dirs.HasFlag(DirectionFlags.NegZ))
                        symbol = "╰";
                    else if (dirs.HasFlag(DirectionFlags.PosX) && dirs.HasFlag(DirectionFlags.NegZ))
                        symbol = "╯";
                    else if (dirs.HasFlag(DirectionFlags.PosX) &&
                             dirs.HasFlag(DirectionFlags.NegX) &&
                             dirs.HasFlag(DirectionFlags.PosZ) &&
                             dirs.HasFlag(DirectionFlags.NegZ))
                        symbol = "🔹";
                    else
                        symbol = "⬛";
                }

                if (new Vector2Int(x, z) == startPoint)
                    symbol = "🅢"; // Start
                else if (new Vector2Int(x, z) == endPoint)
                    symbol = "🅔"; // End

                line += symbol;
            }

            mapPrint += line + "\n";
        }

        Debug.Log(mapPrint);
    }
}
