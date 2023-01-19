using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatheterCollision : MonoBehaviour
{
    public int collisionValue = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "BodyPart")
        {
            collisionValue = 1;
        }
    }
}
