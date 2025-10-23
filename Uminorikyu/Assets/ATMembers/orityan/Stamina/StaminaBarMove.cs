using UnityEngine;

public class StaminaBarMove : MonoBehaviour
{
    GameObject player;
    [SerializeField]float differenceY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            player.transform.position.x,
            player.transform.position.y + differenceY,
            player.transform.position.z);
    }
}
