using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float sensitivity = 300f;
    public Transform playerBody;

    private float xRotation = 0f;

    private Movement move;   // reference to movement script

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        move = playerBody.GetComponent<Movement>();
    }

    void Update()
    {
        if (move != null && move.isInspecting)
            return;   // stop camera movement during inspect

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
