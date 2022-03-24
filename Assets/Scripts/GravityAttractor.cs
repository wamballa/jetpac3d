using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{

    public float gravity = -10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GravityBody>())
        {
            print("entered gravity......");
            other.GetComponent<GravityBody>().planet = this.GetComponent<GravityAttractor>();
            //other.GetComponent<GravityBody>().SetGravity(true);
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.GetComponent<GravityBody>())
    //    {
    //        print("exited gravity......");
    //        other.GetComponent<GravityBody>().planet = new GravityAttractor();

    //    }
    //}


    public void Attract(Transform body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 localUp = body.up;

        // Apply downwards gravity to body
        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);
        // Allign bodies up axis with the centre of planet
        body.rotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;

        Debug.DrawLine(body.position, transform.position, Color.red, 5f);
    }

    private void OnDrawGizmos()
    {
        float radius = transform.parent.localScale.x;
        radius *= transform.localScale.x;
        Gizmos.DrawWireSphere(transform.position, radius / 2f);
    }
}
