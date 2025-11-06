using UnityEngine;
using TMPro;

public class FloatingScoreText : MonoBehaviour
{
    public float moveSpeed = 1f;      // 上に動く速度
    public float fadeSpeed = 1f;      // フェードアウト速度
    private TMP_Text text;
    private Color originalColor;

    void Awake()
    {
        // Instantiate直後でも確実に色が取れる
        text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            originalColor = text.color;
        }
    }

    void Update()
    {
        if (text == null) return;

        // 上方向に移動
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // フェードアウト
        Color c = text.color;
        c.a -= fadeSpeed * Time.deltaTime;
        text.color = c;

        // 消えたら破棄
        if (text.color.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string value)
    {
        if (!text) text = GetComponentInChildren<TextMeshProUGUI>();
        if (text == null) return;

        text.text = value;

        // 強制的に不透明に戻す
        Color c = text.color;
        c.a = 1f;
        text.color = c;
    }
}
