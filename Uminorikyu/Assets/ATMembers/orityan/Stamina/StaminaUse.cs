using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StaminaUse : MonoBehaviour
{
    Gamepad gamepad;
    Slider slider;
    GameObject player;

    [SerializeField] float stickPower;
    [SerializeField] float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�Q�[���p�b�h�̎擾
        gamepad = Gamepad.current;
        if (gamepad != null)
        {
            Debug.Log("�R���g���[��������܂���");
        }

        //�X�^�~�i�ɂ��Ă�X���C�_�[�R���|�[�l���g�̎擾
        slider = GetComponent<Slider>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (gamepad != null)
        {

            // ���X�e�B�b�N�̓��͒l���擾
            Vector2 stickInput = gamepad.leftStick.ReadValue();

            //PS5�́~�{�^���������Ă�Ƃ���
            if (gamepad.buttonSouth.IsPressed())
            {
                //���̃��o�[�������ł��|����Ă���Ƃ�
                if (stickInput.x > stickPower || stickInput.x < -stickPower &&
                    stickInput.y > stickPower || stickInput.y < -stickPower)
                {
                    //�X�^�~�i�������
                    if (slider.value > 0.0f)
                    {
                        slider.value -= speed;
                    }
                }
            }
        }
    }
}
