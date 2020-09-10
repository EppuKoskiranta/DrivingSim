﻿using System;
using System.Collections;
using System.Collections.Generic;
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

    //Motor stuff
    bool automatic = true;
    bool handbreak = false;
    float fuel = 70.0f;
    float roundsPerMinute = 0f;
    float rpmReduction = 200f;
    float speed = 0f; //km/h cause fuck miles
    float horsePower = 200f;
    float motorTorque = 0f;

    int activeGear = 0;
    int maxGear = 5;
    float[] gear = { 3.166f, 1.882f, 1.296f, 0.972f, 0.738f, 0.600f };
    
    float tyreCircumference = 1.99271f;
    float differentialRatio = 4.1f;

    //testing input stuff
    float gasPedal = -3000f;
    float breakPedal = 0.0f;
    float steeringWheel = 0f;

    float maxWheelAngle = 35f; //in euler angles




    //Components
    public WheelCollider wheelFrontLeft, wheelFrontRight, wheelRearLeft, wheelRearRight;
    public GameObject wheelFrontLeftGo, wheelFrontRightGo, wheelRearLeftGo, wheelRearRightGo; 

    //Extras
    //like radio feat. skirmish beats





    void Start()
    {

    }


    void Update()
    {
        ProcessInputKeyboard();
        AddGas();
        SwitchGearAutomatic();
        Steer();
        CalculateTorque();
        DebugSomeValues();
        ApplyTorqueToWheels();
    }


    void ProcessInputKeyboard()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            gasPedal = -3000.0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            gasPedal = 3000.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (steeringWheel < 100f)
                steeringWheel += 30f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (steeringWheel > -100)
                steeringWheel -= 30f * Time.deltaTime;
        }
    }


    void AddGas()
    {
        //increase rounds if gas pedal is pressed
        if (gasPedal > -3000.0f)
        {
            roundsPerMinute += Mathf.InverseLerp(-3000f, 3000f, gasPedal) * gear[activeGear] * 200f * (float)Time.deltaTime;
        }
        else
        {
            if (roundsPerMinute > 0)
                roundsPerMinute -= rpmReduction * (float)Time.deltaTime;
        }

    }


    void Steer()
    {
        float turnProgress = Mathf.InverseLerp(-100, 100, steeringWheel);
        float wheelAngle = Mathf.Lerp(-maxWheelAngle, maxWheelAngle, turnProgress);

        //Turn wheel component
        wheelFrontLeft.steerAngle = wheelAngle;
        wheelFrontRight.steerAngle = wheelAngle;


        //turn actual wheel (visual only)
        wheelFrontLeftGo.transform.localRotation = Quaternion.Euler(0, wheelAngle, 0);
        wheelFrontRightGo.transform.localRotation = Quaternion.Euler(0, wheelAngle, 0);
    }


    void Break()
    {

    }

    void SwitchGearAutomatic()
    {
        float threshold = 2200f;

        float thresholdIncrease = Mathf.InverseLerp(-3000f, 3000f, gasPedal) * 1000f;

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
        if (roundsPerMinute != 0)
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
    }

    void ApplyTorqueToWheels()
    {
        if (gasPedal > -3000f)
        {
            wheelFrontLeft.motorTorque = this.motorTorque;
            wheelFrontRight.motorTorque = this.motorTorque;
        }
        else
        {
            wheelFrontLeft.motorTorque = 0;
            wheelFrontRight.motorTorque = 0;
        }
    }


    //void RoundsPerMinute()
    //{

    //    if (gasPedal > 0)
    //    {
    //        roundsPerMinute += Mathf.InverseLerp(-3000f, 3000f, gasPedal) * 1000f * (float)Time.deltaTime;
    //    }

    //    //Reduction
    //    if (roundsPerMinute > 0)
    //    {

    //        if (roundsPerMinute > 2560f)
    //        {
    //            if (activeGear < maxGear)
    //            {
    //                roundsPerMinute = 1600f;
    //                activeGear++;
    //            }
    //        }
    //        else if (roundsPerMinute < 1000f)
    //        {
    //            if (activeGear > 1)
    //            {
    //                roundsPerMinute = 1600f;
    //                activeGear--;
    //            }
    //        }

    //        if (gasPedal < 1)
    //        {
    //            roundsPerMinute -= rpmReduction * (float)Time.deltaTime;
    //        }
    //    }


    //    float perSecond = roundsPerMinute / 60;
    //    float differentialRounds = perSecond / differentialRatio;
    //    float gearRounds = differentialRounds / gear[activeGear - 1];
    //    float metersPerSeconds = gearRounds * tyreCircumference;

    //    speed = metersPerSeconds * 3.6f;

    //    Debug.Log("Gear" + activeGear + "Ratio: " + gear[activeGear - 1]);
    //    Debug.Log("RPM:" + roundsPerMinute);
    //    Debug.Log("Speed:" + speed);
    //}


}
