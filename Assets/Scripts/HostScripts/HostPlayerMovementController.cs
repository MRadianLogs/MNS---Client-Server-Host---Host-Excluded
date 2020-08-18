using UnityEngine;

public class HostPlayerMovementController : MovementController
{
    [SerializeField] private Player player = null;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;

    [SerializeField] private Transform playerCamera = null;
    [SerializeField] private CharacterController playerController = null;

    private Vector3 velocity;
    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask = default;
    private bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerMovementInputsMove();
        UpdateGravityJumpCheck();
    }

    private void FixedUpdate()
    {
        if(NetworkManager.instance.serverActive)
        {
            ServerSend.PlayerPosition(player);
            ServerSend.PlayerRotation(player);
        }
    }

    private void UpdatePlayerMovementInputsMove()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = (playerCamera.right * xInput) + (new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z) * zInput);

        playerController.Move(moveDirection * moveSpeed * Time.deltaTime);

        //Send position and rotation over server.
    }
    private void UpdateGravityJumpCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
/*
        if (isGrounded && (velocity.y < 0))
        {
            velocity.y = -2;
        }
*/
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);
    }
}
