using UnityEngine;
using UnityEngine.UI;

public class StaminaCopy : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] GameObject player;
    Slider mine;

    private void Start()
    {
        mine = GetComponent<Slider>();
    }

    public void Copy()
    {
        if (mine.value > 0)
        {
            player.GetComponent<OriiPlayerMove>().barnOut = false;
        }
        slider.value = mine.value;
    }
}
