using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArea : MonoBehaviour
{
    public enum AreaType
    {
        STRAIGHT = 0,
        CURVE,
        INTERSECTION,
    }

    //Refences
    public TrafficSystem trafficSystem;
    public BoxCollider boxCollider;


    public List<Vector3> points;

    public AreaType area;
    public float speedLimit = 0;





    private void Awake()
    {
        if (!boxCollider)
            boxCollider = this.transform.GetComponent<BoxCollider>();
    }


    private void OnDrawGizmos()
    {
        if (points.Count != 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < points.Count; ++i)
            {
                if (i != points.Count - 1)
                    Gizmos.DrawLine(points[i], points[i + 1]);
                else
                    Gizmos.DrawLine(points[i], points[0]);
            }
        }
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            SendInformationToTrafficSystem();
        }
    }


    void SendInformationToTrafficSystem()
    {
        trafficSystem.speedLimit = speedLimit;
    }
}
