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
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float speed = 2f;

    // =====================================================
    // サイズ設定
    // =====================================================
    [Header("サイズ設定")]
    [SerializeField] private float growAmount = 0.05f;
    [SerializeField] private float maxScale = 1f;

    // ★変更点：成長速度
    [SerializeField] private float growSpeed = 3f; // スムーズに拡大する速さ

    // =====================================================
    // 回転設定
    // =====================================================
    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 180f;

    // =====================================================
    // 吹き飛び設定
    // =====================================================
    [Header("吹き飛び設定")]
    [SerializeField] private float bouncePower = 5f;
    [SerializeField] private float maxBounceDistance = 2f;
    [SerializeField] private float stopDuration = 1f;

    // =====================================================
    // 内部変数
    // =====================================================
    private Rigidbody2D rb;
    private Rigidbody2D targetRb;
    private MonoBehaviour targetController;

    private Vector3 baseScale;
    private bool isKnockback = false;
    private Vector3 knockbackStartPos;

    private CameraController camera;

    // ★変更点：Lerp用のターゲットスケール
    private Vector3 targetScale;

    // =====================================================
    // 初期化
    // =====================================================
    private void Start()
    {
        baseScale = transform.localScale;
        targetScale = baseScale; // ←初期化（初期スケールと同じに）
        rb = GetComponent<Rigidbody2D>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

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
        // 常に回転
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // スケールを滑らかに補間
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * growSpeed
        );

        // 吹き飛び中は追従しない
        if (isKnockback) return;

        // 追従処理
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
        if (isKnockback) return;

        // ------------------------
        // ゴミとの衝突
        // ------------------------
        if (collision.CompareTag("Trash"))
        {
            float vortexScale = transform.localScale.x;
            float trashScale = collision.transform.localScale.x;

            if (vortexScale >= trashScale)
            {
                Destroy(collision.gameObject);

                //すぐ拡大せず、ターゲットスケールを更新
                float newScale = Mathf.Min(targetScale.x + growAmount, maxScale);
                targetScale = new Vector3(newScale, newScale, 1f);
            }
            return;
        }

        // ------------------------
        // 他の渦との衝突
        // ------------------------
        Vortex otherVortex = collision.GetComponent<Vortex>();
        if (otherVortex == null || otherVortex == this) return;

        float myScale = transform.localScale.x;
        float otherScale = otherVortex.transform.localScale.x;

        Rigidbody2D otherRb = otherVortex.rb;
        Rigidbody2D otherTargetRb = otherVortex.targetRb;

        if (myScale > otherScale)
        {
            camera.ShakeCamera();
            Vector2 dir = (otherVortex.transform.position - transform.position).normalized;

            otherRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherTargetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);

            otherVortex.StartKnockback();
            if (otherVortex.targetController != null)
                otherVortex.targetController.enabled = false;
        }
        else if (myScale < otherScale)
        {
            camera.ShakeCamera();
            Vector2 dir = (transform.position - otherVortex.transform.position).normalized;

            rb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            targetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);

            StartKnockback();
            if (targetController != null)
                targetController.enabled = false;
        }
        else
        {
            camera.ShakeCamera();
            Vector2 dir = (otherVortex.transform.position - transform.position).normalized;

            rb?.AddForce(-dir * bouncePower, ForceMode2D.Impulse);
            targetRb?.AddForce(-dir * bouncePower, ForceMode2D.Impulse);
            StartKnockback();
            if (targetController != null)
                targetController.enabled = false;

            otherRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherTargetRb?.AddForce(dir * bouncePower, ForceMode2D.Impulse);
            otherVortex.StartKnockback();
            if (otherVortex.targetController != null)
                otherVortex.targetController.enabled = false;
        }
    }

    // =====================================================
    // 吹き飛び開始
    // =====================================================
    public void StartKnockback()
    {
        if (isKnockback) return;
        isKnockback = true;
        knockbackStartPos = transform.position;
        StartCoroutine(StopKnockbackAfterDelay());
    }

    // =====================================================
    // 吹き飛び解除
    // =====================================================
    private IEnumerator StopKnockbackAfterDelay()
    {
        yield return new WaitForSeconds(stopDuration);

        rb.Sleep();
        rb.angularVelocity = 0f;
        if (targetRb != null) targetRb.Sleep();

        if (targetController != null)
            targetController.enabled = true;

        isKnockback = false;
    }
}
