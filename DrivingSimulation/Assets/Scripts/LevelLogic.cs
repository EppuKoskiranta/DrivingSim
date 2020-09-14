using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/*
 *  What we need:
 *  Starting position, -mode?
 *  End goal(s)
 *  Events that check correct driving (speed, lane, blinkers, lights, stop signs, collisions).
 *  ("Quests") that need to be completed
 *  
 */

public enum CAR_STATES
{
    IS_ENGINE_ON,
    IS_MOVING_FORWARD,
    IS_MOVING_BACKWARD,
    IS_ACCELERATING,
    IS_BRAKING,
    IS_STEERING,
    IS_MOVING_IN_RIGHT_DIRECTION,
    IS_IN_SPEEDLIMIT,
    IS_IN_TARGET_TRIGGER,
    COUNT
}

public enum DS_EVENTS
{
    COLLISION_OBSTACLE = 0,
    COLLISION_OTHER_CAR,
    COLLISION_PEDESTRIAN, 
    WRONG_DIRECTION, // if car is moving -180deg relative to lane trigger box forward vector
    LEVEL_LOADED,
    LEVEL_STARTED,
    LEVEL_END,
    END_GOAL_REACHED,
    ENTERED_TARGET_TRIGGER
}

public enum QUEST_EVENTS
{
    QUEST_STARTED = 0,
    QUEST_RUNNING,
    QUEST_COMPLETED,
    QUEST_FALLBACK,
    QUEST_FAILED,
}

public struct QUEST_CONDITION
{
    public int state_index;
    public bool state_needed_for_complete;
}

public class Quest : MonoBehaviour
{
    public QUEST_EVENTS state;
    public QUEST_CONDITION[] complete_conditions;
    public QUEST_CONDITION[] fallback_conditions;
    public LevelLogic logic;
    public int complete_quest_index;
    public int fallback_quest_index;

    public Quest(QUEST_CONDITION[] com_conds, QUEST_CONDITION[] fall_conds)
    {
        com_conds.CopyTo(complete_conditions, com_conds.Length);
        fall_conds.CopyTo(fallback_conditions, fall_conds.Length);
    }

    private void Update()
    {
        if (CheckCompleteConditions())
        {
            complete();
        }
        if (CheckFallbackConditions())
        {
            fallback();
        }
    }

    // Returns true if all the complete_conditions are met
    bool CheckCompleteConditions()
    {
        foreach (var condition in complete_conditions)
        {
            // Check all conditions if they're done
            if (!logic.car_states[condition.state_index] == condition.state_needed_for_complete)
            {
                return false;
            }
        }
        return true;
    }

    // Returns true if all the fallback_conditions are met
    bool CheckFallbackConditions()
    {
        foreach (var condition in fallback_conditions)
        {
            // Check all conditions if they're done
            if (!logic.car_states[condition.state_index] == condition.state_needed_for_complete)
            {
                return false;
            }
        }
        return true;
    }

    public QUEST_EVENTS complete()
    {
        logic.change_current_quest(complete_quest_index);
        return QUEST_EVENTS.QUEST_COMPLETED;
    }

    public QUEST_EVENTS fallback()
    {
        logic.change_current_quest(fallback_quest_index);
        return QUEST_EVENTS.QUEST_FALLBACK;
    }
}

public class LevelLogic : MonoBehaviour
{
    // Make a singleton of this class.
    public static LevelLogic current;
    public int level_id;

    public bool[] car_states = new bool[(int)CAR_STATES.COUNT];
    public Quest[] quests;
    public int current_quest;

    public void change_current_quest(int id)
    {
        current_quest = id;
    }

    private void Awake()
    {
        //Initiate the singleton
        current = this;
    }

    public event Action<int> on_level_started;
    public void start_level(int level_id)
    {
        this.level_id = level_id;
        on_level_started?.Invoke(level_id);

        // Add quests
    }
}
