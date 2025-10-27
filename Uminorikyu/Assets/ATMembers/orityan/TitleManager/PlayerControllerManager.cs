using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerManager : MonoBehaviour
{
    public static PlayerControllerManager controllerManager { get; private set; }

    public Gamepad player1Controller;
    public Gamepad player2Controller;


    void Start()
    {
        if (controllerManager == null)
        {
            controllerManager = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("�ʂ��Ă邺");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update() 
    {
        //�v���C���[�P���Q�[���p�b�h��ڑ����Ă��Ȃ�������
        if (player1Controller == null)
        {
            var gamepads = Gamepad.all;

            for (int i = 0; i < gamepads.Count; i++)
            {
                if (player2Controller != gamepads[i])
                {
                    player1Controller = gamepads[i];
                    Debug.Log($"�v���C���[�P��{player1Controller.name}���ڑ�����܂���");
                    break;
                }
            }
        }

        //�v���C���[�P���Q�[���p�b�h��ڑ����Ă��Ȃ�������
        if (player2Controller == null)
        {
            var gamepads = Gamepad.all;

            for (int i = 0; i < gamepads.Count; i++)
            {
                if (player1Controller != gamepads[i])
                {
                    player2Controller = gamepads[i];
                    Debug.Log($"�v���C���[�Q��{player2Controller.name}���ڑ�����܂���");
                    break;
                }
            }
        }
    }
}