using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class RoadCreator : MonoBehaviour
{
    public PathCreation.PathCreator[] paths;


    private void Awake()
    {
        int size = this.transform.childCount;
        paths = new PathCreation.PathCreator[size];

        for (int i = 0; i < size; ++i)
        {
            paths[i] = this.transform.GetChild(i).GetComponent<PathCreation.PathCreator>();
        }
    }
}
