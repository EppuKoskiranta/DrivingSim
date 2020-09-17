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


// Count should always be true.!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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

public enum QUEST_EVENTS
{
    QUEST_STARTED = 0,
    QUEST_RUNNING,
    QUEST_COMPLETED,
    QUEST_FALLBACK,
    QUEST_FAILED,
    COUNT
}

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

public class Quest
{
    public QUEST_EVENTS state;
    public List<QUEST_CONDITION> complete_conditions = new List<QUEST_CONDITION>();
    public List<QUEST_CONDITION> fallback_conditions = new List<QUEST_CONDITION>();
    public LevelLogic logic;
    public int complete_quest_index;
    public int fallback_quest_index;

    public Quest(IEnumerable<QUEST_CONDITION> com_conds, IEnumerable<QUEST_CONDITION> fall_conds, int cqi, int fqi)
    {
        complete_conditions = (List<QUEST_CONDITION>) com_conds;
        fallback_conditions = (List<QUEST_CONDITION>) fall_conds;
        complete_quest_index = cqi;
        fallback_quest_index = fqi;
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

public class Level
{
    public List<Quest> quests = new List<Quest>();
    public int id;
    public string file_name;

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

    public Level(int id, string file_name)
    {
        this.id = id;
        int counter = 0;
        string line;
        int q_index = 0;
        bool q_bool = false;
        int c_index = 0;
        int f_index = 0;
        List<QUEST_CONDITION> complete_condis = new List<QUEST_CONDITION>();
        List<QUEST_CONDITION> fallback_condis = new List<QUEST_CONDITION>();

        // Read the file and display it line by line.  
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/Levels/" + file_name);
        while ((line = file.ReadLine()) != null)
        {
            counter++;
            string[] all = line.Split('|');
            //for (int i = 0; i < str.Length; i++)
            //{
            //    UnityEngine.Debug.Log(str[i]);
            //}
            string[] indices_and_complete = all[0].Split('#');
            string[] indices = indices_and_complete[0].Split(',');
            for (int i = 0; i < indices.Length; i++)
            {
                if (i == 0 | i % 2 == 0)
                {
                    UnityEngine.Debug.Log("ok what now:" + indices[i]);
                    c_index = Int32.Parse(indices[i], NumberStyles.Integer | NumberStyles.AllowThousands,new CultureInfo("en-GB"));
                }
                else
                {
                    f_index = Int32.Parse(indices[i], NumberStyles.Integer | NumberStyles.AllowThousands, new CultureInfo("en-GB"));
                }
            }
            string[] complete = indices_and_complete[1].Split(':');
            for (int i = 0; i < complete.Length; i++)
            {
                if (i % 2 == 0 || i == 0)
                {
                    q_index = (int) get_car_state_from_string(complete[i]);
                }
                else
                {
                    q_bool = get_bool_from_string(complete[i]);
                    complete_condis.Add(new QUEST_CONDITION(q_index , q_bool));
                }
            }
            for (int i = 0; i < all.Length; i++)
            {
                if (i % 2 != 0)
                {
                    string[] fallbacks = all[i].Split(':');
                    for (int j = 0; j < fallbacks.Length; j++)
                    {
                        if (j % 2 == 0 || j == 0)
                        {
                            q_index = (int)get_car_state_from_string(complete[j]);
                        }
                        else
                        {
                            q_bool = get_bool_from_string(complete[i]);
                            fallback_condis.Add(new QUEST_CONDITION(q_index, q_bool));
                        }
                    }
                }
            }
            if (complete_condis != null && fallback_condis != null)
            {
                quests.Add(new Quest(complete_condis, fallback_condis, c_index, f_index));
            }
            complete_condis.Clear();
            fallback_condis.Clear();
            UnityEngine.Debug.Log("count of Cconditions: " + complete_condis.Count);
            UnityEngine.Debug.Log("count of Fconditions: " + fallback_condis.Count);
        }

        file.Close();
    }

    public void add_quest(IEnumerable<QUEST_CONDITION> com, IEnumerable<QUEST_CONDITION> fall, int cqi, int fqi)
    {
        quests.Add(new Quest(com, fall, cqi, fqi));
    }

}

public class LevelLogic : MonoBehaviour
{
    // Make a singleton of this class.
    public List<Level> levels = new List<Level>();
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
        levels.Add(new Level(0, "level1.txt"));

        quests.AddRange(levels[0].quests);
    }

    public event Action<int> on_level_started;
    public void start_level(int level_id)
    {
        on_level_started?.Invoke(level_id);
        this.level_id = level_id;
    }
}
