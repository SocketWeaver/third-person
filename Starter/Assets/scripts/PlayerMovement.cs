using UnityEngine;

/// <summary>
/// Player movement.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6.0F;
    public float gravity = -15f;
    private float verticalVelocity;
    public float jumpForce = 7.0f;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // set CameraFollow target
        Camera m_MainCamera = Camera.main;
        CameraFollow cameraFollow = m_MainCamera.GetComponent<CameraFollow>();
        cameraFollow.target = gameObject;
    }

    void Update()
    {
        // get keyboard inputs
        float speedX = Input.GetAxis("Horizontal") * moveSpeed;
        float speedZ = Input.GetAxis("Vertical") * moveSpeed;

        if (characterController.isGrounded)
        {
            verticalVelocity = gravity;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 movement = new Vector3(speedX, verticalVelocity, speedZ);
        characterController.Move(movement * Time.deltaTime);
    }

    public void Jump()
    {
        verticalVelocity = jumpForce;
    }
}
