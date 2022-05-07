using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipHandler : MonoBehaviour
{
    [Header("Ship UI object")]
    public GameObject shipUI;

    [Header("Ship inventory elements")]
    public GameObject middleShipUI;
    public GameObject topShipUI;

    [Header("Ship parts")]
    public GameObject middleShipInPlaceUI;
    public GameObject topShipInPlaceUI;

    [Header("Ship refuel elements")]
    public GameObject refuelUI;
    public GameObject fuelUI;
    public Image bottomUIImage;
    public Image middleUIImage;
    public Image topUIImage;

    [Header("UI Header text change for final takeoff")]
    public GameObject buildShipText;
    public GameObject enterShipText;


    public bool IsMiddleCarried { get; private set; }
    public bool IsTopCarried { get; private set; }
    public bool IsMiddleLoaded { get; private set; }
    public bool IsTopLoaded { get; private set; }
    public bool IsShipReady { get; private set; }
    public bool IsFuelCarried { get; private set; }
    public bool IsFuelLoaded { get; private set; }
    public bool IsReadyForTakeOff { get; private set; }
    public bool IsPlayerInShip { get; private set; }

    // internal
    private GameObject shipPart;
    private int maxFuel;
    private int currentFuel;

    private void Start()
    {
        IsShipReady = false;
    }
    void Update()
    {
        HandleUI();

        if (IsMiddleCarried)
        {
            if (shipPart != null)
            {
                //Vector3 shipPos = shipPart.GetComponent<Transform>().position;
                shipPart.GetComponent<Transform>().position = transform.position;
                shipPart.GetComponent<BoxCollider>().enabled = false;
            }
        }
        if (IsTopCarried && shipPart != null)
        {
            //Vector3 shipPos = shipPart.GetComponent<Transform>().position;
            shipPart.GetComponent<Transform>().position = transform.position;
            shipPart.GetComponent<BoxCollider>().enabled = false;
        }
        if (IsFuelCarried && shipPart != null)
        {
            //Vector3 shipPos = shipPart.GetComponent<Transform>().position;
            shipPart.GetComponent<Transform>().position = transform.position;
            shipPart.GetComponent<BoxCollider>().enabled = false;
        }

        // check is ship ready
        if (IsTopLoaded && IsMiddleLoaded) IsShipReady = true;

        // check if ship ready for takeoff
        if (currentFuel == 3) IsReadyForTakeOff = true;


    }
    private void HandleUI()
    {
        if (!IsShipReady)
        {
            shipUI.SetActive(true);
            refuelUI.SetActive(false);

            if (IsMiddleCarried)
            {
                middleShipUI.SetActive(true);

            }
            else if (IsTopCarried)
            {
                topShipUI.SetActive(true);
            }
            else
            {
                middleShipUI.SetActive(false);
                topShipUI.SetActive(false);
            }
            if (IsMiddleLoaded)
            {
                middleShipInPlaceUI.SetActive(true);
            }
            if (IsTopLoaded)
            {
                topShipInPlaceUI.SetActive(true);
            }
        }

        if (IsShipReady)
        {
            shipUI.SetActive(false);
            refuelUI.SetActive(true);
            if (IsFuelCarried)
            {
                fuelUI.SetActive(true);
            }
            else fuelUI.SetActive(false);

            if (currentFuel == 1)
            {
                bottomUIImage.color = Color.magenta;
            }
            if (currentFuel == 2)
            {
                middleUIImage.color = Color.magenta;
            }
            if (currentFuel == 3)
            {
                topUIImage.color = Color.magenta;

            }
        }

        if (IsReadyForTakeOff)
        {
            fuelUI.SetActive(false);
            buildShipText.SetActive(false);
            enterShipText.SetActive(true);

        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        // LOAD SHIP WITH STUFF

        // Load with middle
        if (collision.transform.name == "Bottom" && IsMiddleCarried)
        {
            // calculate where to position new part
            GameObject shipBottom = collision.gameObject;
            Vector3 shipBottomPos = shipBottom.transform.position;
            float shipBottomScaleY = shipBottom.transform.localScale.y;
            float middlePosY = shipBottomPos.y + shipBottomScaleY;
            Vector3 newPos = new Vector3(shipBottomPos.x, middlePosY, shipBottomPos.z);

            shipPart.transform.position = newPos;
            IsMiddleCarried = false;
            IsMiddleLoaded = true;

        }
        // Load with top
        if (collision.transform.name == "Bottom" && IsTopCarried)
        {
            // calculate where to position new part
            GameObject shipBottom = collision.gameObject;
            Vector3 shipBottomPos = shipBottom.transform.position;
            float shipBottomScaleY = shipBottom.transform.localScale.y * 2;
            float newPosY = shipBottomPos.y + shipBottomScaleY;
            Vector3 newPos = new Vector3(shipBottomPos.x, newPosY, shipBottomPos.z);

            shipPart.transform.position = newPos;
            IsMiddleCarried = false;
            IsMiddleLoaded = true;
            IsTopCarried = false;
            IsTopLoaded = true;
        }
        // Load with fuel
        if (collision.transform.name == "Bottom" && IsFuelCarried)
        {
            Destroy(shipPart);
            IsFuelCarried = false;
            currentFuel++;
        }

        //
        // Pickup stuff
        //
        if (collision.transform.name == "Middle")
        {
            if (IsMiddleLoaded == false)
            {
                IsMiddleCarried = true;
                IsTopCarried = false;
                shipPart = collision.gameObject;
                print("pickup middle");
            }
        }
        if (collision.transform.name == "Top")
        {
            if (IsMiddleLoaded == true && IsTopLoaded == false)
            {
                IsMiddleCarried = false;
                IsTopCarried = true;
                shipPart = collision.gameObject;
                print("pickup top");
            }
        }
        if (collision.transform.CompareTag("FuelTank"))
        {
            //print("collide fuel tank "+ IsShipReady + " "+!IsFuelCarried);
            if (IsShipReady && !IsFuelCarried)
            {
                print("picked up fuel");
                shipPart = collision.gameObject;
                IsFuelCarried = true;
            }
        }
        // Get in ship
        if (collision.transform.name == "Bottom")
        {
            if (IsReadyForTakeOff)
            {
                IsPlayerInShip = true;
            }
        }
    }

}
