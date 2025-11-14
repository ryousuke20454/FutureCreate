using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameFadeOut : MonoBehaviour
{
    [SerializeField] private RectTransform fusumaLeft;
    [SerializeField] private RectTransform fusumaRight;
    [SerializeField] private float slideDuration = 1.0f; // 開く速度

    private Vector2 leftStartPos;
    private Vector2 rightStartPos;
    private Vector2 leftEndPos;
    private Vector2 rightEndPos;

    private void Start()
    {
        // 画面中央から始める（ぴったり閉まった状態）
        leftStartPos = fusumaLeft.anchoredPosition;
        rightStartPos = fusumaRight.anchoredPosition;

        // 終了位置（左右の外側へ移動）
        leftEndPos = new Vector2(-1920, leftStartPos.y);
        rightEndPos = new Vector2(1920, rightStartPos.y);

        // アニメーション開始
        StartCoroutine(OpenFusuma());
    }

    private IEnumerator OpenFusuma()
    {
        float time = 0f;

        yield return new WaitForSeconds(1f);

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / slideDuration);

            fusumaLeft.anchoredPosition = Vector2.Lerp(leftStartPos, leftEndPos, t);
            fusumaRight.anchoredPosition = Vector2.Lerp(rightStartPos, rightEndPos, t);

            yield return null;
        }

        // 開き終わったあと、不要なら非表示にしてもOK
        gameObject.transform.parent.GetComponent<FadeEventManager>().isFading = false;
        fusumaLeft.gameObject.SetActive(false);
        fusumaRight.gameObject.SetActive(false);
    }
}