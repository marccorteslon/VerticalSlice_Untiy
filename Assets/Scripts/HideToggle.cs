using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HideToggle : MonoBehaviour
{
    public Transform teleportTarget; // Posición del jugador al esconderse
    public Transform cameraTarget;   // Posición y rotación de la cámara al esconderse
    public float interactDistance = 3f;

    private bool playerIsNear = false;
    private Vector3 previousPosition;
    private bool isTeleported = false;

    private Transform player;
    private CharacterController playerController;
    private Movement playerMovement;
    private Transform playerCamera;
    private Quaternion originalCameraRotation;
    private Vector3 originalCameraPosition;

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(KeyCode.E) && teleportTarget != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= interactDistance)
            {
                if (!isTeleported)
                {
                    // Guardamos posición y cámara original
                    previousPosition = player.position;
                    playerCamera = playerMovement.playerCamera;
                    originalCameraPosition = playerCamera.position;
                    originalCameraRotation = playerCamera.rotation;

                    Teleport(player, teleportTarget.position);
                    MoveCamera(cameraTarget.position, cameraTarget.rotation);

                    isTeleported = true;
                    if (playerMovement != null)
                        playerMovement.isHidden = true; // Bloquea movimiento y rotación
                }
                else
                {
                    Teleport(player, previousPosition);
                    RestoreCamera();

                    isTeleported = false;
                    if (playerMovement != null)
                        playerMovement.isHidden = false; // Desbloquea movimiento y rotación
                }
            }
        }
    }

    private void Teleport(Transform target, Vector3 position)
    {
        if (playerController != null)
        {
            playerController.enabled = false;
            target.position = position;
            playerController.enabled = true;
        }
        else
        {
            target.position = position;
        }
    }

    private void MoveCamera(Vector3 position, Quaternion rotation)
    {
        if (playerCamera != null)
        {
            playerCamera.position = position;
            playerCamera.rotation = rotation;
        }
    }

    private void RestoreCamera()
    {
        if (playerCamera != null)
        {
            playerCamera.position = originalCameraPosition;
            playerCamera.rotation = originalCameraRotation;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            player = other.transform;
            playerController = other.GetComponent<CharacterController>();
            playerMovement = other.GetComponent<Movement>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            player = null;
            playerController = null;
            playerMovement = null;
            playerCamera = null;
        }
    }
}
