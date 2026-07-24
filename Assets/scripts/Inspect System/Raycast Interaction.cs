// using UnityEngine;

// public class Interactor : MonoBehaviour
// {
//     public float interactDistance = 3f;
//     public LayerMask interactableMask;
//     public InspectManager inspectManager;

//     [Header("UI")]
//     public GameObject inspectPrompt;

//     private Camera cam;

//     void Start()
//     {
//         cam = Camera.main;
//     }

//     // void Update()
//     // {
    //     if (inspectManager == null)
    //         return;

    //     // Hide prompt while inspecting
    //     if (inspectManager.IsInspecting())
    //     {
    //         inspectPrompt?.SetActive(false);
    //         return;
    //     }

    //     // Raycast forward
    //     Ray ray = new Ray(cam.transform.position, cam.transform.forward);

    //     if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask))
    //     {
    //         if (hit.collider.TryGetComponent(out Inspectable inspectable))
    //         {
    //             inspectPrompt?.SetActive(true);

    //             if (Input.GetKeyDown(KeyCode.E))
    //             {
    //                 inspectManager.StartInspect(hit.transform);
    //             }
    //         }
    //         else
    //         {
    //             inspectPrompt?.SetActive(false);
    //         }
    //     }
    //     else
    //     {
    //         inspectPrompt?.SetActive(false);
    //     }
    // }
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float interactDistance = 3f;
    public LayerMask interactableMask;
    public InspectManager inspectManager;

    [Header("UI")]
    public GameObject inspectPrompt;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Auto‑repair reference if Unity loses it
        if (inspectManager == null)
            inspectManager = UnityEngine.Object.FindFirstObjectByType<InspectManager>();

        if (inspectManager == null)
            return;

        // Hide prompt while inspecting
        if (inspectManager.IsInspecting())
        {
            inspectPrompt?.SetActive(false);
            return;
        }

        // Raycast forward
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableMask))
        {
            if (hit.collider.TryGetComponent(out Inspectable inspectable))
            {
                inspectPrompt?.SetActive(true);

                // ENTER INSPECT MODE (F key)
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("Interactor: calling StartInspect");
                    inspectManager.StartInspect(hit.transform);
                }
            }
            else
            {
                inspectPrompt?.SetActive(false);
            }
        }
        else
        {
            inspectPrompt?.SetActive(false);
        }
    }
}
