using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Intersection : MonoBehaviour
{
    public enum IntersectionType
    {
        LIGHTS = 0,
        EQUAL = 1,
        YIELDHORIZONTAL = 2,
        YIELDVERTICAL = 3
    }


    public enum IntersectionTurnDirection
    {
        STRAIGHT = 0,
        LEFT = 1,
        RIGHT = 2,
        INVALID = 3
    }

    [Serializable]
    public struct CarOnIntersection
    {
        public GameObject entry;
        public GameObject car;

        public CarOnIntersection(GameObject e, GameObject c)
        {
            this.entry = e;
            this.car = c;
        }
    }

    [Serializable]
    public struct PlayerOnIntersection
    {
        public GameObject entry;
        public bool inIntersection;

        public PlayerOnIntersection(GameObject e, bool isIn)
        {
            entry = e;
            inIntersection = isIn;
        }
    }


    //References
    public TrafficSystem trafficSystem;
    public Transform entriesParent;
    public Transform exitsParent;
    public TrafficLights[] trafficLights;
    public BoxCollider[] entries;
    public BoxCollider[] exits;

    public IntersectionType intersectionType;
    public LayerMask carLayerMask;


    public GameObject[] entriesWithCars;
    [SerializeField]
    public CarOnIntersection[] carsInIntersection;
    public GameObject[] exitsWithCars;
    [SerializeField]
    public CarOnIntersection[] carsLeavingIntersection;


    private bool intersectionLogicProcessing = false;


    private IntersectionTurnDirection playerDesiredTurn = IntersectionTurnDirection.INVALID;
    private IntersectionTurnDirection playerTrueTurn = IntersectionTurnDirection.INVALID;
    private GameObject playerEntry = null;

    [SerializeField]
    private PlayerOnIntersection playerOnIntersection = new PlayerOnIntersection(null, false);

    private void Start()
    {
        intersectionLogicProcessing = false;
        trafficLights = this.entriesParent.GetComponentsInChildren<TrafficLights>();
        
        if (exitsParent != null)
        {
            exits = exitsParent.GetComponentsInChildren<BoxCollider>();
        }

        if (entriesParent != null)
        {
            entries = entriesParent.GetComponentsInChildren<BoxCollider>();
        }
        

        switch (intersectionType)
        {
            case IntersectionType.EQUAL:
                DisableLights();
                DisableAllYieldSigns();
                break;
            case IntersectionType.LIGHTS:
                EnableLights();
                DisableHorizontalYieldSigns();
                EnableVerticalYieldSigns();
                break;
            case IntersectionType.YIELDHORIZONTAL:
                DisableLights();
                EnableHorizontalYieldSigns();
                DisableVerticalYieldSigns();
                break;
            case IntersectionType.YIELDVERTICAL:
                DisableLights();
                EnableVerticalYieldSigns();
                DisableHorizontalYieldSigns();
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
                if (intersectionType == IntersectionType.LIGHTS)
                    StartCoroutine(TurnLightsForEntry(entriesWithCars[0]));

                if (intersectionType == IntersectionType.EQUAL)
                {
                    //Check if car on right
                }

                if (intersectionType == IntersectionType.YIELDVERTICAL)
                {

                }

                if (intersectionType == IntersectionType.YIELDHORIZONTAL)
                {

                }
            }

            playerOnIntersection = CheckIfPlayerOnIntersection();
            //Check if player in this intersection
            if (playerOnIntersection.inIntersection)
            {
                playerDesiredTurn = DesiredTurnDirection(trafficSystem.car);
            }

        }
     
        

        if (CarsLeavingIntersection(out exitsWithCars, out carsLeavingIntersection))
        {
            for (int i = 0; i < carsLeavingIntersection.Length; ++i)
            {
                if (carsLeavingIntersection[i].car == trafficSystem.car.gameObject) //if it's player leaving
                {
                    playerTrueTurn = TrueTurnDirection(playerOnIntersection.entry, carsLeavingIntersection[i].entry);

                    if (playerDesiredTurn != playerTrueTurn)
                    {
                        Debug.Log("Blinker usage failure");
                    }

                    playerOnIntersection.inIntersection = false;
                    break;
                }
            }
        }

        //Player was on intersection but left
        if (CheckThatPlayerLeftEntry())
        {
            //Check that there wasnt car on right side if equal intersection
            //Equal check
            if (intersectionType == IntersectionType.EQUAL)
            {

            }
            //Lights check
            else if (intersectionType == IntersectionType.LIGHTS)
            {
                TrafficLights.LightState state = playerOnIntersection.entry.GetComponentInChildren<TrafficLights>().GetState();
                //if light was YELLOW or GREEN ===> OK
                if (state == TrafficLights.LightState.GREEN || state == TrafficLights.LightState.YELLOW)
                {
                    //OK
                }
                //if light was RED or YELLOWRED ==> NO
                if (state == TrafficLights.LightState.RED || state == TrafficLights.LightState.YELLOWRED)
                {
                    //Drove through reds!!!! NOT GUD >:(
                    Debug.Log("Drove reds!!");
                }
            }
            else if (intersectionType == IntersectionType.YIELDHORIZONTAL)
            {

            }
            else if (intersectionType == IntersectionType.YIELDVERTICAL)
            {

            }
        }
    }

    public IntersectionTurnDirection DesiredTurnDirection(Car c)
    {
        if (c.lights.leftBlinker)
            return IntersectionTurnDirection.LEFT;
        else if (c.lights.rightBlinker)
            return IntersectionTurnDirection.RIGHT;
        else
            return IntersectionTurnDirection.STRAIGHT;
    }

    public IntersectionTurnDirection TrueTurnDirection(GameObject entry, GameObject exit)
    {
        if (entry.transform.parent == null || exit.transform.parent == null)
            return IntersectionTurnDirection.INVALID;


        //0, 1, 2, 3 are the indeces

        int entrySiblingIndex = entry.transform.GetSiblingIndex();
        int exitSiblingIndex = exit.transform.GetSiblingIndex();

        int overflow = 4;
        //difference exit - entry
        //1 = right
        //2 = straight
        //3 = left


        //Calculate if we go over 3 when we turn. Ignore if entry is 0

        //default overflow value begin at = 4

        /*
         * exit - entry
            case 1
        1-2 = -1
        1-3 = -2
        1-0 = 1 !!!!! > 0 ==> increment exit with 1-1 = 0 ==> exit = 4 correct

            case 2
        2-3 = -1
        2-0 = 2 !!!!! > 0 ==> increment exit with 2-2 = 0 ==> exit = 4 correct
        2-1 = 1 !!!!! > 0 ==> increment exit with 2-1 = 1 ==> exit = 5 correct

            case 3
        3-0 = 3 !!!!! > 0 ==> increment exit with 3-3 = 0 ==> exit = 4 correct
        3-1 = 2 !!!!! > 0 ==> increment exit with 3-2 = 1 ==> exit = 5 correct
        3-2 = 1 !!!!! > 0 ==> increment exit with 3-1 = 2 ==> exit = 6 correct
         * */

        //calulate increment value for overflow if neccessary
        if (entrySiblingIndex != 0)
        {
            int surplus = entrySiblingIndex - exitSiblingIndex;
            if (surplus > 0)
            {
                int increment = entrySiblingIndex - surplus;
                exitSiblingIndex = overflow + increment;
            }
        }

        int turn = exitSiblingIndex - entrySiblingIndex;

        switch (turn)
        {
            case 1:
                return IntersectionTurnDirection.RIGHT;
            case 2:
                return IntersectionTurnDirection.STRAIGHT;
            case 3:
                return IntersectionTurnDirection.LEFT;
            default:
                return IntersectionTurnDirection.INVALID;
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

    private void DisableAllYieldSigns()
    {
        for (int i = 0; i < entries.Length; ++i)
        {
            entries[i].transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    private void EnableAllYieldSigns()
    {
        for (int i = 0; i < entries.Length; ++i)
        {
            entries[i].transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    private void DisableHorizontalYieldSigns()
    {
        entries[0].transform.GetChild(2).gameObject.SetActive(false);
        entries[2].transform.GetChild(2).gameObject.SetActive(false);
    }

    private void EnableHorizontalYieldSigns()
    {
        entries[0].transform.GetChild(2).gameObject.SetActive(true);
        entries[2].transform.GetChild(2).gameObject.SetActive(true);
    }

    private void DisableVerticalYieldSigns()
    {
        entries[1].transform.GetChild(2).gameObject.SetActive(false);
        entries[3].transform.GetChild(2).gameObject.SetActive(false);
    }

    private void EnableVerticalYieldSigns()
    {
        entries[1].transform.GetChild(2).gameObject.SetActive(true);
        entries[3].transform.GetChild(2).gameObject.SetActive(true);
    }

    public bool CheckThatPlayerLeftEntry()
    {
        if (playerOnIntersection.inIntersection)
        {
            for (int i = 0; i < carsInIntersection.Length; ++i)
            {
                if (carsInIntersection[i].car == trafficSystem.car.gameObject)
                    return false;
            }

            return true;
        }

        return false;
    }

    PlayerOnIntersection CheckIfPlayerOnIntersection()
    {
        for (int i = 0; i < carsInIntersection.Length; ++i)
        {
            if (carsInIntersection[i].car == trafficSystem.car.gameObject)
            {
                PlayerOnIntersection onIntersection = new PlayerOnIntersection(carsInIntersection[i].entry, true);
                return onIntersection;
            } 
        }

        PlayerOnIntersection onIntersectionInvalid = new PlayerOnIntersection(null, false);
        return onIntersectionInvalid;
    }

    bool CarsLeavingIntersection(out GameObject[] exitArray, out CarOnIntersection[] cars)
    {
        bool hit = false;
        List<GameObject> exitsTemp = new List<GameObject>();
        List<CarOnIntersection> carsTemp = new List<CarOnIntersection>();
        Collider[] colliders;
        for (int i = 0; i < exits.Length; ++i)
        {
            colliders = Physics.OverlapBox(exits[i].transform.position + exits[i].center, exits[i].size / 2, exits[i].gameObject.transform.rotation, carLayerMask);

            if (colliders.Length > 0)
            {
                hit = true;
                exitsTemp.Add(exits[i].gameObject);
                for (int j = 0; j < colliders.Length; ++j)
                {
                    Transform p = colliders[j].transform;
                    Transform target = null;
                    while (p != null)
                    {
                        if (p.parent == null)
                            target = p;
                        p = p.parent;
                    }

                    if (target != null)
                    {
                        CarOnIntersection intersectionCar = new CarOnIntersection(exits[i].gameObject, target.gameObject);
                        if (!carsTemp.Contains(intersectionCar))
                            carsTemp.Add(intersectionCar);
                    }
                }
            }
        }

        if (exitsTemp.Count > 0)
            exitArray = exitsTemp.ToArray();
        else
            exitArray = new GameObject[0];

        if (carsTemp.Count > 0)
            cars = carsTemp.ToArray();
        else
            cars = new CarOnIntersection[0];

        return hit;
    }

    bool CarsWaitingOnIntersection(out GameObject[] entryArray, out CarOnIntersection[] cars)
    {
        bool hit = false;
        List<GameObject> entriesTemp = new List<GameObject>();
        List<CarOnIntersection> carsTemp = new List<CarOnIntersection>();
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

                    Transform p = colliders[j].transform;
                    Transform target = null;
                    while (p != null)
                    {
                        if (p.parent == null)
                            target = p;
                        p = p.parent;
                    }


                    if (target != null)
                    {
                        CarOnIntersection intersectionCar = new CarOnIntersection(entries[i].gameObject, target.gameObject);
                        if (!carsTemp.Contains(intersectionCar))
                            carsTemp.Add(intersectionCar);
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
            cars = new CarOnIntersection[0];

        return hit;

    }

}


