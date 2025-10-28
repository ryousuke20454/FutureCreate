using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public struct PlayerInfo
{
    public Gamepad gamepad;
    public int score;
}

public struct RoundInfo
{
    public int weatherNum;
    public int roundNum;
}

public class PlayerControllerManager : MonoBehaviour
{
    //�V���O���g���p�̕ϐ�
    public static PlayerControllerManager controllerManager { get; private set; }
    //�v���C���[��l���̂̏��i�[�p
    public PlayerInfo[] player = new PlayerInfo[2];
    public RoundInfo round;

    void Start()
    {
        //���̃X�N���v�g�̃V���O���g�����y�уX�^�e�B�b�N��
        if (controllerManager == null)
        {
            controllerManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        player[0].score = 100;
        player[1].score = 0;
        round.roundNum = 1;
    }

    void Update() 
    {
        //�v���C���[�P���Q�[���p�b�h��ڑ����Ă��Ȃ�������
        if (player[0].gamepad == null)
        {
            var gamepads = Gamepad.all;

            for (int i = 0; i < gamepads.Count; i++)
            {
                if (player[1].gamepad != gamepads[i])
                {
                    player[0].gamepad = gamepads[i];
                    Debug.Log($"�v���C���[�P��{player[0].gamepad.name}���ڑ�����܂���");
                    break;
                }
            }
        }

        //�v���C���[�P���Q�[���p�b�h��ڑ����Ă��Ȃ�������
        if (player[1].gamepad == null)
        {
            var gamepads = Gamepad.all;

            for (int i = 0; i < gamepads.Count; i++)
            {
                if (player[0].gamepad != gamepads[i])
                {
                    player[1].gamepad = gamepads[i];
                    Debug.Log($"�v���C���[�Q��{player[1].gamepad.name}���ڑ�����܂���");
                    break;
                }
            }
        }
    }
}