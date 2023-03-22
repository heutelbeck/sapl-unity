using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Steuert Mausinteraktion mit Constraint-UI
public class MouseDownConstraintPicker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Constraints constraints;
    [SerializeField] private GameObject FirstPersonPlayer;


    void Awake()
    {
        // Zugriff auf die Constraints-Klasse
        constraints = FirstPersonPlayer.GetComponent<Constraints>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Constraint-Button setzt Constraints in Constraints-Klasse
        if (name == "Worker")
        {
            constraints.user = name;
            print(constraints.user);
        }

        if (name == "Engineer")
        {
            constraints.user = name;
            print(constraints.user);
        }

        if (name == "off-time")
        {
            constraints.time = "off-time";
            print(constraints.time);
        }

        if (name == "Emergency!")
        {
            constraints.emergencyStatus = "emergency";
            print(constraints.emergencyStatus);
        }
        
        if (name == "Run job")
        {
            constraints.jobStatus = "running";
            print(constraints.jobStatus);
        }

        if (name == "Emergency!")
        {
            constraints.emergencyStatus = "emergency";
            print(constraints.emergencyStatus);
        }

        if (name == "Health low")
        {
            constraints.healthStatus = "low";
            print(constraints.healthStatus);
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
