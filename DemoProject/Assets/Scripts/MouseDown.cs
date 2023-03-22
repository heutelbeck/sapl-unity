using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Steuert Mausinteraktion mit Printer und Processing Station
public class MouseDown : MonoBehaviour
{
    Constraints constraints;
    [SerializeField] GameObject FirstPersonPlayer;

    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject buttonRemoveJob;
    [SerializeField] private GameObject buttonCreatePrintJob;
    [SerializeField] private GameObject buttonGetDeviceStatus;
    [SerializeField] private GameObject buttonGetProcessingStationOps;
    [SerializeField] private GameObject buttonGetProcessingStationPowderTanksInfo;
    // [SerializeField] private GameObject MenuEngineer;

    private bool inRange;

    private void Awake()
    {
        // Zugriff auf die Constraints-Klasse
        constraints = FirstPersonPlayer.GetComponent<Constraints>();

        // Menü deaktivieren und jeweilige Buttons aktivieren
        Menu.SetActive(false);

        if (name == "Printer")
        {
            buttonCreatePrintJob.SetActive(true);
            buttonGetDeviceStatus.SetActive(true);
            buttonRemoveJob.SetActive(true);
        }

        if (name == "ProcessingStation")
        {
            buttonGetProcessingStationOps.SetActive(true);
            buttonGetProcessingStationPowderTanksInfo.SetActive(true);
        }

        // Nähe zu Printer oder Processing Station
        inRange = false;
    }

    private void OnMouseEnter()
    {
        print((this.name));
    }

    private void OnTriggerEnter(Collider other)
    {
        inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        // Wenn außer Reichweite Menü, Buttons und  Constraints zurücksetzen
        inRange = false;
        Menu.SetActive(false);

        
        buttonCreatePrintJob.SetActive(true);
        buttonGetDeviceStatus.SetActive(true);
        buttonRemoveJob.SetActive(true);

        buttonGetProcessingStationOps.SetActive(true);
        buttonGetProcessingStationPowderTanksInfo.SetActive(true);

        constraints.user = "no_restrictions";
        constraints.time = "no_restrictions";
        constraints.jobStatus = "no_restrictions";
        constraints.healthStatus = "no_restrictions";
        constraints.emergencyStatus = "no_restrictions";

    }

    private void OnMouseDown()
    {
        // Wenn in Nähe von Printer oder Processing Station, Menü anzeigen und Buttons, je nach Constraints, deaktivieren.
        if (inRange == true)
        {
            Menu.SetActive(true);

            if (name == "Printer") {

                if (constraints.user == "Worker")
                {
                    buttonGetDeviceStatus.SetActive(false);
                    buttonRemoveJob.SetActive(false);

                    buttonGetProcessingStationOps.SetActive(false);
                    buttonGetProcessingStationPowderTanksInfo.SetActive(false);
                }

                if (constraints.user == "Engineer")
                {
                    buttonCreatePrintJob.SetActive(false);
                    buttonRemoveJob.SetActive(false);

                    buttonGetProcessingStationOps.SetActive(false);
                    buttonGetProcessingStationPowderTanksInfo.SetActive(false);
                }

                if (constraints.healthStatus == "low")
                {
                    buttonCreatePrintJob.SetActive(false);
                    buttonRemoveJob.SetActive(false);

                    buttonGetProcessingStationOps.SetActive(false);
                    buttonGetProcessingStationPowderTanksInfo.SetActive(false);
                }

                if (constraints.time == "off-time")
                {
                    buttonCreatePrintJob.SetActive(false);
                    buttonGetDeviceStatus.SetActive(false);
                    buttonRemoveJob.SetActive(false);

                    buttonGetProcessingStationOps.SetActive(false);
                    buttonGetProcessingStationPowderTanksInfo.SetActive(false);
                }

                if (constraints.jobStatus == "running")
                {
                    buttonCreatePrintJob.SetActive(false);
                    buttonRemoveJob.SetActive(false);

                    buttonGetProcessingStationOps.SetActive(false);
                    buttonGetProcessingStationPowderTanksInfo.SetActive(false);
                }

                if (constraints.emergencyStatus == "emergency")
                {
                    buttonCreatePrintJob.SetActive(false);
                    buttonGetDeviceStatus.SetActive(true);
                    buttonRemoveJob.SetActive(true);

                    buttonGetProcessingStationOps.SetActive(false);
                    buttonGetProcessingStationPowderTanksInfo.SetActive(false);
                }

            }

            if (name == "ProcessingStation")
            {

                if (constraints.user == "Worker")
                {
                    buttonGetProcessingStationPowderTanksInfo.SetActive(false);

                    buttonCreatePrintJob.SetActive(false);
                    buttonGetDeviceStatus.SetActive(false);
                    buttonRemoveJob.SetActive(false);
                }

                if (constraints.user == "Engineer")
                {
                    buttonGetProcessingStationOps.SetActive(false);

                    buttonCreatePrintJob.SetActive(false);
                    buttonGetDeviceStatus.SetActive(false);
                    buttonRemoveJob.SetActive(false);
                }

                if (constraints.healthStatus == "low")
                {
                    buttonGetProcessingStationOps.SetActive(false);

                    buttonCreatePrintJob.SetActive(false);
                    buttonGetDeviceStatus.SetActive(false);
                    buttonRemoveJob.SetActive(false);
                }

                if (constraints.time == "off-time")
                {
                    buttonGetProcessingStationOps.SetActive(false);
                    buttonGetProcessingStationPowderTanksInfo.SetActive(false);

                    buttonCreatePrintJob.SetActive(false);
                    buttonGetDeviceStatus.SetActive(false);
                    buttonRemoveJob.SetActive(false);
                }

                if (constraints.jobStatus == "running")
                {
                    buttonCreatePrintJob.SetActive(false);
                    buttonGetDeviceStatus.SetActive(false);
                    buttonRemoveJob.SetActive(false);
                }

                if (constraints.emergencyStatus == "emergency")
                {
                    buttonGetProcessingStationOps.SetActive(true);
                    buttonGetProcessingStationPowderTanksInfo.SetActive(true);

                    buttonCreatePrintJob.SetActive(false);
                    buttonGetDeviceStatus.SetActive(false);
                    buttonRemoveJob.SetActive(false);
                }

            }             

        }
        
    }

}
