using System.Collections;
using UnityEngine;

public class EventBannerMove : MonoBehaviour
{
    [SerializeField] float waitTime;

    RectTransform rect;
    float time = 0;
    bool freeze;
    float freezeTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (time <= 3.14f / 2)
        {
            time += Time.deltaTime;
            rect.anchoredPosition = new Vector2(590.0f - Mathf.Sin(time) * 600.0f, rect.anchoredPosition.y);
        }
        else
        {
            freeze = true;
        }

        if (freeze)
        {
            freezeTime += Time.deltaTime;
        }

        if (freezeTime > waitTime && time <= 3.14f)
        {
            time += Time.deltaTime;
            rect.anchoredPosition = new Vector2(590.0f - Mathf.Sin(time) * 600.0f,rect.anchoredPosition.y);
        }
        else if (time > 3.14f)
        {
            Destroy(gameObject);
        }

    }
}
