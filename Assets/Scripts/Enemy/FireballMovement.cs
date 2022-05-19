using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.DebugTool;

public class FireballMovement : MonoBehaviour
{
    [Header("Speed")]
    public float speed = 15f;

    Vector3 targetAxis;
    public float rotationSpeed = 25f;

    private GameObject planet;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        planet = GameObject.Find("Planet");
        DebugUtility.HandleErrorIfNullFindObject<GameObject, FireballMovement>(planet, this);
        rb = transform.GetComponent<Rigidbody>();
        DebugUtility.HandleErrorIfNullGetComponent<Rigidbody, FireballMovement>(rb, this, gameObject);

        float randFloat = Random.Range(1, 3);
        rotationSpeed += randFloat;

        targetAxis = Vector3.zero;

        // Look at
        transform.LookAt(planet.transform);


    }

    // Update is called once per frame
    void Update()
    {
        // Set velocity
        rb.velocity = transform.forward * speed;

        // Rotate object
        //transform.Rotate(Vector3.forward, 100 * Time.deltaTime);

        // Spin the object around the target at 20 degrees/second.
        //transform.RotateAround(transform.position, transform.forward, rotationSpeed * Time.deltaTime);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward*12);
    }
}
