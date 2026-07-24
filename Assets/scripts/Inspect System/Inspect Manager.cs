// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;

// public class InspectManager : MonoBehaviour
// {
//     [Header("References")]
//     public Transform inspectPoint;        // Empty object in front of camera
//     public GameObject inspectUI;          // UI overlay
//     public Volume blurVolume;             // URP Global Volume
//     public Camera cam;                    // Main camera
//     public Movement move;                 // Player movement script

//     [Header("Settings")]
//     public float moveSpeed = 10f;
//     public float rotateSpeed = 5f;
//     public float zoomSpeed = 0.5f;
//     public float minZoom = 0.2f;
//     public float maxZoom = 1.5f;
//     public float defaultZoom = 0.7f;

//     private Transform currentObj;
//     private Vector3 originalPos;
//     private Quaternion originalRot;
//     private float currentZoom;
//     private bool inspecting = false;

//     private float exitCooldown = 0f;
//     private const float cooldownDuration = 0.2f;

//     void Start()
//     {
//         if (cam == null)
//             cam = Camera.main;

//         if (move == null)
//             move = UnityEngine.Object.FindFirstObjectByType<Movement>();

//         currentZoom = defaultZoom;
//     }

//     public void StartInspect(Transform obj)
//     {
//         if (cam == null)
//             cam = Camera.main;

//         if (move == null)
//             move = UnityEngine.Object.FindFirstObjectByType<Movement>();

//         if (inspectPoint == null)
//             inspectPoint = cam.transform; // or assign your actual inspectPoint

//         currentObj = obj;

//         // Save original transform
//         originalPos = obj.position;
//         originalRot = obj.rotation;

//         inspecting = true;
//         move.isInspecting = true;

//         Cursor.lockState = CursorLockMode.None;
//         Cursor.visible = true;

//         if (inspectUI != null)
//             inspectUI.SetActive(true);

//         if (blurVolume != null)
//             blurVolume.enabled = false;

//         exitCooldown = cooldownDuration;

//         // Make object face the camera right-side-up
//         currentObj.rotation = Quaternion.LookRotation(cam.transform.forward, Vector3.up) * Quaternion.Euler(180f, 0f, 0f);

//         // Move object directly in front of camera
//         currentObj.position = inspectPoint.position + cam.transform.forward * defaultZoom;

//         Debug.Log("InspectManager: Inspect started");
//     }


//     void Update()
//     {
//         if (exitCooldown > 0f)
//             exitCooldown -= Time.deltaTime;

//         if (!inspecting || currentObj == null)
//             return;

//         HandleMovement();
//         HandleRotation();
//         HandleZoom();

//         // Exit inspect mode (E key)
//         if (exitCooldown <= 0f && Input.GetKeyDown(KeyCode.E))
//         {
//             StopInspect();
//         }
//     }

//     private void HandleMovement()
//     {
//         Vector3 targetPos = inspectPoint.position + cam.transform.forward * currentZoom;
//         currentObj.position = Vector3.Lerp(currentObj.position, targetPos, Time.deltaTime * moveSpeed);
//     }

//     private void HandleRotation()
//     {
//         if (!Input.GetMouseButton(0))
//             return;

//         float mx = Input.GetAxis("Mouse X") * rotateSpeed;
//         float my = Input.GetAxis("Mouse Y") * rotateSpeed;

//         // Horizontal rotation (left/right)
//         currentObj.Rotate(Vector3.up, mx, Space.World);

//         // Vertical rotation (up/down) — inverted so dragging down makes that side go down
//         currentObj.Rotate(cam.transform.right, my, Space.World);
//     }


//     private void HandleZoom()
//     {
//         float scroll = Input.mouseScrollDelta.y;
//         currentZoom = Mathf.Clamp(currentZoom - scroll * zoomSpeed, minZoom, maxZoom);
//     }

//     public void StopInspect()
//     {
//         inspecting = false;
//         move.isInspecting = false;

//         // Restore original transform
//         currentObj.position = originalPos;
//         currentObj.rotation = originalRot;

//         currentObj = null;
//         currentZoom = defaultZoom;

//         Cursor.lockState = CursorLockMode.Locked;
//         Cursor.visible = false;

//         if (inspectUI != null)
//             inspectUI.SetActive(false);

//         if (blurVolume != null)
//             blurVolume.enabled = false;

//         exitCooldown = cooldownDuration;

//         Debug.Log("InspectManager: Inspect stopped");
//     }

//     public bool IsInspecting()
//     {
//         return inspecting || exitCooldown > 0f;
//     }
// }
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InspectManager : MonoBehaviour
{
    [Header("References")]
    public Transform inspectPoint;
    public GameObject inspectUI;
    public Volume blurVolume;
    public Camera cam;
    public Movement move;

    [Header("Settings")]
    public float moveSpeed = 10f;
    public float rotateSpeed = 5f;
    public float zoomSpeed = 0.5f;
    public float minZoom = 0.2f;
    public float maxZoom = 1.5f;
    public float defaultZoom = 0.7f;

    private Transform currentObj;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private float currentZoom;
    private bool inspecting = false;

    public static InspectManager Instance { get; private set; }

    void Awake()
    {
        // --- OPTION 1: Force parent + self to stay active ---
        if (transform.parent != null && !transform.parent.gameObject.activeSelf)
        {
            Debug.LogWarning("Parent object was inactive at startup, forcing activation: " + transform.parent.name);
            transform.parent.gameObject.SetActive(true);
        }

        if (!gameObject.activeSelf)
        {
            Debug.LogWarning("InspectManager object was inactive at startup, forcing activation.");
            gameObject.SetActive(true);
        }

        // Assign this instance
        Instance = this;

        // Destroy duplicates
        var managers = FindObjectsByType<InspectManager>(FindObjectsSortMode.None);
        foreach (var m in managers)
        {
            if (m != this)
            {
                Debug.LogWarning("Duplicate InspectManager detected, destroying extra instance: " + m.name);
                Destroy(m.gameObject);
            }
        }
    }

    void OnEnable()
    {
        Debug.Log("InspectManager enabled");
    }

void OnDisable()
{
    if (!Application.isPlaying) return;

    Debug.LogWarning("InspectManager disabled unexpectedly, scheduling safe reactivation.");

    // Use Invoke instead of coroutine — runs after activation finishes
    Invoke(nameof(ReenableSafely), 0.05f);
}

private void ReenableSafely()
{
    if (transform.parent != null && !transform.parent.gameObject.activeSelf)
        transform.parent.gameObject.SetActive(true);

    if (!gameObject.activeSelf)
        gameObject.SetActive(true);

    enabled = true;

    Debug.Log("InspectManager reactivated successfully.");
}

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (move == null)
            move = UnityEngine.Object.FindFirstObjectByType<Movement>();

        currentZoom = defaultZoom;
    }

    public void StartInspect(Transform obj)
    {
        if (cam == null)
            cam = Camera.main;

        if (move == null)
            move = UnityEngine.Object.FindFirstObjectByType<Movement>();

        if (inspectPoint == null)
            inspectPoint = cam.transform;

        currentObj = obj;

        // Save original transform
        originalPos = obj.position;
        originalRot = obj.rotation;

        inspecting = true;
        move.isInspecting = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (inspectUI != null)
            inspectUI.SetActive(true);

        if (blurVolume != null)
            blurVolume.enabled = false;

        currentZoom = defaultZoom;

        // Start right-side-up, bottom flipped to top
        currentObj.rotation = Quaternion.LookRotation(cam.transform.forward, Vector3.up) * Quaternion.Euler(0f, 0f, 180f);

        // Move object in front of camera
        currentObj.position = inspectPoint.position + cam.transform.forward * defaultZoom;

        Debug.Log("InspectManager: Inspect started");
    }

    void Update()
    {
        if (!inspecting || currentObj == null)
            return;

        HandleMovement();
        HandleRotation();
        HandleZoom();

        if (Input.GetKeyDown(KeyCode.E))
        {
            StopInspect();
        }
    }

    private void HandleMovement()
    {
        Vector3 targetPos = inspectPoint.position + cam.transform.forward * currentZoom;
        currentObj.position = Vector3.Lerp(currentObj.position, targetPos, Time.deltaTime * moveSpeed);
    }

    private void HandleRotation()
    {
        if (!Input.GetMouseButton(0))
            return;

        float mx = Input.GetAxis("Mouse X") * rotateSpeed;
        float my = Input.GetAxis("Mouse Y") * rotateSpeed;

        currentObj.Rotate(Vector3.up, mx, Space.World);
        currentObj.Rotate(cam.transform.right, my, Space.World);
    }

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        currentZoom = Mathf.Clamp(currentZoom - scroll * zoomSpeed, minZoom, maxZoom);
    }

    public void StopInspect()
    {
        inspecting = false;
        move.isInspecting = false;

        currentObj.position = originalPos;
        currentObj.rotation = originalRot;

        currentObj = null;
        currentZoom = defaultZoom;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (inspectUI != null)
            inspectUI.SetActive(false);

        if (blurVolume != null)
            blurVolume.enabled = false;

        Debug.Log("InspectManager: Inspect stopped");
    }

    public bool IsInspecting()
    {
        return inspecting;
    }
}
