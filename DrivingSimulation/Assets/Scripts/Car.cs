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

    LightMode lightMode = LightMode.DEFAULT;
    uint lights = 0x01;

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


    public enum AutomaticGearMode
    {
        PARK = 0,
        REVERSE,
        NEUTRAL,
        DRIVE,
    }

    AutomaticGearMode automaticGearMode = AutomaticGearMode.PARK;

    //Motor stuff
    bool automatic = true;
    bool handbrake = false;
    bool engineOn = false;
    float maxSpeed = 250f;
    float maxRPM = 6500f;
    float fuel = 70.0f;
    float roundsPerMinute = 0f;
    float rpmReduction = 1000f;
    float speed = 0f; //    km/h
    float horsePower = 200f;
    float motorTorque = 0f;


    int activeGear = 0;
    int maxGear = 5;
    float[] gear = { 3.166f, 1.882f, 1.296f, 0.972f, 0.738f, 0.600f };

    float reverseGear = 2.6f;
    
    float tyreCircumference = 1.99271f;
    float differentialRatio = 4.1f;

    public float brakeTorque = 1620f;

    float maxWheelAngle = 35f; //in euler angles


    //Air resistance
    float Cd = 0.35f; //coefficient
    float volume = 3.11f; //volume
    float density = 0f;
    float dragArea = 2.5f; //squaremeters (estimate)
     

    //References
    public CarController controller;
    public Rigidbody rb;

    //Extras
    //like radio feat. skirmish beats




    void Start()
    {
        CarInit();
    }

    void Update()
    {
        if (!engineOn)
            StartEngine();

        if (engineOn)
        {
            CarUpdate();
        }
    }

    void CarInit()
    {
        //Setup controller
        if (!controller)
            controller = this.gameObject.GetComponent<CarController>();

        if (!rb)
            rb = this.gameObject.GetComponent<Rigidbody>();


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


        wheels[0].wheel.ConfigureVehicleSubsteps(5.0f, 5, 2);
    }


    void CarUpdate()
    {
        //Take all inputs from controller
        /*
        

        SwitchDriveMode()

        Gas()
        {
            Read gasPedal value 0 - 1
            Increase roundsPerMinute with gasPedal
                
            SwitchGear();
            
            CalculateTorque()
            ApplyTorque()
            
            CalculateForcesAgainstMovement()
            {
                AirResistance
                InternalExternal friction (includes engine friction, friction with gears, wheels friction)
                
            }


            ReduceRoundsPerMinute()
        }

        Brake()
        {
            Read brakePedal value 0 - 1
            ApplyBrakeTorque to wheels
        }


        Handbrake()
        {
            check if handbrake on
            applybraketorque
        }

        Steer()
        {
            Read steeringWheel value -1 to 1
            apply angle to wheels
        }
        
        Inputs()
        {
            Wipers/Turn signals
        }
        

        DebugValues()

         */


        //Gas
        SwitchDriveMode();
        AddGas();
        Brake();
        Handbrake();
        Steer();
        CalculateSpeed();
        DebugSomeValues();
    }

    void StartEngine()
    {
        if (controller.startEngine)
        {
            engineOn = true;
            controller.startEngine = false;
            Debug.Log("Started Engine!! VROM VROM!");
            //play audio
        }
    }


    void Gas()
    {
        //increase rounds if gas pedal is pressed
        if (controller.gasPedal > 0 && speed < maxSpeed && engineOn)
        {
            roundsPerMinute += controller.gasPedal * gear[activeGear] * 200f * Time.deltaTime;
        }
        else
        {
            if (roundsPerMinute > 0 && wheels[0].wheel.rpm != 0)
                roundsPerMinute -= (rpmReduction * Time.deltaTime);
            else if (roundsPerMinute > 0 && wheels[0].wheel.rpm == 0)
                roundsPerMinute -= rpmReduction * 1.5f * Time.deltaTime;
            else
                roundsPerMinute = 0;
        }

    }


    void AddGas()
    {

        float gas = controller.gasPedal;
        if (roundsPerMinute < maxRPM)
            roundsPerMinute += gas * 5000f * Time.deltaTime;

        if (automatic)
            SwitchGearAutomatic();

        CalculateTorque();
        ApplyTorqueToWheels();

        ReduceRoundsPerMinute();
    }


    /// <summary>
    /// Air resistance, engine friction, wheels friction, gears friction
    /// </summary>
    float CalculateFrictionForces()
    {
        float totalFriction = 0;

        rb.AddForce(-this.transform.forward * AirResistance());

        //TODO calculate real values

        totalFriction += 0f;

        totalFriction += 100 * ((1 / gear[activeGear]) * (roundsPerMinute / speed));

        
        return totalFriction;
    }


    void ReduceRoundsPerMinute()
    {
        if (roundsPerMinute > 0)
            roundsPerMinute -= CalculateFrictionForces() * Time.deltaTime;
        else
            roundsPerMinute = 0;
    }

    void Steer()
    {
        float turnProgress = Mathf.InverseLerp(-1, 1, controller.steeringWheel);
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


    void SwitchDriveMode()
    {
        if (controller.brakePedal > 0)
        {
            if (controller.gearDown)
            {
                int activeGearMode = (int)automaticGearMode;

                if (activeGearMode != 0)
                    activeGearMode--;

                automaticGearMode = (AutomaticGearMode)activeGearMode;

                controller.gearDown = false;
            }
            else if (controller.gearUp)
            {
                int activeGearMode = (int)automaticGearMode;

                if (activeGearMode != 3)
                    activeGearMode++;

                automaticGearMode = (AutomaticGearMode)activeGearMode;

                controller.gearUp = false;
            }
        }


        Debug.Log("ActiveGearMode: " + automaticGearMode);
    }

    void CalculateSpeed()
    {
        //TODO make a way calculate speed
        speed = rb.velocity.magnitude * 3.6f;

    }

    float AirResistance()
    {
        return 0;
        //return Cd * (density * Mathf.Pow(speed, 2) / 2) * dragArea;
    }


    void Handbrake()
    {
        if (handbrake)
        {
            foreach (Wheel w in wheels)
            {
                w.wheel.brakeTorque = 10000f;
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
        if (engineOn)
        {
            if (roundsPerMinute != 0 && controller.gasPedal != 0)
                motorTorque = horsePower / (roundsPerMinute / 5252);
            else
                motorTorque = 0;
        }


        if (motorTorque < 0)
            motorTorque = 0;

    }


    void DebugSomeValues()
    {
        Debug.Log("RPM: " + roundsPerMinute);
        //Debug.Log("Speed: " + speed);
    }

    void ApplyTorqueToWheels()
    {
        float motorTorqueDir = 0f;
        if (automaticGearMode == AutomaticGearMode.DRIVE)
            motorTorqueDir = this.motorTorque;

        if (automaticGearMode == AutomaticGearMode.REVERSE)
            motorTorqueDir = -this.motorTorque;


        if (controller.gasPedal > 0)
        {
            switch (wheelMode)
            {
                case WheelDrive.FRONTWHEEL:
                    foreach (Wheel w in wheels)
                    {
                        if (w.axis == CarAxis.FRONT)
                        {
                            w.wheel.motorTorque = motorTorqueDir;
                        }
                    }
                    break;
                case WheelDrive.REARWHEEL:
                    foreach (Wheel w in wheels)
                    {
                        if (w.axis == CarAxis.REAR)
                        {
                            w.wheel.motorTorque = motorTorqueDir;
                        }
                    }
                    break;
                case WheelDrive.ALLWHEEL:
                    foreach (Wheel w in wheels)
                    {
                        w.wheel.motorTorque = motorTorqueDir;
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


    void WidgetInputs()
    {

    }







}
