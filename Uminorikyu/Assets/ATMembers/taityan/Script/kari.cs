using UnityEngine;

public class kari : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    [SerializeField] GameObject obj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            Instantiate(obj);
        }
    }
}
