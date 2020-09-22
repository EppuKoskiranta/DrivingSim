using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Count should always be true.!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
[Serializable]
public enum CAR_STATES
{
    IS_ENGINE_ON = 0,
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

[Serializable]
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
    ENTERED_TARGET_TRIGGER,
    COUNT
}

[Serializable]
public enum QUEST_EVENTS
{
    QUEST_STARTED = 0,
    QUEST_RUNNING,
    QUEST_COMPLETED,
    QUEST_FALLBACK,
    QUEST_FAILED,
    COUNT
}

[Serializable]
public struct QUEST_CONDITION
{
    public int state_index;
    public bool state_needed_for_complete;
    public QUEST_CONDITION(int i, bool s)
    {
        state_index = i;
        state_needed_for_complete = s;
    }
}

[Serializable]
public class Quest
{
    public QUEST_EVENTS state;
    public List<QUEST_CONDITION> complete_conditions = new List<QUEST_CONDITION>();
    public List<QUEST_CONDITION> fallback_conditions = new List<QUEST_CONDITION>();
    public LevelLogic logic;
    public int complete_quest_index;
    public int fallback_quest_index;

    public Quest()
    {

    }

    public void Update()
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

[Serializable]
public class Level
{
    public List<Quest> quests = new List<Quest>();
    public int id;

    List<QUEST_CONDITION> complete_condis = new List<QUEST_CONDITION>();
    List<QUEST_CONDITION> fallback_condis = new List<QUEST_CONDITION>();
    int this_index = 0;
    int c_index = 0;
    int f_index = 0;

    public CAR_STATES get_car_state_from_string(string s)
    {
        switch (s)
        {
            case "IS_ENGINE_ON":
                return CAR_STATES.IS_ENGINE_ON;
            case "IS_MOVING_FORWARD":
                return CAR_STATES.IS_MOVING_FORWARD;
            case "IS_MOVING_BACKWARD":
                return CAR_STATES.IS_MOVING_BACKWARD;
            case "IS_ACCELERATING":
                return CAR_STATES.IS_ACCELERATING;
            case "IS_BRAKING":
                return CAR_STATES.IS_BRAKING;
            case "IS_STEERING":
                return CAR_STATES.IS_STEERING;
            case "IS_MOVING_IN_RIGHT_DIRECTION":
                return CAR_STATES.IS_MOVING_IN_RIGHT_DIRECTION;
            case "IS_IN_SPEEDLIMIT":
                return CAR_STATES.IS_IN_SPEEDLIMIT;
            case "IS_IN_TARGET_TRIGGER":
                return CAR_STATES.IS_IN_TARGET_TRIGGER;
            case "COUNT":
                return CAR_STATES.COUNT;
            default:
                return CAR_STATES.COUNT;
        }
    }
    public bool get_bool_from_string(string s)
    {
        if (s == "true")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void add_quest(IEnumerable<QUEST_CONDITION> com, IEnumerable<QUEST_CONDITION> fall, int cqi, int fqi)
    {
        quests.Add(new Quest());
    }

}
