using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
        }
    }

    public event Action<DS_EVENTS> CollisionEvent;

    public void OnCollision(DS_EVENTS e)
    {
        CollisionEvent?.Invoke(e);
    }
}
