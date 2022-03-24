using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityBody))]
public class FirstPersonMovement : MonoBehaviour
{
    // public vars
    public float mouseSensitivityX = 250;
    public float mouseSensitivityY = 250;
    public float walkSpeed = 6;
    float jumpForce = 600;
    Vector3 jumpDirection;
    public LayerMask groundedMask;

    // System vars
    bool isGrounded;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    float verticalLookRotation;
    public Transform cameraTransform;
    Rigidbody rigidBody;
    float multi = 1.6f;

    bool canSeePlanet = false;
    GameObject currentPlanet;

    void Awake()
    {
        //Screen.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //cameraTransform = Camera.main.transform;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;

        jumpDirection = transform.up;
    }

    private void Update()
    {
        LookRotation();
        CheckIsGrounded();
        CheckCanSeePlanet();

        // Calculate movement:
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
        Vector3 targetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                if (canSeePlanet)
                {
                    print("Jump towards planet");
                    rigidBody.AddForce(cameraTransform.forward * jumpForce);

                }
                else
                {
                    print("Jump");
                    rigidBody.AddForce(jumpDirection * jumpForce);
                }

                //rigidBody.AddForce((Vector3.left * verticalLookRotation) * jumpForce);
            }
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void CheckCanSeePlanet()
    {
        if (cameraTransform)
        {
            // Grounded check
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward * 1000f);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, groundedMask))
            {
                if (hit.transform.gameObject != currentPlanet)
                {
                    //print("Player see planet " + hit.transform.name);
                    canSeePlanet = true;
                    //jumpDirection = cameraTransform.forward;
                }
                else
                {
                    //canSeePlanet = false;
                    //jumpDirection = transform.up;
                }

            }
            else
            {
                canSeePlanet = false;
                jumpDirection = transform.up;
            }
        }

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

        Ray ray = new Ray(transform.position, -transform.up * multi);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, multi, groundedMask))
        {
            isGrounded = true;
            currentPlanet = hit.transform.gameObject;
            //Debug.DrawRay(transform.position, -transform.up * multi, Color.red, 5f);
            //print("Grounded ");
        }
        else
        {
            isGrounded = false;
            currentPlanet = null;
        }
    }
    private void LookRotation()
    {
        // Look rotation:
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime);
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraTransform.position, cameraTransform.forward * 1000f);
        Gizmos.DrawRay(transform.position, -transform.up * multi);
        if (canSeePlanet) Gizmos.color = Color.green; else Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}