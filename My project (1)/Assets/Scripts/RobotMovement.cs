using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    [Header("Setup")]
    public GameObject robotPrefab;         
    public float moveDelay = 0.5f;
    public Vector3 tileOffset = new Vector3(0, 0.5f, 0); // raise robot a bit for visibility

    [Header("Debug")]
    public bool showPathInScene = true;

    private GameObject robotInstance;     

    public void StartTestPath(List<Vector2Int> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("❌ RobotMovement received an empty path.");
            return;
        }

        if (robotInstance != null)
            Destroy(robotInstance);

        robotInstance = Instantiate(robotPrefab);
        robotInstance.name = "Robot";
        robotInstance.tag = "Player";

        StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<Vector2Int> path)
    {
        Vector2Int current = path[0];
        robotInstance.transform.position = new Vector3(current.x, 0, current.y) + tileOffset;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int next = path[i];
            Vector2Int delta = next - current;

            RotateTowardsDirection(delta);

            Vector3 targetPosition = new Vector3(next.x, 0, next.y) + tileOffset;
            robotInstance.transform.position = targetPosition;

            current = next;

            Debug.Log($"🤖 Moved to {current}");
            yield return new WaitForSeconds(moveDelay);
        }

        Debug.Log("✅ Robot reached destination.");
    }

    private void RotateTowardsDirection(Vector2Int delta)
    {
        if (delta == Vector2Int.right)
            robotInstance.transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (delta == Vector2Int.left)
            robotInstance.transform.rotation = Quaternion.Euler(0, 270, 0);
        else if (delta == Vector2Int.up)
            robotInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (delta == Vector2Int.down)
            robotInstance.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showPathInScene || robotInstance == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(robotInstance.transform.position, 0.2f);
    }
#endif
}
