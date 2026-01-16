using UnityEngine;

// プレハブを画面右端に生成するスクリプト
public class PrefabSpawner : MonoBehaviour
{
    [Header("プレハブ設定")]
    [SerializeField] private GameObject prefab; // Inspectorでプレハブを設定

    [Header("出現位置設定")]
    [SerializeField] private float spawnYPosition = 0.2f; // 出現位置（0=下端, 0.5=中央, 1=上端）

    // この関数を呼ぶとプレハブが画面右端に生成される
    public void SpawnPrefabAtRightEdge()
    {
        if (prefab == null)
        {
            Debug.LogWarning("プレハブが設定されていません");
            return;
        }

        // カメラの画面右端のワールド座標を取得（2D用）
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("メインカメラが見つかりません");
            return;
        }

        // 画面右端の座標を計算（2D用: Z座標は0）
        Vector3 rightEdgePos = cam.ViewportToWorldPoint(new Vector3(1f, spawnYPosition, 0f));
        rightEdgePos.z = 0f; // 2Dなので確実にZ=0にする

        // プレハブを生成（Z軸を90度回転）
        Quaternion rotation = Quaternion.Euler(0f, 0f, 90f);
        GameObject spawnedObj = Instantiate(prefab, rightEdgePos, rotation);

        // WaveMoverコンポーネントを追加（プレハブに付いていない場合）
        if (spawnedObj.GetComponent<WaveMover>() == null)
        {
            spawnedObj.AddComponent<WaveMover>();
        }

        Debug.Log($"プレハブを生成: {rightEdgePos}");
    }

    // テスト用: スペースキーで生成
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPrefabAtRightEdge();
        }
    }
}

// 生成されたオブジェクトを左に波打つように動かすスクリプト
public class WaveMover : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 10f; // 左への移動速度

    [Header("波の設定")]
    [SerializeField] private float waveAmplitude = 2f; // 波の振幅（上下の幅）
    [SerializeField] private float waveFrequency = 2f; // 波の周波数（波の速さ）

    private float startY; // 開始時のY座標
    private float time; // 経過時間

    void Start()
    {
        startY = transform.position.y;
        time = 0f;
    }

    void Update()
    {
        // 経過時間を更新
        time += Time.deltaTime;

        // 現在の位置を取得
        Vector3 pos = transform.position;

        // 左に移動
        pos.x -= moveSpeed * Time.deltaTime;

        // 波打つように上下移動（サイン波）
        pos.y = startY + Mathf.Sin(time * waveFrequency) * waveAmplitude;

        // 位置を更新
        transform.position = pos;

        // 画面左端を超えたら削除
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 leftEdge = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f));
            if (pos.x < leftEdge.x - 2f) // 画面左端より2ユニット左で削除
            {
                Destroy(gameObject);
            }
        }
    }
}