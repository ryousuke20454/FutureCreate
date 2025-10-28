using UnityEngine;

public class MoveRightTest : MonoBehaviour
{
    [SerializeField] private float speed = 2f; // 移動速度

    private void Update()
    {
        // 右方向に移動（X座標を増加させる）
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
