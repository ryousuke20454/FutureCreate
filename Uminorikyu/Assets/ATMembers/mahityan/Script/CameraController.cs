using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public Transform player1;
    public Transform player2;

    [Header("カメラ挙動")]
    public float smoothSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float zoomLimiter = 10f;

    [Header("カメラシェイク設定")]
    public float shakeDuration = 0.2f;   // 振動する時間
    public float shakeMagnitude = 0.3f;  // 振動の強さ

    private Camera cam;
    private Vector3 targetPosition;
    private bool isShaking = false;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null || isShaking) return;

        MoveCamera();
        ZoomCamera();
    }

    void MoveCamera()
    {
        Vector3 centerPoint = (player1.position + player2.position) / 2f;
        targetPosition = new Vector3(centerPoint.x, centerPoint.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }

    void ZoomCamera()
    {
        float distance = Vector2.Distance(player1.position, player2.position);
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * smoothSpeed);
    }

    // 🔥 外部（プレイヤーなど）から呼ぶ用
    public void ShakeCamera()
    {
        if (!isShaking)
            StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        Vector3 originalPos = transform.position;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPos.x + offsetX, originalPos.y + offsetY, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        isShaking = false;
    }
}
