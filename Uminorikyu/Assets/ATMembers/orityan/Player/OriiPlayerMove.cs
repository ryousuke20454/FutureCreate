using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;


public class OriiPlayerMove : MonoBehaviour
{
    [SerializeField] public float moveSpeed;
    public bool dash;
    public bool barnOut;
    public bool nowEvent;

    PlayerInputScript input;
    Gamepad gamepad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = GetComponent<PlayerInputScript>();
        gamepad = input.controller;

        dash = false;
        barnOut = true;
        nowEvent = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(input.controller != null && !barnOut)
        {
            Vector2 stickInput = input.GetStickValue(gamepad);

            if (input.GetSquareButton(gamepad))
            {
                Debug.Log(" ‚ª‰Ÿ‚³‚ê‚Ü‚µ‚½");
            }
            if (input.GetTriangleButton(gamepad))
            {
                Debug.Log("¢‚ª‰Ÿ‚³‚ê‚Ü‚µ‚½");
            }
            if (input.GetCircleButton(gamepad))
            {
                Debug.Log("›‚ª‰Ÿ‚³‚ê‚Ü‚µ‚½");
            }

            if (input.GetCrossButton(gamepad))
            {
                Debug.Log("~‚ª‰Ÿ‚³‚ê‚Ü‚µ‚½");
                if (stickInput.x != 0.0f && stickInput.y != 0.0f)
                {
                    dash = true;
                }
            }
            else
            {
                dash = false;
            }

            //ˆÚ“®
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


        if (transform.position.x > 15.0f)
        {
            transform.position = new Vector3(15.0f,
                transform.position.y,
                transform.position.z);
        }
        else if (transform.position.x < -15.0f)
        {
            transform.position = new Vector3(-15.0f,
            transform.position.y,
            transform.position.z);
        }

        if (transform.position.y > 10.0f)
        {
            transform.position = new Vector3(
                transform.position.x,
                10.0f,
                transform.position.z);
        }
        else if (transform.position.y < -10.0f)
        {
            transform.position = new Vector3(
                transform.position.x,
                -10.0f,
                transform.position.z);
        }
    }
}