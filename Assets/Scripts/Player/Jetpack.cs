using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.DebugTool;

public class Jetpack : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Audio source for jetpack sfx")]
    public AudioSource AudioSource;

    [Tooltip("Particles for jetpack vfx")] public ParticleSystem[] JetpackVfx;

    [Header("Parameters")]
    [Tooltip("Whether the jetpack is unlocked at the begining or not")]
    public bool IsJetpackUnlockedAtStart = true;

    [Tooltip("The strength with which the jetpack pushes the player up")]
    public float JetpackAcceleration = 7f;

    [Range(0f, 1f)]
    [Tooltip(
        "This will affect how much using the jetpack will cancel the gravity value, to start going up faster. 0 is not at all, 1 is instant")]
    public float JetpackDownwardVelocityCancelingFactor = 1f;

    [Header("Durations")]
    [Tooltip("Time it takes to consume all the jetpack fuel")]
    public float ConsumeDuration = 1.5f;

    [Tooltip("Time it takes to completely refill the jetpack while on the ground")]
    public float RefillDurationGrounded = 2f;

    [Tooltip("Time it takes to completely refill the jetpack while in the air")]
    public float RefillDurationInTheAir = 5f;

    [Tooltip("Delay after last use before starting to refill")]
    public float RefillDelay = 1f;

    [Header("Audio")]
    [Tooltip("Sound played when using the jetpack")]
    public AudioClip JetpackSfx;

    bool m_CanUseJetpack;
    PlayerController playerController;
    InputHandler inputHandler;
    Rigidbody rb;

    bool jetpackIsInUse;

    //PlayerInputHandler m_InputHandler;
    float m_LastTimeOfUse;

    // stored ratio for jetpack resource (1 is full, 0 is empty)
    public float CurrentFillRatio { get; private set; }
    public bool IsJetpackUnlocked { get;  set; }

    //public bool IsPlayergrounded() => m_PlayerCharacterController.IsGrounded;
    public bool IsPlayergrounded() => playerController.IsGrounded;


    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        DebugUtility.HandleErrorIfNullGetComponent<PlayerController, Jetpack>(playerController,
                this, gameObject);
        inputHandler = GetComponent<InputHandler>();
        DebugUtility.HandleErrorIfNullGetComponent<InputHandler, Jetpack>(inputHandler,
                this, gameObject);

        CurrentFillRatio = 1f;

        AudioSource.clip = JetpackSfx;
        AudioSource.loop = true;

        IsJetpackUnlocked = true;

        rb = transform.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        // jetpack can only be used if not grounded and jump has been pressed again once in-air
        if (IsPlayergrounded())
        {
            m_CanUseJetpack = false;
        }
        else if (!playerController.HasJumpedThisFrame && inputHandler.GetJumpInputDown())
        {
            m_CanUseJetpack = true;
        }

        bool oldJetPackInUse = jetpackIsInUse;
        // jetpack usage
        jetpackIsInUse = m_CanUseJetpack && IsJetpackUnlocked && CurrentFillRatio > 0f &&
                              inputHandler.GetJumpInputHeld();

        // stop velocity going into using the jetpack
        if (jetpackIsInUse && !oldJetPackInUse)
        {
            rb.velocity = Vector3.zero;
        }

        if (jetpackIsInUse)
        {
            // store the last time of use for refill delay
            m_LastTimeOfUse = Time.time;

            float totalAcceleration = JetpackAcceleration;

            // Turn of play gravity while jetpack used
            playerController.SetGravity(false);

            // Add force
            Vector3 thrustDirection = Vector3.up;
            rb.AddForce(thrustDirection, ForceMode.Force);

            Debug.DrawRay(transform.position, thrustDirection * 10, Color.red, 1f);
            print("vel " + rb.velocity);

            // consume fuel
            CurrentFillRatio = CurrentFillRatio - (Time.deltaTime / ConsumeDuration);

            for (int i = 0; i < JetpackVfx.Length; i++)
            {
                var emissionModulesVfx = JetpackVfx[i].emission;
                emissionModulesVfx.enabled = true;
            }
            if (!AudioSource.isPlaying) AudioSource.Play();
        }
        else
        {
            // refill the meter over time
            if (IsJetpackUnlocked && Time.time - m_LastTimeOfUse >= RefillDelay)
            {
                float refillRate = 1 / (playerController.IsGrounded
                    ? RefillDurationGrounded
                    : RefillDurationInTheAir);
                CurrentFillRatio = CurrentFillRatio + Time.deltaTime * refillRate;
            }

            for (int i = 0; i < JetpackVfx.Length; i++)
            {
                var emissionModulesVfx = JetpackVfx[i].emission;
                emissionModulesVfx.enabled = false;
            }

            // keeps the ratio between 0 and 1
            CurrentFillRatio = Mathf.Clamp01(CurrentFillRatio);

            if (AudioSource.isPlaying)
                AudioSource.Stop();


            playerController.SetGravity(true);


        }
    }
}
