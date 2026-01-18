using UnityEngine;

public class WaveAppearanceEffect : MonoBehaviour
{
    [SerializeField]Vector2 firstPosition;
    RectTransform rectTransform;

    [SerializeField] float speed;
    [SerializeField] float multi;
    float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = firstPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;

        rectTransform.anchoredPosition =
            new Vector2(
                rectTransform.anchoredPosition.x + speed,
                100 + Mathf.Sin(time) * multi
                ) ;

    }
}
