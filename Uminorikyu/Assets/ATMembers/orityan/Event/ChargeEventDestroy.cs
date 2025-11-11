using UnityEngine;

public class ChargeEventDestroy : MonoBehaviour
{
    public bool destroy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destroy = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (destroy)
        {
            OnDestroy();
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
