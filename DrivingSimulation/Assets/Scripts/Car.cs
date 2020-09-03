using System;
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
    bool automatic;
    bool handbreak;
    float fuel;
    float roundsPerMinute = 0f;
    float rpmReduction = 200f;
    float speed = 0f; //km/h cause fuck miles
    float maxSpeed = 120f;
    int gear = 1;
    int maxGear = 6;

    //testing stuff
    float gasPedal;
    float breakPedal;

    //Extras
    //like radio feat. skirmish beats





    void Start()
    {

    }


    void Update()
    {
        ProcessInputKeyboard();
        RoundsPerMinute();


    }


    void ProcessInputKeyboard()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            gasPedal = 0.0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("Pressed W");
            gasPedal = 3000.0f;
        }
    }


    void RoundsPerMinute()
    {

        if (gasPedal > 0)
        {
            roundsPerMinute += gasPedal * (1.0f / ((float)gear * (0.8f))) * (float)Time.deltaTime;
        }

        //Reduction
        if (roundsPerMinute > 0)
        {
            
            if (roundsPerMinute > 2560f)
            {
                if (gear < maxGear)
                {
                    roundsPerMinute = 1600f;
                    gear++;
                }
            }
            else if (roundsPerMinute < 1000f)
            {
                if (gear > 1)
                {
                    roundsPerMinute = 1600f;
                    gear--;
                }
            }

            if (gasPedal < 1)
            {
                roundsPerMinute -= rpmReduction * (float)Time.deltaTime;
            }
        }


        speed = roundsPerMinute * (float)gear * 0.2f;

        Debug.Log("RPM:" + roundsPerMinute);
        Debug.Log("Gear:" + gear);
        Debug.Log("Speed:" + speed / 3.6f);
    }


}
