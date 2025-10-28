using UnityEngine;
using UnityEngine.UI;

public class GetScore : MonoBehaviour
{
    [SerializeField] int playerNum;
    Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<Text>();
        text.text = PlayerControllerManager.controllerManager.player[playerNum].score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
