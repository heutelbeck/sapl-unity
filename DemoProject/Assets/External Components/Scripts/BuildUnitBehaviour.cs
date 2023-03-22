using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildUnitBehaviour : MonoBehaviour { 

    private float timeToWait = 0.5f;
    private BuildUnit buildUnit;

    public Material materialStandard;
    public Material materialInProgress;
    public Material materialReady;

    private void Start() {
        // BuildUnit anhand des Namens des GameObjects ermitteln
        buildUnit = TestDataManager.getBuildUnitObjectWithId(gameObject.name);
    }

    void Update() {

        if (buildUnit == null) return;

        buildUnit.UpdateStatus(Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "MousePointer") {
            StopAllCoroutines();
            StartCoroutine(StartTimer());
        } else if (other.tag == "ProcessingStation") {
            buildUnit.assignmentToProcessingStationPossible = true;
            buildUnit.processingStation = TestDataManager.getProcessingStationObjectWithId(other.gameObject.name);
        } else if (other.tag == "Printer") {
            buildUnit.assignmentToPrinterPossible = true;
            buildUnit.printer = TestDataManager.getPrinterObjectWithId(other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "MousePointer") {
            StopAllCoroutines();
            BuildUnitHoverMenu.OnMouseLoseFocus();
        } else if (other.tag == "ProcessingStation") {
            buildUnit.assignmentToProcessingStationPossible = false;
        } else if (other.tag == "Printer") {
            buildUnit.assignmentToPrinterPossible = false;
        }
    }

    private IEnumerator StartTimer() {
        yield return new WaitForSeconds(timeToWait);
        ShowHoverMenu();
    }

    private void ShowHoverMenu() {
        BuildUnitHoverMenu.OnMouseHover(buildUnit, Input.mousePosition);
    }

}