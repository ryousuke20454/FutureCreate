using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;


public class OriiPlayerMove : MonoBehaviour
{
    [SerializeField] public float moveSpeed;
    public bool dash;
    public bool barnOut;

    PlayerInputScript input;
    Gamepad gamepad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<PlayerInputScript>();
        gamepad = input.controller;

        dash = false;
        barnOut = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(input.controller != null && !barnOut)
        {
            Vector2 stickInput = input.GetStickValue(gamepad);

            if (input.GetSquareButton(gamepad))
            {
                Debug.Log("����������܂���");
            }
            if (input.GetTriangleButton(gamepad))
            {
                Debug.Log("����������܂���");
            }
            if (input.GetCircleButton(gamepad))
            {
                Debug.Log("����������܂���");
            }
            if (input.GetCrossButton(gamepad))
            {
                Debug.Log("�~��������܂���");
                if (stickInput.x != 0.0f && stickInput.y != 0.0f)
                {
                    dash = true;
                }
            }
            else
            {
                dash = false;
            }

            //�ړ�
            transform.position =
                new Vector3(
                    transform.position.x + stickInput.x * moveSpeed,
                    transform.position.y + stickInput.y * moveSpeed,
                    -1.0f);
        }
        else
        {
            if (input.PlayerNum == 0)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    transform.position = new Vector3(
                        transform.position.x, transform.position.y + moveSpeed, -1.0f);
                }
                if (Input.GetKey(KeyCode.A))
                {
                    transform.position = new Vector3(
                        transform.position.x - moveSpeed, transform.position.y, -1.0f);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    transform.position = new Vector3(
                        transform.position.x, transform.position.y - moveSpeed, -1.0f);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    transform.position = new Vector3(
                        transform.position.x + moveSpeed, transform.position.y, -1.0f);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    transform.position = new Vector3(
                        transform.position.x, transform.position.y + moveSpeed, -1.0f);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.position = new Vector3(
                        transform.position.x - moveSpeed, transform.position.y, -1.0f);
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    transform.position = new Vector3(
                        transform.position.x, transform.position.y - moveSpeed, -1.0f);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.position = new Vector3(
                        transform.position.x + moveSpeed, transform.position.y, -1.0f);
                }
            }
        }
    }
}