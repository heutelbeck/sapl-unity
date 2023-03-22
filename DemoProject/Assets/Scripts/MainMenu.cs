using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sapl.Components;

public class MainMenu : MonoBehaviour
{
    public GameObject panelMainMenu;
    public GameObject prefabMainMenuButton;

    public void load(string user)
    {

        //if (APIManager.instance.IsViewPrinterStatusEnabled(user) == true)
        //{
        EventMethodEnforcement script = gameObject.GetComponent<EventMethodEnforcement>();
        script.ExecuteMethod();

            //addButton("Gerätestatus anzeigen");
        //}

        //if (APIManager.instance.IsViewPrintJobsEnabled(user) == true)
        //{
            //addButton("Druckaufträge anzeigen");
        //}

        //if (APIManager.instance.IsStartPrintJobEnabled(user) == true)
        //{
            //addButton("Druckauftrag starten");
        //}

        //if (APIManager.instance.IsViewMaterialEnabled(user) == true)
        //{
            //addButton("Material anzeigen");
        //}

        //if (APIManager.instance.IsAddMaterialEnabled(user) == true)
        //{
            //addButton("Material hinzufügen");
        //}

        //if (APIManager.instance.IsCheckPrintHeadsEnabled(user) == true)
        //{
            //addButton("Druckköpfe prüfen");
        //}

    }

    public void addButton(string text)
    {
        GameObject button;

        button = Instantiate(prefabMainMenuButton);
        button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = text;
        button.transform.SetParent(panelMainMenu.transform, false);
    }

    public void unload()
    {

        List<GameObject> objectsInScene = new List<GameObject>();
        foreach (GameObject gameObjectTemp in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (gameObjectTemp.tag == "ButtonMainMenu")
            {
                Destroy(gameObjectTemp);
            }
        }

    }
}