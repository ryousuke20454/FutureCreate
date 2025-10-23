using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;

public class PlayerMove : MonoBehaviour
{
    Gamepad gamepad;
    [SerializeField] float speed;
    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gamepad = Gamepad.current;
        if (gamepad == null)
        {
            Debug.Log("�R���g���[�����ڑ�����Ă��܂���");
        }

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ���X�e�B�b�N�̓��͒l���擾
        Vector2 stickInput = gamepad.leftStick.ReadValue();
        //�ړ�
        gameObject.transform.position = 
            new Vector2(gameObject.transform.position .x + stickInput.x * speed,
            gameObject.transform.position.y + stickInput.y * speed);
    }
}
