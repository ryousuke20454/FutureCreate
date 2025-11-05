using UnityEngine;

public class TestDemoEvent : MonoBehaviour
{
    float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;

        if (time > 5.0f)
        {
            Destroy(gameObject);
        }
    }
}
