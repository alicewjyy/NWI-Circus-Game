// using UnityEngine;

// public class CollectableFollower : MonoBehaviour
// {
//     public float followSpeed = 5f;

//     private Transform target;
//     private bool isCollected = false;

//     public bool IsCollected => isCollected;

//     public void Collect(Transform player)
//     {
//         target = player;
//         isCollected = true;
//     }

    // private void Update()
    // {
    //     if (!isCollected || target == null)
    //         return;

    //     Vector3 followPos =
    //         target.position
    //         - target.forward * 0.8f
    //         + Vector3.up * 0.5f;

    //     transform.position = Vector3.Lerp(
    //         transform.position,
    //         followPos,
    //         Time.deltaTime * followSpeed
    //     );
    // }
using UnityEngine;

public class CollectableFollower : MonoBehaviour
{
    public float followSpeed = 10f;

    [Header("Carry Position")]
    public float carryDistance = 1.5f;
    public float carryHeight = -0.2f;

    private Camera playerCam;
    private bool isCollected = false;

    public bool IsCollected => isCollected;

    public void Collect(Transform player)
    {
        if (isCollected)
            return;

        playerCam = Camera.main;
        isCollected = true;
    }

    private void Update()
    {
        if (!isCollected || playerCam == null)
            return;

        Vector3 targetPosition =
            playerCam.transform.position +
            playerCam.transform.forward * carryDistance +
            playerCam.transform.up * carryHeight;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * followSpeed
        );

        transform.rotation = playerCam.transform.rotation;
    }
}
