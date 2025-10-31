using UnityEngine;

public class TrashStatus : MonoBehaviour
{
    [SerializeField] float size;
    [SerializeField] public int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(size, size, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
