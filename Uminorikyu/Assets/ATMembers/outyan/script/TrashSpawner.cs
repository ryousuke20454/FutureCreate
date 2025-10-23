using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrashSpawnData
{
    [SerializeField] private GameObject trashPrefab; // �S�~�̃v���n�u
    [SerializeField] private int totalLimit; // ���̃S�~�̃X�|�[��������

    private int totalSpawned = 0;

    public GameObject TrashPrefab => trashPrefab;
    public int TotalLimit => totalLimit;
    public int TotalSpawned => totalSpawned;

    public void IncrementSpawned()
    {
        totalSpawned++;
    }
}

public class TrashSpawner : MonoBehaviour
{
    //[SerializeField] WaveManager waveManager;

    [Header("�S�~�^�C�v�ݒ�")]
    [SerializeField] private TrashSpawnData[] trashTypes;

    [Header("�X�|�[���ݒ�")]
    [SerializeField] private int maxEnemiesOnScreen = 25; // ��ʂɕ\�������S�~�̍ő吔
    [SerializeField] private float spawnDelayAfterDeath = 2.0f; // �S�~���|����Ă���̃X�|�[���ҋ@����

    [Header("�v���C���[�ݒ�")]
    [SerializeField] private Transform player;
    [SerializeField] private float minDistanceFromPlayer = 0.8f; // �v���C���[����̍ŏ������i��`�O�̏ꍇ�j
    [SerializeField] private float minSpawnRadius = 3.0f; // �v���C���[���X�|�[���̈���ɂ���Ƃ��̍ŏ�����

    [Header("�Z�`�X�|�[���ݒ�")]
    [Tooltip("��`�̕��iX����, world units�j")]
    [SerializeField] private float spawnAreaWidth = 22f;
    [Tooltip("��`�̍����iY����, world units�j")]
    [SerializeField] private float spawnAreaHeight = 8f;

    [Header("�X�|�[���ʒu�ݒ�")]
    [SerializeField] private int maxSpawnAttempts = 100; // �X�|�[���ʒu�T���̍ő厎�s��

    [Header("�f�o�b�O�\���ݒ�")]
    [SerializeField] private bool showSpawnRange = true; // �X�|�[���͈͂�\��
    private Color spawnRangeColor = Color.yellow; // �X�|�[����`�̐F
    private Color playerRangeColor = Color.blue; // �v���C���[����̍ŏ������\���F

    // �v���p�e�B
    public TrashSpawnData[] TrashTypes
    {
        get => trashTypes;
        set => trashTypes = value;
    }

    public int MaxEnemiesOnScreen
    {
        get => maxEnemiesOnScreen;
        set => maxEnemiesOnScreen = value;
    }

    public float SpawnDelayAfterDeath
    {
        get => spawnDelayAfterDeath;
        set => spawnDelayAfterDeath = value;
    }

    public Transform Player => player;
    public float MinDistanceFromPlayer => minDistanceFromPlayer;
    public float MinSpawnRadius => minSpawnRadius;
    public float SpawnAreaWidth => spawnAreaWidth;
    public float SpawnAreaHeight => spawnAreaHeight;
   // public float SpawnZ => spawnZ;
    public int MaxSpawnAttempts => maxSpawnAttempts;
    public int CurrentEnemiesOnScreen => currentEnemiesOnScreen;

    private int currentEnemiesOnScreen = 0; // ���݉�ʏ�ɂ���S�~
    private float spawnTimer = 0.0f; // �X�|�[���^�C�}�[
    private bool canSpawn = true; // �X�|�[���\�t���O

    // �X�|�[�������S�~���Ǘ����郊�X�g
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        // �v���C���[����������
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Wave�J�n - �ŏ����烉���_���ŃX�|�[���i��`���j
        SpawnEnemies();
    }

    private void Update()
    {
        if (!canSpawn)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnDelayAfterDeath)
            {
                canSpawn = true;
                spawnTimer = 0.0f;
            }
        }

        // �X�|�[���\�ɂȂ�����X�|�[�����s
        if (canSpawn)
        {
            SpawnEnemies();
        }

        // �����ȃS�~�I�u�W�F�N�g�����X�g����폜
        CleanupDestroyedEnemies();

        // �f�o�b�O�\���p�̍X�V����
        if (showSpawnRange)
        {
            DrawSpawnRanges();
        }
    }

    void SpawnEnemies()
    {
        // ��ʏ�̃S�~�����ő喢���ł܂��X�|�[���\�ȃS�~�����邩
        while (currentEnemiesOnScreen < maxEnemiesOnScreen && CanSpawnAnyTrash())
        {
            SpawnRandomTrash();
        }

        //// waveManager �� null �łȂ��ꍇ�̂݃t���O��ݒ�
        //if (waveManager != null)
        //{
        //    waveManager.AllTrashSpawned = true;
        //}
    }

    bool CanSpawnAnyTrash()
    {
        if (trashTypes == null || trashTypes.Length == 0) return false;

        // �X�|�[�������ɒB���Ă��Ȃ��S�~�^�C�v�����邩�`�F�b�N
        for (int i = 0; i < trashTypes.Length; i++)
        {
            if (trashTypes[i] != null &&
                trashTypes[i].TrashPrefab != null &&
                trashTypes[i].TotalSpawned < trashTypes[i].TotalLimit)
            {
                return true;
            }
        }
        return false;
    }

    void SpawnRandomTrash()
    {
        if (trashTypes == null || trashTypes.Length == 0) return;

        // �X�|�[���\�ȃS�~�^�C�v�̃��X�g�쐬
        List<int> availableTrashTypes = new List<int>();

        for (int i = 0; i < trashTypes.Length; i++)
        {
            if (trashTypes[i] != null &&
                trashTypes[i].TrashPrefab != null &&
                trashTypes[i].TotalSpawned < trashTypes[i].TotalLimit)
            {
                availableTrashTypes.Add(i);
            }
        }

        // �X�|�[���\�ȃS�~�����Ȃ��ꍇ�I��
        if (availableTrashTypes.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, availableTrashTypes.Count);
        int selectedTrashType = availableTrashTypes[randomIndex];

        SpawnTrash(selectedTrashType);
    }

    bool SpawnTrash(int trashTypeIndex)
    {
        if (trashTypes == null || trashTypeIndex < 0 || trashTypeIndex >= trashTypes.Length) return false;

        TrashSpawnData trashData = trashTypes[trashTypeIndex];

        if (trashData == null || trashData.TrashPrefab == null) return false;

        // �L���ȃX�|�[���ʒu��T��
        Vector3 spawnPosition;
        if (!FindValidSpawnPosition(out spawnPosition))
        {
            return false;
        }

        // 2D�p�� Z ���Œ�
        //spawnPosition.z = spawnZ;

        GameObject newTrash = Instantiate(trashData.TrashPrefab, spawnPosition, Quaternion.identity);

        // DisableComponent ���������̂��߁A�Q�ƂƏ����͍폜���܂����B
        // �K�v�Ȃ�S�~�v���n�u���ɁuDisableAfterDelay�v���̃X�N���v�g��t���Đ��䂵�Ă��������B

        // ���ĂȂ���΃S�~��Trash�^�O��t����
        if (newTrash != null && newTrash.tag != "Trash")
        {
            newTrash.tag = "Trash";
        }

        // �X�|�[�������S�~�����X�g�ɒǉ�
        spawnedEnemies.Add(newTrash);

        currentEnemiesOnScreen++;

        //if (waveManager != null)
        //{
        //    waveManager.CurrentTrash++;
        //}

        trashData.IncrementSpawned();

        return true;
    }

    bool FindValidSpawnPosition(out Vector3 spawnPosition)
    {
        spawnPosition = Vector3.zero;
        Vector3 spawnerCenter = transform.position;

        float halfW = spawnAreaWidth * 0.5f;
        float halfH = spawnAreaHeight * 0.5f;

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            // ��`�̈���̃����_���ʒu�i�ϓ����z�j
            float rx = Random.Range(-halfW, halfW);
            float ry = Random.Range(-halfH, halfH);
            Vector3 candidatePosition = new Vector3(spawnerCenter.x + rx, spawnerCenter.y + ry/*, spawnZ*/) ;

            // �v���C���[����̋����`�F�b�N�iXY ���ʁj
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(new Vector2(candidatePosition.x, candidatePosition.y),
                                                          new Vector2(player.position.x, player.position.y));

                // �v���C���[����`���ɂ��邩�`�F�b�N
                bool playerInsideRect = Mathf.Abs(player.position.x - spawnerCenter.x) <= halfW &&
                                        Mathf.Abs(player.position.y - spawnerCenter.y) <= halfH;

                if (playerInsideRect)
                {
                    if (distanceToPlayer >= minSpawnRadius)
                    {
                        spawnPosition = candidatePosition;
                        return true;
                    }
                }
                else
                {
                    if (distanceToPlayer >= minDistanceFromPlayer)
                    {
                        spawnPosition = candidatePosition;
                        return true;
                    }
                }
            }
            else
            {
                // �v���C���[�����Ȃ��ꍇ�͂��̂܂܃X�|�[��
                spawnPosition = candidatePosition;
                return true;
            }
        }

        return false;
    }

    // �j�󂳂ꂽ�S�~�I�u�W�F�N�g�����X�g����폜
    void CleanupDestroyedEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] == null)
            {
                spawnedEnemies.RemoveAt(i);
                currentEnemiesOnScreen--;
                //if (waveManager != null)
                //{
                //    waveManager.CurrentTrash--;
                //}
            }
        }
    }

    // �O������S�~���|���ꂽ�Ƃ��ɌĂ΂��
    public void OnTrashDestroyed(GameObject trashObject)
    {
        if (spawnedEnemies.Contains(trashObject))
        {
            spawnedEnemies.Remove(trashObject);
            currentEnemiesOnScreen--;

            //if (waveManager != null)
            //{
            //    waveManager.CurrentTrash--;
            //}

            // �X�|�[���ҋ@�J�n
            canSpawn = false;
            spawnTimer = 0.0f;
        }
    }

    // �O���������̃S�~��|��
    public void DestroyTrash(GameObject trashObject)
    {
        if (spawnedEnemies.Contains(trashObject))
        {
            OnTrashDestroyed(trashObject);
            Destroy(trashObject);
        }
    }

    // �S�ẴS�~��|��
    public void DestroyAllEnemies()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] != null)
            {
                Destroy(spawnedEnemies[i]);
            }
        }
        spawnedEnemies.Clear();
        currentEnemiesOnScreen = 0;

        //if (waveManager != null)
        //{
        //    waveManager.CurrentTrash = 0;
        //}
    }

    // �X�|�[�������S�~�̃��X�g���擾
    public List<GameObject> GetSpawnedEnemies()
    {
        // null�`�F�b�N���ėL���ȃS�~�̂ݕԂ�
        List<GameObject> validEnemies = new List<GameObject>();
        foreach (GameObject trash in spawnedEnemies)
        {
            if (trash != null)
            {
                validEnemies.Add(trash);
            }
        }
        return validEnemies;
    }

    void DrawSpawnRanges()
    {
        Vector3 spawnerPos = transform.position;
        //spawnerPos.z = spawnZ;

        // ��`��`��iXY ���ʁj
        DrawRectangle(spawnerPos, spawnAreaWidth, spawnAreaHeight, spawnRangeColor);

        // �v���C���[������ꍇ�A�v���C���[����̍ŏ������~��`��
        if (player != null)
        {
            Vector3 playerPos = new Vector3(player.position.x, player.position.y/*, spawnZ*/);
            DrawCircle(playerPos, minDistanceFromPlayer, playerRangeColor);
        }
    }

    void DrawRectangle(Vector3 center, float width, float height, Color color)
    {
        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        Vector3 bl = center + new Vector3(-halfW, -halfH, 0f); // bottom-left
        Vector3 br = center + new Vector3(halfW, -halfH, 0f);  // bottom-right
        Vector3 tr = center + new Vector3(halfW, halfH, 0f);   // top-right
        Vector3 tl = center + new Vector3(-halfW, halfH, 0f);  // top-left

        Debug.DrawLine(bl, br, color);
        Debug.DrawLine(br, tr, color);
        Debug.DrawLine(tr, tl, color);
        Debug.DrawLine(tl, bl, color);
    }

    void DrawCircle(Vector3 center, float radius, Color color)
    {
        int segments = 64;
        float angleStep = 360f / segments;

        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle),
                0f
            );

            Debug.DrawLine(prevPoint, newPoint, color);
            prevPoint = newPoint;
        }
    }
}