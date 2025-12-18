using UnityEngine;
using System.Collections;
using TMPro;
using KanKikuchi.AudioManager;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Vortex : MonoBehaviour
{
    [Header("追従設定")]
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private float speed = 2f;

    [Header("サイズ設定")]
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float growSpeed = 3f;

    [Header("回転設定")]
    [SerializeField] private float rotationSpeed = 180f;

    [Header("吹き飛び設定")]
    [SerializeField] private float bounceDistance = 1.5f;
    [SerializeField] private float bounceDuration = 0.6f;
    [SerializeField] private float stopDuration = 0.5f;
    [SerializeField] private float scaleTolerance = 0.02f;
    [SerializeField] private float blinkInterval = 0.15f;

    [Header("バーンアウト吹き飛び倍率")]
    [SerializeField] private float barnOutbounce = 1.5f;

    [Header("スタミナダメージ")]
    [SerializeField] private float damageStamina = 10f;

    [Header("スコアポップアップ設定")]
    [SerializeField] private GameObject floatingScorePrefab;
    [SerializeField] private Canvas worldSpaceCanvas;

    [Header("プレイヤー番号設定")]
    [SerializeField] private int playerNum = 0;

    [Header("ヒットエフェクト設定")]
    [SerializeField] private GameObject particle;

    [Header("ゴミ取得時エフェクト設定")]
    [SerializeField] private GameObject particle2;

    private Rigidbody2D rb;
    private Collider2D col;
    private CameraController camera;
    private bool isKnockback = false;
    private Vector3 baseScale;
    private Vector3 targetScale;

    private Rigidbody2D playerRb;
    private OriiPlayerMove playerController;
    private SpriteRenderer playerRenderer;
    private PlayerAndStaminaInfo playerStamina;
    private Coroutine blinkCoroutine;
    private Color originalColor;
    private bool isInvincible = false;  // 12/18追加、点滅中に移動できるようにするためのもの(起き攻め防止)

    public Transform TargetToFollow => targetToFollow;

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
            playerStamina = targetToFollow.GetComponent<PlayerAndStaminaInfo>();

            originalColor = playerRenderer.color;
        }
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * growSpeed);

        // ===== バーンアウト中は移動不可 =====
        if (playerController != null)
        {
            if (playerController.barnOut)
            {
                playerController.enabled = false;
            }
            else if (!isKnockback) // 吹き飛び中でなければ
            {
                playerController.enabled = true;
            }
        }

        if (targetToFollow != null)
        {
            transform.position = new Vector3(
                targetToFollow.position.x,
                targetToFollow.position.y,
                0f
            );
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isKnockback || isInvincible) return;

        // ---- ゴミとの接触 ----
        if (collision.CompareTag("Trash"))
        {
            // コリジョンは触れたまま
            if (playerController != null && playerController.barnOut)
            {
                // 取得はスキップするだけ
                return;
            }

            float vortexScale = transform.localScale.x;
            float trashScale = collision.transform.localScale.x;
            float growAmount = collision.gameObject.GetComponent<TrashStatus>().glowAmount;

            if (vortexScale >= trashScale)
            {
                if (worldSpaceCanvas != null)
                    ShowFloatingText(collision.transform.position, collision.GetComponent<TrashStatus>().score);

                PlayerControllerManager.controllerManager.SetScore(playerNum, collision.GetComponent<TrashStatus>().score);

                SEManager.Instance.Play(SEPath.PON);

                if (particle2 != null)
                {
                    GameObject p = Instantiate(particle2, collision.transform.position, Quaternion.identity);

                    // ゴミの大きさを取得してパーティクルに反映
                    p.transform.localScale *= trashScale * 0.8f;  // ←倍率は調整可能
                }

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

        bool amIBurnOut = playerController != null && playerController.barnOut;
        bool isOtherBurnOut = otherVortex.playerController != null && otherVortex.playerController.barnOut;

        Vector2 dir = (transform.position - otherVortex.transform.position).normalized;
        if (dir.sqrMagnitude < 0.01f)
            dir = Random.insideUnitCircle.normalized;

        SEManager.Instance.Play(SEPath.ZABAN);
        camera?.ShakeCamera();

        if (particle != null)
            Instantiate(particle, otherVortex.transform.position, Quaternion.identity);
            Instantiate(particle, transform.position, Quaternion.identity);

        // --- スタミナダメージ処理 ---
        ApplyStaminaDamage(otherVortex, myScale, otherScale);

        // =====================================================
        // バーンアウト優先判定
        // =====================================================
        if (amIBurnOut && !isOtherBurnOut)
        {
            StartCoroutine(KnockbackWithPlayer(dir, barnOutbounce));
            return;
        }
        if (!amIBurnOut && isOtherBurnOut)
        {
            otherVortex.StartCoroutine(otherVortex.KnockbackWithPlayer(-dir, otherVortex.barnOutbounce));
            return;
        }

        // =====================================================
        // 同サイズ判定（バーンアウトなしの場合のみ）
        // =====================================================
        if (Mathf.Abs(myScale - otherScale) < scaleTolerance)
        {
            StartCoroutine(KnockbackWithPlayer(dir));
            otherVortex.StartCoroutine(otherVortex.KnockbackWithPlayer(-dir));
            return;
        }

        // =====================================================
        // サイズ差による勝敗
        // =====================================================
        if (myScale > otherScale)
        {
            otherVortex.StartCoroutine(otherVortex.KnockbackWithPlayer(-dir));
        }
        else
        {
            StartCoroutine(KnockbackWithPlayer(dir));
        }
    }


    private void ApplyStaminaDamage(Vortex other, float myScale, float otherScale)
    {
        if (playerStamina == null || other.playerStamina == null) return;

        // 小さい方が大きく受ける
        float myDamage = damageStamina * (otherScale / myScale);
        float otherDamage = damageStamina * (myScale / otherScale);

        playerStamina.TakeDamage(myDamage);
        other.playerStamina.TakeDamage(otherDamage);
    }

    private IEnumerator KnockbackWithPlayer(Vector2 direction, float bounceMultiplier = 1f)
    {
        // ===== 吹き飛び開始 =====
        isKnockback = true;
        isInvincible = false;

        if (col != null) col.enabled = false;
        if (playerController != null) playerController.enabled = false;

        Vector3 startPosVortex = transform.position;
        Vector3 startPosPlayer = targetToFollow.position;

        float scaleFactor = Mathf.Clamp(transform.localScale.x, 0.8f, 3f);
        float power = Mathf.Lerp(bounceDistance, bounceDistance * 0.5f, (scaleFactor - 0.8f) / 2.2f);
        power *= bounceMultiplier;

        Vector3 targetOffset = direction.normalized * power;

        float elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / bounceDuration);

            Vector3 offset = targetOffset * t;
            transform.position = startPosVortex + offset;
            targetToFollow.position = startPosPlayer + offset;

            yield return null;
        }

        // ===== 吹き飛び終了 → 点滅無敵開始 =====
        isKnockback = false;
        isInvincible = true;

        if (playerController != null) playerController.enabled = true;

        if (playerRenderer != null)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkPlayer());
        }

        // 点滅（無敵）時間
        yield return new WaitForSeconds(stopDuration);

        // ===== 完全復帰 =====
        isInvincible = false;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
            playerRenderer.color = originalColor;
        }

        if (col != null) col.enabled = true;
    }


    private IEnumerator BlinkPlayer()
    {
        Color damageColor = new Color(1f, 0.3f, 0.3f, 1f);

        while (true)
        {
            // 赤点滅
            playerRenderer.color = damageColor;
            yield return new WaitForSeconds(blinkInterval);

            // 元の色に戻す
            playerRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }
    }


    private void ShowFloatingText(Vector3 worldPos, int score)
    {
        if (floatingScorePrefab == null) return;

        GameObject popup;
        if (worldSpaceCanvas != null)
        {
            popup = Instantiate(floatingScorePrefab, worldPos, Quaternion.identity, worldSpaceCanvas.transform);
        }
        else
        {
            popup = Instantiate(floatingScorePrefab, worldPos, Quaternion.identity);
        }

        FloatingScoreText textComp = popup.GetComponent<FloatingScoreText>();
        if (textComp != null)
        {
            textComp.SetText(score.ToString());
        }

        if (Camera.main != null)
        {
            popup.transform.forward = Camera.main.transform.forward;
        }
    }

}
