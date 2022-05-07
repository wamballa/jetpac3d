using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("THis is the fuel spawner")]
    public GroupSpawner fuelSpawner;

    private ShipHandler shipHandler;
    public bool IsShipReady { get; set; }



    void Start()
    {
        shipHandler = GameObject.Find("Jetman").GetComponent<ShipHandler>();
        if (shipHandler == null) print("ERRROR: cant find ship handler");
    }

    // Update is called once per frame
    void Update()
    {
        if (shipHandler.IsShipReady && !IsShipReady && !shipHandler.IsReadyForTakeOff)
        {
            print("Ship ready to load");
            IsShipReady = true;
            fuelSpawner.isEnabled = true;
        }
        if (shipHandler.IsReadyForTakeOff)
        {
            print("Ship ready for take off");
            fuelSpawner.isEnabled = false;
        }

    }
}
