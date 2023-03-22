using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataProvider : MonoBehaviour {

    private int indexNextFreePositionProcessingStation;
    [SerializeField] private GameObject prefabProcessingStation;
    [SerializeField] private GameObject positionsForProcessingStations;

    private int indexNextFreePositionPrinter;
    [SerializeField] private GameObject prefabPrinter;
    [SerializeField] private GameObject positionsForPrinters;

    private int indexNextFreePositionBuildUnit;
    [SerializeField] private GameObject prefabBuildUnit;
    [SerializeField] private GameObject positionsForBuildUnits;

    private int indexNextFreePositionCoolingUnit;
    [SerializeField] private GameObject prefabCoolingUnit;
    [SerializeField] private GameObject positionsForCoolingUnits;

    GameObject newObject;

    void Start() {

        for (int i = 0; i < positionsForProcessingStations.transform.childCount - 2; i++) {
            AddProcessingStation();
        }

        for (int i = 0; i < positionsForPrinters.transform.childCount - 2; i++) {
            AddPrinter();
        }

        for (int i = 0; i < positionsForBuildUnits.transform.childCount - 2; i++) {
            AddBuildUnit();
        }

        for (int i = 0; i < positionsForCoolingUnits.transform.childCount - 2; i++) {
            AddCoolingUnit();
        }

    }

    private void AddObject(string name, GameObject prefab, GameObject positions, ref int indexNextFreePosition) {
        if (indexNextFreePosition < positions.transform.childCount) {
            newObject = Instantiate(prefab,
                                    positions.transform.GetChild(indexNextFreePosition).position,
                                    prefab.transform.rotation);
            newObject.name = name + (indexNextFreePosition + 1);
            indexNextFreePosition++;
        } else {
            Debug.Log("(#) No free space for " + name + " anymore");
        }
    }

    public void AddProcessingStation() {
        AddObject("ProcessingStation", prefabProcessingStation, positionsForProcessingStations, ref indexNextFreePositionProcessingStation);
    }

    public void AddPrinter() {
        AddObject("Printer", prefabPrinter, positionsForPrinters, ref indexNextFreePositionPrinter);
    }

    public void AddBuildUnit() {
        AddObject("BuildUnit", prefabBuildUnit, positionsForBuildUnits, ref indexNextFreePositionBuildUnit);
    }

    public void AddCoolingUnit() {
        AddObject("CoolingUnit", prefabCoolingUnit, positionsForCoolingUnits, ref indexNextFreePositionCoolingUnit);
    }

}