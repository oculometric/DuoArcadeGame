using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControl : MonoBehaviour
{
    public GameObject button;
    public bool stay_depressed;
    private float button_timer = 0.0f;
    private bool pressed = false;
    public GameObject target;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            pressed = true;
            try { target.GetComponent<ButtonReceiver>().Trigger(); } catch { }
        }
    }

    private void Update()
    {
        if (pressed)
        {
            button_timer = Mathf.Clamp01(button_timer + Time.deltaTime);
        } else
        {
            button_timer = Mathf.Clamp01(button_timer - Time.deltaTime);
        }

        button.transform.localPosition = new Vector3(0, 0, 0.05f) * button_timer;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && !stay_depressed)
        {
            pressed = false;
            try { target.GetComponent<ButtonReceiver>().UnTrigger(); } catch { }
        }
    }
}
