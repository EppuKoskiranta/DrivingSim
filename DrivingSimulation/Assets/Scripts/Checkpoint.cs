using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool activated = false;
    public static GameObject[] CheckpointsList;
    GameObject player;

    void Start()
    {
        CheckpointsList = GameObject.FindGameObjectsWithTag("Checkpoint");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void ActivateCheckPoint()
    {
        foreach (GameObject cp in CheckpointsList)
        {
            //TODO so we cant infinitely activate just one checkpoint.
            //all must be activated before the level can end
            //also in order I guess /shrug
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ActivateCheckPoint();
            Debug.Log("Checkpoint!");
        }
    }
}
