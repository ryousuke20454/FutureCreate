using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TitleFadeIn : MonoBehaviour
{
    [SerializeField] private RectTransform fusumaLeft;
    [SerializeField] private RectTransform fusumaRight;
    [SerializeField] private float slideDuration = 1.0f; // ふすまが閉まる速度
    [SerializeField] private string nextSceneName = "SampleGameScene";

    private Vector2 leftStartPos;
    private Vector2 rightStartPos;
    private Vector2 leftEndPos;
    private Vector2 rightEndPos;

    private void Start()
    {
        // 初期位置（画面外）
        leftStartPos = fusumaLeft.anchoredPosition;
        rightStartPos = fusumaRight.anchoredPosition;

        // 終了位置（中央でぴったり閉まる位置）
        leftEndPos = new Vector2(0-50, leftStartPos.y);
        rightEndPos = new Vector2(0+50, rightStartPos.y);

        // アニメーション開始
        StartCoroutine(CloseFusumaAndLoadScene());
    }

    private IEnumerator CloseFusumaAndLoadScene()
    {
        float time = 0f;

        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / slideDuration);

            fusumaLeft.anchoredPosition = Vector2.Lerp(leftStartPos, leftEndPos, t);
            fusumaRight.anchoredPosition = Vector2.Lerp(rightStartPos, rightEndPos, t);

            yield return null;
        }

        // 閉まりきってから1秒待つ
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(nextSceneName);
    }
}