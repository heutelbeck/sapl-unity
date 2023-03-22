using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDataManager : MonoBehaviour {
    [SerializeField] private GameObject prefabBuildUnit;
    [SerializeField] private GameObject prefabProcessingStation;
    [SerializeField] private GameObject positionForProcessingStation1;
    [SerializeField] private GameObject positionsForProcessingStations;
    [SerializeField] private GameObject positionsForBuildUnits;
    private static List<Printer> printers;
    private static List<BuildUnit> buildUnits;
    private static List<NaturalCoolingUnit> coolingUnits;
    private static List<ProcessingStation> processingStations;

    void Awake() {

        int index;
        GameObject newObject;
        List<Device> devices;
        Transform transformForNewObject;

        printers = new List<Printer>();
        printers.Add(new Printer("Printer", "Printer 1"));

        coolingUnits = new List<NaturalCoolingUnit>();
        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_1", "Cooling Unit 1"));
        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_2", "Cooling Unit 2", new PrintJob("Job1", 60), 28 * 3600 + 30 * 60 + 10));
        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_3", "Cooling Unit 3"));
        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_4", "Cooling Unit 4", new PrintJob("Job2", 60), 15 * 60 + 20));
        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_5", "Cooling Unit 5"));
        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_6", "Cooling Unit 6"));
        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_7", "Cooling Unit 7", new PrintJob("Job3", 60), 10));
        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_8", "Cooling Unit 8"));
        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_9", "Cooling Unit 9"));

        coolingUnits.Add(new NaturalCoolingUnit("NaturalCoolingUnit_19", "Cooling Unit19"));

        PrintJob printJob = new PrintJob("Test-Job", 60);

        buildUnits = new List<BuildUnit>();
        BuildUnit buildUnit = new BuildUnit("Build Unit1", "Build Unit");
        buildUnit.StartFillingInMaterial("Testmaterial");
        buildUnit.EndFillingInMaterial();
        buildUnit.StartPrinting(printJob);
        buildUnit.EndPrinting();
        buildUnits.Add(buildUnit);

        buildUnits.Add(new BuildUnit("Buildy", "Build Units 1"));

        // BuildUnits erstellen und instanziieren
        index = 0;
        devices = RESTClient.getBuildUnits();
        foreach (var device in devices) {
            buildUnits.Add(new BuildUnit(device.id, device.name));
            transformForNewObject = positionsForBuildUnits.transform.GetChild(index);
            newObject = Instantiate(prefabBuildUnit, transformForNewObject.position, prefabBuildUnit.transform.rotation);
            newObject.name = device.id;
            index = index + 1;
        }

        processingStations = new List<ProcessingStation>();
        ProcessingStation processingStation = new ProcessingStation("ProcessingStation", "ProcessingStation");
        //processingStation.AssignBuildUnit(buildUnit);
        processingStations.Add(processingStation);

        // ProcessingStations erstellen und instanziieren
        index = 0;
        devices = RESTClient.getProcessingStations();
        foreach (var device in devices) {
            processingStations.Add(new ProcessingStation(device.id, device.name));
            transformForNewObject = positionsForProcessingStations.transform.GetChild(index);
            newObject = Instantiate(prefabProcessingStation, transformForNewObject.position, prefabProcessingStation.transform.rotation);
            newObject.name = device.id;
            index = index + 1;
        }
        Debug.Log(processingStations.ToString());
    }

    public static ObjectHPJF5200 getObjectWithId(string id) {
        // CoolingUnits
        foreach (var actualCoolingUnit in coolingUnits) {
            if (actualCoolingUnit.id == id) {
                return actualCoolingUnit;
            }
        }
        // BuildUnits
        foreach (var actualBuildUnit in buildUnits)
        {
            if (actualBuildUnit.id == id)
            {
                return actualBuildUnit;
            }
        }
        // ProcessingStations
        foreach (var actualProcessingStation in processingStations) {
            if (actualProcessingStation.Id == id) {
                return actualProcessingStation;
            }
        }
      
        return null;
    }

    public static NaturalCoolingUnit getCoolingUnitObjectWithId(string id) {
        foreach (var actualCoolingUnit in coolingUnits) {
            if (actualCoolingUnit.id == id) {
                return actualCoolingUnit;
            }
        }
        return new NaturalCoolingUnit("###", "###");
    }

    public static ProcessingStation getProcessingStationObjectWithId(string id) {
        foreach (var actualProcessingStation in processingStations) {
            if (actualProcessingStation.Id == id) {
                return actualProcessingStation;
            }
        }
        return new ProcessingStation("###", "###");
    }

    public static Printer getPrinterObjectWithId(string id) {
        foreach (var actualPrinter in printers) {
            if (actualPrinter.Id == id) {
                return actualPrinter;
            }
        }
        return new Printer("###", "###");
    }

    public static BuildUnit getBuildUnitObjectWithId(string id) {
        foreach (var actualBuildUnit in buildUnits) {
            if (actualBuildUnit.id == id) {
                return actualBuildUnit;
            }
        }
        return new BuildUnit("###", "###");
    }

    public static NaturalCoolingUnit getCoolingUnitAssignedToProcessingStationWithId(string id) {
        foreach (var actualCoolingUnit in coolingUnits) {
            if (actualCoolingUnit.processingStation != null && actualCoolingUnit.processingStation.Id == id) {
                return actualCoolingUnit;
            }
        }
        return null;
    }

    public static BuildUnit getBuildUnitAssignedToPrinterWithId(string id) {
        foreach (var actualBuildUnit in buildUnits) {
            if (actualBuildUnit.processingStation != null && actualBuildUnit.processingStation.Id == id) {
                return actualBuildUnit;
            }
        }
        return null;
    }

}