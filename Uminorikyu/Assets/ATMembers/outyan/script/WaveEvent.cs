using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEvent : MonoBehaviour
{
    [System.Serializable]
    private class WaveInstance
    {
        public GameObject waveObject;
        public Vector2 position;
        public Vector2 direction;
        public float radius;
        public float strength;
        public float spawnTime;
        public Bounds fieldBounds;
        public float pulseTimer;
    }

    [Header("波のプレハブ")]
    [SerializeField] private GameObject wavePrefab; //波のプレハブ

    [Header("波の設定")]
    [SerializeField] private float minWaveRadius = 0.5f; // 波の最小サイズ
    [SerializeField] private float maxWaveRadius = 3f; // 波の最大サイズ
    [SerializeField] private int maxWaves = 6; // 生成する波の数
    [SerializeField] private float waveBaseStrength = 180f; // 波の押し出す力（waveBaseStrength * 波のサイズ）
    [SerializeField] private float waveLifeTime = 30f; // 波が存在する時間（秒）
    [SerializeField] private float minSpacingBetweenWaves = 3f; // 波同士の最小距離
    [SerializeField] private float avoidFieldEdgeMargin = 0.5f; // 波がフィールド端に近すぎないようにするやつ


    [Header("プレイヤーへの影響")]
    [SerializeField] private float playerResistMultiplier = 0.6f; // 波に逆らった時の減速率
    [SerializeField] private float playerAssistMultiplier = 0.5f; // 波と同じ方向に進む時の加速率

    [Header("波への影響")]
    [SerializeField] private float trashForceMultiplier = 1.0f; // ゴミが波から受ける力の倍率（小さいほど遅く動く）

    [Header("その他")]
    [SerializeField] private bool affectTrash = true; //ゴミが波に影響されるかどうか
    [SerializeField] private bool debugLogs = false; // デバッグ表示ONOFF

    private List<WaveInstance> activeWaves = new List<WaveInstance>();
    private GameObject[] fieldObjects;
    private bool hasSpawnedInitialWaves = false;

    private void Start()
    {
        RefreshFields();
        StartCoroutine(InitialSpawn());
        StartCoroutine(CleanupLoop());
    }

    private IEnumerator InitialSpawn()
    {
        if (!hasSpawnedInitialWaves)
        {
            for (int i = 0; i < maxWaves; i++)
            {
                TrySpawnWave();
                yield return new WaitForSeconds(0.5f);
            }
            hasSpawnedInitialWaves = true;

            if (debugLogs)
                Debug.Log($"[WaveEvent] Initial wave spawn complete. Total waves: {activeWaves.Count}");
        }
    }

    private IEnumerator CleanupLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            for (int i = activeWaves.Count - 1; i >= 0; i--)
            {
                float elapsed = Time.time - activeWaves[i].spawnTime;
                if (elapsed > waveLifeTime)
                {
                    if (activeWaves[i].waveObject != null)
                        Destroy(activeWaves[i].waveObject);
                    activeWaves.RemoveAt(i);

                    if (debugLogs)
                        Debug.Log($"[WaveEvent] Wave removed after {waveLifeTime}s. Remaining waves: {activeWaves.Count}");
                }
            }
        }
    }

    private void TrySpawnWave()
    {
        if (wavePrefab == null)
        {
            if (debugLogs) Debug.LogWarning("[WaveEvent] Wave prefab is not assigned!");
            return;
        }

        Camera mainCam = Camera.main;
        if (mainCam == null || fieldObjects == null || fieldObjects.Length == 0) return;

        Bounds cameraBounds = GetCameraBounds(mainCam);

        var field = fieldObjects[Random.Range(0, fieldObjects.Length)];
        if (field == null) return;

        var colliders = field.GetComponentsInChildren<Collider2D>(true);
        if (colliders == null || colliders.Length == 0) return;

        Bounds fieldBounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
            fieldBounds.Encapsulate(colliders[i].bounds);

        float minX = fieldBounds.min.x + avoidFieldEdgeMargin;
        float maxX = fieldBounds.max.x - avoidFieldEdgeMargin;
        float minY = fieldBounds.min.y + avoidFieldEdgeMargin;
        float maxY = fieldBounds.max.y - avoidFieldEdgeMargin;

        Bounds innerBounds = new Bounds(
            new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0),
            new Vector3(maxX - minX, maxY - minY, 0)
        );

        Bounds spawnBounds = GetIntersection(innerBounds, cameraBounds);
        if (spawnBounds.size.x <= 0f || spawnBounds.size.y <= 0f) return;

        // 重複チェックを含めて最大10回試行
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 position = new Vector2(
                Random.Range(spawnBounds.min.x, spawnBounds.max.x),
                Random.Range(spawnBounds.min.y, spawnBounds.max.y)
            );

            float radius = Random.Range(minWaveRadius, maxWaveRadius);

            bool tooClose = false;
            foreach (var w in activeWaves)
            {
                if (Vector2.Distance(position, w.position) < radius + w.radius + minSpacingBetweenWaves)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;

            Vector2 toCenter = ((Vector2)spawnBounds.center - position).normalized;
            Vector2 direction = (toCenter + Random.insideUnitCircle * 0.5f).normalized;
            if (direction == Vector2.zero) direction = Vector2.up;

            // プレハブをインスタンス化
            GameObject waveObj = Instantiate(wavePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            waveObj.transform.localScale = Vector3.one * radius * 2f;

            // プレハブを波の方向に向ける
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            waveObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            WaveInstance wave = new WaveInstance
            {
                waveObject = waveObj,
                position = position,
                direction = direction,
                radius = radius,
                strength = waveBaseStrength * radius,
                spawnTime = Time.time,
                fieldBounds = spawnBounds,
                pulseTimer = 0f
            };

            activeWaves.Add(wave);

            if (debugLogs)
                Debug.Log($"[WaveEvent] Spawned wave at {position}, radius={radius:F2}");

            return;
        }

        if (debugLogs)
            Debug.LogWarning("[WaveEvent] Failed to spawn wave after 10 attempts (space constraints)");
    }

    private void FixedUpdate()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
            ApplyWavesToObject(player, true);

        if (affectTrash)
        {
            var trashes = GameObject.FindGameObjectsWithTag("Trash");
            foreach (var trash in trashes)
                ApplyWavesToObject(trash, false);
        }

        UpdateWaveVisuals();
    }

    private void UpdateWaveVisuals()
    {
        foreach (var wave in activeWaves)
        {
            if (wave.waveObject == null) continue;

            float elapsedTime = Time.time - wave.spawnTime;

            // 全体の寿命によるフェード（最後の5秒でフェードアウト）
            float alpha = 1f;
            if (elapsedTime > waveLifeTime - 5f)
            {
                float fadeProgress = (elapsedTime - (waveLifeTime - 5f)) / 5f;
                alpha = Mathf.Clamp01(1f - fadeProgress);
            }

            // SpriteRendererの透明度を更新
            var spriteRenderers = wave.waveObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in spriteRenderers)
            {
                Color col = sr.color;
                col.a = alpha;
                sr.color = col;
            }

            // ParticleSystemがあれば更新
            var particleSystems = wave.waveObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                var main = ps.main;
                Color startColor = main.startColor.color;
                startColor.a = alpha;
                main.startColor = startColor;

                var emission = ps.emission;
                emission.enabled = alpha > 0.1f;
            }
        }
    }

    private void ApplyWavesToObject(GameObject obj, bool isPlayer)
    {
        if (obj == null) return;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        Vector2 objPos = rb != null ? rb.position : (Vector2)obj.transform.position;

        bool isInAnyWave = false;

        foreach (var wave in activeWaves)
        {
            float dist = Vector2.Distance(objPos, wave.position);
            if (dist > wave.radius) continue;

            // 波の範囲内
            isInAnyWave = true;

            float elapsedTime = Time.time - wave.spawnTime;
            float strengthMultiplier = 1f;
            if (elapsedTime > waveLifeTime - 5f)
            {
                float fadeProgress = (elapsedTime - (waveLifeTime - 5f)) / 5f;
                strengthMultiplier = Mathf.Clamp01(1f - fadeProgress);
            }

            float distFactor = 1f - (dist / wave.radius);

            Vector2 pushBase = wave.direction * wave.strength * distFactor * strengthMultiplier * Time.fixedDeltaTime;

            Vector2 projectedPos = objPos + pushBase;
            Vector2 clampedPos = new Vector2(
                Mathf.Clamp(projectedPos.x, wave.fieldBounds.min.x, wave.fieldBounds.max.x),
                Mathf.Clamp(projectedPos.y, wave.fieldBounds.min.y, wave.fieldBounds.max.y)
            );
            pushBase = clampedPos - objPos;

            float maxMove = Mathf.Max(0f, wave.radius - dist);
            if (pushBase.magnitude > maxMove)
                pushBase = pushBase.normalized * maxMove;

            if (rb == null)
            {
                // Trash用に移動量を大幅に減らす
                Vector3 movement = (Vector3)pushBase;
                if (!isPlayer)
                {
                    movement *= 0.01f; // Trashの移動速度を1%に抑える
                }
                obj.transform.position += movement;
                continue;
            }

            if (isPlayer)
            {
                Vector2 vel = rb.linearVelocity;
                float speed = vel.magnitude;

                if (speed < 0.1f)
                {
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, rb.linearVelocity + pushBase, 0.5f);
                }
                else
                {
                    float dot = Vector2.Dot(vel.normalized, wave.direction);

                    if (dot < -0.2f)
                    {
                        float resist = playerResistMultiplier * distFactor * strengthMultiplier;
                        rb.linearVelocity *= Mathf.Clamp01(1f - resist * Time.fixedDeltaTime * 5f);
                    }
                    else if (dot > 0.2f)
                    {
                        Vector2 assist = wave.direction * wave.strength * playerAssistMultiplier * distFactor * strengthMultiplier * Time.fixedDeltaTime;
                        rb.AddForce(assist, ForceMode2D.Force);
                    }
                    else
                    {
                        rb.AddForce(pushBase, ForceMode2D.Force);
                    }
                }
            }
            else
            {
                if (rb.bodyType == RigidbodyType2D.Dynamic)
                {
                    //Vector2 desiredVel = rb.linearVelocity + pushBase;
                    //rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVel, 0.25f);

                    Vector2 force = wave.direction * wave.strength * distFactor * strengthMultiplier * trashForceMultiplier;
                    rb.AddForce(force, ForceMode2D.Force);
                }
            }
        }

        if(isPlayer && !isInAnyWave && rb != null) 
        {
            rb.linearVelocity *= 0.95f;
        }
    }

    private Bounds GetCameraBounds(Camera cam)
    {
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        return new Bounds(cam.transform.position, new Vector3(width, height, 0f));
    }

    private Bounds GetIntersection(Bounds a, Bounds b)
    {
        float minX = Mathf.Max(a.min.x, b.min.x);
        float maxX = Mathf.Min(a.max.x, b.max.x);
        float minY = Mathf.Max(a.min.y, b.min.y);
        float maxY = Mathf.Min(a.max.y, b.max.y);

        if (minX > maxX || minY > maxY)
            return new Bounds(Vector3.zero, Vector3.zero);

        return new Bounds(
            new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f),
            new Vector3(maxX - minX, maxY - minY, 0f)
        );
    }

    public void RefreshFields()
    {
        fieldObjects = GameObject.FindGameObjectsWithTag("Field");
        if (debugLogs) Debug.Log($"[WaveEvent] Found {fieldObjects.Length} Field objects.");
    }

    private void OnDrawGizmos()
    {
        if (activeWaves == null) return;

        foreach (var wave in activeWaves)
        {
            float elapsedTime = Time.time - wave.spawnTime;
            float alpha = 1f;

            if (elapsedTime > waveLifeTime - 5f)
            {
                float fadeProgress = (elapsedTime - (waveLifeTime - 5f)) / 5f;
                alpha = Mathf.Clamp01(1f - fadeProgress);
            }

            Gizmos.color = new Color(0f, 1f, 1f, alpha);
            Gizmos.DrawWireSphere(wave.position, wave.radius);
            Gizmos.DrawRay(wave.position, wave.direction * wave.radius);
        }
    }
}