using UnityEngine;

public class TestGameManager : MonoBehaviour
{
    [SerializeField] GameObject ui;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            Instantiate(ui);
        }
    }
}
