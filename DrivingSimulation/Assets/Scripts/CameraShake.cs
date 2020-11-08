using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Car car;
    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Car").GetComponent<Car>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
