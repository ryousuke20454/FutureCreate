using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] GameObject fade;
    bool use;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        use = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerControllerManager.controllerManager.player[0].gamepad != null)
        {
            if (PlayerControllerManager.controllerManager.player[0].gamepad.buttonSouth.IsPressed())
            {
                if (!use)
                {
                    use = true;
                    PlayerControllerManager.controllerManager.round.roundNum = Round.Title;
                    Instantiate(fade);
                }
            }
        }
    }
}
