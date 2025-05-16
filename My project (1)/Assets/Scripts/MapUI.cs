using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
        int mapSize = (int)sizeSlider.value;
        Debug.Log("Generating map of size: " + mapSize);

        prototypeGenerator.GeneratePrototypes();
        waveFunctionCollapse.prototypeGenerator = prototypeGenerator;
        waveFunctionCollapse.size = new Vector2(mapSize, mapSize);
        waveFunctionCollapse.InitializeWaveFunction();

        MapData mapData = waveFunctionCollapse.latestMapData; 

        // Log start and end direction flags
        var startDirs = mapData.walkableDirections[mapData.startPoint.x, mapData.startPoint.y];
        var endDirs = mapData.walkableDirections[mapData.endPoint.x, mapData.endPoint.y];

        Debug.Log($"🅢 Start {mapData.startPoint} → directions: {startDirs}");
        Debug.Log($"🅔 End {mapData.endPoint} → directions: {endDirs}");

        bool startWalkable = mapData.isWalkable[mapData.startPoint.x, mapData.startPoint.y];
        bool endWalkable = mapData.isWalkable[mapData.endPoint.x, mapData.endPoint.y];
        Debug.Log($"Start walkable: {startWalkable}, End walkable: {endWalkable}");


        // Optional: log to verify
        Debug.Log($"Robot start at: {mapData.startPoint}, end at: {mapData.endPoint}");


        Debug.Log("Converting MapData to Grid...");
        var gridConverter = UnityEngine.Object.FindFirstObjectByType<MapDataToPathfindingGrid>();
        gridConverter.LoadMapData(mapData);
        

        Vector3 start = new Vector3(mapData.startPoint.x, 0, mapData.startPoint.y);
        Vector3 end = new Vector3(mapData.endPoint.x, 0, mapData.endPoint.y);

        Debug.Log("Finding path...");
        var pathfinder = UnityEngine.Object.FindFirstObjectByType<AStarPathfinding>();
        List<Vector3> path = pathfinder.FindPath(start, end);


        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("⚠️ No path found!");
            return;
        }



        Debug.Log("Converting path for robot movement...");
        List<Vector2Int> intPath = path.ConvertAll(p => new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z)));

        if (robotMovement != null)
        {
            Debug.Log("Starting robot movement...");
            robotMovement.StartTestPath(intPath);
        }
        else
        {
            Debug.LogWarning("RobotMovement reference is missing.");
        }

    }
}
