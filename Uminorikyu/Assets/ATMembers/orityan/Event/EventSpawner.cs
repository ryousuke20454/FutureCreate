using System.Linq;
using UnityEngine;

public class EventSpawner : MonoBehaviour
{
    //オリジナルクラスの宣言
    [System.Serializable]
    public struct EventState
    {
        [SerializeField] public GameObject events;
        public GameObject target;
        public float percent;
        public bool appearance;
    }


    int weather = 1;
    [SerializeField] int eventMax;
    //インスペクターでイベントの種類ごとに設定する
    [SerializeField] EventState[] eventStates;
    [SerializeField] GameObject bannerCanvas;

    RoundTimer timer;//ラウンドの残り時間を取得する用
    bool flag;       //一秒間に何度も呼ばれない様にするための制御用
    int eventCount;  //イベント総数カウント用
    int firstTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weather = PlayerControllerManager.controllerManager.round.weatherNum;
        timer = GetComponent<RoundTimer>();
        flag = false;
        eventCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //対戦時間が20秒以上の時は波の抽選
        if (timer.nowTime >= 20)
        {
            if (timer.nowTime % 5 == 0)
            {
                if (!flag)
                {
                    flag = true;

                    if (Random.Range(1, 100) < eventStates[0].percent && !eventStates[0].appearance)
                    {
                        eventStates[0].appearance = true;
                        eventStates[0].target = Instantiate(eventStates[0].events);
                        eventCount++;
                        Debug.Log("抽選成功！");

                        //イベント通知の生成
                        bannerCanvas.GetComponent<EventNotification>().IsNotification(0);
                    }
                    else
                    {
                        Debug.Log("抽選失敗！");
                    }
                }
            }
            else
            {
                flag = false;
            }
        }
        else if (timer.nowTime < 20)
        {
            if (timer.nowTime % 5 == 0)
            {
                if (!flag)
                {
                    flag = true;


                    if (eventCount < eventMax + weather)
                    {
                        for (int i = 1; i < eventStates.Length; i++)
                        {
                            if (Random.Range(1, 100) < eventStates[i].percent && !eventStates[i].appearance)
                            {
                                eventStates[i].appearance = true;
                                eventStates[i].target = Instantiate(eventStates[i].events);
                                eventCount++;
                                Debug.Log("抽選成功！");

                                //イベント通知の生成
                                bannerCanvas.GetComponent<EventNotification>().IsNotification(i);
                            }
                            else
                            {
                                Debug.Log("抽選失敗！");
                            }
                        }
                    }
                }
            }
            else
            {
                flag = false;
            }
        }
    }
}
