using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSystem : MonoBehaviour
{
    //References
    public LevelLogic levelLogic;
    public Car car;
    public Road currentRoad;
    public Intersection currentIntersection;
    

    public Vector3 correctDirection = Vector3.zero;
    public float speedLimit = 0;

    [Range(90, 180)]
    public float WrongDirectionAngle = 90f;


    private void Awake()
    {
        
    }



    void Update()
    {
        RightDirection();
        InSpeedLimit();
    }



    bool RightDirection()
    {
        if (currentRoad != null)
        {
            Vector2 direction2D = new Vector2(correctDirection.x, correctDirection.z);
            Vector2 carRotVector = new Vector2(car.transform.forward.x, car.transform.forward.z);

            float angle = Mathf.Acos(Vector2.Dot(carRotVector, direction2D)) * Mathf.Rad2Deg;


            if (angle > WrongDirectionAngle)
            {
                Debug.Log("Car is going wrong direction");
                return false;
            }

            return true;

        }
        else if (currentIntersection != null)
        {
            Debug.Log("Intersection");
            return true;
        }
        else
        {
            return false;
        }
    }


    bool InSpeedLimit()
    {
        if (currentRoad != null)
        {
            if (car.speed > speedLimit)
            {
                Debug.Log("You are going too fast!");
                return false;
            }

            return true;
        }
        else
        {
            return true;
        }
    }
}
