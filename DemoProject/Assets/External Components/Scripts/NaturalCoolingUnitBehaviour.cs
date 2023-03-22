using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NaturalCoolingUnitBehaviour : MonoBehaviour {

    private float timeToWait = 0.5f;
    private NaturalCoolingUnit coolingUnit;

    public Material materialStandard;
    public Material materialInLongProgress;
    public Material materialInShortProgress;
    public Material materialReady;

    private void Start() {
        // CoolingUnit anhand des Namens des GameObjects ermitteln
        coolingUnit = TestDataManager.getCoolingUnitObjectWithId(gameObject.name);
    }

    void Update() {

        if (coolingUnit == null) return;

        coolingUnit.UpdateStatus(Time.deltaTime);

        if (coolingUnit.status != Status_CoolingUnit.InProgress_Cooling &&
            coolingUnit.status != Status_CoolingUnit.Finished_Cooling) {
            gameObject.GetComponent<Renderer>().material = materialStandard;
        } else {
            float remainingCoolingTime = coolingUnit.remainingCoolingTime;
            if (remainingCoolingTime > 3600) {
                gameObject.GetComponent<Renderer>().material = materialInLongProgress;
            } else if (remainingCoolingTime > 0) {
                gameObject.GetComponent<Renderer>().material = materialInShortProgress;
            } else {
                gameObject.GetComponent<Renderer>().material = materialReady;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "MousePointer") {
            StopAllCoroutines();
            StartCoroutine(StartTimer());
        } else if (other.tag =="ProcessingStation") {
            coolingUnit.assignmentToProcessingStationPossible = true;
            Debug.Log("We have reached the processing station unit assignement");
            coolingUnit.processingStation = TestDataManager.getProcessingStationObjectWithId(other.gameObject.name);
          
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "MousePointer") {
            StopAllCoroutines();
            NaturalCoolingUnitHoverMenu.OnMouseLoseFocus();
        } else if (other.tag == "ProcessingStation") {
            coolingUnit.assignmentToProcessingStationPossible = false;
        }
    }

    private IEnumerator StartTimer() {
        yield return new WaitForSeconds(timeToWait);
        ShowHoverMenu();
    }

    private void ShowHoverMenu() {
        NaturalCoolingUnitHoverMenu.OnMouseHover(coolingUnit, Input.mousePosition);
    }

}