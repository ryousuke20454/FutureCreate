using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEvent : MonoBehaviour
{
    [Header("波のプレハブ")]
    [SerializeField] private GameObject waveVisualPrefab;

    [Header("波の大きさ（半径）")]
    [SerializeField] private float minWaveRadius = 1f;
    [SerializeField] private float maxWaveRadius = 3f;

    [Header("スポーン設定")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxWaves = 6;
    [SerializeField] private float minSpacingBetweenWaves = 3f; // 波の中心間最小距離
    [SerializeField] private float avoidFieldEdgeMargin = 0.5f; // フィールド端から離す余白
    [SerializeField] private int spawnAttemptLimit = 30;

    [Header("波の挙動")]
    [SerializeField] private float waveBaseStrength = 180f;
    [SerializeField] private float playerResistMultiplier = 0.6f; // 逆らった時の減速係数
    [SerializeField] private float playerAssistMultiplier = 0.5f; // 流れに乗るときの加速係数
    [SerializeField] private float waveLifeTime = 12f;

    [Header("ゴミを流すかどうか")]
    [SerializeField] private bool affectTrash = true;

    // 内部管理
    private readonly List<Wave> activeWaves = new List<Wave>();
    private GameObject[] fieldObjects;

    private void Awake()
    {
        fieldObjects = GameObject.FindGameObjectsWithTag("Field");
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            TrySpawnWave();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void TrySpawnWave()
    {
        if (activeWaves.Count >= maxWaves) return;
        if (fieldObjects == null || fieldObjects.Length == 0) return;

        for (int attempt = 0; attempt < spawnAttemptLimit; attempt++)
        {
            var field = fieldObjects[Random.Range(0, fieldObjects.Length)];
            if (field == null) continue;

            var col = field.GetComponent<Collider2D>();
            if (col == null) continue;

            var bounds = col.bounds;
            // フィールド端を avoidFieldEdgeMargin 分避ける
            float x = Random.Range(bounds.min.x + avoidFieldEdgeMargin, bounds.max.x - avoidFieldEdgeMargin);
            float y = Random.Range(bounds.min.y + avoidFieldEdgeMargin, bounds.max.y - avoidFieldEdgeMargin);
            Vector2 candidate = new Vector2(x, y);

            float radius = Random.Range(minWaveRadius, maxWaveRadius);

            // 既存波と重ならないかチェック（中心距離ベース）
            bool ok = true;
            foreach (var w in activeWaves)
            {
                if (w == null) continue;
                float minDist = radius + w.Radius + minSpacingBetweenWaves;
                if (Vector2.Distance(candidate, w.Center) < minDist)
                {
                    ok = false;
                    break;
                }
            }
            if (!ok) continue;

            // 生成
            GameObject go;
            if (waveVisualPrefab != null)
            {
                go = Instantiate(waveVisualPrefab, candidate, Quaternion.identity, transform);
            }
            else
            {
                // 簡易的な視覚オブジェクトを動的生成（無くても動作する）
                go = new GameObject("Wave");
                go.transform.position = candidate;
                go.transform.parent = transform;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.color = new Color(0.5f, 0.8f, 1f, 0.4f);
                // 注意: 実プロジェクトでは専用スプライトを割り当ててください。
            }

            var waveComp = go.AddComponent<Wave>();
            // 必要なパラメータを渡す（外側のインスタンスフィールドをネストクラス内で直接参照しない）
            Vector2 randDir = Random.insideUnitCircle.normalized;
            if (randDir == Vector2.zero) randDir = Vector2.up;
            waveComp.Initialize(this, radius, randDir, waveBaseStrength, waveLifeTime, playerResistMultiplier, playerAssistMultiplier);
            activeWaves.Add(waveComp);
            return;
        }
    }

    private void NotifyWaveDestroyed(Wave w)
    {
        if (activeWaves.Contains(w)) activeWaves.Remove(w);
    }

    internal bool ShouldAffectTrash() => affectTrash;

    // ネストクラス（Wave）は private にしてファイル内で完結
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    private class Wave : MonoBehaviour
    {
        private CircleCollider2D circle;
        private Rigidbody2D rb;
        private WaveEvent owner;
        private Vector2 direction;
        private float radius;
        private float strength;
        private float lifeTime;
        private float spawnTime;

        // プレイヤー向け補正は外側から渡して保持して使う（静的参照エラーを回避）
        private float localPlayerResistMultiplier;
        private float localPlayerAssistMultiplier;

        internal Vector2 Center => (Vector2)transform.position;
        internal float Radius => radius;

        private void Awake()
        {
            circle = GetComponent<CircleCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            circle.isTrigger = true;
            if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;
        }

        internal void Initialize(WaveEvent ownerSystem, float r, Vector2 dir, float baseStrength, float life, float resist, float assist)
        {
            owner = ownerSystem;
            radius = Mathf.Max(0.01f, r);
            direction = dir;
            if (direction == Vector2.zero) direction = Vector2.up;
            strength = baseStrength * (radius / Mathf.Max(1f, 1f));
            lifeTime = life;
            spawnTime = Time.time;

            localPlayerResistMultiplier = resist;
            localPlayerAssistMultiplier = assist;

            // Collider 半径を設定（localScale に依存しない場合）
            circle.radius = radius;

            // 視覚スケールがあれば合わせる（任意）
            transform.localScale = Vector3.one * 1f;

            StartCoroutine(AutoDestroy());
        }

        private IEnumerator AutoDestroy()
        {
            yield return new WaitForSeconds(lifeTime);
            DestroySelf();
        }

        private void DestroySelf()
        {
            // 外側の非公開メソッドを呼び出す。ネストクラスは外側インスタンス参照を持つことができるので呼べる。
            if (owner != null) owner.NotifyWaveDestroyed(this);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (owner != null) owner.NotifyWaveDestroyed(this);
        }

        // 物理影響処理（FixedUpdate と同タイミングで呼ばれる）
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other == null) return;
            var otherRb = other.attachedRigidbody;
            if (otherRb == null) return;

            // Trash の扱いはオーナーに従う
            if (other.CompareTag("Trash") && owner != null && !owner.ShouldAffectTrash()) return;

            Vector2 toObj = otherRb.position - (Vector2)transform.position;
            float dist = toObj.magnitude;
            float distFactor = Mathf.Clamp01(1f - (dist / Mathf.Max(radius, 0.0001f))); // 0..1

            // Player は特別扱い
            if (other.CompareTag("Player"))
            {
                // NOTE: Rigidbody2D.velocity は非推奨 -> linearVelocity を使用する
                Vector2 vel = otherRb.linearVelocity;
                float speed = vel.magnitude;
                Vector2 velDir = speed > 0.001f ? vel.normalized : Vector2.zero;
                float dot = Vector2.Dot(velDir, direction); // -1..1

                // --- スタミナ処理箇所（未実装） ---
                // TODO: ここでプレイヤーのスタミナ（例: PlayerStamina コンポーネント）を参照して、
                // 逆らう（dot < -閾値）場合は高いスタミナ消費を行い、スタミナ不足なら逆らえない等の制御を追加してください。
                // 例:
                // var stamina = other.GetComponent<PlayerStamina>();
                // if (dot < -0.2f) { if (stamina.Current >= cost) stamina.Consume(cost); else /*スタミナ不足時の挙動*/ }

                // 停止している場合は素直に流される
                if (speed < 0.1f)
                {
                    Vector2 push = direction * strength * distFactor * Time.fixedDeltaTime;
                    otherRb.AddForce(push, ForceMode2D.Force);
                    return;
                }

                // 逆らっている（dot が負）
                if (dot < -0.2f)
                {
                    float resist = localPlayerResistMultiplier * distFactor;

                    // 旧コード: otherRb.velocity = otherRb.velocity * ... (非推奨)
                    // 警告対応: linearVelocity を直接書き換える短期対応
                    Vector2 currentLin = otherRb.linearVelocity;
                    Vector2 targetLin = currentLin * Mathf.Clamp01(1f - resist * Time.fixedDeltaTime * 5f);
                    // 直接代入（非推奨警告を解消する）:
                    otherRb.linearVelocity = targetLin;

                    // 少し波に押される（逆向きの力）
                    Vector2 pushBack = -direction * (strength * 0.2f) * distFactor * Time.fixedDeltaTime;
                    otherRb.AddForce(pushBack, ForceMode2D.Force);

                    // 代替（より物理的に自然な方法）: インパルスで Δv を与える方法（コメント）
                    // Vector2 deltaVel = targetLin - currentLin;
                    // otherRb.AddForce(deltaVel * otherRb.mass, ForceMode2D.Impulse);
                }
                else if (dot > 0.2f)
                {
                    // 流れに乗っている → 加速（一定の消費想定）
                    Vector2 assist = direction * (strength * localPlayerAssistMultiplier) * distFactor * Time.fixedDeltaTime;
                    otherRb.AddForce(assist, ForceMode2D.Force);

                    // TODO: ここで「流れに乗ると一定のスタミナ消費」を入れる場合はスタミナ参照を実装してください。
                }
                else
                {
                    // 横向き程度なら少し流す
                    Vector2 lateral = direction * (strength * 0.6f) * distFactor * Time.fixedDeltaTime;
                    otherRb.AddForce(lateral, ForceMode2D.Force);
                }
            }
            else
            {
                // Player 以外（Trash 等）は単純に押す
                Vector2 push = direction * strength * distFactor * Time.fixedDeltaTime;
                otherRb.AddForce(push, ForceMode2D.Force);
            }
        }
    }
}