using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    public enum IntersectionType
    {
        LIGHTS = 0,
        EQUAL = 1,
        YIELDHORIZONTAL = 2,
        YIELDVERTICAL = 3
    }


    //References
    public TrafficSystem trafficSystem;
    public Transform entriesParent;
    public Transform exitsParent;
    public TrafficLights[] trafficLights;
    public BoxCollider[] entries;
    public BoxCollider[] exists;

    public IntersectionType intersectionType;
    public LayerMask carLayerMask;


    public GameObject[] entriesWithCars;
    public GameObject[] carsInIntersection;


    private bool intersectionLogicProcessing = false;

    private void Start()
    {
        intersectionLogicProcessing = false;
        trafficLights = this.entriesParent.GetComponentsInChildren<TrafficLights>();
        
        if (exitsParent != null)
        {
            exists = exitsParent.GetComponentsInChildren<BoxCollider>();
        }

        if (entriesParent != null)
        {
            entries = entriesParent.GetComponentsInChildren<BoxCollider>();
        }
        

        switch (intersectionType)
        {
            case IntersectionType.EQUAL:
                DisableLights();
                DisableYieldSigns();
                break;
            case IntersectionType.LIGHTS:
                EnableLights();
                DisableYieldSigns();
                break;
            case IntersectionType.YIELDHORIZONTAL:

                break;
        }
    }


    private void Update()
    {

        if (CarsWaitingOnIntersection(out entriesWithCars, out carsInIntersection))
        {
            Debug.Log("Cars on intersection");
            if (!intersectionLogicProcessing)
            {
                StartCoroutine(TurnLightsForEntry(entriesWithCars[0]));
            }

            if (CheckIfPlayerOnIntersection())
            {
                Debug.Log("Player on intersection");
            }
        }
    }


    public IEnumerator TurnLightsForEntry(GameObject entry)
    {
        Debug.Log("Started courutine");
        intersectionLogicProcessing = true;
        StartCoroutine(entry.transform.GetChild(0).GetComponent<TrafficLights>().TurnGreen());
        StartCoroutine(entry.transform.GetChild(1).GetComponent<TrafficLights>().TurnGreen());
        yield return new WaitForSecondsRealtime(5f);
        StartCoroutine(entry.transform.GetChild(0).GetComponent<TrafficLights>().TurnRed());
        StartCoroutine(entry.transform.GetChild(1).GetComponent<TrafficLights>().TurnRed());
        intersectionLogicProcessing = false;
    }


    private void DisableLights()
    {
        if (entriesParent != null)
        {
            foreach (TrafficLights t in trafficLights)
            {
                t.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < trafficLights.Length; ++i)
            {
                trafficLights[i].gameObject.SetActive(false);
            }
        }
    }

    private void EnableLights()
    {
        if (entriesParent != null)
        {
            foreach (TrafficLights t in trafficLights)
            {
                t.gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < trafficLights.Length; ++i)
            {
                trafficLights[i].gameObject.SetActive(true);
            }
        }
    }

    private void DisableYieldSigns()
    {

    }

    private void EnableYieldSigns()
    {

    }


    bool CheckIfPlayerOnIntersection()
    {
        for (int i = 0; i < carsInIntersection.Length; ++i)
        {
            if (carsInIntersection[i] == trafficSystem.car.gameObject)
            {
                return true;
            }
        }

        return false;
    }


    bool CarsWaitingOnIntersection(out GameObject[] entryArray, out GameObject[] cars)
    {
        bool hit = false;
        List<GameObject> entriesTemp = new List<GameObject>();
        List<GameObject> carsTemp = new List<GameObject>();
        Collider[] colliders;
        for (int i = 0; i < entries.Length; ++i)
        {
            colliders = Physics.OverlapBox(entries[i].transform.position + entries[i].center, entries[i].size / 2, entries[i].gameObject.transform.rotation, carLayerMask);

            if (colliders.Length > 0)
            {
                hit = true;
                entriesTemp.Add(entries[i].gameObject);
                for (int j = 0; j < colliders.Length; ++j)
                {

                    Transform p = colliders[i].transform;
                    Transform target = null;
                    while (p != null)
                    {
                        if (p.parent == null)
                            target = p;
                        p = p.parent;
                    }


                    if (target != null)
                    {
                        if (!carsTemp.Contains(target.gameObject))
                            carsTemp.Add(target.gameObject);
                    }
                }
            }
        }

        if (entriesTemp.Count > 0)
            entryArray = entriesTemp.ToArray();
        else
            entryArray = new GameObject[0];

        if (carsTemp.Count > 0)
            cars = carsTemp.ToArray();
        else
            cars = new GameObject[0];

        return hit;

    }

}


