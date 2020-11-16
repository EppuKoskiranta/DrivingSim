using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;


[System.Serializable]
public struct SpeedLimit
{
    [Range(0, 1)]
    public float position; //goes from 0 to 1
    public float speedLimit;
}

public class Road : MonoBehaviour
{
    //Refences
    public TrafficSystem trafficSystem;
    public PathCreator pathCreator;
    public Mesh directionMesh;
    public Mesh speedLimitMesh;


    [SerializeField]
    public List<SpeedLimit> speedLimits = new List<SpeedLimit>();


    private Vector3 globalDirection = Vector3.zero;
    public Vector3 currentDirection = Vector3.zero;


    public float speedLimit = 0;

    public bool displayDirection = true;
    public bool displaySpeedLimits = true;


    private void Start()
    {
        if (speedLimits.Count == 0)
        {
            SpeedLimit limit;
            limit.position = 0;
            limit.speedLimit = 40;
            speedLimits.Add(limit);
        }

        //For now do this start of the scene //TODO: Make system that we can bake meshcolliders for roads
        if (transform.childCount > 0)
            this.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
    }


    private void Update()
    {
        SendInformationToTrafficSystem();
    }


    private void OnDrawGizmos()
    {
        if (displayDirection)
        {
            if (pathCreator != null)
            {
                Gizmos.color = Color.white;
                for (int i = 0; i < pathCreator.path.NumPoints; ++i)
                {
                    if (i < pathCreator.path.NumPoints - 1)
                    {
                        Vector3 pos = (pathCreator.path.GetPoint(i) + pathCreator.path.GetPoint(i + 1)) / 2 + new Vector3(0, 1, 0);
                        Vector3 dir = pathCreator.path.GetDirectionAtDistance(pathCreator.path.GetClosestDistanceAlongPath(pos));
                        float zAngle = Vector3.Angle(new Vector3(0, 0, 90), dir);
                        Vector3 displayDir = new Vector3(90, 0, -zAngle);
                        Gizmos.DrawMesh(directionMesh, 0, pos, Quaternion.Euler(displayDir), new Vector3(10, 10, 10));
                    }
                }
            }
        }


        if (displaySpeedLimits)
        {
            Gizmos.color = Color.red;
            {
                for (int i = 0; i < speedLimits.Count; ++i)
                {
                    Vector3 pos = pathCreator.path.GetPointAtTime(speedLimits[i].position) + new Vector3(0, 0.5f, 0);
                    Vector3 rotation = pathCreator.path.GetDirection(speedLimits[i].position);
                    Gizmos.DrawMesh(speedLimitMesh, 0, pos, Quaternion.identity, new Vector3(0.5f, 1f, .5f));
                }
            }
        }
    }

    /// <summary>
    /// Sends current speedlimit and correct direction to traffic system if this is the current road
    /// </summary>
    void SendInformationToTrafficSystem()
    {
        if (this == trafficSystem.currentRoad)
        {
            CarProgressOnRoad();
            trafficSystem.speedLimit = speedLimit;
            trafficSystem.correctDirection = currentDirection;
        }
    }



    void CarProgressOnRoad()
    {
        Vector3 carPos = trafficSystem.car.transform.position;
        float carProgress = pathCreator.path.GetClosestTimeOnPath(carPos);
        globalDirection = pathCreator.path.GetDirection(carProgress);
        speedLimit = SetSpeedLimitRelativeToCarProgress(carProgress);

        bool Right = OnRightSide(carProgress);

        if (Right)
            currentDirection = globalDirection;
        else
            currentDirection = globalDirection * -1;
    }


    bool OnRightSide(float progress)
    {
        bool onRight = false;

        Vector2 directionTangent = new Vector2(globalDirection.z, -globalDirection.x).normalized;

        Vector2 carPos2D = new Vector2(trafficSystem.car.transform.position.x, trafficSystem.car.transform.position.z);
        Vector2 roadPos2D = new Vector2(pathCreator.path.GetPointAtTime(progress).x, pathCreator.path.GetPointAtTime(progress).z);

        float carPos = Vector2.Dot(carPos2D, directionTangent);
        float roadPos = Vector2.Dot(roadPos2D, directionTangent);

        if (carPos > roadPos)
            onRight = true;
        else
            onRight = false;


        return onRight;
    }


    float SetSpeedLimitRelativeToCarProgress(float progress)
    {
        float limit = 0;
        for (int i = 0; i < speedLimits.Count; ++i)
        {
            //if we have passed this position
            if (progress >= speedLimits[i].position)
            {
                //Check if limit after current limit
               if (i+1 != speedLimits.Count)
               {
                    //if progress is less than next 
                    if (progress < speedLimits[i+1].position)
                    {
                        limit = speedLimits[i].speedLimit;
                        return limit;
                    }
               }
               else
                {
                    limit = speedLimits[i].speedLimit;
                    return limit;
                }
            }
        }


        return limit;
    }
}
