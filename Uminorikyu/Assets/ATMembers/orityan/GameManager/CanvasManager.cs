using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] Canvas[] canvas;

    private void Start()
    {
        canvas[0].gameObject.SetActive(true);
        canvas[1].gameObject.SetActive(false);
        canvas[2].gameObject.SetActive(false);
        canvas[3].gameObject.SetActive(false);
    }


    public void CanvasSwitch(int number,bool use)
    {
        canvas[number].gameObject.SetActive(use);
    }
}