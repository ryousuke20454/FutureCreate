using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    private enum PlayerEffectState { None, Riding }

    private class PlayerEffectData
    {
        public GameObject player;
        public ParticleSystem rideInstance;
        public PlayerEffectState currentState = PlayerEffectState.None;
    }

    [Header("波のプレハブ")]
    [SerializeField] private GameObject wavePrefab;

    [Header("波の設定")]
    [SerializeField] private float minWaveRadius = 0.5f;
    [SerializeField] private float maxWaveRadius = 3f;
    [SerializeField] private int maxWaves = 6;
    [SerializeField] private float waveFlowSpeed = 5f;
    [SerializeField] private float waveInfluenceStrength = 10f;
    [SerializeField] private float waveLifeTime = 30f;
    [SerializeField] private float minSpacingBetweenWaves = 3f;
    [SerializeField] private float avoidFieldEdgeMargin = 0.5f;
    [SerializeField][Range(0f, 1f)] private float initialAlpha = 0.6f;

    [Header("UV スクロール設定")]
    [SerializeField] private float uvScrollSpeed = 0.5f;
    [SerializeField] private string texturePropertyName = "_MainTex";

    [Header("プレイヤー向けエフェクト判定")]
    [SerializeField][Range(-1f, 1f)] private float rideDotThreshold = 0.5f;

    [Header("プレイヤー用パーティクル (WaveEvent 内で完結)")]
    [Tooltip("プレイヤーの乗っている時に表示する ParticleSystem のプレハブ（Scene のプレイヤーに子としてインスタンス化されます）")]
    [SerializeField] private ParticleSystem rideParticlePrefab;
    [SerializeField] private Vector3 particleLocalOffset = Vector3.zero;

    [Header("Particle Emission")]
    [SerializeField] private float minEmission = 5f;
    [SerializeField] private float maxEmission = 60f;

    [Header("ゴミへの影響")]
    [SerializeField] private float trashMovementSpeed = 2f;

    [Header("その他")]
    [SerializeField] private bool affectTrash = true;
    [SerializeField] private bool debugLogs = false;

    private List<WaveInstance> activeWaves = new List<WaveInstance>();
    private GameObject[] fieldObjects;
    private bool hasSpawnedInitialWaves = false;

    private Dictionary<GameObject, PlayerEffectData> playerEffects = new Dictionary<GameObject, PlayerEffectData>();

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

            var keysToRemove = new List<GameObject>();
            foreach (var kv in playerEffects)
            {
                if (kv.Key == null)
                {
                    DestroyPlayerEffectData(kv.Value);
                    keysToRemove.Add(kv.Key);
                }
            }
            foreach (var k in keysToRemove) playerEffects.Remove(k);
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

            GameObject waveObj = Instantiate(wavePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            waveObj.transform.localScale = Vector3.one * radius * 2f;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            waveObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            SetWaveAlpha(waveObj, initialAlpha);

            WaveInstance wave = new WaveInstance
            {
                waveObject = waveObj,
                position = position,
                direction = direction,
                radius = radius,
                strength = 1f,
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

        var removeList = new List<GameObject>();
        foreach (var kv in playerEffects)
        {
            if (kv.Key == null)
            {
                DestroyPlayerEffectData(kv.Value);
                removeList.Add(kv.Key);
            }
        }
        foreach (var k in removeList) playerEffects.Remove(k);
    }

    private void UpdateWaveVisuals()
    {
        foreach (var wave in activeWaves)
        {
            if (wave.waveObject == null) continue;

            float elapsedTime = Time.time - wave.spawnTime;

            // アルファ値の計算（フェードアウト）
            float alpha = initialAlpha;
            if (elapsedTime > waveLifeTime - 5f)
            {
                float fadeProgress = (elapsedTime - (waveLifeTime - 5f)) / 5f;
                alpha = Mathf.Lerp(initialAlpha, 0f, fadeProgress);
            }

            // UV スクロールの計算（縦方向にスクロール）
            // 波は常にY軸方向（縦）にスクロール
            Vector2 uvOffset = new Vector2(0, -uvScrollSpeed * elapsedTime);

            // SpriteRenderer の更新
            var spriteRenderers = wave.waveObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in spriteRenderers)
            {
                // マテリアルのインスタンス化（既にインスタンス化されていなければ）
                if (!sr.material.name.Contains("(Instance)"))
                {
                    sr.material = new Material(sr.material);
                }

                // UV オフセットの設定
                if (sr.material.HasProperty(texturePropertyName))
                {
                    sr.material.SetTextureOffset(texturePropertyName, uvOffset);
                }

                // マテリアルのアルファ値を設定
                if (sr.material.HasProperty("_Color"))
                {
                    Color matColor = sr.material.GetColor("_Color");
                    matColor.a = alpha;
                    sr.material.SetColor("_Color", matColor);
                }
            }

            // ParticleSystem の更新
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

    private void SetWaveAlpha(GameObject waveObj, float alpha)
    {
        var spriteRenderers = waveObj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in spriteRenderers)
        {
            Color col = sr.color;
            col.a = alpha;
            sr.color = col;

            // マテリアルのインスタンス化
            if (!sr.material.name.Contains("(Instance)"))
            {
                sr.material = new Material(sr.material);
            }
        }

        var particleSystems = waveObj.GetComponentsInChildren<ParticleSystem>();
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

    private void ApplyWavesToObject(GameObject obj, bool isPlayer)
    {
        if (obj == null) return;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        Vector2 objPos = rb != null ? rb.position : (Vector2)obj.transform.position;

        bool isInAnyWave = false;
        Vector3 totalMovement = Vector3.zero;

        WaveInstance bestWave = null;
        float bestWaveWeight = 0f;

        foreach (var wave in activeWaves)
        {
            float dist = Vector2.Distance(objPos, wave.position);
            if (dist > wave.radius) continue;

            isInAnyWave = true;

            float elapsedTime = Time.time - wave.spawnTime;
            float strengthMultiplier = 1f;
            if (elapsedTime > waveLifeTime - 5f)
            {
                float fadeProgress = (elapsedTime - (waveLifeTime - 5f)) / 5f;
                strengthMultiplier = Mathf.Clamp01(1f - fadeProgress);
            }

            float distFactor = 1f - (dist / wave.radius);

            float weight = distFactor * strengthMultiplier;
            if (weight > bestWaveWeight)
            {
                bestWaveWeight = weight;
                bestWave = wave;
            }

            if (rb == null && !isPlayer)
            {
                Vector2 movement = wave.direction * trashMovementSpeed * distFactor * strengthMultiplier * Time.fixedDeltaTime;

                Vector2 projectedPos = objPos + movement;
                Vector2 clampedPos = new Vector2(
                    Mathf.Clamp(projectedPos.x, wave.fieldBounds.min.x, wave.fieldBounds.max.x),
                    Mathf.Clamp(projectedPos.y, wave.fieldBounds.min.y, wave.fieldBounds.max.y)
                );
                movement = clampedPos - objPos;

                totalMovement += (Vector3)movement;

                if (debugLogs && Time.frameCount % 60 == 0)
                    Debug.Log($"[WaveEvent] Trash movement this wave: {movement.magnitude:F4}");
            }
            else if (isPlayer && rb != null)
            {
                Vector2 targetVelocity = wave.direction * waveFlowSpeed * (wave.radius / maxWaveRadius);
                float influenceRate = waveInfluenceStrength * distFactor * strengthMultiplier * Time.fixedDeltaTime;
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, influenceRate);
            }
        }

        if (rb == null && totalMovement != Vector3.zero)
        {
            obj.transform.position += totalMovement;

            if (debugLogs && !isPlayer && Time.frameCount % 60 == 0)
                Debug.Log($"[WaveEvent] Total movement applied: {totalMovement.magnitude:F4}");
        }

        if (isPlayer && !isInAnyWave && rb != null)
        {
            rb.linearVelocity *= 0.95f;
        }

        if (isPlayer && rb != null)
        {
            if (bestWave == null)
            {
                UpdatePlayerEffectState(obj, PlayerEffectState.None, 0f);
            }
            else
            {
                Vector2 playerVel = rb.linearVelocity;
                Vector2 playerDir;
                if (playerVel.sqrMagnitude > 0.001f)
                    playerDir = playerVel.normalized;
                else
                    playerDir = (obj.transform.right != Vector3.zero) ? (Vector2)obj.transform.right.normalized : Vector2.zero;

                float dot = Vector2.Dot(playerDir, bestWave.direction.normalized);
                float strength = Mathf.Clamp01(bestWaveWeight);

                if (dot >= rideDotThreshold)
                {
                    UpdatePlayerEffectState(obj, PlayerEffectState.Riding, strength);
                }
                else
                {
                    UpdatePlayerEffectState(obj, PlayerEffectState.None, strength);
                }

                if (debugLogs && Time.frameCount % 60 == 0)
                {
                    Debug.Log($"[WaveEvent] playerDir={playerDir}, waveDir={bestWave.direction.normalized}, dot={dot:F3}, weight={bestWaveWeight:F3}");
                }
            }
        }
    }

    private PlayerEffectData EnsurePlayerEffectData(GameObject player)
    {
        if (player == null) return null;

        if (playerEffects.TryGetValue(player, out var data))
            return data;

        data = new PlayerEffectData { player = player, currentState = PlayerEffectState.None };

        if (rideParticlePrefab != null)
        {
            var inst = Instantiate(rideParticlePrefab.gameObject, player.transform).GetComponent<ParticleSystem>();
            inst.gameObject.name = "RideParticle_Instance";
            inst.transform.localPosition = particleLocalOffset;
            StopAndDisableEmission(inst);
            data.rideInstance = inst;
        }

        playerEffects[player] = data;
        return data;
    }

    private void UpdatePlayerEffectState(GameObject player, PlayerEffectState newState, float strength)
    {
        if (player == null) return;

        var data = EnsurePlayerEffectData(player);
        if (data == null) return;

        float rate = Mathf.Lerp(minEmission, maxEmission, Mathf.Clamp01(strength));

        if (data.currentState == newState)
        {
            switch (newState)
            {
                case PlayerEffectState.Riding:
                    if (data.rideInstance != null)
                    {
                        SetEmission(data.rideInstance, rate, true);
                        if (!data.rideInstance.isPlaying)
                        {
                            data.rideInstance.Play();
                        }
                    }
                    break;
                case PlayerEffectState.None:
                default:
                    break;
            }
            return;
        }

        if (data.rideInstance != null)
        {
            StopAndDisableEmission(data.rideInstance);
        }

        switch (newState)
        {
            case PlayerEffectState.Riding:
                if (data.rideInstance != null)
                {
                    SetEmission(data.rideInstance, rate, true);
                    data.rideInstance.Play();
                }
                break;
            case PlayerEffectState.None:
            default:
                break;
        }

        data.currentState = newState;
    }

    private void StopAndDisableEmission(ParticleSystem ps)
    {
        if (ps == null) return;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        var em = ps.emission;
        em.enabled = false;
    }

    private void SetEmission(ParticleSystem ps, float rate, bool enabled)
    {
        if (ps == null) return;
        var em = ps.emission;
        em.enabled = enabled;
        em.rateOverTime = new ParticleSystem.MinMaxCurve(rate);
    }

    private void DestroyPlayerEffectData(PlayerEffectData data)
    {
        if (data == null) return;
        if (data.rideInstance != null)
        {
            Destroy(data.rideInstance.gameObject);
            data.rideInstance = null;
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
            float alpha = initialAlpha;

            if (elapsedTime > waveLifeTime - 5f)
            {
                float fadeProgress = (elapsedTime - (waveLifeTime - 5f)) / 5f;
                alpha = Mathf.Lerp(initialAlpha, 0f, fadeProgress);
            }

            Gizmos.color = new Color(0f, 1f, 1f, alpha);
            Gizmos.DrawWireSphere(wave.position, wave.radius);
            Gizmos.DrawRay(wave.position, wave.direction * wave.radius);
        }
    }
}