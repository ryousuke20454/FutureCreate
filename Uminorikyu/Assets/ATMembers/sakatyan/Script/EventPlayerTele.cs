using UnityEngine;
using System.Collections;

/// <summary>
/// プレイヤーがテレポーターに入ったとき、
/// 渦をフェードアウトしてプレイヤーを拡大、
/// その後プレイヤーと渦をワープ、
/// 少し遅れて渦をフェードイン再表示する。
/// フェードアウト中とワープ中は渦の当たり判定を無効化する。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EventPlayerTele : MonoBehaviour
{
    [Header("タグ設定")]
    [SerializeField] private string playerTag = "Player";   // プレイヤータグ

    [Header("カメラ余白設定")]
    [SerializeField] private float margin = 0.5f;           // カメラ端の余白

    [Header("参照カメラ設定")]
    [SerializeField] private Camera mainCamera;             // カメラ参照（未設定なら自動）

    [Header("クールダウン")]
    [SerializeField] private float teleportCooldown = 1.5f; // クールダウン

    [Header("拡大時間")]
    [SerializeField] private float enlargeDuration = 1.0f;  // 拡大時間

    [Header("拡大スケール倍率")]
    [SerializeField] private float targetScale = 1.5f;      // 拡大後のスケール倍率

    [Header("渦の再表示遅延")]
    [SerializeField] private float vortexReappearDelay = 0.5f; // 渦の再表示までの遅延

    [Header("フェード速度")]
    [SerializeField] private float vortexFadeDuration = 0.8f;  // 渦フェード時間

    private Collider2D teleCollider;
    private static bool isAnyWarping = false; // 同時発動防止フラグ

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        teleCollider = GetComponent<Collider2D>();
        teleCollider.isTrigger = true;
    }

    private void Update()
    {
        if (isAnyWarping)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        foreach (GameObject player in players)
        {
            if (player == null)
                continue;

            Vector2 playerCenter = player.transform.position;

            if (teleCollider.OverlapPoint(playerCenter))
            {
                StartCoroutine(WarpSequence(player.transform));
                break;
            }
        }
    }

    /// <summary>
    /// プレイヤーと渦のワープ演出全体
    /// </summary>
    private IEnumerator WarpSequence(Transform player)
    {
        isAnyWarping = true;

        // プレイヤーに追従している渦を取得
        Vortex[] allVortexes = FindObjectsByType<Vortex>(FindObjectsSortMode.None);
        Vortex targetVortex = null;
        foreach (var vortex in allVortexes)
        {
            if (vortex != null && vortex.TargetToFollow == player)
            {
                targetVortex = vortex;
                break;
            }
        }

        // プレイヤーの移動スクリプト停止
        var moveScript = player.GetComponent<OriiPlayerMove>();
        if (moveScript != null)
            moveScript.enabled = false;

        // === 1. 渦フェードアウト（同時に当たり判定を無効化） ===
        if (targetVortex != null)
        {
            SetVortexColliderEnabled(targetVortex, false); // ← ここで当たり判定OFF
            yield return StartCoroutine(FadeVortex(targetVortex, 1f, 0f, vortexFadeDuration));
        }

        // === 2. プレイヤー拡大アニメーション ===
        Vector3 originalScale = player.localScale;
        Vector3 targetScaleVec = originalScale * targetScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / enlargeDuration;
            player.localScale = Vector3.Lerp(originalScale, targetScaleVec, t);
            yield return null;
        }

        // === 3. ワープ処理 ===
        //Vector3 randomPos = GetRandomPositionInCamera();
        Vector3 randomPos = new Vector3(Random.Range(-15.0f,15.0f), Random.Range(-10.0f, 10.0f),-1.0f);

        player.position = randomPos;
        if (targetVortex != null)
        {
            randomPos.z = 0.0f;
            targetVortex.transform.position = randomPos;
        }

        Debug.Log($"プレイヤーと渦をワープ → {randomPos}");

        // === 4. 一定時間後に渦をフェードイン ===
        yield return new WaitForSeconds(vortexReappearDelay);
        if (targetVortex != null)
        {
            yield return StartCoroutine(FadeVortex(targetVortex, 0f, 1f, vortexFadeDuration));
            SetVortexColliderEnabled(targetVortex, true); // ← フェードイン完了後に当たり判定ON
        }

        // === 5. プレイヤーを元のサイズに戻す ===
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / enlargeDuration;
            player.localScale = Vector3.Lerp(targetScaleVec, originalScale, t);
            yield return null;
        }

        // === 6. 移動スクリプト再有効化 ===
        if (moveScript != null)
            moveScript.enabled = true;

        // === 7. クールダウン ===
        yield return new WaitForSeconds(teleportCooldown);
        isAnyWarping = false;
    }

    /// <summary>
    /// カメラ内のランダム位置を取得
    /// </summary>
    private Vector3 GetRandomPositionInCamera()
    {
        if (mainCamera == null)
            return Vector3.zero;

        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Vector3 camPos = mainCamera.transform.position;

        float minX = camPos.x - camWidth / 2f + margin;
        float maxX = camPos.x + camWidth / 2f - margin;
        float minY = camPos.y - camHeight / 2f + margin;
        float maxY = camPos.y + camHeight / 2f - margin;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        return new Vector3(randomX, randomY, 0f);
    }

    /// <summary>
    /// 渦のフェード処理（α値補間）
    /// </summary>
    private IEnumerator FadeVortex(Vortex vortex, float startAlpha, float endAlpha, float duration)
    {
        if (vortex == null)
            yield break;

        Renderer[] renderers = vortex.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            yield break;

        float time = 0f;
        Color[] baseColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] is SpriteRenderer sr)
                baseColors[i] = sr.color;
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);

            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] is SpriteRenderer sr)
                {
                    Color c = baseColors[i];
                    c.a = alpha;
                    sr.color = c;
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// 渦に含まれる Collider2D を一括で有効／無効にする
    /// </summary>
    private void SetVortexColliderEnabled(Vortex vortex, bool enabled)
    {
        Collider2D[] colliders = vortex.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = enabled;
        }
    }
}
