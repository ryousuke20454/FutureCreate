using UnityEngine;
using UnityEngine.UI;

public class ImageMove : MonoBehaviour
{
    Image image;
    float time;
    int num;

    [SerializeField] float animationSpeed;
    [SerializeField] Sprite[] sprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        num = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > animationSpeed)
        {
            time = 0;
            num++;
            image.sprite = sprites[num % sprites.Length];
        }
    }
}
