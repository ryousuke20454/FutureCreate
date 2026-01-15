using UnityEngine;
public class FallTrash : MonoBehaviour
{
    [SerializeField] private GameObject FallTrash_01;
    [SerializeField] private GameObject FallTrash_02;
    [SerializeField] private GameObject FallTrash_03;
    [SerializeField] private GameObject FallTrash_04;
    [Header("落下設定")]
    [SerializeField] private float spawnInterval = 0.1f; // 生成間隔(秒)
    [SerializeField] private int spawnCountPerInterval = 3; // 一度に生成する数
    [SerializeField] private float fallSpeedMin = 3f; // 落下速度(最小)
    [SerializeField] private float fallSpeedMax = 8f; // 落下速度(最大)
    [SerializeField] private float rotationSpeedMin = 50f; // 回転速度(最小)
    [SerializeField] private float rotationSpeedMax = 200f; // 回転速度(最大)
    [Header("時間制限")]
    [SerializeField] private float spawnDuration = 5f; // 生成する時間(秒)
    private float timer;
    private float elapsedTime;
    private Camera mainCamera;
    private bool isStarted = false; // 開始フラグ
    private GameObject[] trashPrefabs; // ゴミの配列

    void Start()
    {
        mainCamera = Camera.main;
        // 配列にまとめる
        trashPrefabs = new GameObject[] { FallTrash_01, FallTrash_02, FallTrash_03, FallTrash_04 };
    }
    void Update()
    {
        // エンターキーで開始
        if (!isStarted && Input.GetKeyDown(KeyCode.Return))
        {
            isStarted = true;
        }
        // 開始していなければ何もしない
        if (!isStarted) return;
        elapsedTime += Time.deltaTime;
        if (elapsedTime > spawnDuration) return;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            for (int i = 0; i < spawnCountPerInterval; i++)
            {
                SpawnTrash();
            }
            timer = 0f;
        }
    }
    void SpawnTrash()
    {
        // 4種類からランダムに選択
        GameObject trashPrefab = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

        if (trashPrefab == null) return;
        // カメラの上端の位置を取得
        float spawnY = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
        // カメラの左端と右端を取得
        float leftX = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float rightX = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        // ランダムな横位置(カメラの範囲内)
        Vector3 spawnPos = new Vector3(
            Random.Range(leftX, rightX),
            spawnY,
            0f
        );
        GameObject trash = Instantiate(trashPrefab, spawnPos, Quaternion.identity);

        // ランダムな落下速度と回転速度
        float randomFallSpeed = Random.Range(fallSpeedMin, fallSpeedMax);
        float randomRotSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);

        trash.AddComponent<FallingTrash>().Initialize(randomFallSpeed, randomRotSpeed, mainCamera);
    }
}
public class FallingTrash : MonoBehaviour
{
    private float speed;
    private float rotSpeed;
    private Camera mainCamera;
    private int rotationDirection; // 回転方向(1 or -1)

    public void Initialize(float fallSpeed, float rotationSpeed, Camera camera)
    {
        speed = fallSpeed;
        rotSpeed = rotationSpeed;
        mainCamera = camera;

        // ランダムに回転方向を決定(時計回り or 反時計回り)
        rotationDirection = Random.value > 0.5f ? 1 : -1;
    }
    void Update()
    {
        // 落下
        transform.position += Vector3.down * speed * Time.deltaTime;

        // Z軸で回転(回転方向と速度はランダム)
        transform.Rotate(0, 0, rotSpeed * rotationDirection * Time.deltaTime);

        // カメラの下端を超えたら削除
        float bottomY = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        if (transform.position.y < bottomY - 2f)
        {
            Destroy(gameObject);
        }
    }
}