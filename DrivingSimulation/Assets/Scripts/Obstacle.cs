using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    
    void OnCollisionEnter(Collision collision)
    {
        EventsManager.instance.OnCollision(DS_EVENTS.COLLISION_OBSTACLE);
    }
}
