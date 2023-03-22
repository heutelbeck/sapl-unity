using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropObjects : MonoBehaviour {

    public ProcessingStationBehaviour processingStationBehaviour;

    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private GameObject positionForNaturalCoolingUnit;
    [SerializeField] private GameObject positionForBuildUnit;

    private ObjectHPJF5200 objectClicked;
    private ObjectGrabbable objectGrabbable;
    private RaycastHit raycastHit;

    private BuildUnit buildUnit;
    private NaturalCoolingUnit coolingUnit;

    private GameObject gameObjectClicked;

    private void Update() {

        GameObject gameObject;

        // --------------------------------
        // MAUS KLICKEN (OBJEKT AUFNEHMEN)
        // --------------------------------
        if (Input.GetMouseButtonDown(0)) {
            float pickUpDistance = 25f;
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out raycastHit, pickUpDistance, pickUpLayerMask)) {
                Debug.Log(raycastHit.transform.gameObject);
                objectClicked = TestDataManager.getObjectWithId(raycastHit.transform.gameObject.name);
                if (objectClicked != null) {
                    gameObjectClicked = raycastHit.transform.gameObject;
                    // ------------------
                    // Falls CoolingUnit
                    // ------------------
                    if (objectClicked.GetType() == typeof(NaturalCoolingUnit)) {
                        coolingUnit = (NaturalCoolingUnit)objectClicked;
                        buildUnit = null;
                        if (coolingUnit.IsDraggable() == true) {
                            if (raycastHit.transform.TryGetComponent(out objectGrabbable)) {
                                objectGrabbable.Grab(objectGrabPointTransform);
                            }
                        }
                    // ----------------
                    // Falls BuildUnit
                    // ----------------
                    } else if (objectClicked.GetType() == typeof(BuildUnit)) {
                        buildUnit = (BuildUnit)objectClicked;
                        coolingUnit = null;
                        if (buildUnit.IsDraggable() == true) {
                            if (raycastHit.transform.TryGetComponent(out objectGrabbable)) {
                                objectGrabbable.Grab(objectGrabPointTransform);
                            }
                        }
                    }
                }
            }
        // ---------------------------------------
        // MAUS LOSLASSEN (OBJEKT FALLEN LASSEN)
        // ---------------------------------------
        } else if (Input.GetMouseButtonUp(0)) {
            if (objectGrabbable != null) {
                // ------------------
                // Falls CoolingUnit
                // ------------------
                if (coolingUnit != null) {
                    // ----------------------------------------
                    // CoolingUnit in Processing Station setzen
                    // ----------------------------------------
                    if (coolingUnit.IsReadyToBeInsertedIntoProcessingStation() == true &&
                    coolingUnit.processingStation.IsAssignmentOfCoolingUnitPossible == true) {
                        // CoolingUnit positionieren
                        gameObject = GameObject.Find(coolingUnit.processingStation.Id);
                        ProcessingStationBehaviour other = (ProcessingStationBehaviour)gameObject.GetComponent(typeof(ProcessingStationBehaviour));
                        other.PositionCoolingUnit(objectGrabbable);
                        // CoolingUnit und ProcessingStation einander zuordnen
                        coolingUnit.processingStation.AssignCoolingUnit();
                        coolingUnit.AssignProcessingStation(coolingUnit.processingStation);
                        // Prozess starten: PushingContentFromBuildUnitToCoolingUnit
                        if (coolingUnit.processingStation.IsBuildUnitAssigned == true &&
                        coolingUnit.processingStation.BuildUnitAssigned.status == Status_BuildUnit.Finished_Printing) {
                            coolingUnit.processingStation.StartPushingContentOfBuildUnitToCoolingUnit();
                        }
                    // -------------------------------------------------------------
                    // CoolingUnit aus Processing Station herausnehmen bzw. bewegen
                    // -------------------------------------------------------------
                    } else {
                        objectGrabbable.Drop();
                        // Ggf. Verbindung zu Processing Station lösen
                        if (coolingUnit.processingStation != null) {
                            coolingUnit.processingStation.UnassignCoolingUnit();
                            coolingUnit.UnassignProcessingStation();
                        }
                        // Ggf. Kühlungsvorgang starten
                        if (coolingUnit.status == Status_CoolingUnit.Finished_PushingContentFromBuildUnitToCoolingUnit) {
                            coolingUnit.StartCooling();
                        }
                    }
                    objectGrabbable = null;
                    coolingUnit = null;
                // -----------------
                // Falls BuildUnit
                // -----------------
                } else if (buildUnit != null) {
                    // ---------------------------------------
                    // BuildUnit in Processing Station setzen
                    // ---------------------------------------
                    if (buildUnit.IsReadyToBeInsertedIntoProcessingStation() == true &&
                    buildUnit.processingStation.IsAssignmentOfBuildUnitPossible == true) {
                        // BuildUnit positionieren
                        gameObject = GameObject.Find(buildUnit.processingStation.Id);
                        ProcessingStationBehaviour other = (ProcessingStationBehaviour)gameObject.GetComponent(typeof(ProcessingStationBehaviour));
                        other.PositionBuildUnit(objectGrabbable);
                        // BuildUnit und ProcessingStation einander zuordnen
                        buildUnit.processingStation.AssignBuildUnit();
                        buildUnit.AssignProcessingStation(buildUnit.processingStation);
                        // Prozess starten: PushingContentFromBuildUnitToCoolingUnit
                        if (buildUnit.status == Status_BuildUnit.Finished_Printing &&
                        buildUnit.processingStation.IsCoolingUnitAssigned == true &&
                        buildUnit.processingStation.CoolingUnitAssigned.status == Status_CoolingUnit.Ready) {
                            buildUnit.processingStation.StartPushingContentOfBuildUnitToCoolingUnit();
                        // Prozess starten: FillingInMaterial
                        } else if (buildUnit.status == Status_BuildUnit.Ready &&
                        buildUnit.processingStation.IsCoolingUnitAssigned == false) {
                            buildUnit.processingStation.StartFillingInMaterial("Neues Material");
                        }
                    // ----------------------------
                    // BuildUnit in Printer setzen    
                    // ----------------------------
                    } else if (buildUnit.IsReadyToBeInsertedIntoPrinter() == true &&
                    buildUnit.printer.IsAssignmentOfBuildUnitPossible == true) {
                        // BuildUnit positionieren
                        gameObject = GameObject.Find(buildUnit.printer.Id);
                        PrinterBehaviour other = (PrinterBehaviour)gameObject.GetComponent(typeof(PrinterBehaviour));
                        other.positionBuildUnit(objectGrabbable);
                        // BuildUnit und Printer einander zuordnen
                        buildUnit.printer.BuildUnitAssigned = buildUnit;
                        buildUnit.AssignPrinter(buildUnit.printer);
                        // Prozess starten: Printing
                        buildUnit.printer.StartPrinting(new PrintJob("Testjob", 5));
                    // ---------------------------------------------
                    // BuildUnit aus ProcessingStation herausnehmen
                    // ---------------------------------------------
                    } else if (buildUnit.assignmentToProcessingStationPossible == false &&
                    buildUnit.processingStation != null && 
                    (buildUnit.status == Status_BuildUnit.Finished_FillingInMaterial ||
                    buildUnit.status == Status_BuildUnit.Finished_PushingContentFromBuildUnitToCoolingUnit)) {
                        buildUnit.processingStation.UnassignBuildUnit();
                        buildUnit.UnassignProcessingStation();
                        if (buildUnit.status == Status_BuildUnit.Finished_PushingContentFromBuildUnitToCoolingUnit) {
                            buildUnit.Reset();
                        }
                        objectGrabbable.Drop();
                    // -----------------------------------
                    // BuildUnit aus Printer herausnehmen
                    // -----------------------------------
                    } else if (buildUnit.assignmentToPrinterPossible == false &&
                    buildUnit.printer != null &&
                    buildUnit.status == Status_BuildUnit.Finished_Printing) {
                        buildUnit.printer.BuildUnitAssigned = null;
                        buildUnit.UnassignPrinter();
                        objectGrabbable.Drop();
                    // ------------------
                    // BuildUnit bewegen
                    // ------------------
                    } else {
                        objectGrabbable.Drop();
                    }
                    objectGrabbable = null;
                    buildUnit = null;
                }
            }
        }
    }

}