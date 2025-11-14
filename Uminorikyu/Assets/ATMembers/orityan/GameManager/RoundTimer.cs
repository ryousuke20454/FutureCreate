using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundTimer : MonoBehaviour
{
    [SerializeField] float[] timeLimits = new float[3];
    [SerializeField] string[] scenes = new string[3];
    Text text;

    float timeLimit;
    float elapsedTime;
    public int nowTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerControllerManager.controllerManager.round.roundNum == 1)
        {
            timeLimit = timeLimits[0];
        }
        else if (PlayerControllerManager.controllerManager.round.roundNum == 2)
        {
            timeLimit = timeLimits[1];
        }
        else if (PlayerControllerManager.controllerManager.round.roundNum == 3)
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

        text.text = nowTime.ToString();

        if (nowTime <= 0)
        {
            nowTime = 0;

            if (PlayerControllerManager.controllerManager.round.roundNum == 1)
            {
                PlayerControllerManager.controllerManager.round.roundNum = 2;
                SceneManager.LoadScene(scenes[0]);
            }
            else if (PlayerControllerManager.controllerManager.round.roundNum == 2)
            {
                PlayerControllerManager.controllerManager.round.roundNum = 3;
                SceneManager.LoadScene(scenes[1]);
            }
            else if (PlayerControllerManager.controllerManager.round.roundNum == 3)
            {
                SceneManager.LoadScene(scenes[2]);
            }
        }
    }
}
