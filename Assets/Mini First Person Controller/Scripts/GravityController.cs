using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// References
/// https://www.youtube.com/watch?v=aZOyZJhreSU
/// https://www.youtube.com/watch?v=gHeQ8Hr92P4
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
    public GravityOrbit gravity;
    private float rotationSpeed = 20;
    private Transform myTransform;
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.useGravity = false;
        myTransform = transform;
    }

    void FixedUpdate()
    {
        if (gravity)
        {
            print("rotate");
            // Move to centre of gravity
            Vector3 gravityUp = Vector3.zero;
            gravityUp = (
                myTransform.position - gravity.transform.position
                ).normalized;
            Vector3 bodyUp = myTransform.up;

            rigidbody.AddForce(-gravityUp * gravity.gravity);

            // Rotate
            //Quaternion targetRotation = Quaternion.FromToRotation(
            //    bodyUp,
            //     gravityUp) * myTransform.rotation;
            myTransform.rotation = Quaternion.FromToRotation(
                bodyUp,
                 gravityUp) * myTransform.rotation;
            //rigidbody.rotation = targetRotation;

            //myTransform.rotation = Quaternion.Slerp(
            //    myTransform.rotation,
            //    targetRotation,
            //    rotationSpeed * Time.deltaTime);
            //myTransform.localRotation = Quaternion.Lerp(
            //    myTransform.rotation,
            //    targetRotation,
            //    rotationSpeed * Time.deltaTime);


        }
    }
}
