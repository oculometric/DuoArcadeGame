using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftController : ButtonReceiver
{
    private bool ascending = false;
    public float speed = 0.2f;
    private Vector3 origin;
    public Vector3 destination;
    private float mix;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (ascending)
        {
            mix = Mathf.Clamp01(mix + (Time.deltaTime * speed));
        }
        else
        {
            mix = Mathf.Clamp01(mix - (Time.deltaTime * speed));
        }

        transform.position = Vector3.Lerp(origin, destination, mix);
    }

    public override void Trigger()
    {
        ascending = true;
    }

    public override void UnTrigger()
    {
        ascending = false;
    }
}
