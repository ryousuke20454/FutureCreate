using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class StaminaUse : MonoBehaviour
{
    Slider slider;

    [SerializeField] GameObject player;
    [SerializeField] float stickPower;
    [SerializeField] float dashSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�X�^�~�i�ɂ��Ă�X���C�_�[�R���|�[�l���g�̎擾
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        //�_�b�V����True��������
        if (player.GetComponent<OriiPlayerMove>().dash)
        {
            //�X�^�~�i�������
            if (slider.value > 0.0f)
            {
                slider.value -= 0.1f;
                player.GetComponent<OriiPlayerMove>().moveSpeed = dashSpeed;
            }
            else
            {
                player.GetComponent<OriiPlayerMove>().moveSpeed = 0.2f;
            }
        }
        else
        {
            player.GetComponent<OriiPlayerMove>().moveSpeed = 0.2f;
        }

    }
}
