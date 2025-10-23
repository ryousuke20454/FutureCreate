using UnityEditor.SceneManagement;
using UnityEngine;

public class HomingPlayer : MonoBehaviour
{
    Transform playerTr;
    [SerializeField] float speed = 2f;

    [Header("サイズ設定")]
    [SerializeField] float growAmount = 0.05f; // 拡大量
    [SerializeField] float maxScale = 1f;     // 最大スケール
    private Vector3 baseScale;                 // 初期スケールを保持

    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        


        // インスペクターで設定した初期スケールを保存
        baseScale = transform.localScale;
    }

    //プレイヤー追従
    private void Update()
    {
        if (Vector2.Distance(transform.position, playerTr.position) < 0.1f)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            playerTr.position,
            speed * Time.deltaTime);
    }

    //ゴミとの当たり判定、サイズ変更
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trash"))
        {
            Debug.Log("Trash detected!");

            // 渦のスケール(自分のスケール)
            float vortexScale = transform.localScale.x;

            // ゴミのスケール
            float trashScale = collision.transform.localScale.x;

            Debug.Log($"渦スケール: {vortexScale}, ゴミスケール: {trashScale}");

            // 渦の方が大きい場合のみゴミを消す
            if (vortexScale >= trashScale)
            {
                Debug.Log("ゴミを吸い込みました！");
                Destroy(collision.gameObject);

                // 現在スケールを基準に拡大
                float newScale = transform.localScale.x + growAmount;
                newScale = Mathf.Min(newScale, maxScale); // 最大値制限
                transform.localScale = new Vector3(newScale, newScale, 1f);
            }
            else
            {
                Debug.Log("ゴミが大きすぎて吸い込めません！");
            }
        }
    }
}
