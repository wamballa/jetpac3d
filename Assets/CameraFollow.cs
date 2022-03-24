using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    Vector3 deltaPos;
    public Transform playerTransform;

    GravityBody gravityBody;
    GameObject planet;

    // Start is called before the first frame update
    void Start()
    {
        deltaPos = transform.position - playerTransform.position;
        gravityBody = playerTransform.gameObject.GetComponent<GravityBody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 newDelta = transform.position - playerTransform.position;
        //print(newDelta);

        //transform.position = playerTransform.position + deltaPos;
        transform.LookAt(playerTransform.position);
        Vector3 rotation = transform.eulerAngles;
        print(rotation);
        //rotation.y = 0;

        //transform.eulerAngles = rotation;

        //transform.LookAt(playerTransform.position, Vector3.up);
        //transform.rotation = Quaternion.AngleAxis(0, Vector3.up);

        //planet = gravityBody.GetPlanet();
        //if (planet != null)
        //{
        //    transform.RotateAround(planet.transform.position, Vector3.up, playerTransform.rotation.x);

        //}


        //print("Planet is " + planet.name);

    }
}
