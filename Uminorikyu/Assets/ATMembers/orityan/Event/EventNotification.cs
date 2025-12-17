using UnityEngine;
using UnityEngine.UI;

public class EventNotification : MonoBehaviour
{
    [SerializeField] GameObject[] imageObj;
    [SerializeField] Canvas canvas;
    GameObject[] list;

    float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        list = new GameObject[5];
    }

    // Update is called once per frame

    public void IsNotification(int eventNum)
    {
        for (int i = 0; i < 5; i++)
        {
            if (list[i] == null)
            {
                list[i] = Instantiate(imageObj[eventNum],canvas.transform);
                list[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(600, i * -200 - 50);
                break;
            }
        }
    }
}
