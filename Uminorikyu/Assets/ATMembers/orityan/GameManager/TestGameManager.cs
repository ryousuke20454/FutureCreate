using UnityEngine;

public class TestGameManager : MonoBehaviour
{
    [SerializeField] GameObject[] seas;
    [SerializeField] ParticleSystem[] particles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int weather = PlayerControllerManager.controllerManager.round.weatherNum;

        Instantiate(seas[weather - 1]);

        if (weather == 2)
        {
            Instantiate(particles[0]);
        }
        else if (weather == 3)
        {
            Instantiate(particles[1]);
        }
    }
}
