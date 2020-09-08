using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class cartest : MonoBehaviour
{

    public WheelCollider wheelRF, wheelRB, wheelLF, wheelLB;

    float x = 0f;

    public float horsePower = 200f;
    public float rpm = 3000f;


    float differentialRatio = 4.1f;
    float gearRatio = 3.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float torque = horsePower / (rpm / 5252f);
        wheelLF.motorTorque = torque * differentialRatio * gearRatio;
        wheelRF.motorTorque = torque * differentialRatio * gearRatio;
    }
}
