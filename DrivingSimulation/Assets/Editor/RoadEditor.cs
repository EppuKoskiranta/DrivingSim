using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(RoadCreator))]
public class RoadEditor : Editor
{
    private enum SelectedSide
    {
        NULL = 0,
        START = 1,
        END,
    }

    SelectedSide selectedSide = SelectedSide.NULL;

    private void OnSceneGUI()
    {
        CheckInput();

    }


    private void OnEnable()
    {
        selectedSide = SelectedSide.NULL;
    }



    void CheckInput()
    {
        Event e = Event.current;
        EventType eventType = Event.current.type;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (Event.current.button == 0 && e.isMouse)
                {
                    Debug.Log("Left click");
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    Vector3 mouseWorld = ray.GetPoint(10f);


                    //Lock to zx
                    float yDir = ray.direction.y;
                    if (yDir != 0)
                    {
                        float dstToXZPlane = Mathf.Abs(ray.origin.y / yDir);
                        mouseWorld = ray.GetPoint(dstToXZPlane);
                    }


                    PathCreation.PathCreator[] pathCreators = Selection.activeGameObject.GetComponentsInChildren<PathCreation.PathCreator>();

                    if (selectedSide == SelectedSide.NULL)
                    {
                        float minDisToPoint = Vector3.Distance(pathCreators[0].bezierPath.GetPoint(0), mouseWorld);
                        selectedSide = SelectedSide.START;
                        if (Vector3.Distance(pathCreators[0].bezierPath.GetPoint(pathCreators[0].bezierPath.NumPoints - 1), mouseWorld) < minDisToPoint)
                        {
                            selectedSide = SelectedSide.END;
                        }

                    }

                    if (Event.current.shift)
                    {
                        for (int i = 0; i < pathCreators.Length; ++i)
                        {
                            if (selectedSide == SelectedSide.START)
                                pathCreators[i].bezierPath.AddSegmentToStart(mouseWorld);
                            else
                                pathCreators[i].bezierPath.AddSegmentToEnd(mouseWorld);


                            pathCreators[i].TriggerPathUpdate();
                        
                        }
                    }
                    


                }
                break;
        }




        using (EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope())
        {
            // Don't allow clicking over empty space to deselect the object
            if (eventType == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(0);
            }

            if (check.changed)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
    }
}

