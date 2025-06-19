using UnityEngine;

public class BallerFPSController : MonoBehaviour
{
    [Header("References")]
    public BallerComponents ballerComponents;
    public float walkingSpeed = 2.5f;
    public float runningSpeed = 4.5f;
    public float jumpSpeed = 4f;
    public float gravity = 9.0f;
    public float lookSpeed = 3f;
    public float lookXLimit = 66.0f;
    public float bobSpeed = 20f;
    public float bobAmount = 0.05f;

    // Private parameters
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float defaultYPos = 0;
    private float bobTimer = 0;

    [HideInInspector]
    public bool canMove = true;

    // Initialize default settings
    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultYPos = ballerComponents.playerCamera.transform.localPosition.y;
    }

    // Update is called once per frame
    public void Update()
    {
        // Get movement directions
        Vector3 forward = ballerComponents.player.transform.TransformDirection(Vector3.forward);
        Vector3 right = ballerComponents.player.transform.TransformDirection(Vector3.right);

        // Handle running state
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Adjust Field of View (FOV) when running
        AdjustFOV(isRunning);

        // Get movement inputs and set move direction
        float curSpeedX = GetCurrentSpeedX(isRunning);
        float curSpeedY = GetCurrentSpeedY(isRunning);
        float movementDirectionY = moveDirection.y;

        // Update Animator with movement speed
        UpdateAnimator(curSpeedX, curSpeedY);

        // Calculate movement direction based on user input
        moveDirection = CalculateMovementDirection(forward, right, curSpeedX, curSpeedY);

        // Handle jumping
        HandleJumping(movementDirectionY);

        // Apply gravity if not grounded
        ApplyGravityIfNeeded();

        // Move the character
        MoveCharacter();

        // Handle camera rotation and headbobbing
        HandleCameraRotationAndHeadbobbing(curSpeedX, curSpeedY);
    }

    // Adjust Field of View (FOV) when running
    private void AdjustFOV(bool isRunning)
    {
        ballerComponents.playerCamera.fieldOfView = isRunning
            ? Mathf.Lerp(ballerComponents.playerCamera.fieldOfView, 90, Time.deltaTime * 3)
            : Mathf.Lerp(ballerComponents.playerCamera.fieldOfView, 75, Time.deltaTime * 3);
    }

    // Get horizontal speed based on user input
    private float GetCurrentSpeedX(bool isRunning)
    {
        return canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
    }

    // Get vertical speed based on user input
    private float GetCurrentSpeedY(bool isRunning)
    {
        return canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
    }

    // Update the animator parameters for movement
    private void UpdateAnimator(float curSpeedX, float curSpeedY)
    {
        ballerComponents.ballerAnimator.SetFloat("x", curSpeedX);
        ballerComponents.ballerAnimator.SetFloat("y", curSpeedY);
    }

    // Calculate movement direction based on inputs
    private Vector3 CalculateMovementDirection(Vector3 forward, Vector3 right, float curSpeedX, float curSpeedY)
    {
        return (forward * curSpeedX) + (right * curSpeedY);
    }

    // Handle jumping if the jump button is pressed and the player is grounded
    private void HandleJumping(float movementDirectionY)
    {
        if (Input.GetButton("Jump") && canMove && ballerComponents.characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
            ballerComponents.ballerAnimator.SetBool("jumping", true);
            Invoke(nameof(CompleteJumping), 0.8f);
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
    }

    // Apply gravity if the player is not grounded
    private void ApplyGravityIfNeeded()
    {
        if (!ballerComponents.characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
    }

    // Move the character based on the calculated move direction
    private void MoveCharacter()
    {
        ballerComponents.characterController.Move(moveDirection * Time.deltaTime);
    }

    // Handle camera rotation and headbobbing based on speed and mouse input
    private void HandleCameraRotationAndHeadbobbing(float curSpeedX, float curSpeedY)
    {
        if (canMove)
        {
            // Camera rotation
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            ballerComponents.playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            ballerComponents.player.transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

            // Headbobbing effect based on movement
            if (Mathf.Abs(curSpeedX) > 0.1f || Mathf.Abs(curSpeedY) > 0.1f)
            {
                bobTimer += Time.deltaTime * bobSpeed;

                float bobOffset = Mathf.Sin(bobTimer) * bobAmount * (ballerComponents.characterController.velocity.magnitude / runningSpeed);
                
                ballerComponents.playerCamera.transform.localPosition = new Vector3(
                    ballerComponents.playerCamera.transform.localPosition.x,
                    defaultYPos + bobOffset,
                    ballerComponents.playerCamera.transform.localPosition.z);
            }
            else
            {
                ballerComponents.playerCamera.transform.localPosition = new Vector3(
                    ballerComponents.playerCamera.transform.localPosition.x, 
                    defaultYPos, 
                    ballerComponents.playerCamera.transform.localPosition.z);
            }
        }
    }

    // Complete the jumping animation after a delay
    public void CompleteJumping()
    {
        ballerComponents.ballerAnimator.SetBool("jumping", false);
    }
}