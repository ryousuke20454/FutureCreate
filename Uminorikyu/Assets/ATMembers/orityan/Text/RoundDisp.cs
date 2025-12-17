using UnityEngine;
using UnityEngine.UI;

public class RoundDisp : MonoBehaviour
{
    [SerializeField] GameObject textObj;
    //ラウンド用かジャッジ
    [SerializeField] bool round;
    public bool dispEnd = false;

    Text text;
    float velocity = 0.02f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<Text>();

        if (round)
        {
            if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round1)
                text.text = "Round 1";
            if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round2)
                text.text = "Round 2";
            if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round3)
                text.text = "Round 3";
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (textObj == null || textObj.GetComponent<RoundDisp>().dispEnd)
        {
            text.color = new Color(
                text.color.r,
                 text.color.b,
                  text.color.g,
                   text.color.a + velocity);

            if (text.color.a > 1.0f)
            {
                velocity *= -1f;
            }

            if (text.color.a < 0.0f)
            {
                text.color = new Color(
                text.color.r,
                 text.color.b,
                  text.color.g,
                   0.0f);
                dispEnd = true;
            }
        }
    }
}
