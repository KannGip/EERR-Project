using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MapUI : MonoBehaviour
{
    public Button generateMapButton;
    public Slider sizeSlider;
    public TextMeshProUGUI sizeText;

    public WaveFunctionCollapse waveFunctionCollapse;
    public PrototypeGenerator prototypeGenerator;
    public RobotMovement robotMovement;

    private void Start()
    {
        sizeSlider.onValueChanged.AddListener(UpdateSizeText);
        generateMapButton.onClick.AddListener(GenerateMap);
        UpdateSizeText(sizeSlider.value);
    }

    void UpdateSizeText(float value)
    {
        sizeText.text = "Size: " + value.ToString("F0");
    }

    void GenerateMap()
    {
        if (prototypeGenerator == null || waveFunctionCollapse == null || robotMovement == null)
        {
            Debug.LogError("💥 Missing component references in MapUI!");
            return;
        }

        int mapSize = Mathf.RoundToInt(sizeSlider.value);
        Debug.Log($"🧭 Generating map of size: {mapSize}x{mapSize}");

        prototypeGenerator.GeneratePrototypes();
        waveFunctionCollapse.prototypeGenerator = prototypeGenerator;
        waveFunctionCollapse.size = new Vector2(mapSize, mapSize);
        waveFunctionCollapse.InitializeWaveFunction();

        int width = (int)waveFunctionCollapse.size.x;
        int height = (int)waveFunctionCollapse.size.y;

        GridNode[,] grid = CellToGridNodeMapper.GenerateGridFromCells(
            waveFunctionCollapse.cells, width, height
        );

        Vector2Int start = waveFunctionCollapse.latestMapData.startPoint;
        Vector2Int end = waveFunctionCollapse.latestMapData.endPoint;

        List<Vector3> path = AStarPathfinder.FindPath(start, end, grid);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("⚠️ No path found!");
            return;
        }

        Debug.Log($"✅ Path found! Steps: {path.Count}");

        List<Vector2Int> intPath = path.ConvertAll(p => new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z)));

        robotMovement.StartTestPath(intPath);
    }
}
