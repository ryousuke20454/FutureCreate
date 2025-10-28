using UnityEngine;

public class StaminaBarMove : MonoBehaviour
{
    [SerializeField]GameObject player;
    [SerializeField]GameObject spiral;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            player.transform.position.x,
            player.transform.position.y + spiral.transform.localScale.y,
            player.transform.position.z);
    }
}
