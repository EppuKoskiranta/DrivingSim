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
    //References
    Car car;

    // Make a singleton of this class.
    public Level[] levels = new Level[10];
    public static LevelLogic current;
    public int level_id;

    public bool[] car_states;
    public int current_quest;

    public void change_current_quest(int id)
    {
        current_quest = id;
    }

    public void Awake()
    {
        car = GameObject.FindGameObjectWithTag("Car").GetComponent<Car>();
        //Initiate the singleton
        current = this;
        car_states = new bool[(int)CAR_STATES.COUNT + 1];
        start_level(0);
    }

    public void Update()
    {
        ReadCarStates();
        current_quest = levels[level_id].UpdateFromLL(current_quest, car_states);
        UnityEngine.Debug.Log("Current Quest: " + current_quest);

    }

    public event Action<int> on_level_started;
    public void start_level(int level_id)
    {
        on_level_started?.Invoke(level_id);
        this.level_id = level_id;
    }


    public void ReadCarStates()
    {
        car_states[0] = car.engineOn;
        car_states[1] = car.direction > 0.001 ? true : false;
        car_states[2] = car.direction < -0.001 ? true : false;
        car_states[3] = car.controller.gasPedal > 0 ? true : false;
        car_states[4] = car.controller.brakePedal > 0 ? true : false;
        car_states[5] = car.controller.steeringWheel > 0.05f || car.controller.steeringWheel < -0.05f ? true : false;
        // Updates Inside target of the quest of the level
        if (current_quest != 0 && current_quest != -999)
        {
            car_states[8] = levels[level_id].targets[levels[level_id].quests[levels[level_id].GetIndexFromID(current_quest)].target_id].GetComponent<TargetTrigger>().inside_target;
        }
    }
}
