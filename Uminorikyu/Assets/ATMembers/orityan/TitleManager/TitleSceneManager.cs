using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


class TitleSceneManager : MonoBehaviour
{
    [SerializeField] string scene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerControllerManager.controllerManager.player[0].gamepad.buttonSouth.IsPressed())
        {
            SceneManager.LoadScene(scene);
        }
    }
}
