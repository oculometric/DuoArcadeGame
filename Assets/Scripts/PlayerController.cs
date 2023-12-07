using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float stretch_max = 30f;
    public float movement_speed = 2f;
    public float max_adhesion_distance = 0.5f;
    public float max_turn_angle = 55.0f;

    public GameObject camera_parent;
    public GameObject camera_actual;
    public GameObject character_mesh;
    public InputActionMap input_map;

    public GameObject other_character;

    // if active, this player receives input actions and moves the other one
    public bool is_active_charater;
    public float switch_delay_timer;

    private Vector2 camera_stretch;
    private InputAction move_action;
    private InputAction look_action;
    private InputAction switch_action;
    private PlayerController other_player_controller;

    // Start is called before the first frame update
    void Start()
    {
        if (!is_active_charater)
        {
            camera_actual.SetActive(false);
        }

        other_player_controller = other_character.GetComponent<PlayerController>();

        input_map.Enable();
        move_action = input_map.FindAction("MoveAction");
        look_action = input_map.FindAction("LookAction");
        switch_action = input_map.FindAction("SwitchAction");

        move_action.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        look_action.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    // Update is called once per frame
    void Update()
    {
        // align the camera parent
        camera_parent.transform.rotation = Quaternion.LookRotation(transform.up, Vector3.up);
        // if active, handle input
        if (is_active_charater) HandleCharacterInput();
    }

    private void HandleCharacterInput()
    {
        // get input values
        Vector2 move_vector = move_action.ReadValue<Vector2>();
        Vector2 look_vector = look_action.ReadValue<Vector2>();
        bool switch_pressed = switch_action.ReadValue<float>() > 0;

        if (switch_delay_timer > 0f) switch_delay_timer -= Time.deltaTime;

        // swap players if button pressed
        if (switch_pressed && switch_delay_timer <= 0f)
        {
            HandOffActive();
            return;
        }

        // update camera stretching
        camera_stretch += ((look_vector - camera_stretch) * Time.deltaTime * 4); // TODO better smooothing
        // update camera angle according to stretch
        UpdateCameraAngle();

        // calculate movement vector in world space
        Vector3 move_vector_ws = (camera_actual.transform.right * move_vector.x) + (camera_actual.transform.up * move_vector.y); // TODO should camera_actual be camera_parent instead?
        move_vector_ws.Normalize();
        MoveInDirection(move_vector_ws);
    }

    private float ConvertStretch(float f)
    {
        return Mathf.Pow(Mathf.Abs(f), 2.2f) * stretch_max * Mathf.Sign(f);
    }

    private void UpdateCameraAngle()
    {
        Vector3 new_euler = Vector3.zero;
        new_euler.x = -ConvertStretch(camera_stretch.y);
        new_euler.y = ConvertStretch(camera_stretch.x);
        camera_actual.transform.localEulerAngles = new_euler;
    }

    private void MoveInDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude == 0) return;
        // find out where the spider is expecting to end up
        Vector3 new_position = other_character.transform.position + (direction * Time.deltaTime * movement_speed);

        // ray pointing to that point
        Vector3 direction_to_new = (new_position - camera_actual.transform.position).normalized;
        RaycastHit rch;
        Physics.Raycast(camera_actual.transform.position, direction_to_new, out rch, 100000, ~(1 << 3));
        if (!rch.transform) return;

        if ((180.0f*Mathf.Acos(Vector3.Dot(rch.normal, other_character.transform.up)))/Mathf.PI > max_turn_angle) return;

        // direction in which the spider should actually move
        Vector3 new_direction = (rch.point - other_character.transform.position);
        if (new_direction.magnitude > max_adhesion_distance) return; // prevents spiders from crossing big gaps, probably reduce it from 1 metre
        new_direction.Normalize();

        float mult = Time.deltaTime * movement_speed;

        // check that the new spider origin would be visible to the camera
        new_position = other_character.transform.position + (new_direction * mult);
        Vector3 vp = camera_actual.GetComponent<Camera>().WorldToViewportPoint(new_position);
        if (vp.x < 0 || vp.x > 1 || vp.y < 0 || vp.y > 1 || vp.z < 0) return;

        // align spider rotation to be looking in the direction it is walking, pointing directly away from the surface below it
        other_character.transform.rotation = Quaternion.LookRotation(new_direction, rch.normal);
        // move spider to new position
        other_character.transform.position = new_position;
    }

    private void HandOffActive()
    {
        Debug.Log("switch");
        is_active_charater = false;
        other_player_controller.is_active_charater = true;
        other_player_controller.switch_delay_timer = 0.5f;
        camera_actual.SetActive(false);
        other_player_controller.camera_actual.SetActive(true);
    }
}

//
