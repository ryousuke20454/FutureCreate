using UnityEngine;
using UnityEngine.UI;

public class ChargeCountdown: MonoBehaviour
{
    [SerializeField] GameObject[] players;
    [SerializeField] float countTimeMax;
    [SerializeField] GameObject manager;
    [SerializeField] Slider[] stamina;

    float count;
    Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;

        int now = Mathf.FloorToInt(countTimeMax - count);
        text.text = now.ToString();

        if (now == -1)
        {
            manager.GetComponent<CanvasManager>().CanvasSwitch(1, true);
            manager.GetComponent<CanvasManager>().CanvasSwitch(2, true);
            stamina[0].GetComponent<StaminaCopy>().Copy();
            stamina[1].GetComponent<StaminaCopy>().Copy();
            players[0].GetComponent<OriiPlayerMove>().nowEvent = false;
            players[1].GetComponent<OriiPlayerMove>().nowEvent = false;
            manager.GetComponent<CanvasManager>().CanvasSwitch(0, false);
        }
    }
}
