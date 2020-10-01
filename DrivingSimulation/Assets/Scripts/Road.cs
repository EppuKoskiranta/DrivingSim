using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class Road : MonoBehaviour
{
    public enum Direction { FORWARD = 1, BACKWARD = -1};

    //Refences
    public TrafficSystem trafficSystem;
    public PathCreator pathCreator;
    public Mesh arrowMesh;

    public Direction direction;

    public float speedLimit = 0;
    
    private void Awake()
    {
        Debug.Log("Point " + pathCreator.path.GetPoint(0));


    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int i = 0; i < pathCreator.path.NumPoints; ++i)
        {
            if (i < pathCreator.path.NumPoints - 1)
            {
                Vector3 pos = (pathCreator.path.GetPoint(i) + pathCreator.path.GetPoint(i + 1)) / 2 + new Vector3(0, 1, 0);
                Vector3 dir = pathCreator.path.localNormals[i];
                Debug.Log(dir);
                //Gizmos.DrawMesh(arrowMesh, 0, pathCreator)
                Gizmos.DrawMesh(arrowMesh, 0, pos, Quaternion.Euler(90, 0, -90), new Vector3(10, 10, 10));
            }
        }
    }

    void SendInformationToTrafficSystem()
    {
        trafficSystem.speedLimit = speedLimit;
    }
}
