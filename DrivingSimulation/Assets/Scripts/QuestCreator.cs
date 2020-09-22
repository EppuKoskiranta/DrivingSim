using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class QuestCreator : ScriptableWizard
{
    public LevelLogic level_logic;
    public Quest quest;
    public QUEST_CONDITION new_condition;
    public int level_id;
    

    [MenuItem("QuestCreator/Create quest")]

    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<QuestCreator>("Create Quest", "Create", "New");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<QuestCreator>("Create Light", "Create");
    }

    void OnWizardCreate()
    {
        level_logic.levels[level_id].quests.Add(quest);
    }

    void OnWizardUpdate()
    {
        level_logic = GameObject.FindGameObjectWithTag("levellogic").GetComponent<LevelLogic>();
        quest = new Quest();
        new_condition = new QUEST_CONDITION();
        

        helpString = "yepppp";
    }

    // When the user presses the "Apply" button OnWizardOtherButton is called.
    void OnWizardOtherButton()
    {
        // Create new quest for chosen level
        level_logic.levels[level_id] = new Level();
    }
}