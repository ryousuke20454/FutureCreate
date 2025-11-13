using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤーがこのオブジェクトに触れたら、
/// ランダムな位置にワープさせ、
/// 渦のフェードアウト／インやプレイヤー縮小・再拡大などの演出を行うスクリプト。
/// </summary>
public class TestWarpHall : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public string playerTag = "Player"; // 対象のプレイヤータグ

    [Header("ワープ設定")]
    public Transform[] warpPoints; // ワープ先候補
    public float warpCooldown = 1f; // ワープ後クールタイム

    [Header("判定設定")]
    public float triggerRadius = 0.5f; // 中心判定半径

    [Header("演出設定")]
    public float vortexFadeDuration = 1f; // 渦フェード時間
    public float shrinkDuration = 1f;     // 縮小・拡大にかける時間
    public float shrinkScale = 0.2f;      // 縮小倍率（例: 0.2 = 元の20%）
    public float vortexReappearDelay = 0.5f; // 渦が再表示されるまでの遅延

    // 内部管理
    private Dictionary<Transform, float> playerCooldowns = new Dictionary<Transform, float>();
    private bool isAnyWarping = false;

    private void Update()
    {
        if (isAnyWarping) return; // ワープ中は他の判定を無視

        // シーン内のプレイヤーを検索
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);

        foreach (var playerObj in players)
        {
            Transform player = playerObj.transform;

            // クールタイム中ならスキップ
            if (playerCooldowns.ContainsKey(player) && Time.time < playerCooldowns[player])
                continue;

            // プレイヤーとこのテレポーターの距離
            float distance = Vector3.Distance(player.position, transform.position);

            if (distance <= triggerRadius)
            {
                StartCoroutine(WarpSequence(player));
                playerCooldowns[player] = Time.time + warpCooldown;
            }
        }
    }

    /// <summary>
    /// ワープ演出全体をまとめたコルーチン
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

        // プレイヤー移動停止
        var moveScript = player.GetComponent<OriiPlayerMove>();
        if (moveScript != null)
            moveScript.enabled = false;

        // 1. 渦をフェードアウト（当たり判定OFF）
        if (targetVortex != null)
        {
            SetVortexColliderEnabled(targetVortex, false);
            yield return StartCoroutine(FadeVortex(targetVortex, 1f, 0f, vortexFadeDuration));
        }

        // 2. プレイヤーを縮小（吸い込まれる演出）
        Vector3 originalScale = player.localScale;
        Vector3 shrinkScaleVec = originalScale * shrinkScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / shrinkDuration;
            player.localScale = Vector3.Lerp(originalScale, shrinkScaleVec, t);
            yield return null;
        }

        // 3. ワープ実行
        if (warpPoints != null && warpPoints.Length > 0)
        {
            Transform targetPoint = warpPoints[Random.Range(0, warpPoints.Length)];
            player.position = targetPoint.position;
            if (targetVortex != null)
                targetVortex.transform.position = targetPoint.position;
            Debug.Log($"プレイヤーをワープ → {targetPoint.name}");
        }

        // 4. 少し待って渦をフェードイン
        yield return new WaitForSeconds(vortexReappearDelay);
        if (targetVortex != null)
        {
            yield return StartCoroutine(FadeVortex(targetVortex, 0f, 1f, vortexFadeDuration));
            SetVortexColliderEnabled(targetVortex, true);
        }

        // 5. プレイヤーを元のサイズに戻す（出現演出）
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / shrinkDuration;
            player.localScale = Vector3.Lerp(shrinkScaleVec, originalScale, t);
            yield return null;
        }

        // 6. 移動再開
        if (moveScript != null)
            moveScript.enabled = true;

        yield return new WaitForSeconds(warpCooldown);
        isAnyWarping = false;
    }

    /// <summary>
    /// 渦のフェード演出
    /// </summary>
    private IEnumerator FadeVortex(Vortex vortex, float fromAlpha, float toAlpha, float duration)
    {
        if (vortex == null) yield break;
        SpriteRenderer sr = vortex.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float t = 0f;
        Color color = sr.color;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
            sr.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }

    /// <summary>
    /// 渦の当たり判定を切り替え
    /// </summary>
    private void SetVortexColliderEnabled(Vortex vortex, bool enabled)
    {
        Collider2D col = vortex.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = enabled;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
