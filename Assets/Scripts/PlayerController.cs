    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 6f;
    public float crouchSpeed = 2f;
    public float leftRightWalkSpeed = 2f;
    public float leftRightRunSpeed = 3f;
    public float leftRightCrrouchSpeed = 1f;


    [Header("Crouch Settings")]
    public float crouchHeight = 0.5f;
    private float normalHeight;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float rotationX = 0f;
    private bool isCrouching = false;

    [Header("Physics")]
    public float gravity = 9.8f;
    private float verticalVelocity;

    [Header("Flashlight Settings")]
    public Light flashlight;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        normalHeight = characterController.height;
        flashlight.enabled = false;
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleCrouch();
        HandleFlashlight();
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool canSprint = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        float speedX = canSprint ? leftRightRunSpeed : (isCrouching ? leftRightCrrouchSpeed : leftRightWalkSpeed);
        float speedZ = canSprint ? runSpeed : (isCrouching ? crouchSpeed : walkSpeed);

        float moveDirectionX = Input.GetAxis("Horizontal") * speedX;
        float moveDirectionZ = Input.GetAxis("Vertical") * speedZ;

        if (characterController.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        moveDirection = forward * moveDirectionZ + right * moveDirectionX;
        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleCamera()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        float rotationY = Input.GetAxis("Mouse X") * lookSpeed;
        transform.Rotate(0, rotationY, 0);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            StartCoroutine(SmoothCrouchTransition(isCrouching ? crouchHeight : normalHeight));
        }
    }

    private IEnumerator SmoothCrouchTransition(float targetHeight)
    {
        float initialHeight = characterController.height;
        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            characterController.height = Mathf.Lerp(initialHeight, targetHeight, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
    }

    void HandleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F) && flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }
}
