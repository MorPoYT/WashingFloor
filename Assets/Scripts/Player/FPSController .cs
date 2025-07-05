using UnityEngine;

/// <summary>
/// FPS контроллер с бегом, стаминой, приседанием и покачиванием камеры
/// Требует CharacterController на объекте
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Mouse Settings")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Stamina Settings")]
    public float maxStamina = 5f;
    public float staminaDrainPerSecond = 1f;
    public float staminaRecoveryPerSecond = 0.5f;
    public float staminaRecoveryDelay = 1f; 
    private float lastStaminaUseTime;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchTransitionSpeed = 8f;
    public float cameraCrouchHeight = 0.5f;

    [Header("Head Bob Settings")]
    public float walkBobAmount = 0.05f;
    public float runBobAmount = 0.1f;
    public float crouchBobAmount = 0.02f;
    public float bobFrequency = 8f;

    [Header("UI Settings")]
    public UnityEngine.UI.Image staminaBarFill;

    private CharacterController characterController;
    private Vector3 velocity;
    private float cameraPitch = 0f;
    private float stamina;
    private float bobTimer;

    private Vector3 originalCameraLocalPos;
    private float targetCameraY;

    private bool isCrouching;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null)
            Debug.LogError("Camera Transform не назначен!");

        originalCameraLocalPos = cameraTransform.localPosition;
        targetCameraY = originalCameraLocalPos.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        stamina = maxStamina;
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleHeadBob();
        HandleCrouch();
        UpdateUI();
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        bool wantsToRun = Input.GetKey(KeyCode.LeftShift);
        bool isMoving = inputX != 0 || inputZ != 0;
        bool canRun = wantsToRun && stamina > 0f && !isCrouching;

        float currentSpeed = walkSpeed;

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        if (canRun && isMoving)
        {
            currentSpeed = runSpeed;
            stamina -= staminaDrainPerSecond * Time.deltaTime;
            if (stamina < 0f) stamina = 0f;
            lastStaminaUseTime = Time.time; // Запоминаем время последнего использования
        }
        else if (Time.time > lastStaminaUseTime + staminaRecoveryDelay) // Восстанавливаем только после задержки
        {
            stamina += staminaRecoveryPerSecond * Time.deltaTime;
            if (stamina > maxStamina) stamina = maxStamina;
        }

        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        if (characterController.isGrounded)
        {
            velocity.y = -2f;

            if (Input.GetButtonDown("Jump") && !isCrouching)
                velocity.y = jumpForce;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    private void HandleHeadBob()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        bool isMoving = inputX != 0 || inputZ != 0;

        if (isMoving && characterController.isGrounded)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift) && stamina > 0f && !isCrouching;
            float bobAmount = isRunning ? runBobAmount : (isCrouching ? crouchBobAmount : walkBobAmount);

            bobTimer += bobFrequency * Time.deltaTime;
            float bobOffset = Mathf.Sin(bobTimer) * bobAmount;

            Vector3 newPos = originalCameraLocalPos;
            newPos.y = targetCameraY + bobOffset;

            cameraTransform.localPosition = newPos;
        }
        else
        {
            Vector3 targetPos = originalCameraLocalPos;
            targetPos.y = targetCameraY;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, Time.deltaTime * 5f);
            bobTimer = 0f;
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                characterController.height = crouchHeight;
                targetCameraY = cameraCrouchHeight;
            }
            else
            {
                characterController.height = standHeight;
                targetCameraY = originalCameraLocalPos.y;
            }
        }

        // Плавное движение камеры
        Vector3 camPos = cameraTransform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * crouchTransitionSpeed);
        cameraTransform.localPosition = camPos;
    }

    private void UpdateUI()
    {
        if (staminaBarFill != null)
        {
            staminaBarFill.fillAmount = stamina / maxStamina;
        }
    }
}
