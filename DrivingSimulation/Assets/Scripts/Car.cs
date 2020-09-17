using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Car : MonoBehaviour
{
    //Light stuff
    enum LightMode
    {
        DEFAULT,
        ON,
        MIST,

    }

    enum Lights
    {
        ON = 0x01,
        LONG = 0x02,
        LEFTBLINKER = 0x03,
        RIGHTBLINKER = 0x04,
    }

    LightMode lightMode;
    uint lights;

    //Wipers
    enum Wipers
    {
        OFF,
        ON,
        ONCE,
        FAST,
        FASTER,
        FASTEST,
    }

    Wipers wipers;

    //Wheel stuff
    public enum CarAxis
    {
        FRONT = 0,
        REAR,
    }


    /// <summary>
    /// Is car Front-wheel drive, Rear-wheel drive or All-wheel drive
    /// </summary>
    public enum WheelDrive
    {
        FRONTWHEEL = 0,
        REARWHEEL,
        ALLWHEEL,
    }


    public struct Wheel
    {
        public GameObject model;
        public WheelCollider wheel;
        public CarAxis axis;
    }

    public WheelDrive wheelMode = WheelDrive.FRONTWHEEL;

    [SerializeField]
    public Wheel[] wheels = new Wheel[4];


    //Automatic or Manual
    public enum GearSystem
    {
        AUTOMATIC,
        MANUAL,
    }

    GearSystem gearSystem = GearSystem.AUTOMATIC;

    //Motor stuff
    bool automatic = true;
    bool handbreak = false;
    float fuel = 70.0f;
    float roundsPerMinute = 0f;
    float rpmReduction = 200f;
    float speed = 0f; //    km/h
    float horsePower = 200f;
    float motorTorque = 0f;

    int activeGear = 0;
    int maxGear = 5;
    float[] gear = { 3.166f, 1.882f, 1.296f, 0.972f, 0.738f, 0.600f };
    
    float tyreCircumference = 1.99271f;
    float differentialRatio = 4.1f;

    public float brakeTorque = 1620f;

    float maxWheelAngle = 35f; //in euler angles


    //Air resistance
    float Cd = 0.35f; //coefficient
    float volume = 3.11f; //volume
    float density = 0f;
    float dragArea = 1f; //squaremeters (estimate)
     

    //References
    public CarController controller;

    //Extras
    //like radio feat. skirmish beats




    void Start()
    {
        CarInit();
    }


    void CarInit()
    {
        //Setup controller
        if (!controller)
            controller = this.gameObject.GetComponent<CarController>();



        //Setup wheels
        SetupWheels();


        density = this.GetComponent<Rigidbody>().mass / volume;
    }


    void SetupWheels()
    {
        wheels[0] = new Wheel(); //FrontLeft
        wheels[0].axis = CarAxis.FRONT;
        wheels[0].model = this.transform.GetChild(3).GetChild(18).gameObject;
        wheels[0].wheel = this.transform.GetChild(3).GetChild(25).GetChild(0).GetComponent<WheelCollider>();
        wheels[1] = new Wheel(); //FrontRight
        wheels[1].axis = CarAxis.FRONT;
        wheels[1].model = this.transform.GetChild(3).GetChild(19).gameObject;
        wheels[1].wheel = this.transform.GetChild(3).GetChild(25).GetChild(1).GetComponent<WheelCollider>();
        wheels[2] = new Wheel(); //RearLeft
        wheels[2].axis = CarAxis.REAR;
        wheels[2].model = this.transform.GetChild(3).GetChild(20).gameObject;
        wheels[2].wheel = this.transform.GetChild(3).GetChild(25).GetChild(2).GetComponent<WheelCollider>();
        wheels[3] = new Wheel(); //RearRight
        wheels[3].axis = CarAxis.REAR;
        wheels[3].model = this.transform.GetChild(3).GetChild(21).gameObject;
        wheels[3].wheel = this.transform.GetChild(3).GetChild(25).GetChild(3).GetComponent<WheelCollider>();


        tyreCircumference = wheels[0].wheel.radius * Mathf.PI * 2;
    }



    void Update()
    {
        AddGas();
        SwitchGearAutomatic();
        Steer();
        Brake();
        CalculateTorque();
        //DebugSomeValues();
        ApplyTorqueToWheels();
        CalculateSpeed();
    }




    void AddGas()
    {
        //increase rounds if gas pedal is pressed
        if (controller.gasPedal > 0)
        {
            roundsPerMinute += controller.gasPedal * gear[activeGear] * 200f * (float)Time.deltaTime;
        }
        else
        {
            if (roundsPerMinute > 0)
                roundsPerMinute -= rpmReduction * (float)Time.deltaTime;
        }

    }


    void CalculateSpeed()
    {
        float speedAverage = 0f;
        switch (wheelMode)
        {
            case WheelDrive.FRONTWHEEL:
                foreach (Wheel w in wheels)
                {
                    if (w.axis == CarAxis.FRONT)
                        speedAverage += w.wheel.rpm / 60 * tyreCircumference;
                }

                speed = speedAverage / 2;
                break;
            case WheelDrive.REARWHEEL:
                foreach (Wheel w in wheels)
                {
                    if (w.axis == CarAxis.REAR)
                        speedAverage += w.wheel.rpm / 60 * tyreCircumference;
                }

                speed = speedAverage / 2;
                break;
            case WheelDrive.ALLWHEEL:
                foreach (Wheel w in wheels)
                {
                     speedAverage += w.wheel.rpm / 60 * tyreCircumference;
                }

                speed = speedAverage / 4 * 3.6f;
                break;
        }
    }

    void Steer()
    {
        float turnProgress = Mathf.InverseLerp(-100, 100, controller.steeringWheel);
        float wheelAngle = Mathf.Lerp(-maxWheelAngle, maxWheelAngle, turnProgress);

        //Turn wheel component
        foreach (Wheel w in wheels)
        {
            if (w.axis == CarAxis.FRONT)
            {
                w.wheel.steerAngle = wheelAngle; //wheelcomponent
                w.model.transform.localRotation = Quaternion.Euler(0, wheelAngle, 0); //model (visual only)
            }
        }



    }


    void Brake()
    {
        if (controller.brakePedal > 0)
        {
            foreach (Wheel w in wheels)
            {
                w.wheel.brakeTorque = this.brakeTorque;
            }
        }
        else
        {
            foreach (Wheel w in wheels)
            {
                w.wheel.brakeTorque = 0f;
            }
        }
    }

    void SwitchGearAutomatic()
    {
        float threshold = 2200f;

        float thresholdIncrease = Mathf.InverseLerp(-3000f, 3000f, controller.gasPedal) * 1000f;

        threshold += thresholdIncrease;


        //Handle gear up
        if (activeGear != maxGear)
        {
            if (roundsPerMinute > threshold)
            {
                //switch gear up
                roundsPerMinute = roundsPerMinute * gear[activeGear + 1] / gear[activeGear];
                activeGear++;
            }
        }



        //Handle gear down
        if (activeGear != 0)
        {
            if (roundsPerMinute < 1500f)
            {
                //switch gear down
                roundsPerMinute = roundsPerMinute * gear[activeGear - 1] / gear[activeGear];
                activeGear--;
            }
        }
    }

    void CalculateTorque()
    {
        if (roundsPerMinute != 0 && controller.gasPedal != 0)
            motorTorque = horsePower / (roundsPerMinute / 5252);
        else
            motorTorque = 0;


        if (motorTorque < 0)
            motorTorque = 0;
    }


    void DebugSomeValues()
    {
        //Debug.Log("InverseLerp value: " + Mathf.InverseLerp(-3000f, 3000f, gasPedal));
        Debug.Log("Gear" + activeGear + "Ratio: " + gear[activeGear]);
        Debug.Log("RPM:" + roundsPerMinute);
        Debug.Log("Torque:" + motorTorque);
        Debug.Log("Speed: " + speed);
    }

    void ApplyTorqueToWheels()
    {
        if (controller.gasPedal > 0)
        {
            switch (wheelMode)
            {
                case WheelDrive.FRONTWHEEL:
                    foreach (Wheel w in wheels)
                    {
                        if (w.axis == CarAxis.FRONT)
                        {
                            w.wheel.motorTorque = this.motorTorque;
                        }
                    }
                    break;
                case WheelDrive.REARWHEEL:
                    foreach (Wheel w in wheels)
                    {
                        if (w.axis == CarAxis.REAR)
                        {
                            w.wheel.motorTorque = this.motorTorque;
                        }
                    }
                    break;
                case WheelDrive.ALLWHEEL:
                    foreach (Wheel w in wheels)
                    {
                        w.wheel.motorTorque = this.motorTorque;
                    }
                    break;
            }
        }
        else
        {
            foreach (Wheel w in wheels)
            {
                w.wheel.motorTorque = 0f;
            }
        }
    }


}
