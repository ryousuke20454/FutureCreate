using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class StaminaUse : MonoBehaviour
{
    Gamepad gamepad;
    Slider slider;
    GameObject player;
    XInputController controller;

    [SerializeField] float stickPower;
    [SerializeField] float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�Q�[���p�b�h�̎擾
        gamepad = Gamepad.current;
        controller = Gamepad.current as XInputController;

        if (controller != null)
        {
            Debug.Log("XInput Controller detected!");
        }

        if (gamepad != null)
        {
            Debug.Log("�Q�[���p�b�h���ڑ�����Ă��܂�");
        }

        //�X�^�~�i�ɂ��Ă�X���C�_�[�R���|�[�l���g�̎擾
        slider = GetComponent<Slider>();
        //�v���C���[�̎擾
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //�Q�[���p�b�h�������Ă����
        if (gamepad != null)
        {
            // ���X�e�B�b�N�̓��͒l���擾
            Vector2 stickInput = gamepad.leftStick.ReadValue();

            //�R���g���[���[�́~�{�^���������Ă�Ƃ���
            if (gamepad.buttonSouth.IsPressed())
            {
                //���̃��o�[�������ł��|����Ă���Ƃ�
                if (stickInput.x > stickPower || stickInput.x < -stickPower &&
                    stickInput.y > stickPower || stickInput.y < -stickPower)
                {
                    //�X�^�~�i�������
                    if (slider.value > 0.0f)
                    {
                        slider.value -= 0.1f;
                        player.GetComponent<OriiPlayerMove>().speed = speed;
                    }
                    else
                    {
                        player.GetComponent<OriiPlayerMove>().speed = 0.2f;
                    }
                }
            }
            else
            {
                player.GetComponent<OriiPlayerMove>().speed = 0.2f;
            }
        }

        if (controller != null)
        {
            // ���X�e�B�b�N�̓��͒l���擾
            Vector2 stickInput = controller.dpad.ReadValue();

            //�R���g���[���[�́~�{�^���������Ă�Ƃ���
            if (controller.bButton.IsPressed())
            {
                //���̃��o�[�������ł��|����Ă���Ƃ�
                if (stickInput.x > stickPower || stickInput.x < -stickPower &&
                    stickInput.y > stickPower || stickInput.y < -stickPower)
                {
                    //�X�^�~�i�������
                    if (slider.value > 0.0f)
                    {
                        slider.value -= 0.1f;
                        player.GetComponent<OriiPlayerMove>().speed = speed;
                    }
                    else
                    {
                        player.GetComponent<OriiPlayerMove>().speed = 0.2f;
                    }
                }
            }
            else
            {
                player.GetComponent<OriiPlayerMove>().speed = 0.2f;
            }
        }
    }
}
