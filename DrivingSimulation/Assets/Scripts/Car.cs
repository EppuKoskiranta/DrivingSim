using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Car : MonoBehaviour
{
    //Light stuff
    public enum LightMode
    {
        DEFAULT = 0,
        ON,
        MIST,

    }

    //public enum Lights
    //{
    //    DEFAULT = 0b_1,
    //    LONG = 0b_10,
    //    LEFTBLINKER = 0b_100,
    //    RIGHTBLINKER = 0b_1000,
    //}

    public struct Lights
    {
        public bool defaultLights;
        public bool longLights;
        public bool leftBlinker;
        public bool rightBlinker;

        public Lights(bool d = true, bool ll = false, bool lb = false, bool rb = false)
        {
            defaultLights = d;
            longLights = ll;
            leftBlinker = lb;
            rightBlinker = rb;
        }
    }

    public LightMode lightMode = LightMode.DEFAULT;
    public Lights lights = new Lights();
    Light[] lightComponents;

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

    //Sounds
    public AudioClip engineStart;
    public AudioClip engineRunnning;


    //Motor stuff
    public   bool automatic = true;
    public   bool handbrake = false;
    public   bool engineOn = false;
    public   float maxSpeed = 250f;
    public   float maxRPM = 6500f;
    public   float fuel = 70.0f;
    public   float roundsPerMinute = 0f;

    public   float rpmReduction = 1000f;
    public float direction = 0f;
    public   float speed = 0f; //    km/h
    public   float horsePower = 200f;
    public   float motorTorque = 0f;


    public   int activeGear = 0;
    public   int maxGear = 5;
    public   float[] gear = { 3.166f, 1.882f, 1.296f, 0.972f, 0.738f, 0.600f };

    public   float reverseGear = 2.6f;

    public   float tyreCircumference = 1.99271f;
    public   float differentialRatio = 4.1f;

    public   float brakeTorque = 1620f;

    public   float maxWheelAngle = 35f; //in euler angles


    //Air resistance
    public   float Cd = 0.35f; //coefficient

    public   float density = 1f;
    public   float dragArea = 1.5f; //squaremeters (estimate)


    public LayerMask layerMask;

    //References
    public AudioSource carAudio;
    public CarController controller;
    public TrafficSystem trafficSystem;
    public Rigidbody rb;


    //Extras
    //like radio feat. skirmish beats




    void Start()
    {
        SetupCar();
    }

    void Update()
    {
        CheckWhichRoadCarIsOn();

        if (!engineOn)
            StartEngine();

        if (engineOn)
        {
            CarUpdate();
        }
    }

    void SetupCar()
    {
        //Setup controller
        if (!controller)
            controller = this.gameObject.GetComponent<CarController>();

        if (!rb)
            rb = this.gameObject.GetComponent<Rigidbody>();


        if (!carAudio)
        {
            carAudio = this.gameObject.GetComponentInChildren<AudioSource>();
        }

        //Setup wheels
        SetupWheels();


        //SetupLights
        SetupLights();
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



    void SetupLights()
    {
        //0 LeftDefaultLight, 1 RightDefaultLight, 2 LeftLongLight, 
        //3 RightLongLight, 4 LeftBrakeLight, 5 RightBrakeLight, 
        //6 LeftFrontBlinker, 7 LeftRearBlinker, 8 RightFrontBlinker, 9 RightRearBlinker

        lightComponents = this.transform.GetComponentsInChildren<Light>();


    }


    void CarUpdate()
    {
        SwitchDriveMode();
        AddGas();
        Brake();
        Handbrake();
        Steer();
        CalculateSpeed();
        WidgetUpdate();
        Sounds();
        DebugSomeValues();
    }

    void StartEngine()
    {
        if (controller.startEngine)
        {
            engineOn = true;
            controller.startEngine = false;

            lightComponents[0].enabled = true;
            lightComponents[1].enabled = true;
            Debug.Log("Started Engine!! VROM VROM!");
            carAudio.PlayOneShot(engineStart);
            StartCoroutine(EngineRunning());
        }
    }


    void Sounds()
    {
        
    }

    IEnumerator EngineRunning()
    {
        yield return new WaitForSecondsRealtime(engineStart.length);
        carAudio.clip = engineRunnning;
        carAudio.loop = true;
        carAudio.Play();
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
            roundsPerMinute += gas * 10000f * Time.deltaTime;

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

        totalFriction += 0f;

        totalFriction += 100 * ((1 / gear[activeGear]) * (roundsPerMinute / speed));

        
        return totalFriction;
    }


    void ReduceRoundsPerMinute()
    {
        if (roundsPerMinute > 0)
            roundsPerMinute -= CalculateFrictionForces() * Time.deltaTime;
        else if (roundsPerMinute < 0)
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

    }

    void CalculateSpeed()
    {
        direction = transform.InverseTransformDirection(rb.velocity).z;
        speed = rb.velocity.magnitude * 3.6f;
    }

    float AirResistance()
    {
        return 0;
        //return 0.5f * density * Mathf.Pow(speed, 2) * Cd * dragArea;
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
        //Debug.Log("Light bits: " + Convert.ToString(lights, toBase: 2));
        //Debug.Log("RPM: " + roundsPerMinute);
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


    void WidgetUpdate()
    {
        //Switch mode //default, on, mist
        LightSwitchMode();

        //Turn lights on/off //blinkers, long lights
        LightsUpdate();

    }


    void LightSwitchMode()
    {

        if (controller.lightsModeLeft)
        {
            if (lightMode != LightMode.DEFAULT)
            {
                lightMode--;
            }

            controller.lightsModeLeft = false;
        }

        if (controller.lightsModeRight)
        {
            if (lightMode != LightMode.MIST)
            {
                lightMode++;
            }

            controller.lightsModeRight = false;
        }
    }



    void LightsUpdate()
    {
        if (controller.blinkerLeft)
        {
            lights.leftBlinker = !lights.leftBlinker;
            lights.rightBlinker = false;
            controller.blinkerLeft = false;
        }


        if (controller.blinkerRight)
        {
            lights.rightBlinker = !lights.rightBlinker;
            lights.leftBlinker = false;
            controller.blinkerRight = false;
        }

        if (controller.longLights)
        {
            if (lightMode == LightMode.ON)
            {
                //risky to toggle them on/off. Since if they get changed outside of this
                lights.defaultLights = !lights.defaultLights;
                lights.longLights = !lights.longLights;

                //Setting the actual unity Light component on/off
                //default lights
                lightComponents[1].enabled = lights.defaultLights;
                lightComponents[2].enabled = lights.defaultLights;

                //long lights
                lightComponents[3].enabled = lights.longLights;
                lightComponents[4].enabled = lights.longLights;

            }
            controller.longLights = false;
        }
    }



    void CheckWhichRoadCarIsOn()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position + new Vector3(0, 1, 0), new Vector3(0, -1, 0), out hit,  Mathf.Infinity, layerMask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Road"))
            {
                Road road = hit.collider.gameObject.transform.parent.GetComponent<Road>();
                trafficSystem.currentRoad = road;
                trafficSystem.currentIntersection = null;
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Intersection"))
            {
                Intersection intersection = hit.collider.GetComponent<Intersection>();
                trafficSystem.currentIntersection = intersection;
                trafficSystem.currentRoad = null;
            }
        }
    }



}
