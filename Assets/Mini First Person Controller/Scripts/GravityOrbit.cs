using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrbit : MonoBehaviour
{

    public float gravity = 9.8f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GravityController>())
        {
            print("entered gravity");
            other.GetComponent<GravityController>().gravity = this.GetComponent<GravityOrbit>();
        }
    }

}
