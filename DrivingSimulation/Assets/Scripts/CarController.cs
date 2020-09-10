using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles all input variables for car
public class CarController : MonoBehaviour
{

    public float steeringWheel = 0f; //from x to y value
    public float gasPedal = 0f; //from x to y value
    public float brakePedal = 0f; //from x to y value

    public bool gearUp = false;
    public bool gearDown = false;
    public bool button3 = false;
    public bool button4 = false;
    public bool button5 = false;



    LogitechGSDK.LogiControllerPropertiesData properties;

    private void Start()
    {
        InitSteeringWheel();
    }


    //Initializes logitech steering wheel
    void InitSteeringWheel()
    {
        LogitechGSDK.LogiSteeringInitialize(false);
    }


    //Called when application is closed
    private void OnApplicationQuit()
    {
        LogitechGSDK.LogiSteeringShutdown();
    }


    private void Update()
    {
        ProcessInput();
    }


    void ProcessInput()
    {
        //Logitech Steering wheel car input


#if UNITY_EDITOR //Mouse and keyboard input

        float turningRate = 100f;

        if (Input.GetKeyDown(KeyCode.W))
        {
            gasPedal = 1f;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            gasPedal = 0f;
        }


        if (Input.GetKeyDown(KeyCode.S))
        {
            brakePedal = 1f;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            brakePedal = 0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (steeringWheel > -100)
                steeringWheel -= turningRate * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (steeringWheel < 100)
                steeringWheel += turningRate * Time.deltaTime;
        }


        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            steeringWheel = 0;
        }


#endif
    }
}
