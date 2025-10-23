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
            Debug.Log("コントローラが接続されていません");
        }

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 左スティックの入力値を取得
        Vector2 stickInput = gamepad.leftStick.ReadValue();
        //移動
        gameObject.transform.position = 
            new Vector2(gameObject.transform.position .x + stickInput.x * speed,
            gameObject.transform.position.y + stickInput.y * speed);
    }
}
