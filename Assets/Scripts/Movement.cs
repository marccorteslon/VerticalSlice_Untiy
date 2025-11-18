using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Ratón")]
    public float mouseSensitivity = 200f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    public Transform playerCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // OTACIÓN
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Cámara rota solo verticalmente
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // El cuerpo rota solo en Y
        transform.Rotate(Vector3.up * mouseX);

        // ==== MOVIMIENTO ====
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // RAVEDAD + SALTO
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
