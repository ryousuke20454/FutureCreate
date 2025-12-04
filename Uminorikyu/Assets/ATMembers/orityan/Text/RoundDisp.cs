using UnityEngine;
using UnityEngine.UI;

public class RoundDisp : MonoBehaviour
{
    public bool dispEnd = false;

    string round;
    Text text;
    float velocity = 0.01f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round1)
            round = "1";
        if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round2)
            round = "2";
        if (PlayerControllerManager.controllerManager.round.roundNum == Round.Round3)
            round = "3";

        text = GetComponent<Text>();
        text.text = "Round " + round;
    }

    // Update is called once per frame
    void FixedUpdate()
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
