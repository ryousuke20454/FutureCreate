using UnityEngine;

public class PlayerImageMove : MonoBehaviour
{
    Vector3 firstPosition;
    [SerializeField] float time = 0;
    [SerializeField] float multi = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        firstPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time += Time.deltaTime * multi;

        transform.position =
            new Vector3(
                firstPosition.x,
                firstPosition.y + (1 - Mathf.Sin(time)) * 50.0f,
                firstPosition.z
            );
    }
}
