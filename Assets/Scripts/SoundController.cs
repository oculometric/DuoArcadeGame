using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    private float wait_timer = 5.0f;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying)
        {
            if (wait_timer > 0)
            {
                wait_timer -= Time.deltaTime;
            }
            else
            {
                source.Play();
                wait_timer = 30f;
            }
        }
    }
}
