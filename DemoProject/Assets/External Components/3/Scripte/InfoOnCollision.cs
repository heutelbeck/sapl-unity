using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoOnCollision : MonoBehaviour
{
    public GameObject infopanel;
    public GameObject menupanel;

    public void OnTriggerEnter(Collider collision)
    {
        infopanel.SetActive(true);
        Console.WriteLine("Infobox aktiviert");
        
    }
    public void OnTriggerExit(Collider collision)
        
    {
        infopanel.SetActive(false);
        Console.WriteLine("infobox deaktiviert");
        

    }

    public void Update()
    {
      if (menupanel.activeSelf)
        {
            infopanel.SetActive(false);
        }
    }
}
