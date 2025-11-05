using System.Linq;
using System.Threading;
using UnityEditor.XR;
using UnityEngine;

public class EventSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EventState
    {
        public GameObject events;
        public float percent;
        public bool appearance;
    }

    [SerializeField] EventState[] eventStates;
    RoundTimer timer;
    int eventCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = GetComponent<RoundTimer>();

        for (int i = 0; i < eventStates.Count(); i++)
        {
            eventStates[i].appearance = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < eventStates.Count(); i++)
        {
            if (eventStates[i].appearance)
            {
                eventCount++;
            }
        }

        if (eventCount < 3)
        {
            if (timer.nowTime % 5 == 0)
            {
                for (int i = 0; i < eventStates.Count(); i++)
                {
                    if (!eventStates[i].appearance)
                    {
                        if (Random.Range(1, 100) < eventStates[i].percent)
                        {
                            Instantiate(eventStates[i].events);
                            eventStates[i].appearance = true;
                        }
                    }
                }
            }
        }
    }
}
