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

    [SerializeField] int eventMax;
    //インスペクターでイベントの種類ごとに設定する
    [SerializeField] EventState[] eventStates;


    RoundTimer timer;//ラウンドの残り時間を取得する用
    bool flag;       //一秒間に何度も呼ばれない様にするための制御用
    int eventCount;  //イベント総数カウント用

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = GetComponent<RoundTimer>();
        flag = false;
        eventCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.nowTime % 5 == 0)
        {
            if (!flag)
            {
                flag = true;

                for (int i = 0; i < eventStates.Length; i++)
                {
                    if (eventCount == eventMax)
                        break;

                    if (Random.Range(1, 100) < eventStates[i].percent && !eventStates[i].appearance)
                    {
                        eventStates[i].appearance = true;
                        eventStates[i].target = Instantiate(eventStates[i].events);
                        eventCount++;
                        Debug.Log("抽選成功！");
                    }
                    else
                    {
                        Debug.Log("抽選失敗！");
                    }
                }
            }
        }
        else
        {
            flag = false;
        }

        if (eventCount > 0)
        {
            for (int i = 0; i < eventStates.Length; i++)
            {
                if (eventStates[i].appearance && eventStates[i].target == null)
                {
                    eventCount--;
                    eventStates[i].appearance = false;
                    Debug.Log($"{i + 1}番目のイベントを消したよ！");
                }
            }
        }
    }
}
