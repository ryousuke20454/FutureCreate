using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEvent : MonoBehaviour
{
    [Header("�g�̃v���n�u")]
    [SerializeField] private GameObject waveVisualPrefab;

    [Header("�g�̑傫���i���a�j")]
    [SerializeField] private float minWaveRadius = 1f;
    [SerializeField] private float maxWaveRadius = 3f;

    [Header("�X�|�[���ݒ�")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxWaves = 6;
    [SerializeField] private float minSpacingBetweenWaves = 3f; // �g�̒��S�ԍŏ�����
    [SerializeField] private float avoidFieldEdgeMargin = 0.5f; // �t�B�[���h�[���痣���]��
    [SerializeField] private int spawnAttemptLimit = 30;

    [Header("�g�̋���")]
    [SerializeField] private float waveBaseStrength = 180f;
    [SerializeField] private float playerResistMultiplier = 0.6f; // �t��������̌����W��
    [SerializeField] private float playerAssistMultiplier = 0.5f; // ����ɏ��Ƃ��̉����W��
    [SerializeField] private float waveLifeTime = 12f;

    [Header("�S�~�𗬂����ǂ���")]
    [SerializeField] private bool affectTrash = true;

    // �����Ǘ�
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
            // �t�B�[���h�[�� avoidFieldEdgeMargin ��������
            float x = Random.Range(bounds.min.x + avoidFieldEdgeMargin, bounds.max.x - avoidFieldEdgeMargin);
            float y = Random.Range(bounds.min.y + avoidFieldEdgeMargin, bounds.max.y - avoidFieldEdgeMargin);
            Vector2 candidate = new Vector2(x, y);

            float radius = Random.Range(minWaveRadius, maxWaveRadius);

            // �����g�Əd�Ȃ�Ȃ����`�F�b�N�i���S�����x�[�X�j
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

            // ����
            GameObject go;
            if (waveVisualPrefab != null)
            {
                go = Instantiate(waveVisualPrefab, candidate, Quaternion.identity, transform);
            }
            else
            {
                // �ȈՓI�Ȏ��o�I�u�W�F�N�g�𓮓I�����i�����Ă����삷��j
                go = new GameObject("Wave");
                go.transform.position = candidate;
                go.transform.parent = transform;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.color = new Color(0.5f, 0.8f, 1f, 0.4f);
                // ����: ���v���W�F�N�g�ł͐�p�X�v���C�g�����蓖�ĂĂ��������B
            }

            var waveComp = go.AddComponent<Wave>();
            // �K�v�ȃp�����[�^��n���i�O���̃C���X�^���X�t�B�[���h���l�X�g�N���X���Œ��ڎQ�Ƃ��Ȃ��j
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

    // �l�X�g�N���X�iWave�j�� private �ɂ��ăt�@�C�����Ŋ���
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

        // �v���C���[�����␳�͊O������n���ĕێ����Ďg���i�ÓI�Q�ƃG���[������j
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

            // Collider ���a��ݒ�ilocalScale �Ɉˑ����Ȃ��ꍇ�j
            circle.radius = radius;

            // ���o�X�P�[��������΍��킹��i�C�Ӂj
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
            // �O���̔���J���\�b�h���Ăяo���B�l�X�g�N���X�͊O���C���X�^���X�Q�Ƃ������Ƃ��ł���̂ŌĂׂ�B
            if (owner != null) owner.NotifyWaveDestroyed(this);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (owner != null) owner.NotifyWaveDestroyed(this);
        }

        // �����e�������iFixedUpdate �Ɠ��^�C�~���O�ŌĂ΂��j
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other == null) return;
            var otherRb = other.attachedRigidbody;
            if (otherRb == null) return;

            // Trash �̈����̓I�[�i�[�ɏ]��
            if (other.CompareTag("Trash") && owner != null && !owner.ShouldAffectTrash()) return;

            Vector2 toObj = otherRb.position - (Vector2)transform.position;
            float dist = toObj.magnitude;
            float distFactor = Mathf.Clamp01(1f - (dist / Mathf.Max(radius, 0.0001f))); // 0..1

            // Player �͓��ʈ���
            if (other.CompareTag("Player"))
            {
                // NOTE: Rigidbody2D.velocity �͔񐄏� -> linearVelocity ���g�p����
                Vector2 vel = otherRb.linearVelocity;
                float speed = vel.magnitude;
                Vector2 velDir = speed > 0.001f ? vel.normalized : Vector2.zero;
                float dot = Vector2.Dot(velDir, direction); // -1..1

                // --- �X�^�~�i�����ӏ��i�������j ---
                // TODO: �����Ńv���C���[�̃X�^�~�i�i��: PlayerStamina �R���|�[�l���g�j���Q�Ƃ��āA
                // �t�炤�idot < -臒l�j�ꍇ�͍����X�^�~�i������s���A�X�^�~�i�s���Ȃ�t�炦�Ȃ����̐����ǉ����Ă��������B
                // ��:
                // var stamina = other.GetComponent<PlayerStamina>();
                // if (dot < -0.2f) { if (stamina.Current >= cost) stamina.Consume(cost); else /*�X�^�~�i�s�����̋���*/ }

                // ��~���Ă���ꍇ�͑f���ɗ������
                if (speed < 0.1f)
                {
                    Vector2 push = direction * strength * distFactor * Time.fixedDeltaTime;
                    otherRb.AddForce(push, ForceMode2D.Force);
                    return;
                }

                // �t����Ă���idot �����j
                if (dot < -0.2f)
                {
                    float resist = localPlayerResistMultiplier * distFactor;

                    // ���R�[�h: otherRb.velocity = otherRb.velocity * ... (�񐄏�)
                    // �x���Ή�: linearVelocity �𒼐ڏ���������Z���Ή�
                    Vector2 currentLin = otherRb.linearVelocity;
                    Vector2 targetLin = currentLin * Mathf.Clamp01(1f - resist * Time.fixedDeltaTime * 5f);
                    // ���ڑ���i�񐄏��x������������j:
                    otherRb.linearVelocity = targetLin;

                    // �����g�ɉ������i�t�����̗́j
                    Vector2 pushBack = -direction * (strength * 0.2f) * distFactor * Time.fixedDeltaTime;
                    otherRb.AddForce(pushBack, ForceMode2D.Force);

                    // ��ցi��蕨���I�Ɏ��R�ȕ��@�j: �C���p���X�� ��v ��^������@�i�R�����g�j
                    // Vector2 deltaVel = targetLin - currentLin;
                    // otherRb.AddForce(deltaVel * otherRb.mass, ForceMode2D.Impulse);
                }
                else if (dot > 0.2f)
                {
                    // ����ɏ���Ă��� �� �����i���̏���z��j
                    Vector2 assist = direction * (strength * localPlayerAssistMultiplier) * distFactor * Time.fixedDeltaTime;
                    otherRb.AddForce(assist, ForceMode2D.Force);

                    // TODO: �����Łu����ɏ��ƈ��̃X�^�~�i����v������ꍇ�̓X�^�~�i�Q�Ƃ��������Ă��������B
                }
                else
                {
                    // ���������x�Ȃ班������
                    Vector2 lateral = direction * (strength * 0.6f) * distFactor * Time.fixedDeltaTime;
                    otherRb.AddForce(lateral, ForceMode2D.Force);
                }
            }
            else
            {
                // Player �ȊO�iTrash ���j�͒P���ɉ���
                Vector2 push = direction * strength * distFactor * Time.fixedDeltaTime;
                otherRb.AddForce(push, ForceMode2D.Force);
            }
        }
    }
}