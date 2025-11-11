using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public CanvasManager canvasManager;
    [SerializeField] Canvas[] canvas;


    void Start()
    {
        canvasManager = this;
    }


    public void CanvasSwitch(int number,bool use)
    {
        canvas[number].enabled = use;
    }
}
