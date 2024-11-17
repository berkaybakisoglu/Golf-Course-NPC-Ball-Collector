using UnityEngine;

public class FreeCamController : MonoBehaviour
{
    public float lookSpeed = 2f;
    public float moveSpeed = 10f;
    public float boostMultiplier = 2f;

    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0f;

        if (Input.GetKeyDown(KeyCode.Q)) moveY = -1f;
        if (Input.GetKeyDown(KeyCode.E)) moveY = 1f;

        Vector3 moveDirection = transform.right * moveX + transform.up * moveY + transform.forward * moveZ;


        float currentSpeed = moveSpeed;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSpeed *= boostMultiplier;
        }

        // Move the camera
        transform.position += moveDirection * currentSpeed * Time.deltaTime;
    }

    public void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}