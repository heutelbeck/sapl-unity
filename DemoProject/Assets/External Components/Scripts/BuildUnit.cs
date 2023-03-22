using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status_BuildUnit {
    Ready,
    InProgress_FillingInMaterial,
    InProgress_Printing,
    InProgress_PushingContentFromBuildUnitToCoolingUnit,
    Finished_FillingInMaterial,
    Finished_Printing,
    Finished_PushingContentFromBuildUnitToCoolingUnit
}

public class BuildUnit : ObjectHPJF5200 {

    public string id;
    public string name;
    private string material;
    private PrintJob printJob;
    public Status_BuildUnit status;

    public ProcessingStation processingStation;
    public bool isProcessingStationAssigned;
    public bool assignmentToProcessingStationPossible;

    public Printer printer;
    public bool isPrinterAssigned;
    public bool assignmentToPrinterPossible;

    public BuildUnit(string id, string name) {
        this.id = id;
        this.name = name;
        status = Status_BuildUnit.Ready;
    }

    public string Id { get { return id; } set { id = value; } }
    public string Name { get { return name; } set { name = value; } }

    public void StartFillingInMaterial(string material) {
        this.material = material;
        status = Status_BuildUnit.InProgress_FillingInMaterial;
    }

    public void EndFillingInMaterial() {
        status = Status_BuildUnit.Finished_FillingInMaterial;
    }

    public void StartPrinting(PrintJob printJob) {
        this.printJob = printJob;
        status = Status_BuildUnit.InProgress_Printing;
    }

    public void EndPrinting() {
        status = Status_BuildUnit.Finished_Printing;
    }

    public void StartPushingContentFromBuildUnitToCoolingUnit() {
        status = Status_BuildUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit;
    }

    public void EndPushingContentFromBuildUnitToCoolingUnit() {
        this.material = "";
        this.printJob = null;
        status = Status_BuildUnit.Finished_PushingContentFromBuildUnitToCoolingUnit;
    }

    public void Reset() {
        status = Status_BuildUnit.Ready;
    }

    public string GetMaterial() {
        if (status != Status_BuildUnit.Ready &&
            status != Status_BuildUnit.InProgress_FillingInMaterial &&
            status != Status_BuildUnit.Finished_PushingContentFromBuildUnitToCoolingUnit) {
            return material;
        } else {
            return "";
        }
    }

    public PrintJob GetPrintJob() {
        return printJob;
    }

    public string GetNameOfPrintJob() {
        if (status == Status_BuildUnit.InProgress_Printing ||
            status == Status_BuildUnit.Finished_Printing ||
            status == Status_BuildUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit) {
            return printJob.name;
        }
        else {
            return "";
        }
    }

    public void UpdateStatus(float time) {

    }

    public void AssignProcessingStation(ProcessingStation processingStation) {
        this.processingStation = processingStation;
        isProcessingStationAssigned = true;
    }

    public void UnassignProcessingStation() {
        processingStation = null;
        isProcessingStationAssigned = false;
    }

    public void AssignPrinter(Printer printer) {
        this.printer = printer;
        isPrinterAssigned = true;
    }

    public void UnassignPrinter() {
        printer = null;
        isPrinterAssigned = false;
    }

    public bool IsReadyToBeInsertedIntoPrinter() {
        return (assignmentToPrinterPossible == true && 
                isPrinterAssigned == false &&
                status == Status_BuildUnit.Finished_FillingInMaterial);
    }

    public bool IsReadyToBeInsertedIntoProcessingStation() {
        return (assignmentToProcessingStationPossible == true && 
                isProcessingStationAssigned == false && 
                (status == Status_BuildUnit.Ready ||
                status == Status_BuildUnit.Finished_Printing));
    }

    public bool IsDraggable() {
        return (status != Status_BuildUnit.InProgress_FillingInMaterial &&
                status != Status_BuildUnit.InProgress_Printing &&
                status != Status_BuildUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit);
    }

}