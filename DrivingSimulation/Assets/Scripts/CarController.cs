using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


//Handles all input variables for car
public class CarController : MonoBehaviour
{
    private string actualState;
    private string activeForces;
    private string propertiesEdit;
    private string buttonStatus;
    private string forcesLabel;
    string[] activeForceAndEffect;


    public float gasPedal = 0f; //from x to y value
    public float brakePedal = 0f; //from x to y value
    public float steeringWheel = 0f; //from x to y value

    public bool gearUp = false;
    public bool gearUpReleased = true;
    public bool gearDown = false;
    public bool gearDownReleased = true;
    public bool startEngine = false;
    public bool handbrake = false;
    public bool blinkerLeft = false;
    public bool blinkerRight = false;
    public bool longLights = false;
    public bool lightsModeRight = false;
    public bool lightsModeLeft = false;
    public bool wiperUp = false;
    public bool wiperDown = false;


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

        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            StringBuilder deviceName = new StringBuilder(256);
            LogitechGSDK.LogiGetFriendlyProductName(0, deviceName, 256);
            propertiesEdit = "Current Controller : " + deviceName + "\n";
            propertiesEdit += "Current controller properties : \n\n";
            LogitechGSDK.LogiControllerPropertiesData actualProperties = new LogitechGSDK.LogiControllerPropertiesData();
            LogitechGSDK.LogiGetCurrentControllerProperties(0, ref actualProperties);



            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);

            float xaxis = (float)rec.lX;
            steeringWheel = xaxis / 32000.0f;
            gasPedal = Mathf.InverseLerp(32000, -32000, rec.lY);
            brakePedal = Mathf.InverseLerp(32000, -32000, rec.lRz);



            for (int i = 0; i < 128; i++)
            {
                switch (i)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                    case 10:
                        break;
                    case 11:
                        break;
                    case 12:
                        break;
                    case 13:
                        break;
                    case 14: //start engine
                        if (rec.rgbButtons[i] == 128)
                        {
                            startEngine = true;
                        }
                        break;
                }
            }
        }

#if UNITY_EDITOR //Mouse and keyboard input

        float turningRate = 1f;

        if (!LogitechGSDK.LogiIsConnected(0))
        { 
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
                if (steeringWheel > -1)
                    steeringWheel -= turningRate * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (steeringWheel < 1)
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

            if (Input.GetKeyDown(KeyCode.X))
            {
                longLights = true;
            }


            if (Input.GetKeyDown(KeyCode.Z))
            {
                lightsModeLeft = true;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                lightsModeRight = true;
            }
        }

#endif
    }
}
