using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6f;
    public float crouchSpeed = 3f; // Velocidad al agacharse
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Ratón")]
    public float mouseSensitivity = 800f;

    [Header("Estado")]
    public bool isHidden = false; // Si true, el jugador no se puede mover

    [Header("Agacharse")]
    public float crouchHeight = 1f; // Altura del CharacterController agachado
    private float standHeight; // Altura normal
    public float crouchCameraY = 0.5f; // Altura relativa de la cámara al agacharse
    private float standCameraY;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    public Transform playerCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        standHeight = controller.height;
        standCameraY = playerCamera.localPosition.y;
    }

    void Update()
    {
        if (isHidden) return; // Si está escondido, no hacer nada

        HandleMouseLook();
        HandleMovement();
        HandleCrouch();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Cambiar velocidad si está agachado
        float currentSpeed = (controller.height == crouchHeight) ? crouchSpeed : speed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Gravedad + salto
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = crouchHeight;
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, crouchCameraY, playerCamera.localPosition.z);
        }
        else
        {
            controller.height = standHeight;
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, standCameraY, playerCamera.localPosition.z);
        }
    }
}
