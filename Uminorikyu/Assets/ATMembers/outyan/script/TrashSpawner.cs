using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrashSpawnData
{
    [SerializeField] private GameObject trashPrefab; // ゴミのプレハブ
    [SerializeField] private int totalLimit; // そのゴミのスポーン制限数

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

    [Header("ゴミタイプ設定")]
    [SerializeField] private TrashSpawnData[] trashTypes;

    [Header("スポーン設定")]
    [SerializeField] private int maxEnemiesOnScreen = 25; // 画面に表示されるゴミの最大数
    [SerializeField] private float spawnDelayAfterDeath = 2.0f; // ゴミが倒されてからのスポーン待機時間

    [Header("プレイヤー設定")]
    [SerializeField] private Transform player;
    [SerializeField] private float minDistanceFromPlayer = 0.8f; // プレイヤーからの最小距離（矩形外の場合）
    [SerializeField] private float minSpawnRadius = 3.0f; // プレイヤーがスポーン領域内にいるときの最小距離

    [Header("短形スポーン設定")]
    [Tooltip("矩形の幅（X方向, world units）")]
    [SerializeField] private float spawnAreaWidth = 22f;
    [Tooltip("矩形の高さ（Y方向, world units）")]
    [SerializeField] private float spawnAreaHeight = 8f;

    [Header("スポーン位置設定")]
    [SerializeField] private int maxSpawnAttempts = 100; // スポーン位置探索の最大試行回数

    [Header("デバッグ表示設定")]
    [SerializeField] private bool showSpawnRange = true; // スポーン範囲を表示
    private Color spawnRangeColor = Color.yellow; // スポーン矩形の色
    private Color playerRangeColor = Color.blue; // プレイヤーからの最小距離表示色

    // プロパティ
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

    private int currentEnemiesOnScreen = 0; // 現在画面上にいるゴミ
    private float spawnTimer = 0.0f; // スポーンタイマー
    private bool canSpawn = true; // スポーン可能フラグ

    // スポーンしたゴミを管理するリスト
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        // プレイヤーを自動検索
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Wave開始 - 最初からランダムでスポーン（矩形内）
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

        // スポーン可能になったらスポーン実行
        if (canSpawn)
        {
            SpawnEnemies();
        }

        // 無効なゴミオブジェクトをリストから削除
        CleanupDestroyedEnemies();

        // デバッグ表示用の更新処理
        if (showSpawnRange)
        {
            DrawSpawnRanges();
        }
    }

    void SpawnEnemies()
    {
        // 画面上のゴミ数が最大未満でまだスポーン可能なゴミがいるか
        while (currentEnemiesOnScreen < maxEnemiesOnScreen && CanSpawnAnyTrash())
        {
            SpawnRandomTrash();
        }

        //// waveManager が null でない場合のみフラグを設定
        //if (waveManager != null)
        //{
        //    waveManager.AllTrashSpawned = true;
        //}
    }

    bool CanSpawnAnyTrash()
    {
        if (trashTypes == null || trashTypes.Length == 0) return false;

        // スポーン制限に達していないゴミタイプがいるかチェック
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

        // スポーン可能なゴミタイプのリスト作成
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

        // スポーン可能なゴミがいない場合終了
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

        // 有効なスポーン位置を探索
        Vector3 spawnPosition;
        if (!FindValidSpawnPosition(out spawnPosition))
        {
            return false;
        }

        // 2D用に Z を固定
        //spawnPosition.z = spawnZ;

        GameObject newTrash = Instantiate(trashData.TrashPrefab, spawnPosition, Quaternion.identity);

        // DisableComponent が無い環境のため、参照と処理は削除しました。
        // 必要ならゴミプレハブ側に「DisableAfterDelay」等のスクリプトを付けて制御してください。

        // ついてなければゴミにTrashタグを付ける
        if (newTrash != null && newTrash.tag != "Trash")
        {
            newTrash.tag = "Trash";
        }

        // スポーンしたゴミをリストに追加
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
            // 矩形領域内のランダム位置（均等分布）
            float rx = Random.Range(-halfW, halfW);
            float ry = Random.Range(-halfH, halfH);
            Vector3 candidatePosition = new Vector3(spawnerCenter.x + rx, spawnerCenter.y + ry/*, spawnZ*/) ;

            // プレイヤーからの距離チェック（XY 平面）
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(new Vector2(candidatePosition.x, candidatePosition.y),
                                                          new Vector2(player.position.x, player.position.y));

                // プレイヤーが矩形内にいるかチェック
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
                // プレイヤーがいない場合はそのままスポーン
                spawnPosition = candidatePosition;
                return true;
            }
        }

        return false;
    }

    // 破壊されたゴミオブジェクトをリストから削除
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

    // 外部からゴミが倒されたときに呼ばれる
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

            // スポーン待機開始
            canSpawn = false;
            spawnTimer = 0.0f;
        }
    }

    // 外部から特定のゴミを倒す
    public void DestroyTrash(GameObject trashObject)
    {
        if (spawnedEnemies.Contains(trashObject))
        {
            OnTrashDestroyed(trashObject);
            Destroy(trashObject);
        }
    }

    // 全てのゴミを倒す
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

    // スポーンしたゴミのリストを取得
    public List<GameObject> GetSpawnedEnemies()
    {
        // nullチェックして有効なゴミのみ返す
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

        // 矩形を描画（XY 平面）
        DrawRectangle(spawnerPos, spawnAreaWidth, spawnAreaHeight, spawnRangeColor);

        // プレイヤーがいる場合、プレイヤーからの最小距離円を描画
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