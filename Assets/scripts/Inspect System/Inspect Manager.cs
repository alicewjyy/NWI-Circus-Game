// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;

// public class InspectManager : MonoBehaviour
// {
//     [Header("References")]
//     public Transform inspectPoint;        // Empty object in front of camera
//     public GameObject inspectUI;          // UI overlay
//     public Volume blurVolume;             // URP Global Volume with Depth of Field
//     public Camera cam;                    // Main camera reference
//     public Movement move;               // Player movement script

//     [Header("Settings")]
//     public float moveSpeed = 10f;
//     public float rotateSpeed = 5f;
//     public float zoomSpeed = 0.5f;
//     public float minZoom = 0.2f;
//     public float maxZoom = 1.5f;
//     public float defaultZoom = 0.7f;

//     // Internal state
//     private Transform currentObj;
//     private Vector3 originalPos;
//     private Quaternion originalRot;
//     private float currentZoom;
//     private bool inspecting = false;

//     // Cooldown to prevent instant re‑inspection
//     private float exitCooldown = 0f;
//     private const float cooldownDuration = 0.2f; // 200 ms

//     void Start()
//     {
//         if (cam == null)
//             cam = Camera.main;

//         currentZoom = defaultZoom;
//     }

//     // Called by Interactor when pressing E and raycast hits an Inspectable
//     public void StartInspect(Transform obj)
//     {
//         currentObj = obj;

//         // Save original transform
//         originalPos = obj.position;
//         originalRot = obj.rotation;

//         inspecting = true;
//         move.isInspecting = true;

//         Cursor.lockState = CursorLockMode.None;
//         Cursor.visible = true;

//         // Enable UI
//         if (inspectUI != null)
//             inspectUI.SetActive(true);

//         // Enable blur (URP uses .active)
//         if (blurVolume != null)
//             blurVolume.enabled = true;
//     }

//     void Update()
//     {
//         // Handle cooldown timer
//         if (exitCooldown > 0f)
//             exitCooldown -= Time.deltaTime;

//         if (!inspecting || currentObj == null)
//             return;

//         HandleMovement();
//         HandleRotation();
//         HandleZoom();

//         // Exit inspect mode
//         if (Input.GetKeyDown(KeyCode.E))
//         {
//             StopInspect();
//         }
//     }

//     private void HandleMovement()
//     {

//         // Desired position = inspect point + zoom offset
//         Vector3 targetPos = inspectPoint.position + cam.transform.forward * currentZoom;

//         // Smooth movement
//         currentObj.position = Vector3.Lerp(
//             currentObj.position,
//             targetPos,
//             Time.deltaTime * moveSpeed
//         );
//     }

//     private void HandleRotation()
//     {
//         float mx = Input.GetAxis("Mouse X");
//         float my = Input.GetAxis("Mouse Y");

//         // Rotate around world up (horizontal)
//         currentObj.Rotate(Vector3.up, mx * rotateSpeed, Space.World);

//         // Rotate around camera right (vertical)
//         currentObj.Rotate(cam.transform.right, -my * rotateSpeed, Space.World);
//     }

//     private void HandleZoom()
//     {
//         float scroll = Input.mouseScrollDelta.y;

//         currentZoom = Mathf.Clamp(
//             currentZoom - scroll * zoomSpeed,
//             minZoom,
//             maxZoom
//         );
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

//         // Disable UI
//         if (inspectUI != null)
//             inspectUI.SetActive(false);

//         // Disable blur
//         if (blurVolume != null)
//             blurVolume.enabled = false;

//         // Start cooldown to prevent instant re‑inspection
//         exitCooldown = cooldownDuration;
//     }

//     // Helper for Interactor
//     public bool IsInspecting()
//     {
//         //Also block re‑inspection during cooldown
//         return inspecting || exitCooldown > 0f;
//     }
// }

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InspectManager : MonoBehaviour
{
    [Header("References")]
    public Transform inspectPoint;        // Empty object in front of camera
    public GameObject inspectUI;          // UI overlay
    public Volume blurVolume;             // URP Global Volume
    public Camera cam;                    // Main camera
    public Movement move;                 // Player movement script

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

    private float exitCooldown = 0f;
    private const float cooldownDuration = 0.2f;

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

        exitCooldown = cooldownDuration;

        // Make object face the camera right-side-up
        currentObj.rotation = Quaternion.LookRotation(cam.transform.forward, Vector3.up) * Quaternion.Euler(180f, 0f, 0f);

        // Move object directly in front of camera
        currentObj.position = inspectPoint.position + cam.transform.forward * defaultZoom;

        Debug.Log("InspectManager: Inspect started");
    }


    void Update()
    {
        if (exitCooldown > 0f)
            exitCooldown -= Time.deltaTime;

        if (!inspecting || currentObj == null)
            return;

        HandleMovement();
        HandleRotation();
        HandleZoom();

        // Exit inspect mode (E key)
        if (exitCooldown <= 0f && Input.GetKeyDown(KeyCode.E))
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

        // Horizontal rotation (left/right)
        currentObj.Rotate(Vector3.up, mx, Space.World);

        // Vertical rotation (up/down) — inverted so dragging down makes that side go down
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

        // Restore original transform
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

        exitCooldown = cooldownDuration;

        Debug.Log("InspectManager: Inspect stopped");
    }

    public bool IsInspecting()
    {
        return inspecting || exitCooldown > 0f;
    }
}
