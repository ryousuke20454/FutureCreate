using UnityEngine;
using System.Collections;
using TMPro;
using KanKikuchi.AudioManager;

/// <summary>
/// プレイヤーを追従し、
/// 他の渦と衝突した際に吹き飛び演出を行うスクリプト。
/// プレイヤーも同じ距離だけ一緒に吹き飛び、
/// 吹き飛び中は移動が無効化＆点滅演出される。
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Vortex : MonoBehaviour
{
    // =====================================================
    // 設定値
    // =====================================================
    [Header("追従設定")]
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float speed = 2f;

    [Header("サイズ設定")]
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float growSpeed = 3f;

    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 180f;

    [Header("吹き飛び設定")]
    [SerializeField] private float bounceDistance = 1.5f;   // 吹き飛び距離
    [SerializeField] private float bounceDuration = 0.6f;   // 吹き飛び時間
    [SerializeField] private float stopDuration = 0.5f;     // 停止時間
    [SerializeField] private float scaleTolerance = 0.02f;  // 同サイズ誤差
    [SerializeField] private float blinkInterval = 0.15f;   // 点滅間隔


    [Header("スコアポップアップ設定")]
    [SerializeField, Tooltip("スコアポップアップ用プレハブ(Resources内)")]
    private GameObject floatingScorePrefab;
    [SerializeField, Tooltip("World Space Canvas（UI表示先）")]
    private Canvas worldSpaceCanvas;

    [Header("プレイヤー番号設定")]
    [SerializeField] private int playerNum = 0;



    // =====================================================
    // 内部変数
    // =====================================================
    private Rigidbody2D rb;
    private Collider2D col;
    private CameraController camera;
    private bool isKnockback = false;
    private Vector3 baseScale;
    private Vector3 targetScale;

    // プレイヤー関連
    private Rigidbody2D playerRb;
    private OriiPlayerMove playerController;
    private SpriteRenderer playerRenderer;
    private Coroutine blinkCoroutine;

    public Transform TargetToFollow => targetToFollow;

    // =====================================================
    // 初期化
    // =====================================================
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        camera = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<CameraController>();

        baseScale = transform.localScale;
        targetScale = baseScale;

        if (targetToFollow == null)
            targetToFollow = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (targetToFollow != null)
        {
            playerRb = targetToFollow.GetComponent<Rigidbody2D>();
            playerController = targetToFollow.GetComponent<OriiPlayerMove>();
            playerRenderer = targetToFollow.GetComponent<SpriteRenderer>();
        }
    }

    // =====================================================
    // 更新処理
    // =====================================================
    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * growSpeed);

        //if (isKnockback) return;

        if (targetToFollow != null)
        {
            // 追従を完全固定に変更
            transform.position = new Vector3(
                targetToFollow.position.x,
                targetToFollow.position.y,
                0.0f);
            
        }
    }

    // =====================================================
    // 衝突判定
    // =====================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーが burnOut 中なら衝突無効化
        if (playerController != null && playerController.barnOut)
        {
            return;
        }

        // 自分が吹き飛び中なら無視
        if (isKnockback) return;

        // ---- ゴミとの接触 ----
        if (collision.CompareTag("Trash"))
        {
            float vortexScale = transform.localScale.x;
            float trashScale = collision.transform.localScale.x;

            float growAmount = collision.gameObject.GetComponent<TrashStatus>().glowAmount;

            if (vortexScale >= trashScale)
            {
                if (worldSpaceCanvas != null)
                {
                    ShowFloatingText(collision.gameObject.transform.position,
                        collision.gameObject.GetComponent<TrashStatus>().score);
                }

                PlayerControllerManager.controllerManager.SetScore(playerNum, collision.gameObject.GetComponent<TrashStatus>().score);

                SEManager.Instance.Play(SEPath.PON);
                Destroy(collision.gameObject);
                float newScale = Mathf.Min(targetScale.x + growAmount, maxScale);
                targetScale = new Vector3(newScale, newScale, 1f);
            }
            return;
        }

        // ---- 他の渦との衝突 ----
        Vortex otherVortex = collision.GetComponent<Vortex>();
        if (otherVortex == null || otherVortex == this) return;
        if (otherVortex.isKnockback) return;

        float myScale = transform.localScale.x;
        float otherScale = otherVortex.transform.localScale.x;

        Vector2 dir = (transform.position - otherVortex.transform.position).normalized;
        if (dir.sqrMagnitude < 0.01f)
            dir = Random.insideUnitCircle.normalized;

        SEManager.Instance.Play(SEPath.ZABAN);

        //自分と相手の大きさを比較→誤差判定以下だったら一緒に吹き飛ぶ
        if (Mathf.Abs(myScale - otherScale) < scaleTolerance)
        {
            camera?.ShakeCamera();
            StartCoroutine(KnockbackWithPlayer(dir));
            otherVortex.StartCoroutine(otherVortex.KnockbackWithPlayer(-dir));
        }

        else if (myScale > otherScale)
        {
            camera?.ShakeCamera();
            otherVortex.StartCoroutine(otherVortex.KnockbackWithPlayer(-dir));
        }

        else
        {
            camera?.ShakeCamera();
            StartCoroutine(KnockbackWithPlayer(dir));
        }
    }

    // =====================================================
    // 吹き飛ばし処理
    // =====================================================
    private IEnumerator KnockbackWithPlayer(Vector2 direction)
    {
        isKnockback = true;
        if (col != null) col.enabled = false; // 衝突無効化

        // プレイヤー操作停止
        if (playerController != null)
            playerController.enabled = false;

        // プレイヤー点滅開始（既存があれば停止）
        if (playerRenderer != null)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkPlayer());
        }

        // 初期位置
        Vector3 startPosVortex = transform.position;
        Vector3 startPosPlayer = targetToFollow != null ? targetToFollow.position : Vector3.zero;

        // 吹き飛び距離（サイズで軽減されすぎないよう補正）
        float scaleFactor = Mathf.Clamp(transform.localScale.x, 0.8f, 3f);
        float power = Mathf.Lerp(bounceDistance, bounceDistance * 0.5f, (scaleFactor - 0.8f) / 2.2f);
        Vector3 targetOffset = (Vector3)(direction.normalized * power);

        // スムーズ補間
        float elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / bounceDuration);

            Vector3 offset = targetOffset * t;
            transform.position = startPosVortex + offset;
            if (targetToFollow != null)
                targetToFollow.position = startPosPlayer + offset;

            yield return null;
        }

        yield return new WaitForSeconds(stopDuration);

        // プレイヤー操作再開
        if (playerController != null)
            playerController.enabled = true;

        // 点滅停止
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            if (playerRenderer != null)
                playerRenderer.enabled = true;
        }

        // 衝突再有効化
        if (col != null) col.enabled = true;

        isKnockback = false;
    }

    // =====================================================
    // 点滅処理
    // =====================================================
    private IEnumerator BlinkPlayer()
    {
        while (true)
        {
            if (playerRenderer != null)
                playerRenderer.enabled = !playerRenderer.enabled;

            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void ShowFloatingText(Vector3 worldPos, int score)
    {
        if (floatingScorePrefab == null) return;

        // World Space Canvas の子として生成
        GameObject popup;
        if (worldSpaceCanvas != null)
        {
            popup = Instantiate(floatingScorePrefab, worldPos, Quaternion.identity, worldSpaceCanvas.transform);
        }
        else
        {
            popup = Instantiate(floatingScorePrefab, worldPos, Quaternion.identity);
        }

        // テキスト設定
        FloatingScoreText textComp = popup.GetComponent<FloatingScoreText>();
        if (textComp != null)
        {
            textComp.SetText(score.ToString());
        }

        // カメラの方向を向かせる（ビルボード効果）
        if (Camera.main != null)
        {
            popup.transform.forward = Camera.main.transform.forward;
        }

        Debug.Log($"★ FloatingScoreText 生成！ pos={worldPos}, alpha={popup.GetComponentInChildren<TextMeshProUGUI>().color.a}");
    }
}
