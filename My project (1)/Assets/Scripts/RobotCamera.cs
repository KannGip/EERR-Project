using UnityEngine;

public class RobotCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10f, -10f);
    public float followSpeed = 5f;
    public KeyCode toggleKey = KeyCode.C;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.enabled = false; 
            cam.rect = new Rect(0.7f, 0.7f, 0.3f, 0.3f); // Picture-in-picture view
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && cam != null)
        {
            cam.enabled = !cam.enabled;
            Debug.Log("🎥 RobotCamera toggled: " + cam.enabled);
        }
    }

    void LateUpdate()
    {
        if (!cam || !cam.enabled) return;

        if (target == null)
        {
            GameObject robot = GameObject.FindWithTag("Player");
            if (robot != null)
            {
                target = robot.transform;
                Debug.Log("🎥 RobotFollowCamera: Target assigned to " + target.name);
            }
            else
            {
                return;
            }
        }

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
