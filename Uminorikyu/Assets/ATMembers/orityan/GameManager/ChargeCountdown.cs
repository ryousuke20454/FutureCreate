using UnityEngine;
using UnityEngine.UI;

public class ChargeCountdown: MonoBehaviour
{
    [SerializeField] float countTimeMax;
    [SerializeField] GameObject canvas;

    float count;
    Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        count += Time.deltaTime;

        int now = Mathf.FloorToInt(countTimeMax - count);
        text.text = now.ToString();

        if (now == -1)
        {
        }
    }
}
