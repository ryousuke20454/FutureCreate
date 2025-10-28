using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{
    //�C���X�y�N�^�[�Ńv���C���[�ԍ�����͂���
    [SerializeField] public int PlayerNum;
    //�R���g���[�^�[���
    public Gamepad controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //�R���g���[���[�����擾����
        if (PlayerControllerManager.controllerManager != null)
            controller = PlayerControllerManager.controllerManager.player[PlayerNum].gamepad;

        //�f�o�b�O
        if (controller == null)
        {
            Debug.Log("�R���g���[���[���ڑ�����Ă��܂���");
        }
    }

    public Vector2 GetStickValue(Gamepad gamepad)
    {
        Vector2 value = new Vector2(0.0f, 0.0f);

        if (gamepad != null)
        {
            //�f���A���Z���X��������
            if (gamepad.name == "DualSenseGamepadHID")
            {
                // ���X�e�B�b�N�̓��͒l���擾
                value = gamepad.leftStick.ReadValue();
            }
            //�A�P�R����������
            else if (gamepad.name == "XInputControllerWindows")
            {
                // ���X�e�B�b�N�̓��͒l���擾
                value = gamepad.dpad.ReadValue();
            }
        }

        return value;
    }

    public bool GetTriangleButton(Gamepad gamepad)
    {
        bool button = false;
        if (gamepad != null)
        {
            button = gamepad.buttonNorth.IsPressed();
        }
        return button;
    }

    public bool GetSquareButton(Gamepad gamepad)
    {
        bool button = false;
        if (gamepad != null)
        {
            button = gamepad.buttonWest.IsPressed();
        }
        return button;
    }

    public bool GetCircleButton(Gamepad gamepad)
    {
        bool button = false;
        if (gamepad != null)
        {
            button = gamepad.buttonEast.IsPressed();
        }
        return button;
    }

    public bool GetCrossButton(Gamepad gamepad)
    {
        bool button = false;
        if (gamepad != null)
        {
            button = gamepad.buttonSouth.IsPressed();
        }
        return button;
    }
}
