using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status_ProcessingStation {
    Ready,
    InProgress_FillingInMaterial,
    InProgress_PushingContentFromBuildUnitToCoolingUnit,
    InProgress_EmptyingCoolingUnit,
    Finished_FillingInMaterial,
    Finished_PushingContentFromBuildUnitToCoolingUnit,
    Finished_EmptyingCoolingUnit
}

    public class ProcessingStation : ObjectHPJF5200
    {

        // General
        private string id;
        private string name;
        private Status_ProcessingStation status;
        private float remainingProcessingTime;

        public string Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public Status_ProcessingStation Status { get { return status; } set { status = value; } }
        public float RemainingProcessingTime { get { return remainingProcessingTime; } set { remainingProcessingTime = value; } }

        // Workflow
        private BuildUnit buildUnitAssigned;
        private BuildUnit buildUnitPossibleToAssign;
        private NaturalCoolingUnit coolingUnitAssigned;
        private NaturalCoolingUnit coolingUnitPossibleToAssign;

        public bool IsBuildUnitAssigned { get { return (buildUnitAssigned != null); } }
        public BuildUnit BuildUnitAssigned { get { return buildUnitAssigned; } set { buildUnitAssigned = value; } }
        public bool IsAssignmentOfBuildUnitPossible { get { return (buildUnitPossibleToAssign != null && IsBuildUnitAssigned == false); } }
        public BuildUnit BuildUnitPossibleToAssign { get { return buildUnitPossibleToAssign; } set { buildUnitPossibleToAssign = value; } }
        public bool IsCoolingUnitAssigned { get { return (coolingUnitAssigned != null); } }
        public NaturalCoolingUnit CoolingUnitAssigned { get { return coolingUnitAssigned; } set { coolingUnitAssigned = value; } }
        public bool IsAssignmentOfCoolingUnitPossible { get { return (coolingUnitPossibleToAssign != null && IsCoolingUnitAssigned == false); } }
        public NaturalCoolingUnit CoolingUnitPossibleToAssign { get { return coolingUnitPossibleToAssign; } set { coolingUnitPossibleToAssign = value; } }

        public ProcessingStation(string id, string name)
        {
            this.id = id;
            this.name = name;
            status = Status_ProcessingStation.Ready;
        }

        public void Reset()
        {
            status = Status_ProcessingStation.Ready;
            buildUnitAssigned = null;
            buildUnitPossibleToAssign = null;
            coolingUnitAssigned = null;
            coolingUnitPossibleToAssign = null;
        }

        public void AssignBuildUnit()
        {
            if (IsAssignmentOfBuildUnitPossible == true)
                buildUnitAssigned = buildUnitPossibleToAssign;
        }

        public void UnassignBuildUnit()
        {
            if (IsCoolingUnitAssigned == false)
            {
                Reset();
            }
        }

        public void AssignCoolingUnit()
        {
            if (IsAssignmentOfCoolingUnitPossible == true)
                coolingUnitAssigned = coolingUnitPossibleToAssign;
        }

        public void UnassignCoolingUnit()
        {
            if (IsBuildUnitAssigned == false)
            {
                Reset();
            }
        }

        public void StartFillingInMaterial(string material)
        {
            if (IsBuildUnitAssigned == true)
            {
                buildUnitAssigned.StartFillingInMaterial(material);
                status = Status_ProcessingStation.InProgress_FillingInMaterial;
                remainingProcessingTime = 5f;
            }
        }

        public void EndFillingInMaterial()
        {
            buildUnitAssigned.EndFillingInMaterial();
            status = Status_ProcessingStation.Finished_FillingInMaterial;
        }

        public void StartPushingContentOfBuildUnitToCoolingUnit()
        {
            if (IsBuildUnitAssigned == true && IsCoolingUnitAssigned == true)
            {
                buildUnitAssigned.StartPushingContentFromBuildUnitToCoolingUnit();
                coolingUnitAssigned.StartPushingContentFromBuildUnitToCoolingUnit(buildUnitAssigned.GetPrintJob());
                status = Status_ProcessingStation.InProgress_PushingContentFromBuildUnitToCoolingUnit;
                remainingProcessingTime = 10f;
            }
        }

        public void EndPushingContentOfBulidUnitToCoolingUnit()
        {
            buildUnitAssigned.EndPushingContentFromBuildUnitToCoolingUnit();
            coolingUnitAssigned.EndPushingContentFromBuildUnitToCoolingUnit();
            status = Status_ProcessingStation.Finished_PushingContentFromBuildUnitToCoolingUnit;
        }

        public void UpdateRemainingProcessingTime(float time)
        {
            remainingProcessingTime = (remainingProcessingTime - time) < 0 ? 0 : (remainingProcessingTime - time);
            if (remainingProcessingTime == 0)
            {
                if (status == Status_ProcessingStation.InProgress_FillingInMaterial)
                {
                    EndFillingInMaterial();
                }
                else if (status == Status_ProcessingStation.InProgress_PushingContentFromBuildUnitToCoolingUnit)
                {
                    EndPushingContentOfBulidUnitToCoolingUnit();
                }
                else if (status == Status_ProcessingStation.InProgress_EmptyingCoolingUnit)
                {
                    status = Status_ProcessingStation.Finished_EmptyingCoolingUnit;
                }
            }
        }

        public void UpdateStatus(float time)
        {
            if (status == Status_ProcessingStation.InProgress_FillingInMaterial ||
                status == Status_ProcessingStation.InProgress_PushingContentFromBuildUnitToCoolingUnit ||
                status == Status_ProcessingStation.InProgress_EmptyingCoolingUnit)
            {
                UpdateRemainingProcessingTime(time);
            }
        }
    
}