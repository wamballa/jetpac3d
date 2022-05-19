using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voice : MonoBehaviour
{
    public AudioSource speaker;
    public AudioClip moan;

    // Start is called before the first frame update
    void Start()
    {
        speaker.volume = 0.4f;
        speaker.clip = moan;
        speaker.loop = true;
        speaker.Play();

        speaker.maxDistance = 5;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
