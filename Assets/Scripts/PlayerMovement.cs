using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //Components
    private CharacterController characterController;
    private Transform cameraTransform;
    private Camera camera;
    private WeaponController weaponController;
    private GranadeController granadeController;

    [Header("Player Move Setting")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float multiplier = 2f;
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float gravity = Physics.gravity.y;
    private float targetSpeed;

    //Input fields for movement and look actions
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 velocity;
    private float verticalVelocity;
    private float verticalRotation = 0;

    [Header("Camera Setting")]
    [SerializeField] private float lookSensibility = 1f;
    [SerializeField] private float maxLookUpAngle = 80f;
    [SerializeField] private float maxLookDownAngle = 50f;

    [Header("Stamina Bar")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 10f;
    [SerializeField] private float staminaRegenRate = 5f;
    private float currentStamina;

    //Reference to the slider
    [SerializeField] private Slider staminaBar;

    [Header("Jetpack Settings")]
    [SerializeField] private float jetpackForce = 10f;   
    [SerializeField] private float maxJetpackFuel = 100f; 
    [SerializeField] private float jetpackRechargeRate = 10f;
    [SerializeField] private float jetpackDrainRate = 20f;
    private float currentJetpackFuel;
    private bool isJetpacking;
    private bool canUseJetpack = true;

    [SerializeField] private Slider jetpackBar;

    [Header("Jetpack Limit")]
    [SerializeField] private float maxJetpackHeight = 20f;


    //Is Sprinting state
    private bool isSprinting;
    private bool isMoving;
    private bool isJumping;
    private bool activatePanelPause;

    //Animator
    private Animator animator;

    public bool IsSprinting { get => isSprinting; set => isSprinting = value; }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        camera = Camera.main;
        weaponController = GetComponent<WeaponController>();
        granadeController = GetComponent<GranadeController>();

        //Hide mouse coursor
        if(int.Parse(SceneManager.GetActiveScene().name.Substring(5)) == 1)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        animator = GetComponent<Animator>();
        //Initialize StaminaBar to Max
        currentStamina = maxStamina;
        currentJetpackFuel = maxJetpackFuel;
        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = currentStamina;
        }

        if (jetpackBar != null)
        {
            jetpackBar.maxValue = maxJetpackFuel;
            jetpackBar.value = currentJetpackFuel;
        }
        

    }
    private void Update()
    {
        
        MovePlayer();
        LookAround();
        HandleStamina();
        HandleJetpackUI();
        if (isJetpacking)
        {
            AudioManager.Instance.PlayJetpackSound();
        }
        else
        {
            AudioManager.Instance.StopJetpackSound();
        }

        if (isMoving && isSprinting && !isJetpacking)
        {
                AudioManager.Instance.PlaySprintSound();
        }
        else
        {
            AudioManager.Instance.StopSprintSound();
        }

        if (isMoving && !isSprinting && !isJetpacking)
        {
                AudioManager.Instance.PlayStepsSound();
        }
        else
        {
            AudioManager.Instance.StopStepsSound();
        }
    }

    private void MovePlayer()
    {
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0)
                verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Jetpack
        if (isJetpacking && currentJetpackFuel > 0f)
        {
            if (transform.position.y < maxJetpackHeight)
            {
                verticalVelocity += jetpackForce * Time.deltaTime;
            }
            else
            {
                verticalVelocity = Mathf.Min(verticalVelocity, 0f);
            }
            currentJetpackFuel -= jetpackDrainRate * Time.deltaTime;
        }
        else if (characterController.isGrounded)
        {
            currentJetpackFuel += jetpackRechargeRate * Time.deltaTime;
            currentJetpackFuel = Mathf.Min(currentJetpackFuel, maxJetpackFuel);
        }

        if (isJetpacking)
        {
            targetSpeed = speed;
        }
        else
        {
           targetSpeed = isSprinting ? speed * multiplier : speed;
        }  
        // Move
        Vector3 move = new Vector3(moveInput.x * targetSpeed,verticalVelocity,moveInput.y * targetSpeed);

        move = transform.TransformDirection(move);
        characterController.Move(move * Time.deltaTime);

        float targetAnimSpeed = moveInput.magnitude > 0.1f
            ? (isSprinting && !isJetpacking ? 6f : 2f)
            : 0f;

        animator.SetFloat("Speed", targetAnimSpeed, 0.1f, Time.deltaTime);

        //Move camera because if dont move camera you can see when run the interior character
        float near = isSprinting && !isJetpacking ? 0.3f : 0.01f;
        camera.nearClipPlane = Mathf.Lerp(camera.nearClipPlane,near,Time.deltaTime*5f);
    }

    /// <summary>
    /// Handles camera rotation based on Look Input
    /// </summary>
    private void LookAround()
    {
        //Horizontal rotation (X-axis) based on sensibility and input
        float horizontalRotation = lookInput.x * lookSensibility;
        transform.Rotate(Vector3.up * horizontalRotation);

        //Vertical rotation (Y-axis) with clamping to prevent over-rotation
        verticalRotation -= lookInput.y * lookSensibility;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookUpAngle, maxLookDownAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

    }
    private void HandleStamina()
    {
        //Using Stamina
        if (isSprinting && isMoving && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;

            //if totally used
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprinting = false;
            }
        }
        //Regenerate Stamins
        else if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        //Update Stamina bar
        staminaBar.value = currentStamina;
    }
    private void HandleJetpackUI()
    {
        if (jetpackBar != null)
        {
            jetpackBar.value = currentJetpackFuel;
            if (jetpackBar.value==maxJetpackFuel)
            {
                jetpackBar.gameObject.SetActive(false);
            }
            else
            {
                jetpackBar.gameObject.SetActive(true);
            }
        }
    }


    #region INPUT SYSTEM
    public void Move(InputAction.CallbackContext context)
    {
        
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput != Vector2.zero;
    }

    public void Look(InputAction.CallbackContext context)
    {
        
        lookInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            if (characterController.isGrounded)
            {
                // Salto normal
                verticalVelocity = Mathf.Sqrt(jumpForce * -1f * gravity);
            }
        }

        // Jetpack mientras se mantiene la tecla
        if ((context.started || context.performed) && !characterController.isGrounded && canUseJetpack && currentJetpackFuel > 0f)
        {
            isJetpacking = true;
            if (verticalVelocity < 0f)
                verticalVelocity = 0f;

            verticalVelocity += jetpackForce * 0.2f;
        }

        if (context.canceled)
        {
            isJetpacking = false;
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        
        if (!isJetpacking)
        {
            isSprinting = context.started || context.performed;
        }
        else
        {
            isSprinting = false;
        }
    }


    public void Shoot(InputAction.CallbackContext context)
    {
        
        if (weaponController.CanShoot()) weaponController.Shoot();
    }

    public void Aim(InputAction.CallbackContext context)
    {
        
        if ((context.started || context.performed))
        {
            weaponController.AimShoot();
        }
        else if (context.canceled)
        {
            weaponController.StopAim(); 
        }
    }

    public void TrowItem(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            granadeController.putGranade();
        }
        /*if (context.performed){
            granadeController.PositionThrow();
        }*/
        if (context.canceled)
        {
            granadeController.ThrowGranade();
        }

    }
    public void RechargeWeapon(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            weaponController.Recharge();
        }
    }
    public void Pause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!activatePanelPause)
            {
                activatePanelPause = true;   
                LevelManager.Instance.ActivatePausePanel();
            }
            else
            {
                activatePanelPause = false;
                LevelManager.Instance.DesactivatePauseLevel();
            }
            
        }
    }

    #endregion
}
