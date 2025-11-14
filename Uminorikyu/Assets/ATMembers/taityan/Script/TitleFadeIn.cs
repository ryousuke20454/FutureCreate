using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.Experimental.GraphView;

public class TitleFadeIn : MonoBehaviour
{
    [SerializeField] private RectTransform fusumaLeft;
    [SerializeField] private RectTransform fusumaRight;
    [SerializeField] private float slideDuration = 1.0f; // ふすまが閉まる速度
    [SerializeField] private string[] nextSceneName;

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
        leftEndPos = new Vector2(-960 / 2, leftStartPos.y);
        rightEndPos = new Vector2(960 / 2, rightStartPos.y);

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

        if (PlayerControllerManager.controllerManager != null)
        {
            switch (PlayerControllerManager.controllerManager.round.roundNum)
            {
                case Round.Title:
                    SceneManager.LoadScene(nextSceneName[0]);
                    break;
                case Round.Round1:
                    SceneManager.LoadScene(nextSceneName[1]);
                    break;
                case Round.Round2:
                    SceneManager.LoadScene(nextSceneName[1]);
                    break;
                case Round.Round3:
                    SceneManager.LoadScene(nextSceneName[1]);
                    break;
                case Round.Result:
                    SceneManager.LoadScene(nextSceneName[2]);
                    break;
            }
        }
        else
        {
            SceneManager.LoadScene("SampleGameScene");
        }
    }
}


