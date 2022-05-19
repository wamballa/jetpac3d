using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.DebugTool;

public class BeaconFlash : MonoBehaviour
{
    [Header("Beacon sound")]
    public AudioClip beaconSFX;

    [Header("Beacon audiosource")]
    public AudioSource beaconAudioSource;

    public bool IsFlashing { get; set; }

    float flashTimer;
    float flashDelay = 1f;

    bool isOn;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // flashing timings
        if (Time.time > flashTimer)
        {
            isOn = !isOn;
            flashTimer = Time.time + flashDelay;
            if (beaconSFX != null && isOn)
            {
                beaconAudioSource.Play();
                beaconAudioSource.PlayOneShot(beaconSFX, 0.6f);
            }

        }
        // turn object on / off
        //gameObject.SetActive(isOn);

        gameObject.GetComponent<MeshRenderer>().enabled = isOn;
    }

    private void OnDrawGizmos()
    {
        float d = beaconAudioSource.maxDistance;
        Gizmos.DrawWireSphere(transform.position, d);
    }
}
