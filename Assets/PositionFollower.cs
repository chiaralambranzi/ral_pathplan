using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFollower : MonoBehaviour
{
    public Transform Target;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = Target.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Target.position;
    }
}
