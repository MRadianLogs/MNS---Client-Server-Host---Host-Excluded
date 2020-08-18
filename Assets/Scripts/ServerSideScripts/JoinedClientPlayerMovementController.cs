using UnityEngine;

/// <summary>
/// This class moves a client whom has joined a host, by having inputs set, using the SetInputs method, over the network.
/// </summary>
public class JoinedClientPlayerMovementController : MovementController
{
    [SerializeField] private Player player = null;
    [SerializeField] private CharacterController controller = null;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    
    private bool[] movementInputs;
    private float yVelocity = 0;

    private void Awake()
    {
        movementInputs = new bool[5];
    }

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void FixedUpdate()
    {
        Vector2 inputDirection = Vector2.zero;
        if (movementInputs[0])
        {
            inputDirection.y += 1; //W
        }
        if (movementInputs[1])
        {
            inputDirection.x -= 1; //A  //T:S
        }
        if (movementInputs[2])
        {
            inputDirection.y -= 1; //S //T:A
        }
        if (movementInputs[3])
        {
            inputDirection.x += 1; //D
        }

        Move(inputDirection);
    }

    private void Move(Vector2 inputDirection)
    {
        Vector3 moveDirection = transform.right * inputDirection.x + transform.forward * inputDirection.y;
        moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f; //If on ground, no move vertical velocity in any direction.
            if(movementInputs[4])//Jump key pressed.
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        moveDirection.y = yVelocity;
        controller.Move(moveDirection);


        ServerSend.PlayerPosition(player);
        ServerSend.PlayerRotation(player);
    }

    public override void SetInputs(bool[] inputs, Quaternion newRotation)
    {
        movementInputs = inputs;
        transform.rotation = newRotation;
    }
}
