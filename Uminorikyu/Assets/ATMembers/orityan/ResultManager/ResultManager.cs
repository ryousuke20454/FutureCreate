using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] SceneAsset scene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(PlayerControllerManager.controllerManager.round.roundNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerControllerManager.controllerManager.player[0].gamepad.buttonSouth.IsPressed())
        {
            SceneManager.LoadScene(scene.name);
        }
    }
}
