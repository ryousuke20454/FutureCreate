using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    public enum PlayerType { Player1, Player2 }
    [SerializeField] private PlayerType playerType = PlayerType.Player1;
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private PlayerControls controls;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        BindInputs();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void BindInputs()
    {
        if (playerType == PlayerType.Player1)
        {
            controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            controls.Player.Move.canceled += _ => moveInput = Vector2.zero;
        }
        else if (playerType == PlayerType.Player2)
        {
            // 二人目用に別アクションマップを割り当ててもOK
            controls.Player2.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            controls.Player2.Move.canceled += _ => moveInput = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
