using UnityEngine;

public class TrashStatus : MonoBehaviour
{
    [SerializeField] float size;
    [SerializeField] public int score;
    [SerializeField] public float glowAmount;

    Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(size, size, 0.0f);
        transform.localRotation = new Quaternion(0.0f, Random.Range(-0.2f, 0.2f), 0.0f, 0.0f);
        velocity = new Vector3(Random.Range(-0.003f, 0.003f), Random.Range(-0.003f, 0.003f),0.0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += velocity;
    }
}
