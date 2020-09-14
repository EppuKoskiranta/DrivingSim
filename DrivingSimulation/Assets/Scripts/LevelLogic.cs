using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  What we need:
 *  Starting position, -mode?
 *  End goal(s)
 *  Events that check correct driving (speed, lane, blinkers, lights, stop signs, collisions).
 *  Events ("Quests") that need to be completed
 *  
 */

public class LevelLogic : MonoBehaviour
{
    // Make a singleton of this class.
    public static LevelLogic current;
    public int level_id;

    private void Awake()
    {
        //Initiate the singleton
        current = this;
    }

    public event Action<int> on_level_started;
    public void start_level(int level_id)
    {
        this.level_id = level_id;
        if(on_level_started != null)
        {
            on_level_started(level_id);
        }
    }
}
