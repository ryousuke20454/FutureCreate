using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public Transform player1;
    public Transform player2;

    [Header("カメラ挙動")]
    public float smoothSpeed = 5f;       // 移動のスムーズさ（大きいほど速い）
    public float minZoom = 5f;           // 近距離のズーム
    public float maxZoom = 15f;          // 遠距離のズーム
    public float zoomLimiter = 10f;      // ズームの反応度

    private Camera cam;
    private Vector3 targetPosition;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        MoveCamera();
        ZoomCamera();
    }

    void MoveCamera()
    {
        // 2人の中間点を計算
        Vector3 centerPoint = (player1.position + player2.position) / 2f;

        // Z座標は固定
        targetPosition = new Vector3(centerPoint.x, centerPoint.y, transform.position.z);

        // スムーズに追従（Lerpで補間）
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }

    void ZoomCamera()
    {
        // プレイヤー間の距離
        float distance = Vector2.Distance(player1.position, player2.position);

        // 距離に応じてズームを補間
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);

        // スムーズなズーム
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * smoothSpeed);
    }
}
