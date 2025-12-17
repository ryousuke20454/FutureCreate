using UnityEngine;
using UnityEngine.InputSystem;

public class ChargeShake : MonoBehaviour
{
    GameObject mainCamera;
    PlayerInputScript input;
    Gamepad gamepad;
    Vector2 beforeLeverPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera"); 
        input = GetComponent<PlayerInputScript>();
        gamepad = input.controller;
    }

    // Update is called once per frame
    void Update()
    {
        if (input.GetStickValue(gamepad) != beforeLeverPosition)
        {
            mainCamera.GetComponent<CameraController>().ShakeCamera();
        }

        beforeLeverPosition = input.GetStickValue(gamepad);
    }
}
