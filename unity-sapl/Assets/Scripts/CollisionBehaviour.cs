using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBehaviour : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("CollisionBehaviour Collision");
    }
}