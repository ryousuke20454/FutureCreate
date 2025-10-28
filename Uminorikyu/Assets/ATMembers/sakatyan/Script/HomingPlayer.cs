using UnityEngine;
using System.Collections;

/// <summary>
/// プレイヤーを追従し、
/// 衝突時に吹き飛び → 一定距離 or 一定時間後に停止 → 追従再開する渦スクリプト
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Vortex : MonoBehaviour
{
    // =====================================================
    // 追従設定
    // =====================================================
    [Header("追従設定")]
    [SerializeField] private Transform targetToFollow; // 追従する対象（プレイヤーなど）
    [SerializeField] private float speed = 2f;         // 通常の追従速度

    // =====================================================
    // サイズ設定
    // =====================================================
    [Header("サイズ設定")]
    [SerializeField] private float growAmount = 0.05f; // ゴミ吸収時の拡大率
    [SerializeField] private float maxScale = 1f;      // 最大サイズ制限

    // =====================================================
    // 回転設定
    // =====================================================
    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 180f; // 回転速度（度/秒）

    // =====================================================
    // 吹き飛び設定
    // =====================================================
    [Header("吹き飛び設定")]
    [SerializeField] private float bouncePower = 5f;        // 吹き飛びの力
    [SerializeField] private float maxBounceDistance = 2f;  // 吹き飛び距離上限
    [SerializeField] private float stopDuration = 1f;       // 吹き飛び後に停止する時間（秒）

    // =====================================================
    // 内部変数
    // =====================================================
    private Rigidbody2D rb;             // 自分の Rigidbody2D
    private Rigidbody2D targetRb;       // ターゲットの Rigidbody2D
    private MonoBehaviour targetController; // プレイヤー操作スクリプト

    private Vector3 baseScale;          // 初期スケール
    private bool isKnockback = false;   // 吹き飛び中か？
    private Vector3 knockbackStartPos;  // 吹き飛び開始位置

    // =====================================================
    // 初期化処理
    // =====================================================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (targetToFollow != null)
        {
            targetRb = targetToFollow.GetComponent<Rigidbody2D>();
            targetController = targetToFollow.GetComponent<MonoBehaviour>();
        }
    }

    private void Start()
    {
        baseScale = transform.localScale;

        // ターゲット未設定なら自動で Player タグ検索
        if (targetToFollow == null)
            targetToFollow = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (targetToFollow != null)
        {
            targetRb = targetToFollow.GetComponent<Rigidbody2D>();
            targetController = targetToFollow.GetComponent<MonoBehaviour>();
        }
    }

    // =====================================================
    // 毎フレーム処理
    // =====================================================
    private void Update()
    {
        // 常に回転（見た目用）
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // 吹き飛び中は追従しない
        if (isKnockback) return;

        // ターゲットが存在するなら追従
        if (targetToFollow != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetToFollow.position,
                speed * Time.deltaTime
            );
        }
    }

    // =====================================================
    // 衝突処理
    // =====================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 吹き飛び中は衝突処理無効
        if (isKnockback) return;

        // ------------------------
        // ゴミとの衝突処理
        // ------------------------
        if (collision.CompareTag("Trash"))
        {
            float vortexScale = transform.localScale.x;
            float trashScale = collision.transform.localScale.x;

            if (vortexScale >= trashScale)
            {
                Destroy(collision.gameObject);

                // 成長処理（上限あり）
                float newScale = Mathf.Min(transform.localScale.x + growAmount, maxScale);
                transform.localScale = new Vector3(newScale, newScale, 1f);
            }
            return;
        }

        // ------------------------
        // 他の渦との衝突処理
        // ------------------------
        Vortex otherVortex = collision.GetComponent<Vortex>();
        if (otherVortex == null || otherVortex == this) return;

        float myScale = transform.localScale.x;
        float otherScale = otherVortex.transform.localScale.x;

        Rigidbody2D otherRb = otherVortex.rb;
        Rigidbody2D otherTargetRb = otherVortex.targetRb;

        // 大小比較で処理分岐
        if (myScale > otherScale)
        {
            // 自分が大きい → 相手を吹き飛ばす
            Vector2 dir = (otherVortex.transform.position - transform.position).normalized;

            otherRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherTargetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);

            otherVortex.StartKnockback();
            if (otherVortex.targetController != null)
                otherVortex.targetController.enabled = false;
        }
        else if (myScale < otherScale)
        {
            // 自分が小さい → 自分が吹き飛ぶ
            Vector2 dir = (transform.position - otherVortex.transform.position).normalized;

            rb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            targetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);

            StartKnockback();
            if (targetController != null)
                targetController.enabled = false;
        }
        else
        {
            // 同サイズ → 両者吹き飛ぶ
            Vector2 dir = (otherVortex.transform.position - transform.position).normalized;

            // 自分
            rb?.AddForce(-dir * bouncePower, ForceMode2D.Impulse);
            targetRb?.AddForce(-dir * bouncePower, ForceMode2D.Impulse);
            StartKnockback();
            if (targetController != null)
                targetController.enabled = false;

            // 相手
            otherRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherTargetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherVortex.StartKnockback();
            if (otherVortex.targetController != null)
                otherVortex.targetController.enabled = false;
        }
    }

    // =====================================================
    // 吹き飛び開始処理
    // =====================================================
    public void StartKnockback()
    {
        if (isKnockback) return;

        isKnockback = true;
        knockbackStartPos = transform.position;

        // 一定時間後に吹き飛び解除＋追従再開
        StartCoroutine(StopKnockbackAfterDelay());
    }

    // =====================================================
    // 吹き飛び終了処理
    // =====================================================
    private IEnumerator StopKnockbackAfterDelay()
    {
        // 一定時間停止
        yield return new WaitForSeconds(stopDuration);

        rb.Sleep();
        rb.angularVelocity = 0f;

        if (targetRb != null) targetRb.Sleep();

        // プレイヤー操作再開
        if (targetController != null)
            targetController.enabled = true;

        // 状態リセット
        isKnockback = false;
    }
}
