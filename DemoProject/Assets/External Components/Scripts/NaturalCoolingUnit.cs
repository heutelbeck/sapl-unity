using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status_CoolingUnit {
    Ready,
    InProgress_PushingContentFromBuildUnitToCoolingUnit,
    InProgress_Cooling,
    InProgress_Emptying,
    Finished_PushingContentFromBuildUnitToCoolingUnit,
    Finished_Cooling,
    Finished_Emptying
}

public class NaturalCoolingUnit : ObjectHPJF5200 {

    public string id;
    public string name;
    private PrintJob printJob;
    public Status_CoolingUnit status;
    public float remainingCoolingTime;

    public ProcessingStation processingStation;
    public bool isProcessingStationAssigned;
    public bool assignmentToProcessingStationPossible;

    public NaturalCoolingUnit(string unitId, string unitName) {
        id = unitId;
        name = unitName;
        status = Status_CoolingUnit.Ready;
    }

    public NaturalCoolingUnit(string id, string name, PrintJob printJob, float remainingCoolingTime) : this(id, name) {
        StartPushingContentFromBuildUnitToCoolingUnit(printJob);
        EndPushingContentFromBuildUnitToCoolingUnit();
        StartCooling(remainingCoolingTime);
        UpdateRemainingCoolingTime(0);
    }

    public string Id { get { return id; } set { id = value; } }
    public string Name { get { return name; } set { name = value; } }

    public void StartPushingContentFromBuildUnitToCoolingUnit(PrintJob printJob) {
        this.printJob = printJob;
        status = Status_CoolingUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit;
    }

    public void EndPushingContentFromBuildUnitToCoolingUnit() {
        status = Status_CoolingUnit.Finished_PushingContentFromBuildUnitToCoolingUnit;
    }

    public void StartCooling(float time = 60f) {
        remainingCoolingTime = time;
        status = Status_CoolingUnit.InProgress_Cooling;
    }

    public void EndCooling() {
        status = Status_CoolingUnit.Finished_Cooling;
    }

    public void Reset() {
        status = Status_CoolingUnit.Ready;
    }

    public string GetNameOfPrintJob() {
        if (status != Status_CoolingUnit.Ready &&
            status != Status_CoolingUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit &&
            status != Status_CoolingUnit.Finished_Emptying) {
            return printJob.name;
        }
        else {
            return "";
        }
    }

    public void UpdateStatus(float time) {
        if (status == Status_CoolingUnit.InProgress_Cooling) {
            UpdateRemainingCoolingTime(time);
        }
    }

    public void UpdateRemainingCoolingTime(float time) {
        remainingCoolingTime = (remainingCoolingTime - time) < 0 ? 0 : (remainingCoolingTime - time);
        if (remainingCoolingTime == 0) {
            status = Status_CoolingUnit.Finished_Cooling;
        }
    }

    public void AssignProcessingStation(ProcessingStation processingStation) {
        this.processingStation = processingStation;
        isProcessingStationAssigned = true;
    }

    public void UnassignProcessingStation() {
        processingStation = null;
        isProcessingStationAssigned = false;
    }

    public bool IsReadyToBeInsertedIntoProcessingStation() {
        return (assignmentToProcessingStationPossible == true && 
                isProcessingStationAssigned == false &&
                (status == Status_CoolingUnit.Ready ||
                status == Status_CoolingUnit.Finished_Cooling));
    }

    public bool IsDraggable() {
        return (status != Status_CoolingUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit &&
                status != Status_CoolingUnit.InProgress_Cooling &&
                status != Status_CoolingUnit.InProgress_Emptying);
    }

}