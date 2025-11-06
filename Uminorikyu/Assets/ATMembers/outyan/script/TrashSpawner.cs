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

    [Header("プレイヤー設定 (複数対応)")]
    [Tooltip("複数のプレイヤーを扱います。ランタイムで Player タグのオブジェクトを検索します。")]
    private Transform[] players;
    [SerializeField] private float minDistanceFromPlayer = 0.8f; // プレイヤーからの最小距離（矩形外の場合）
    [SerializeField] private float minSpawnRadius = 1.5f; // プレイヤーがスポーン領域内にいるときの最小距離

    [Header("短形スポーン設定")]
    [Tooltip("矩形の幅（X方向, world units）")]
    [SerializeField] private float spawnAreaWidth = 17f;
    [Tooltip("矩形の高さ（Y方向, world units）")]
    [SerializeField] private float spawnAreaHeight = 9.5f;

    [Header("スポーン位置設定")]
    [SerializeField] private int maxSpawnAttempts = 100; // スポーン位置探索の最大試行回数

    [Header("デバッグ表示設定")]
    [SerializeField] private bool showSpawnRange = true; // スポーン範囲を表示
    private Color spawnRangeColor = Color.yellow; // スポーン矩形の色
    private Color playerRangeColor = Color.blue; // プレイヤーからの最小距離表示色（矩形外）
    private Color playerInsideRangeColor = Color.magenta; // プレイヤーが矩形内にいる時の最小距離表示色

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

    // 複数プレイヤー対応プロパティ
    public Transform[] Players => players;

    // 互換性のため、"PrimaryPlayer" を用意（存在しないときは null）
    public Transform PrimaryPlayer => (players != null && players.Length > 0) ? players[0] : null;

    public float MinDistanceFromPlayer => minDistanceFromPlayer;
    public float MinSpawnRadius => minSpawnRadius;
    public float SpawnAreaWidth => spawnAreaWidth;
    public float SpawnAreaHeight => spawnAreaHeight;
    public int MaxSpawnAttempts => maxSpawnAttempts;
    public int CurrentEnemiesOnScreen => currentEnemiesOnScreen;

    private int currentEnemiesOnScreen = 0; // 現在画面上にいるゴミ
    private float spawnTimer = 0.0f; // スポーンタイマー
    private bool canSpawn = true; // スポーン可能フラグ

    // スポーンしたゴミを管理するリスト
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Start()
    {
        // 複数プレイヤーを自動検索（Player タグ）
        RefreshPlayers();

        // Wave開始 - 最初からランダムでスポーン（矩形内）
        SpawnEnemies();
    }

    void FixedUpdate()
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

        // 2D用に Z を固定（プレハブ側で対応）
        GameObject newTrash = Instantiate(trashData.TrashPrefab, spawnPosition, Quaternion.identity);

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
            Vector3 candidatePosition = new Vector3(spawnerCenter.x + rx, spawnerCenter.y + ry /*, spawnZ*/);

            // プレイヤー群の中で最も近いプレイヤーを取得
            Transform nearestPlayer = GetNearestPlayer(candidatePosition, out float distanceToNearest);

            if (nearestPlayer != null)
            {
                // そのプレイヤーが矩形内にいるかチェック（スパナー矩形と比較）
                bool playerInsideRect = Mathf.Abs(nearestPlayer.position.x - spawnerCenter.x) <= halfW &&
                                        Mathf.Abs(nearestPlayer.position.y - spawnerCenter.y) <= halfH;

                if (playerInsideRect)
                {
                    // プレイヤーが矩形内にいる場合は minSpawnRadius を利用
                    if (distanceToNearest >= minSpawnRadius)
                    {
                        spawnPosition = candidatePosition;
                        return true;
                    }
                }
                else
                {
                    // プレイヤーが矩形外にいる場合は minDistanceFromPlayer を利用
                    if (distanceToNearest >= minDistanceFromPlayer)
                    {
                        spawnPosition = candidatePosition;
                        return true;
                    }
                }
            }
            else
            {
                // プレイヤーが存在しない場合はそのままスポーン
                spawnPosition = candidatePosition;
                return true;
            }
        }

        return false;
    }

    // 最も近いプレイヤーを返す。プレイヤーが存在しない場合は null を返す
    Transform GetNearestPlayer(Vector3 point, out float nearestDistance)
    {
        nearestDistance = float.MaxValue;
        Transform nearest = null;

        if (players == null || players.Length == 0) return null;

        for (int i = 0; i < players.Length; i++)
        {
            Transform p = players[i];
            if (p == null) continue;

            float d = Vector2.Distance(new Vector2(point.x, point.y),
                                       new Vector2(p.position.x, p.position.y));
            if (d < nearestDistance)
            {
                nearestDistance = d;
                nearest = p;
            }
        }

        return nearest;
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

    // Player タグの付いたオブジェクトを再検索して players 配列を更新する
    public void RefreshPlayers()
    {
        GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");
        if (playerObjs != null && playerObjs.Length > 0)
        {
            players = new Transform[playerObjs.Length];
            for (int i = 0; i < playerObjs.Length; i++)
            {
                players[i] = playerObjs[i].transform;
            }
        }
        else
        {
            players = new Transform[0];
        }
    }

    void DrawSpawnRanges()
    {
        Vector3 spawnerPos = transform.position;
        //spawnerPos.z = spawnZ;

        // 矩形を描画（XY 平面）
        DrawRectangle(spawnerPos, spawnAreaWidth, spawnAreaHeight, spawnRangeColor);

        // 各プレイヤーについて円を描画（矩形内か外かで色を変える）
        if (players != null && players.Length > 0)
        {
            float halfW = spawnAreaWidth * 0.5f;
            float halfH = spawnAreaHeight * 0.5f;
            Vector3 spawnerCenter = transform.position;

            foreach (Transform p in players)
            {
                if (p == null) continue;

                bool playerInsideRect = Mathf.Abs(p.position.x - spawnerCenter.x) <= halfW &&
                                        Mathf.Abs(p.position.y - spawnerCenter.y) <= halfH;

                Vector3 playerPos = new Vector3(p.position.x, p.position.y /*, spawnZ*/);

                if (playerInsideRect)
                {
                    // プレイヤーが矩形内にいる場合は minSpawnRadius を表示（強調色）
                    DrawCircle(playerPos, minSpawnRadius, playerInsideRangeColor);
                }
                else
                {
                    // 矩形外のプレイヤーには minDistanceFromPlayer を表示
                    DrawCircle(playerPos, minDistanceFromPlayer, playerRangeColor);
                }
            }
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