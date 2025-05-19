using System.Collections.Generic;
using UnityEngine;

public static class EarthquakeGenerator
{
    public static void ApplyEarthquake(MapData mapData, WaveFunctionCollapse wfc, float percentage = 0.02f)
    {
        int width = mapData.isWalkable.GetLength(0);
        int height = mapData.isWalkable.GetLength(1);
        int totalTiles = width * height;
        int tilesToCollapse = Mathf.FloorToInt(totalTiles * percentage);

        List<Vector2Int> candidates = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector2Int coord = new Vector2Int(x, z);
                if (mapData.isWalkable[x, z] && coord != mapData.startPoint && coord != mapData.endPoint)
                {
                    candidates.Add(coord);
                }
            }
        }

        for (int i = 0; i < tilesToCollapse && candidates.Count > 0; i++)
        {
            int index = Random.Range(0, candidates.Count);
            Vector2Int chosen = candidates[index];
            candidates.RemoveAt(index);

            // Replace in MapData
            mapData.UpdateTile(chosen.x, chosen.y, false, DirectionFlags.None);

            // Replace in visuals
            ReplacePrototypeWithEarthquake(wfc, chosen);
        }

        Debug.Log($"🌍 EarthquakeGenerator: {tilesToCollapse} tiles visually and logically destroyed.");
    }

    private static void ReplacePrototypeWithEarthquake(WaveFunctionCollapse wfc, Vector2Int coords)
    {
        if (wfc.earthquakePrototypes == null || wfc.earthquakePrototypes.Count == 0)
        {
            Debug.LogWarning("⚠️ No earthquake prototypes set.");
            return;
        }

        Vector2 key = new Vector2(coords.x, coords.y);
        if (!wfc.activeCells.TryGetValue(key, out Cell cell)) return;

        if (cell.possiblePrototypes.Count == 0) return;

        int currentRotation = cell.possiblePrototypes[0].meshRotation;

        Prototype replacement = wfc.earthquakePrototypes.Find(p => p.meshRotation == currentRotation);
        if (replacement == null)
        {
            replacement = wfc.earthquakePrototypes[0];
        }

        cell.possiblePrototypes.Clear();
        cell.possiblePrototypes.Add(replacement);

        foreach (Transform child in cell.transform)
            Object.Destroy(child.gameObject);

        GameObject newVisual = Object.Instantiate(replacement.prefab, cell.transform);
        newVisual.transform.Rotate(0f, replacement.meshRotation * 90, 0f, Space.Self);
        newVisual.transform.localPosition = Vector3.zero;
    }
}
