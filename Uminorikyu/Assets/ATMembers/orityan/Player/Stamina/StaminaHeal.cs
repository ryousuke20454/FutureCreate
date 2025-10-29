using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StaminaHeal : MonoBehaviour
{
    //�X�e�B�b�N�ʒu�̉ߋ����
    Vector2 stickInputLast = new Vector2(0.0f, 0.0f);
    //�X�^�~�i�̉񕜑��x
    [SerializeField] float healSpeed;

    PlayerInputScript input;
    Gamepad gamepad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�Q�[���p�b�h�̎擾
        input = GetComponent<PlayerInputScript>();
        gamepad = input.controller;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gamepad != null)
        {
            //�ŐV�̃��o�[�̈ʒu�̎擾
            Vector2 stickInput = input.GetStickValue(gamepad);

            if (stickInput != stickInputLast)
            {
                GetComponent<Slider>().value += healSpeed;
                stickInputLast = stickInput;

                if (GetComponent<Slider>().value >= 100)
                {
                    GetComponent<StaminaState>().player.GetComponent<OriiPlayerMove>().barnOut = false;
                }
            }
        }
    }
}
