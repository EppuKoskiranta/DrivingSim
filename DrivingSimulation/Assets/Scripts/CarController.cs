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
    public bool startEngine = false;
    public bool handbrake = false;
    public bool blinkerLeft = false;
    public bool blinkerRight = false;



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



        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            gearUp = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            gearDown = true;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            startEngine = true;
        }



        if (Input.GetKeyDown(KeyCode.Q))
        {
            blinkerLeft = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            blinkerRight = true;
        }

#endif
    }
}
