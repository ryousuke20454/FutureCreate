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

    bool limit;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        if (nowTime <= 0)
        {
            nowTime = 0;

            if (!limit)
            {
                if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round1)
                {
                    PlayerControllerManager.controllerManager.round.roundNum = Round.Round2;
                }
                else if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round2)
                {
                    PlayerControllerManager.controllerManager.round.roundNum = Round.Round3;
                }
                else if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round3)
                {
                    PlayerControllerManager.controllerManager.round.roundNum = Round.Result;
                }

                Instantiate(fade);

                limit = true;
            }
        }

        text.text = nowTime.ToString();
    }
}
