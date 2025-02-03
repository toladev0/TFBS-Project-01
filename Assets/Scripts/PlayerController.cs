using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f; // Speed when walking
    public float runSpeed = 8f; // Speed when running
    public float crouchSpeed = 2.5f; // Speed when crouching

    [Header("Crouch Settings")]
    public float crouchHeight = 1f; // Height of the character when crouching
    private float normalHeight; // Original height of the character

    [Header("Camera Settings")]
    public Transform cameraTransform; // Reference to the player's camera
    public float lookSpeed = 2f; // Sensitivity for looking around
    public float lookXLimit = 45f; // Vertical look angle limit

    private CharacterController characterController; // Reference to the CharacterController component
    private Vector3 moveDirection; // Stores the player's movement direction
    private float rotationX = 0f; // Tracks vertical camera rotation
    private bool isCrouching = false; // Whether the player is crouching

    [Header("Physics")]
    public float gravity = 9.8f; // Gravity applied to the player
    private float verticalVelocity; // Tracks the player's vertical velocity

    [Header("Flashlight Settings")]
    public Light flashlight; // Reference to the flashlight component

    void Start()
    {
        // Initialize references and settings
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the screen
        Cursor.visible = false; // Hide the cursor
        normalHeight = characterController.height; // Save the initial height of the character
        flashlight.enabled = false; // Ensure flashlight is off initially
    }

    void Update()
    {
        // Handle all player inputs and interactions each frame
        HandleMovement(); // Process movement controls
        HandleCamera(); // Process camera rotation
        HandleCrouch(); // Process crouching input
        HandleFlashlight(); // Toggle flashlight
    }

    void HandleMovement()
    {
        // Calculate forward and right directions based on the player's orientation
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Check if the player can sprint (not crouching and holding Left Shift)
        bool canSprint = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        // Determine movement speed based on current state
        float speed = canSprint ? runSpeed : (isCrouching ? crouchSpeed : walkSpeed);

        // Get movement input
        float moveDirectionX = Input.GetAxis("Horizontal") * speed; // Horizontal input
        float moveDirectionZ = Input.GetAxis("Vertical") * speed; // Vertical input

        // Apply gravity to the vertical velocity
        if (characterController.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime; // Reset vertical velocity if grounded
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime; // Apply gravity if airborne
        }

        // Combine movement directions and apply gravity
        moveDirection = forward * moveDirectionZ + right * moveDirectionX;
        moveDirection.y = verticalVelocity;

        // Move the character controller
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleCamera()
    {
        // Rotate the camera vertically based on mouse input
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // Clamp vertical rotation
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Apply vertical rotation

        // Rotate the player horizontally based on mouse input
        float rotationY = Input.GetAxis("Mouse X") * lookSpeed;
        transform.Rotate(0, rotationY, 0);
    }

    void HandleCrouch()
    {
        // Toggle crouch state when the player presses 'C'
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching; // Toggle crouch state
            characterController.height = isCrouching ? crouchHeight : normalHeight; // Adjust character height
        }
    }

    void HandleFlashlight()
    {
        // Toggle flashlight on/off when the player presses 'F'
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashlight != null) // Ensure flashlight reference exists
            {
                flashlight.enabled = !flashlight.enabled; // Toggle flashlight state
            }
        }
    }
}
