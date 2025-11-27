using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using KanKikuchi.AudioManager;

public class ResultWinnerEffect : MonoBehaviour
{
    [SerializeField] private RectTransform player1Image;
    [SerializeField] private RectTransform player2Image;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private TMP_Text pressXText;   // ← 追加（「×を押してタイトルへ」）
    [SerializeField] private ParticleSystem winParticlePrefab;
    [SerializeField] private GameObject fade;

    private int player1Score;
    private int player2Score;
    private bool use;

    void Start()
    {
        player1Score = PlayerControllerManager.controllerManager.player[0].score;
        player2Score = PlayerControllerManager.controllerManager.player[1].score;

        if (pressXText != null)
            pressXText.gameObject.SetActive(false); // 最初は非表示
    }

    private void Update()
    {
        if (!fade.GetComponent<FadeEventManager>().isFading)
        {
            if (!use)
            {
                use = true;
                StartCoroutine(ShowResult());
            }
        }
    }

    IEnumerator ShowResult()
    {
        yield return new WaitForSeconds(1.5f);

        bool isDraw = (player1Score == player2Score);
        RectTransform winner = null;
        string winnerName = "";

        if (isDraw)
        {
            winnerName = "DRAW!";
        }
        else
        {
            winner = (player1Score > player2Score) ? player1Image : player2Image;
            winnerName = (player1Score > player2Score) ? "P1 WIN!" : "P2 WIN!";
        }

        if (isDraw)
        {
            winText.text = winnerName;
            winText.gameObject.SetActive(true);

            // ★ DRAW時も “× を押して…” を点滅開始
            StartCoroutine(BlinkToTitleText());
            yield break;
        }

        // 勝者移動＆拡大
        Vector3 startPos = winner.anchoredPosition;
        Vector3 targetPos = Vector3.zero;
        Vector3 startScale = winner.localScale;
        Vector3 targetScale = startScale * 1.5f;

        float duration = 1.2f;
        float time = 0f;

        

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            winner.anchoredPosition = Vector3.Lerp(startPos, targetPos, t);
            winner.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        SEManager.Instance.Play(SEPath.DONPAF);

        yield return new WaitForSeconds(0.5f);

        // パーティクル
        if (winParticlePrefab != null)
        {
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, winner.position);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));

            ParticleSystem ps = Instantiate(winParticlePrefab, worldPos, Quaternion.identity);
            ps.Play();
        }

        winText.text = winnerName;
        winText.gameObject.SetActive(true);

        // ★ 最後に “× を押してタイトルへ” を点滅表示
        StartCoroutine(BlinkToTitleText());
    }

    // --------------------------------------------------------------
    // ▼ テキストを点滅（永続）
    // --------------------------------------------------------------
    IEnumerator BlinkToTitleText()
    {
        if (pressXText == null) yield break;

        pressXText.gameObject.SetActive(true);

        while (true)
        {
            pressXText.alpha = 1f;
            yield return new WaitForSeconds(0.6f);

            pressXText.alpha = 0f;
            yield return new WaitForSeconds(0.6f);
        }
    }
}
