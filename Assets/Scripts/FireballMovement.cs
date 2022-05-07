using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballMovement : MonoBehaviour
{

    Vector3 targetAxis;
    public float rotationSpeed = 25f;

    public int direction =1;

    // Start is called before the first frame update
    void Start()
    {
        float randFloat = Random.Range(1, 3);
        rotationSpeed += randFloat;

        targetAxis = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate object
        //transform.Rotate(Vector3.forward, 10 * Time.deltaTime);


        // Spin the object around the target at 20 degrees/second.
        //transform.RotateAround(targetAxis.position, transform.up, rotationSpeed * Time.deltaTime);
        transform.RotateAround(targetAxis, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward*12);
    }
}
