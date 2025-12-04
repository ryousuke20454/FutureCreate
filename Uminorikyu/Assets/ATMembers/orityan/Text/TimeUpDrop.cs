using UnityEngine;

public class TimeUpDrop : MonoBehaviour
{
    RectTransform rt;
    [SerializeField] GameObject fade;
    static bool end = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        end = false;
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = new Vector2(
            rt.anchoredPosition.x,
            rt.anchoredPosition.y - 1.0f);

        if (rt.anchoredPosition.y < 0.0f)
        {
            rt.anchoredPosition = new Vector2(
                rt.anchoredPosition.x,
                0.0f);

            if (!end ) 
            {
                Instantiate(fade);
                end = true;
            }
        }
    }
}
