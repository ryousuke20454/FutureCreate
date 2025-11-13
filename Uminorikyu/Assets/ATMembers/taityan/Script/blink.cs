using UnityEngine;
using UnityEngine.UI;

public class blink : MonoBehaviour
{
    public Text targetText;
    public float speed = 2.0f;

    void Update()
    {
        Color c = targetText.color;
        c.a = Mathf.Abs(Mathf.Sin(Time.time * speed)); // 0〜1を往復
        targetText.color = c;
    }
}
