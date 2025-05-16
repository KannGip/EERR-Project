using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    public GameObject robotPrefab;         
    public float moveDelay = 0.5f;
    public Vector3 tileOffset = new Vector3(0, 0.5f, 0); 

    private GameObject robotInstance;     
    public MapData mapData;

    public void StartTestPath(List<Vector2Int> path)
    {
        if (robotInstance != null)
            Destroy(robotInstance);

        //robot start
        robotInstance = Instantiate(robotPrefab);
        robotInstance.tag = "Player"; 

        StartCoroutine(MoveAlongPath(path));
    }

    IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("Path is empty or null.");
            yield break;
        }

        Vector2Int current = path[0];
        robotInstance.transform.position = new Vector3(current.x, 0, current.y) + tileOffset;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int next = path[i];
            Vector2Int delta = next - current;

            Debug.Log($"Moving from {current} to {next}, Δ = {delta}");

            if (delta == Vector2Int.right)
                robotInstance.transform.rotation = Quaternion.Euler(0, 90, 0);
            else if (delta == Vector2Int.left)
                robotInstance.transform.rotation = Quaternion.Euler(0, 270, 0);
            else if (delta == Vector2Int.up)
                robotInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (delta == Vector2Int.down)
                robotInstance.transform.rotation = Quaternion.Euler(0, 180, 0);

            robotInstance.transform.position = new Vector3(next.x, 0, next.y) + tileOffset;
            current = next;

            Debug.Log($"Arrived at {current}");

            yield return new WaitForSeconds(moveDelay);
        }
    }
}
