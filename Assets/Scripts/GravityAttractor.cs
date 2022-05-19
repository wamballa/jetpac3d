using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GravityAttractor : MonoBehaviour
{

    public float gravity = -10f;

    public void Attract(Transform body, bool IsGravityApplied)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 localUp = body.up;

        // Apply downwards gravity to body
        if (IsGravityApplied)
        {
            body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);
        }
        else
        {
            // no gravity
            Vector3 newVel = body.GetComponent<Rigidbody>().velocity;
            newVel.y = 0;

            body.GetComponent<Rigidbody>().velocity = newVel;
        }


        // Allign bodies up axis with the centre of planet
        body.rotation = Quaternion.FromToRotation(localUp, gravityUp) * body.rotation;

        //Debug.DrawLine(body.position, transform.position, Color.red, 5f);
    }

    private void OnDrawGizmos()
    {
        float radius = transform.parent.localScale.x;
        radius *= transform.localScale.x;
        //Gizmos.color = Color.white;
        //Gizmos.DrawWireSphere(transform.position, radius / 2f);
    }
}