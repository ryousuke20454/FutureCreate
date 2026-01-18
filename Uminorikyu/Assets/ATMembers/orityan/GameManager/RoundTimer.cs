using KanKikuchi.AudioManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundTimer : MonoBehaviour
{
    [SerializeField] float[] timeLimits = new float[3];
    [SerializeField] GameObject fade;
    Text text;

    float timeLimit;
    float elapsedTime;
    public int nowTime;

    bool limit = false;
    public bool timeUp = false;
    bool soundUse1 = false;
    bool soundUse2 = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nowTime = 40;

        if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round1)
        {
            timeLimit = timeLimits[0];
        }
        else if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round2)
        {
            timeLimit = timeLimits[1];
        }
        else if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round3)
        {
            timeLimit = timeLimits[2];
        }

        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;
        nowTime = Mathf.FloorToInt(timeLimit - elapsedTime);


        if (nowTime == 10)
        {
            if (!soundUse1)
            {
                soundUse1 = true;
                SEManager.Instance.Play(SEPath.TIMER, 1, 0, 1, true, null);
            }
        }
        else if (nowTime == 5)
        {
            if (!soundUse2)
            {
                soundUse2 = true;
                SEManager.Instance.Stop();
                SEManager.Instance.Play(SEPath.FAST_TIMER, 1, 0, 1, true, null);
            }
        }

        if (nowTime <= 0)
        {
            nowTime = 0;

            if (!limit)
            {
                SEManager.Instance.Stop();
                SEManager.Instance.Play(SEPath.HOISSURU);

                if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round1)
                {
                    PlayerControllerManager.controllerManager.round.roundNum = Round.Round2;
                }
                else if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round2)
                {
                    PlayerControllerManager.controllerManager.round.roundNum = Round.Result;
                }
                else if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round3)
                {
                    PlayerControllerManager.controllerManager.round.roundNum = Round.Result;
                }

                Instantiate(fade);

                timeUp = true;
                limit = true;
            }
        }

        text.text = nowTime.ToString();
    }
}
