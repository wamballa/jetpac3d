using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{

    public GravityAttractor planet;
    private float rotationSpeed = 20;

    private Transform myTransform;

    private Rigidbody rigidbody;

    private bool hasGravity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.useGravity = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (planet)
        {
            planet.Attract(transform);
        }
        else
        {
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void SetGravity(bool b)
    {
        hasGravity = b;
    }
}
