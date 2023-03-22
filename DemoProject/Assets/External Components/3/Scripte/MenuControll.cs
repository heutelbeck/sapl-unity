using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuControll : MonoBehaviour
{
    public GameObject Panel;
    public GameObject PreviousPanel;
    public void ClosePanel()
    {
        if (Panel != null)

            Panel.SetActive(false);
           
    }

    public void OpenPanel()
    {
        if (Panel != null)

            Panel.SetActive(true);
            

    }

   
}
