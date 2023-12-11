using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : ButtonReceiver
{
    private bool opened = false;
    private float timer = 0f;

    public float open_time = 2f;
    public float open_distance = 1.5f;

    public GameObject door_right;
    public GameObject door_left;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (opened) timer += Time.deltaTime;
        float distance = Mathf.Clamp01(timer / open_time) * open_distance;
        door_right.transform.localPosition = new Vector3(distance, 0, 0);
        door_left.transform.localPosition = new Vector3(-distance, 0, 0);
    }

    public override void Trigger()
    {
        if (!opened) opened = true;
    }
}
