using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{



  public GameObject door;
    public GameObject button;
    public GameObject lift;
    public GameObject player;
    public GameObject wall;


private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.CompareTag("button"))
        {
            Destroy(door);
            Destroy(button);

        } else if (collider.gameObject.CompareTag("lift"))
        {
            player.transform.SetParent(lift.transform);
            lift.transform.Translate(0,3,0); 
            transform.parent = null;
        }

    }

    private void OnCollisionStay (Collision collider) {

        if (collider.gameObject.CompareTag("wall button"))
        {
            wall.SetActive(false);
        }
    }

    private void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.CompareTag("wall button"))
        {
            wall.SetActive(true);
        }
    }
}
