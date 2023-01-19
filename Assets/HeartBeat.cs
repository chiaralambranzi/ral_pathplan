using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    public float heartCycleTime = 0.74f;
    public GameObject cylinder;
    float[] heartMovementSpeed;
    float[] MovementSpeed;
    float numCycle = 0;
    int i;
    bool isInit = true;

    private Vector3 initialPos;
    private Quaternion initialRot;

    void Awake()
    {
        //heartMovementSpeed = new float[7];
        MovementSpeed = new float[7];

    }
    void Start()
    {
        initialPos = cylinder.transform.localPosition;
        initialRot = cylinder.transform.localRotation;

        MovementSpeed[0] = 12.92516407f;
        MovementSpeed[1] = 47.33635408f;
        MovementSpeed[2] = -19.250626f;
        MovementSpeed[3] = -11.31518658f;
        MovementSpeed[4] = -22.83593966f;
        MovementSpeed[5] = -18.28874613f;
        MovementSpeed[6] = 12.76720774f;


    }
    void Update()
    {
        if ((heartCycleTime * numCycle + 0 * Time.deltaTime) < Time.time && Time.time < (heartCycleTime * numCycle + 5 * Time.deltaTime))
        {
            cylinder.transform.Translate(Vector3.up * MovementSpeed[6] * Time.deltaTime);
            //Debug.Log("num cycle:  " + numCycle.ToString() + "Time:  " + Time.time.ToString() + " speed:   " + heartMovementSpeed[6].ToString());
        }
        if ((heartCycleTime * numCycle + 5 * Time.deltaTime) < Time.time && Time.time < (heartCycleTime * numCycle + 13 * Time.deltaTime))
        {
            cylinder.transform.Translate(Vector3.up * MovementSpeed[0] * Time.deltaTime);
            //Debug.Log("num cycle:  " + numCycle.ToStrin) + "Time:  " + Time.time.ToString() + " speed:   " + heartMovementSpeed[0].ToString());
        }
        if ((heartCycleTime * numCycle + 13 * Time.deltaTime) < Time.time && Time.time < (heartCycleTime * numCycle + 15 * Time.deltaTime))
        {
            cylinder.transform.Translate(Vector3.up * MovementSpeed[1] * Time.deltaTime);
            //Debug.Log("num cycle:  " + numCycle.ToString() + "Time:  " + Time.time.ToString() + " speed:   " + heartMovementSpeed[1].ToString());
        }
        if ((heartCycleTime * numCycle + 15 * Time.deltaTime) < Time.time && Time.time < (heartCycleTime * numCycle + 21 * Time.deltaTime))
        {
            cylinder.transform.Translate(Vector3.up * MovementSpeed[2] * Time.deltaTime);
            //Debug.Log("num cycle:  " + numCycle.ToString() + "Time:  " + Time.time.ToString() + " speed:   " + heartMovementSpeed[2].ToString());
        }
        if ((heartCycleTime * numCycle + 21 * Time.deltaTime) < Time.time && Time.time < (heartCycleTime * numCycle + 25 * Time.deltaTime))
        {
            cylinder.transform.Translate(Vector3.up * MovementSpeed[3] * Time.deltaTime);
            //Debug.Log("num cycle:  " + numCycle.ToString() + "Time:  " + Time.time.ToString() + " speed:   " + heartMovementSpeed[3].ToString());
        }
        if ((heartCycleTime * numCycle + 25 * Time.deltaTime) < Time.time && Time.time < (heartCycleTime * numCycle + 28 * Time.deltaTime))
        {
            cylinder.transform.Translate(Vector3.up * MovementSpeed[4] * Time.deltaTime);
            //Debug.Log("num cycle:  " + numCycle.ToString() + "Time:  " + Time.time.ToString() + " speed:   " + heartMovementSpeed[4].ToString());
        }
        if ((heartCycleTime * numCycle + 28 * Time.deltaTime) < Time.time && Time.time < (heartCycleTime * numCycle + 35 * Time.deltaTime))
        {
            cylinder.transform.Translate(Vector3.up * MovementSpeed[5] * Time.deltaTime);
            //Debug.Log("num cycle:  " + numCycle.ToString() + "Time:  " + Time.time.ToString() + " speed:   " + heartMovementSpeed[5].ToString());
        }
        if ((heartCycleTime * numCycle + 35 * Time.deltaTime) < Time.time && Time.time < (heartCycleTime * numCycle + 37 * Time.deltaTime))
        {
            cylinder.transform.Translate(Vector3.up * MovementSpeed[6] * Time.deltaTime);
            //Debug.Log("num cycle:  " + numCycle.ToString() + "Time:  " + Time.time.ToString() + " speed:   " + heartMovementSpeed[6].ToString());
        }
        if (Time.time >= heartCycleTime * (numCycle + 1))
        {
            numCycle++;
            cylinder.transform.localPosition = initialPos;
            cylinder.transform.localRotation = initialRot;


        }
    }

}







