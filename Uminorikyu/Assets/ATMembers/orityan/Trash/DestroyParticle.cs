using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    float time = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime;

        if (time >= 120)
        {
            Destroy(gameObject);
        }
    }
}
