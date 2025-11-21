using KanKikuchi.AudioManager;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


class TitleSceneManager : MonoBehaviour
{
    [SerializeField] GameObject fadeIn;
    [SerializeField] GameObject fadeOut;
    bool use;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        use = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fadeOut.GetComponent<FadeEventManager>().isFading)
        {
            if (PlayerControllerManager.controllerManager.player[0].gamepad != null &&
                PlayerControllerManager.controllerManager.player[1].gamepad != null)
            {
                if (PlayerControllerManager.controllerManager.player[0].gamepad.buttonSouth.IsPressed() ||
                    PlayerControllerManager.controllerManager.player[1].gamepad.buttonSouth.IsPressed())
                {
                    if (!use)
                    {
                        SEManager.Instance.Play(SEPath.CLICK);
                        use = true;
                        PlayerControllerManager.controllerManager.round.roundNum = Round.Round1;
                        Instantiate(fadeIn);
                    }
                }
            }
        }
    }
}
