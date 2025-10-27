using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;


public class OriiPlayerMove : MonoBehaviour
{
    public Gamepad gamepad;
    GameObject player;
    XInputController controller;
    [SerializeField] public float moveSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //�ʂ̃v���C���[������
        player = GameObject.FindWithTag("Player");

        //�ʂ̃v���C���[�ƃR���g���[���[�����Ȃ��悤�ɂ���
        if (player.GetComponent<OriiPlayerMove>().gamepad !=
            PlayerControllerManager.controllerManager.player1Controller)
        {
            gamepad = PlayerControllerManager.controllerManager.player1Controller;
        }
        else
        {
            gamepad = PlayerControllerManager.controllerManager.player2Controller;
        }

        //�f�o�b�O
        if (gamepad == null)
        {
            Debug.Log("�R���g���[���[���ڑ�����Ă��܂���");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gamepad != null)
        {
            Vector2 stickInput;

            if (gamepad.name == "DualSenseGamepadHID")
            {
                // ���X�e�B�b�N�̓��͒l���擾
                stickInput = gamepad.leftStick.ReadValue();
            }
            else
            //if (gamepad.name == "XInputControllerWindows")
            {
                // ���X�e�B�b�N�̓��͒l���擾
                stickInput = gamepad.dpad.ReadValue();

                /*
                 * ���͏K���ꗗ
                if (controller.aButton.isPressed)
                {
                    Debug.Log("A Button Pressed");
                }
                if (controller.bButton.isPressed)
                {
                    Debug.Log("B Button Pressed");
                }
                if (controller.xButton.isPressed)
                {
                    Debug.Log("X Button Pressed");
                }
                if (controller.yButton.isPressed)
                {
                    Debug.Log("Y Button Pressed");
                }

                controller.dpad.ReadValue()

                */
            }
            //�ړ�
            gameObject.transform.position =
                new Vector2(gameObject.transform.position.x + stickInput.x * moveSpeed,
                gameObject.transform.position.y + stickInput.y * moveSpeed);
        }
    }
}
