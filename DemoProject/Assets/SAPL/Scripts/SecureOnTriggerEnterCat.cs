using Sapl.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecureOnTriggerEnterCat : MonoBehaviour
{
    private EventMethodEnforcement eventMethodEnforcement;

    public void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<TutorialStepMove>().TutorialStepMoveNext(3);

        eventMethodEnforcement = gameObject.GetComponent<EventMethodEnforcement>();
        eventMethodEnforcement.ExecuteMethod();
    }

    public void OnTriggerStay(Collider other)
    {
        eventMethodEnforcement = gameObject.GetComponent<EventMethodEnforcement>();
        eventMethodEnforcement.ExecuteMethod();
    }

    public void MoveAway()
    {
        //Set walkAway parameter of Cat's Animator to true
        gameObject.GetComponent<Animator>().SetBool("walkAway", true);
    }
}
