using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityBody))]
public class FirstPersonMovement : MonoBehaviour
{
    // public vars
    public float mouseSensitivityX = 250;
    public float mouseSensitivityY = 250;
    public float walkSpeed = 6;
    public float jumpForce = 220;
    public LayerMask groundedMask;

    // System vars
    bool isGrounded;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    float verticalLookRotation;
    public Transform cameraTransform;
    Rigidbody rigidBody;
    float multi = 1.6f;

    void Awake()
    {
        //Screen.lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraTransform = Camera.main.transform;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    private void Update()
    {
        LookRotation();
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
                rigidBody.AddForce(transform.up * jumpForce);
            }
        }

        // Grounded check

        Ray ray = new Ray(transform.position, -transform.up * multi);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, multi, groundedMask))
        {
            isGrounded = true;
            //Debug.DrawRay(transform.position, -transform.up * multi, Color.red, 5f);
            //print("Grounded ");
        }
        else
        {
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // Apply movement to rigidbody
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rigidBody.MovePosition(rigidBody.position + localMove);
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
        Gizmos.DrawRay(transform.position, transform.forward * 2);
        Gizmos.DrawRay(transform.position, -transform.up * multi);
    }
}