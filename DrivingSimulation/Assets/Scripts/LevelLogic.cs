using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

/*
 *  What we need:
 *  Starting position, -mode?
 *  End goal(s)
 *  Events that check correct driving (speed, lane, blinkers, lights, stop signs, collisions).
 *  ("Quests") that need to be completed
 *  
 */

public class LevelLogic : MonoBehaviour
{
    // Make a singleton of this class.
    public Level[] levels = new Level[10];
    public static LevelLogic current;
    public int level_id;

    public bool[] car_states = new bool[(int)CAR_STATES.COUNT];
    public List<Quest> quests = new List<Quest>();
    public int current_quest;

    public void change_current_quest(int id)
    {
        current_quest = id;
    }

    public void Awake()
    {
        car_states[(int)CAR_STATES.COUNT - 1] = true;
        //Initiate the singleton
        current = this;

    }

    public event Action<int> on_level_started;
    public void start_level(int level_id)
    {
        on_level_started?.Invoke(level_id);
        this.level_id = level_id;
    }
}
