using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
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
    public CAR_STATES state_index;
    public bool state_needed_for_complete;
    public QUEST_CONDITION(CAR_STATES i, bool s)
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
    public string instructions;
    public int id;
    public int complete_quest_id;
    public int fallback_quest_id;
    private bool[] car_states = new bool[(int)CAR_STATES.COUNT + 1];

    public Quest(List<QUEST_CONDITION> com, List<QUEST_CONDITION> fall, string instr, int id, int com_id, int fall_id)
    {
        complete_conditions.AddRange(com);
        fallback_conditions.AddRange(fall);
        instructions = instr;
        this.id = id;
        complete_quest_id = com_id;
        fallback_quest_id = fall_id;
    }

    public int UpdateFromLevel(bool[] car_states)
    {
        this.car_states = car_states;
        Debug.Log("INSTRUCTIONS: " + instructions);
        if (CheckCompleteConditions())
        {
            return complete();
        }
        if (CheckFallbackConditions())
        {
            return fallback();
        }
        // Default value
        return id;
    }

    // Returns true if all the complete_conditions are met
    bool CheckCompleteConditions()
    {
        if (complete_conditions.Count > 0)
        {
            foreach (var condition in complete_conditions)
            {
                // Check all conditions if they're done
                if (car_states[(int)condition.state_index] != condition.state_needed_for_complete)
                {
                    return false;
                }
            }
            return true;
            }   
        else
        {
            return false;
        }
    }

    // Returns true if all the fallback_conditions are met
    bool CheckFallbackConditions()
    {
        if (fallback_conditions.Count > 0)
        {
            foreach (var condition in fallback_conditions)
            {
                // Check all conditions if they're done
                if (car_states[(int)condition.state_index] != condition.state_needed_for_complete)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public int complete()
    {
        Debug.LogError("Completed quest: " + id);
        return complete_quest_id;
    }

    public int fallback()
    {
        Debug.LogError("Fallbacked quest: " + id);
        return fallback_quest_id;
    }
}

[Serializable]
public class Level
{
    public List<Quest> quests = new List<Quest>();
    public Quest current_quest;
    List<QUEST_CONDITION> complete_condis = new List<QUEST_CONDITION>();
    List<QUEST_CONDITION> fallback_condis = new List<QUEST_CONDITION>();

    // Returns the next quest id, or the inputted id if not changed
    public int UpdateFromLL(int current_quest_id, bool[] car_states)
    {
        // Finds the quest with current_quest_id in quests list
        current_quest = quests.Find(element => element.id == current_quest_id);
        if (current_quest != null)
        {
            return current_quest.UpdateFromLevel(car_states);
        }
        return current_quest_id;
    }

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

}
