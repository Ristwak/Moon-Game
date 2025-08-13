using UnityEngine;

public class BorderCollision : MonoBehaviour
{
    public HandGesturePlayerMove handGesturePlayerMove;
    public Transform mapCenter; // Assign center of the map in inspector
    public float rotateSpeed = 5f;
    public LayerMask groundLayer; // Assign your ground layer here
    public float groundCheckDistance = 1.2f;
    public float pushBackDistance = 0.5f; // How far to push inward

    private CharacterController controller;
    private bool isRotatingBack = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        handGesturePlayerMove = GetComponent<HandGesturePlayerMove>();
    }

    void Update()
    {
        // Ground check - keep them on the map
        if (!IsGrounded())
        {
            Vector3 directionToCenter = (mapCenter.position - transform.position).normalized;
            // controller.Move(directionToCenter * Time.deltaTime * 3f);
        }

        // If currently rotating back, keep rotating until facing center
        if (isRotatingBack)
        {
            RotateTowardsCenter();
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Border"))
        {
            HandleBorderCollision();
        }
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.CompareTag("Border"))
        {
            HandleBorderCollision();
        }
    }

    void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.CompareTag("Border"))
        {
            Debug.Log("Player left border area - movement enabled");
            handGesturePlayerMove.MovePlayer();
        }
    }

    void HandleBorderCollision()
    {
        Debug.Log("Player hit border - stopping movement");
        handGesturePlayerMove.StopPlayer(); // Stop player movement
        isRotatingBack = true; // Begin rotation process

        // Push player slightly inward
        Vector3 pushDirection = (mapCenter.position - transform.position).normalized;
        pushDirection.y = 0; // Keep movement flat
        controller.Move(pushDirection * pushBackDistance);
    }

    void RotateTowardsCenter()
    {
        Vector3 dirToCenter = (mapCenter.position - transform.position).normalized;
        dirToCenter.y = 0; // keep rotation flat

        Quaternion targetRotation = Quaternion.LookRotation(dirToCenter, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );

        // Stop rotating when facing center
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            isRotatingBack = false;
        }
    }
}
