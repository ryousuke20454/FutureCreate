using UnityEngine;
using UnityEngine.UI;

public class TextFade : MonoBehaviour
{
    [SerializeField] float waitTime;

    float countTime;
    float moveTime;
    Text text;
    public bool end;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<Text>();

        text.color = new Color(
                text.color.r,
                text.color.g,
                text.color.b,
                0.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!end)
        {
            countTime += Time.deltaTime;

            if (countTime > waitTime)
            {
                if (moveTime <= 3.14f)
                {
                    moveTime += Time.deltaTime;

                    text.color = new Color(
                        text.color.r,
                        text.color.g,
                        text.color.b,
                        Mathf.Sin(moveTime));
                }
                else if (moveTime > 3.14f)
                {
                    end = true;
                }
            }
        }
    }
}
