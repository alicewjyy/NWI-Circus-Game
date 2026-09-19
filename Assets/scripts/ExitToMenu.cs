using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Unlock and show the mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Return to menu
            SceneManager.LoadScene("Menu");
        }
    }
}