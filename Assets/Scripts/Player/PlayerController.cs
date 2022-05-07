using System.Collections.Generic;
using UnityEngine;
using Unity.DebugTool;


//[RequireComponent(typeof(GravityBody))]

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera PlayerCamera;

    [Tooltip("Audio source for footsteps, jump, etc...")]
    public AudioSource AudioSource;
    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float MaxSpeedOnGround = 10f;

    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float GroundCheckDistance = 0.05f;

    [Tooltip("Sound played when jumping")] public AudioClip JumpSfx;
    [Tooltip("Sound played when landing")] public AudioClip LandSfx;

    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    public float SprintSpeedModifier = 2f;

    // public vars
    public float mouseSensitivityX = 250;
    public float mouseSensitivityY = 250;
    public float walkSpeed = 6;
    float jumpForce = 600;
    Vector3 jumpDirection;
    public LayerMask groundedMask;

    // System vars
    public bool reverseMouse = false;
    public bool IsJumping { get; private set; }
    public bool CanUseJetpack { get; private set; }

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    float verticalLookRotation;
    public Transform cameraTransform;
    Rigidbody rigidBody;
    float multi = 0.95f;

    Health m_Health;
    PlayerWeaponsManager m_WeaponsManager;
    Actor m_Actor;
    InputHandler m_InputHandler;

    float lastTimeJumped = 0f;
    const float k_JumpGroundingPreventionTime = 0.2f;
    const float k_GroundCheckDistanceInAir = 0.07f;
    const float k_skinWidth = 0.08f;

    GravityBody gravityBody;

    public bool IsGrounded { get; private set; }
    public bool HasJumpedThisFrame { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsCrouching { get; private set; }

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;

        jumpDirection = transform.up;

        // 
        gravityBody = GetComponent<GravityBody>();
        if (gravityBody == null) print("ERROR: cant find gravity body");

        ActorsManager actorsManager = FindObjectOfType<ActorsManager>();
        if (actorsManager != null)
            actorsManager.SetPlayer(gameObject);


    }
    private void Start()
    {
        m_WeaponsManager = GetComponent<PlayerWeaponsManager>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, PlayerController>(m_WeaponsManager, this, gameObject);
        m_Actor = GetComponent<Actor>();
        DebugUtility.HandleErrorIfNullGetComponent<Actor, PlayerController>(m_Actor, this, gameObject);
        m_InputHandler = GetComponent<InputHandler>();
        DebugUtility.HandleErrorIfNullGetComponent<InputHandler, PlayerController>(m_InputHandler, this, gameObject);
        m_Health = GetComponent<Health>();
        DebugUtility.HandleErrorIfNullGetComponent<Health, PlayerController>(m_Health, this, gameObject);

        IsJumping = false;

        //m_Controller.enableOverlapRecovery = true;
        m_Health.OnDie += OnDie;

        // force the crouch state to false when starting
        //SetCrouchingState(false, true);
        //UpdateCharacterHeight(true);

    }
    private void Update()
    {
        // check for kill

        HasJumpedThisFrame = false;

        bool wasGrounded = IsGrounded;
        CheckIsGrounded();

        // Landing
        if (IsGrounded && !wasGrounded)
        {
           // Player had landed - check for damage optionally
        }

        HandlePlayerMovement();
    }

    void OnDie()
    {
        IsDead = true;

        // Tell the weapons manager to switch to a non-existing weapon in order to lower the weapon
        m_WeaponsManager.SwitchToWeaponIndex(-1, true);

        EventManager.Broadcast(Events.PlayerDeathEvent);
    }

    void HandlePlayerMovement()
    {
        // Calculate keyboard input
        {
            Vector3 moveDir = m_InputHandler.GetMoveInput();
            Vector3 targetMoveAmount = moveDir * walkSpeed;
            moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
        }

        // Handle mouse look
        LookRotation();

        // Jump & Jetpack
        {
            if (m_InputHandler.GetJumpInputDown() && IsGrounded)
            {
                rigidBody.AddForce(transform.up * jumpForce);
                //IsJumping = true;

                // play sound
                AudioSource.PlayOneShot(JumpSfx);


                // remember last time we jumped because we need to prevent snapping to ground for a short time
                lastTimeJumped = Time.time;
                HasJumpedThisFrame = true;

                // Force grounding to false
                IsGrounded = false;

            }
        }

    }

    public void SetGravity(bool canApplyGravity)
    {
        gravityBody.IsGravityApplied = canApplyGravity;
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }



    void ApplyMovement()
    {
        // Apply movement to rigidbody
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rigidBody.MovePosition(rigidBody.position + localMove);
    }

    void CheckIsGrounded()
    {
        // Grounded check
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance =
            IsGrounded ? (k_skinWidth + GroundCheckDistance) : k_GroundCheckDistanceInAir;

        // reset values before the ground check
        IsGrounded = false;

        if (Time.time >= lastTimeJumped + k_JumpGroundingPreventionTime)
        {
            Ray ray = new Ray(transform.position, -transform.up * multi);
            //Ray ray = new Ray(transform.position, -transform.up * chosenGroundCheckDistance);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, multi, groundedMask))
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }
    }
    private void LookRotation()
    {
        // Look rotation:
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime);

        if (reverseMouse)
        {
            verticalLookRotation -= Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        }
        else
        {
            verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        }

        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void OnDrawGizmos()
    {

        //Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * 1000f);
        //Gizmos.DrawRay(transform.position, -transform.up * multi);

        float chosenGroundCheckDistance = IsGrounded ? (k_skinWidth + GroundCheckDistance + 2) : k_GroundCheckDistanceInAir;

        Color col = IsGrounded ? Color.green : Color.red;
        Gizmos.color = col;
        Gizmos.DrawRay(transform.position, -transform.up * multi);

    }
}


