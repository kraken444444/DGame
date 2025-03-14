using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Character Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    
    [Header("Camera Settings")]
    public Transform cameraTarget;
    public float cameraSensitivity = 2f;
    public float cameraDistance = 3f;
    public float cameraHeight = 1.5f;
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 60f;
    
    // Private references
    private CharacterController characterController;
    private Transform cameraTransform;
    private Camera playerCamera;
    
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    private float verticalVelocity;
    private float gravity = -9.81f;
    
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTarget = new GameObject("CameraTarget").transform;
        cameraTarget.parent = transform;
        cameraTarget.localPosition = new Vector3(0, cameraHeight, 0);
        
        // Set up camera
        if (Camera.main == null)
        {
            GameObject cam = new GameObject("PlayerCamera");
            playerCamera = cam.AddComponent<Camera>();
        }
        else
        {
            playerCamera = Camera.main;
        }
        
        cameraTransform = playerCamera.transform;
        rotationY = transform.eulerAngles.y;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void Update()
    {
        HandleCameraRotation();
        HandleMovement();
        UpdateCameraPosition();
    }
    
    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;
        
        rotationY += mouseX;
        rotationX -= mouseY;
        
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
    }
    
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 forward = Quaternion.Euler(0, rotationY, 0) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0, rotationY, 0) * Vector3.right;
        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;
        
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f; 
            
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        
        Vector3 movement = moveDirection * currentSpeed;
        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
        
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    private void UpdateCameraPosition()
    {
        cameraTarget.localPosition = new Vector3(0, cameraHeight, 0);
        Vector3 targetPosition = cameraTarget.position;
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        Vector3 offset = rotation * new Vector3(0.5f, 0, -cameraDistance); // slight offset for over the shoulder
        cameraTransform.position = targetPosition + offset;
        cameraTransform.rotation = rotation;
        cameraTransform.LookAt(cameraTarget.position + new Vector3(0, 0.2f, 0));
    }
    
    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

