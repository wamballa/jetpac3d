using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{

    GravityAttractor gravityAttractor;
    private float rotationSpeed = 20;

    private Transform myTransform;

    private Rigidbody rigidbody;

    private bool hasGravity;

    public bool IsGravityApplied { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.useGravity = false;
        IsGravityApplied = true;

        gravityAttractor = GameObject.Find("GravityField").GetComponent<GravityAttractor>();
        if (gravityAttractor == null) print("ERROR: can't find planet");

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        gravityAttractor.Attract(transform, IsGravityApplied);

    }

}



