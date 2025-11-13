using UnityEngine;
using TMPro;

public class PlayerCollision : MonoBehaviour
{
    private CameraController cameraScript;

    [Header("エフェクト設定")]
    [SerializeField, Tooltip("発生させるエフェクト(パーティクル)")]
    private ParticleSystem particle;

    [Header("スコアポップアップ設定")]
    [SerializeField, Tooltip("スコアポップアップ用プレハブ(Resources内)")]
    private GameObject floatingScorePrefab;

    [SerializeField, Tooltip("World Space Canvas（UI表示先）")]
    private Canvas worldSpaceCanvas;

    void Start()
    {
        // メインカメラのスクリプトを取得
        cameraScript = Camera.main.GetComponent<CameraController>();

        // PrefabがInspectorで指定されていない場合、Resourcesから読み込む
        if (floatingScorePrefab == null)
        {
            floatingScorePrefab = Resources.Load<GameObject>("FloatingScoreText");
            if (floatingScorePrefab == null)
                Debug.LogError("❌ FloatingScoreText.prefab が Resources に見つかりません！");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ======== プレイヤー同士の衝突処理 ========
        if (collision.gameObject.CompareTag("Player"))
        {
            cameraScript.ShakeCamera();

            ParticleSystem newParticle = Instantiate(particle, transform.position, Quaternion.identity);
            newParticle.Play();
            Destroy(newParticle.gameObject, 2.0f);
        }

        // ======== ゴミとの衝突処理 ========
        else if (collision.gameObject.CompareTag("Trash"))
        {
            Vector3 hitPos = collision.transform.position;
            ShowFloatingText(hitPos, "+10");

            // パーティクル発生
            ParticleSystem newParticle = Instantiate(particle, hitPos, Quaternion.identity);
            newParticle.Play();
            Destroy(newParticle.gameObject, 2.0f);

            // ゴミを削除（任意）
            // Destroy(collision.gameObject);
        }
    }

    private void ShowFloatingText(Vector3 worldPos, string text)
    {
        if (floatingScorePrefab == null) return;

        // World Space Canvas の子として生成
        GameObject popup;
        if (worldSpaceCanvas != null)
        {
            popup = Instantiate(floatingScorePrefab, worldPos , Quaternion.identity, worldSpaceCanvas.transform);
        }
        else
        {
            popup = Instantiate(floatingScorePrefab, worldPos , Quaternion.identity);
        }

        // テキスト設定
        FloatingScoreText textComp = popup.GetComponent<FloatingScoreText>();
        if (textComp != null)
        {
            textComp.SetText(text);
        }

        // カメラの方向を向かせる（ビルボード効果）
        if (Camera.main != null)
        {
            popup.transform.forward = Camera.main.transform.forward;
        }

        Debug.Log($"★ FloatingScoreText 生成！ pos={worldPos }, alpha={popup.GetComponentInChildren<TextMeshProUGUI>().color.a}");
    }
}
