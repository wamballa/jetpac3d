using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    Rigidbody rb;
    float jumpForce = 600f;
    bool isGrounded = false;
    float groundCheckRayMultiplier = 1.3f;
    public LayerMask groundMask;

    float walkSpeed = 10f;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    public float mouseSensitivityX = 250;
    public float mouseSensitivityY = 250;
    float verticalLookRotation;
    public Transform cameraTransform;

    private void Start()
    {
        //cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        CheckIsGrounded();
        //LookRotation();

        // Calculate movement:
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
        Vector3 targetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            print("Jump");
            rb.AddForce(transform.up * jumpForce);
        }

    }
    void FixedUpdate()
    {
        // Apply movement to rigidbody
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);
    }
    private void LookRotation()
    {
        // Look rotation:
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime);
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    void CheckIsGrounded()
    {
        // Grounded check
        Ray ray = new Ray(transform.position, -transform.up * groundCheckRayMultiplier);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, groundCheckRayMultiplier, groundMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.up * jumpForce);
    }

}
