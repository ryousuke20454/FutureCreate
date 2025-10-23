using UnityEngine;

public class HomingPlayer : MonoBehaviour
{
    Transform playerTr;
    [SerializeField] float speed = 2f;

    [Header("サイズ設定")]
    [SerializeField] float growAmount = 0.05f; // 拡大量
    [SerializeField] float maxScale = 1f;     // 最大スケール
    private Vector3 baseScale;                 // 初期スケールを保持

    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        // インスペクターで設定した初期スケールを保存
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, playerTr.position) < 0.1f)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            playerTr.position,
            speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash"))
        {
            Destroy(collision.gameObject);

            // 現在スケールを基準に拡大（初期値からの相対拡大）
            float newScale = transform.localScale.x + growAmount;
            newScale = Mathf.Min(newScale, maxScale); // 最大値制限
            transform.localScale = new Vector3(newScale, newScale, 1f);
        }
    }
}
