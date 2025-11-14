using UnityEngine;
using UnityEngine.UI;

public class PlayerAndStaminaInfo : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject stamina;

    public void GetExplosionTrash()
    {
        stamina.GetComponent<Slider>().value = 0;
        player.GetComponent<OriiPlayerMove>().barnOut = true;
    }
}
