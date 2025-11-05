using System.Linq;
using UnityEditor.XR;
using UnityEngine;

public class EventSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EventState
    {
        [SerializeField] public GameObject events;
        public float percent;
        public bool appearance;
    }

    [SerializeField] EventState[] eventStates;
    RoundTimer timer;
    float seed;
    int rand;

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
        


        
    }
}
