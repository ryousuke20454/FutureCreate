using UnityEngine;

public class StaminaState : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] StaminaUse use;
    [SerializeField] StaminaHeal heal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<OriiPlayerMove>().barnOut)
        {
            use.enabled = false;
            heal.enabled = true;
        }
        else
        {
            use.enabled = true;
            heal.enabled = false;
        }
    }
}
