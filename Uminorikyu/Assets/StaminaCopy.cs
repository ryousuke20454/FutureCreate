using UnityEngine;
using UnityEngine.UI;

public class StaminaCopy : MonoBehaviour
{
    [SerializeField] Slider slider;
    Slider mine;

    private void Start()
    {
        mine = GetComponent<Slider>();
    }

    public void Copy()
    {
        slider.value = mine.value;
        Debug.Log("ŒÄ‚Ño‚³‚ê‚Ä‚¢‚Ü‚·");
    }
}
