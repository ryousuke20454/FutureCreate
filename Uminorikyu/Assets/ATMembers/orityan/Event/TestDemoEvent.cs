using UnityEngine;

public class TestDemoEvent : MonoBehaviour
{
    [SerializeField] GameObject image;
    [SerializeField] float destroyTime;
    float time;
    float elapsedTime;
    Canvas canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = 0;
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;
        elapsedTime += Time.deltaTime;

        if (time > destroyTime)
        {
            Destroy(gameObject);
        }

        if (time < 3.0f && elapsedTime > 0.25f)
        {
            Instantiate(image,canvas.transform);

            elapsedTime = 0;
        }
    }
}
