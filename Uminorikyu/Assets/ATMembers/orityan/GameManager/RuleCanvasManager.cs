using UnityEngine;
using UnityEngine.UI;

public class RuleCanvasManager : MonoBehaviour
{
    [SerializeField] GameObject readyText;
    [SerializeField] GameObject goText;
    [SerializeField] Canvas mine;

    [SerializeField] GameObject manager;
    [SerializeField] GameObject[] players;
    [SerializeField] float waitTime;

    [SerializeField] GameObject[] stamina;
    float countTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (readyText.GetComponent<TextFade>().end)
        {
            if (countTime == 0)
            {
                Instantiate(goText, mine.transform);
            }

            countTime += Time.deltaTime;

            if (countTime > waitTime)
            {
                manager.GetComponent<CanvasManager>().CanvasSwitch(1, true);
                manager.GetComponent<CanvasManager>().CanvasSwitch(2, true);
                manager.GetComponent<CanvasManager>().CanvasSwitch(3, false);

                //プレイヤーのイベントフラグをfalseにする
                if (stamina[0].GetComponent<Slider>().value > 0)
                {
                    players[0].GetComponent<OriiPlayerMove>().barnOut = false;
                }
                if (stamina[1].GetComponent<Slider>().value > 0)
                {
                    players[1].GetComponent<OriiPlayerMove>().barnOut = false;
                }

                players[0].GetComponent<OriiPlayerMove>().nowEvent = false;
                players[1].GetComponent<OriiPlayerMove>().nowEvent = false;

            }
        }
    }
}
