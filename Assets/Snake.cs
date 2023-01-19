using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Snake : MonoBehaviour
{
    public List<Transform> BodyParts = new List<Transform>();
    public float mindistance = 2.5f;
    private int beginsize = 20;
    public GameObject bodyprefab;
    public Transform agent;
    private float dis;
    private Transform curBodyPart; //current
    private Transform PrevBodypart;
    float vel_percentage;
    bool Forward;
    //float velocity = 1f;
    private Transform centralSegment;
    public float centralAngle;
    private Transform lastSegment;
    public GameObject state;
    public Transform tail;

    // Start is called before the first frame update
    void Start()
    {
        BodyParts.Add(agent); //always the first element
        for (int i = 1; i < beginsize - 1; i++)
        { AddBodyPart(); }
        BodyParts.Add(tail.transform);
    }

    // Update is called once per frame
    void Update()
    {
        curBodyPart = BodyParts[1];
        PrevBodypart = BodyParts[0];
        centralSegment = BodyParts[2];
        dis = Vector3.Distance(PrevBodypart.position, curBodyPart.position);

        // if the agent moves away I follow it, if it comes back I go backward as well
        if (dis > 0f)
        {
            Forward = true;
            UpdateBody();
        }
        else { }

        if (state.activeSelf == false)
        {
            /*GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("BodyPart");   
            foreach (GameObject part in taggedObjects) {
                part.SetActive(false);
            }
            BodyParts = new List<Transform>();
            BodyParts.Add(agent.transform); //always the first element
            for (int i = 1; i < beginsize - 1; i++){   
                AddBodyPart();
                }
            BodyParts.Add(tail.transform);
            state.SetActive(true);*/
            for (int i = 1; i < beginsize - 1; i++)
            {
                BodyParts[i].position = agent.position;
                BodyParts[i].rotation = agent.rotation;
            }
            state.SetActive(true);
        }

        centralAngle = Vector3.Angle(agent.transform.position - centralSegment.position, centralSegment.position - tail.transform.position);

    }

    public void UpdateBody()
    {
        if (Forward)
        {
            for (int i = 1; i < BodyParts.Count; i++)
            {
                curBodyPart = BodyParts[i];
                PrevBodypart = BodyParts[i - 1];
                dis = Vector3.Distance(PrevBodypart.position, curBodyPart.position);
                Vector3 newpos = PrevBodypart.position;
                //float T =  Time.deltaTime * dis / mindistance * velocity;
                //Debug.Log(Time.deltaTime.ToString());
                //if (T > 0.5f)
                //        T = 0.5f;
                float T = Time.deltaTime * dis / mindistance;
                //float T = Time.deltaTime;
                curBodyPart.position = Vector3.Slerp(curBodyPart.position, newpos, T);
                curBodyPart.rotation = Quaternion.Slerp(curBodyPart.rotation, PrevBodypart.rotation, T);
            }
        }
        else
        {
            for (int i = BodyParts.Count - 1; i > 0; i--)
            {
                curBodyPart = BodyParts[i - 1];
                PrevBodypart = BodyParts[i];
                dis = Vector3.Distance(PrevBodypart.position, curBodyPart.position);
                Vector3 newpos = PrevBodypart.position;
                newpos = BodyParts[BodyParts.Count - 1].position;
                /*float T = Time.deltaTime * dis / mindistance * velocity ;
                if (T > 0.5f)
                    T = 0.5f;*/
                float T = Time.deltaTime * dis / mindistance;
                //float T = Time.deltaTime;
                curBodyPart.position = Vector3.Slerp(curBodyPart.position, newpos, T);
                curBodyPart.rotation = Quaternion.Slerp(curBodyPart.rotation, PrevBodypart.rotation, T);
            }
        }
    }

    public void AddBodyPart()
    {
        Transform newpart = (Instantiate(bodyprefab, agent.transform.position, agent.transform.rotation) as GameObject).transform; ;
        newpart.SetParent(transform);
        BodyParts.Add(newpart);
    }
}

