using UnityEngine;

public class TeleporterMove : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        velocity = new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.003f, 0.003f), 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        transform.position += velocity;

        if (transform.position.x > 15.0f)
        {
            transform.position = new Vector3(15.0f,
                transform.position.y,
                transform.position.z);
            velocity.x *= -1;
        }
        else if (transform.position.x < -15.0f)
        {
            transform.position = new Vector3(-15.0f,
            transform.position.y,
            transform.position.z);

            velocity.x *= -1;
        }

        if (transform.position.y > 10.0f)
        {
            transform.position = new Vector3(
                transform.position.x,
                10.0f,
                transform.position.z);
            velocity.y *= -1;
        }
        else if (transform.position.y < -10.0f)
        {
            transform.position = new Vector3(
                transform.position.x,
                -10.0f,
                transform.position.z);
            velocity.y *= -1;
        }
    }
}
