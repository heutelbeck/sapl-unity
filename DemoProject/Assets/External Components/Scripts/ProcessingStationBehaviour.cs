using System.Collections;
using UnityEngine;

public class ProcessingStationBehaviour : MonoBehaviour {

    // General
    private ProcessingStation processingStation;
    private float timeToWait = 0.5f;

    // Materials
    [SerializeField]
    private Material materialStandard;
    [SerializeField]
    private Material materialInProgress;
    [SerializeField]
    private Material materialReady;

    // Positions
    public GameObject door;
    public GameObject positionForBuildUnit;
    public GameObject positionForCoolingUnit;

    private void Start() {
        processingStation = TestDataManager.getProcessingStationObjectWithId(gameObject.name);
    }

    void Update() {

        if (processingStation == null) return;

        processingStation.UpdateStatus(Time.deltaTime);

        if (processingStation.Status == Status_ProcessingStation.Ready) {
            gameObject.GetComponent<Renderer>().material = materialStandard;
            door.GetComponent<Renderer>().material = materialStandard;
        } else if (processingStation.RemainingProcessingTime > 0) {
            gameObject.GetComponent<Renderer>().material = materialInProgress;
            door.GetComponent<Renderer>().material = materialInProgress;
        } else {
            gameObject.GetComponent<Renderer>().material = materialReady;
            door.GetComponent<Renderer>().material = materialReady;
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Trigger Mouse
        if (other.tag == "MousePointer") {
            StopAllCoroutines();
            StartCoroutine(StartTimer());
            // Tür öffnen, wenn MausPointer und PS NICHT arbeitet
            if (processingStation.Status == Status_ProcessingStation.Ready ||
                processingStation.Status == Status_ProcessingStation.Finished_FillingInMaterial ||
                processingStation.Status == Status_ProcessingStation.Finished_PushingContentFromBuildUnitToCoolingUnit ||
                processingStation.Status == Status_ProcessingStation.Finished_EmptyingCoolingUnit) {
                OpenDoor();
            } else {
                CloseDoor();
            }
        // Trigger Cooling Unit
        } else if (other.tag == "CoolingUnit") {
            processingStation.CoolingUnitPossibleToAssign = TestDataManager.getCoolingUnitObjectWithId(other.gameObject.name);
        // Trigger Build Unit
        } else if (other.tag == "BuildUnit") {
            processingStation.BuildUnitPossibleToAssign = TestDataManager.getBuildUnitObjectWithId(other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other) {
        // Trigger Mouse
        if (other.tag == "MousePointer") {
            StopAllCoroutines();
            ProcessingStationHoverMenu.OnMouseLoseFocus();
            CloseDoor();
        // Trigger Cooling Unit
        } else if (other.tag == "CoolingUnit") {
            processingStation.UnassignCoolingUnit();
        // Trigger Build Unit
        } else if (other.tag == "BuildUnit") {
            processingStation.UnassignBuildUnit();
        }
    }

    private IEnumerator StartTimer() {
        yield return new WaitForSeconds(timeToWait);
        ShowHoverMenu();
    }

    private void ShowHoverMenu() {
        ProcessingStationHoverMenu.OnMouseHover(processingStation, Input.mousePosition);
    }

    public void PositionCoolingUnit(ObjectGrabbable objectGrabbable) {
       // objectGrabbable.Drop(gameObject.transform.position + 
       //                      positionForCoolingUnit.transform.localPosition);
    }

    public void PositionBuildUnit(ObjectGrabbable objectGrabbable) {
        objectGrabbable.Drop(gameObject.transform.position + 
                             positionForBuildUnit.transform.localPosition);
        CloseDoor();
    }

    private void OpenDoor() {
        door.GetComponent<Animator>().SetBool("openDoor", true);
    }

    private void CloseDoor() {
        door.GetComponent<Animator>().SetBool("openDoor", false);
    }

}