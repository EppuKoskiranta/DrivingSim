using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class QuestCreator : ScriptableWizard
{
    public LevelLogic level_logic;
    public int level_index;
    public List<QUEST_CONDITION> complete_conditions = new List<QUEST_CONDITION>();
    public List<QUEST_CONDITION> fallback_conditions = new List<QUEST_CONDITION>();
    public string instr;
    public int this_id;
    public int complete_id;
    public int fallback_id;
    

    [MenuItem("QuestCreator/Create quest")]

    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<QuestCreator>("Create Quest", "Exit", "Apply");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<QuestCreator>("Create Light", "Create");
    }

    void OnWizardCreate()
    {
        
    }

    void OnWizardUpdate()
    {
        level_logic = GameObject.FindGameObjectWithTag("levellogic").GetComponent<LevelLogic>();

        helpString = "Create a new quest";
    }

    // When the user presses the "Apply" button OnWizardOtherButton is called.
    void OnWizardOtherButton()
    {
        // Create new quest for chosen level
        level_logic.levels[level_index].quests.Add(
            new Quest(complete_conditions, fallback_conditions,
            instr, this_id, complete_id, fallback_id));
    }
}