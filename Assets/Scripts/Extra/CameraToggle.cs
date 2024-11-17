using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    [SerializeField] private GameObject thirdPersonCamera; 
    [SerializeField] private GameObject freeCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            bool isFreeCamActive = freeCamera.activeSelf;

            thirdPersonCamera.SetActive(isFreeCamActive);
            freeCamera.SetActive(!isFreeCamActive);
            
            if (isFreeCamActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}