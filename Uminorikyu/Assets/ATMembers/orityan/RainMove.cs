using UnityEngine;

public class RainMove : MonoBehaviour
{
    [SerializeField] float duration;
    GameObject camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = 
            new Vector3(camera.transform.position.x,
            camera.transform.position.y + duration,
            camera.transform.position.z);
    }
}
