using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public InputActionMap map;
    private InputAction primary;
    private InputAction secondary;
    // Start is called before the first frame update
    void Start()
    {
        map.Enable();
        primary = map.FindAction("A");
        secondary = map.FindAction("B");
    }

    // Update is called once per frame
    void Update()
    {
        if (primary.ReadValue<float>() > 0)
        {
            SceneManager.LoadSceneAsync("MainScene");
            SceneManager.UnloadSceneAsync("MenuScene");
        }
        if (secondary.ReadValue<float>() > 0)
        {
            Application.Quit();
        }
    }
}
