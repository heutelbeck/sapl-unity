public enum Status_Printer {
    Ready,
    InProgress_Printing,
    Finished_Printing
}

public class Printer : ObjectHPJF5200 {

    // General
    private string id;
    private string name;
    private Status_Printer status;
    private float remainingPrintingTime;

    public string Id { get { return id; } set { id = value; } }
    public string Name { get { return name; } set { name = value; } }
    public Status_Printer Status { get { return status; } set { status = value; } }
    public float RemainingPrintingTime { get { return remainingPrintingTime; } set { remainingPrintingTime = value; } }

    // Workflow
    private BuildUnit buildUnitAssigned;
    private BuildUnit buildUnitPossibleToAssign;

    public bool IsBuildUnitAssigned { get { return (buildUnitAssigned != null); } }
    public BuildUnit BuildUnitAssigned { get { return buildUnitAssigned; } set { buildUnitAssigned = value; } }
    public bool IsAssignmentOfBuildUnitPossible { get { return (buildUnitPossibleToAssign != null && IsBuildUnitAssigned == false); } }
    public BuildUnit BuildUnitPossibleToAssign { get { return buildUnitPossibleToAssign; } set { buildUnitPossibleToAssign = value; } }
    
    public Printer(string id, string name) {
        this.id = id;
        this.name = name;
        status = Status_Printer.Ready;
    }

    public void Reset() {
        status = Status_Printer.Ready;
        buildUnitAssigned = null;
        buildUnitPossibleToAssign = null;
    }

    public void AssignBuildUnit() {
        if (IsAssignmentOfBuildUnitPossible == true) 
            buildUnitAssigned = buildUnitPossibleToAssign;
    }

    public void UnassignBuildUnit() {
        Reset();
    }

    public void StartPrinting(PrintJob printJob) {
        if (IsBuildUnitAssigned == true && status == Status_Printer.Ready) {
            buildUnitAssigned.StartPrinting(printJob);
            remainingPrintingTime = printJob.duration;
            status = Status_Printer.InProgress_Printing;
        }
    }

    public void EndPrinting() {
        buildUnitAssigned.EndPrinting();
        status = Status_Printer.Finished_Printing;
    }

    public void UpdateRemainingPrintingTime(float time) {
        remainingPrintingTime = (remainingPrintingTime - time) < 0 ? 0 : (remainingPrintingTime - time);
        if (remainingPrintingTime == 0) {
            EndPrinting();
        }
    }

    public void UpdateStatus(float time) {
        if (status == Status_Printer.InProgress_Printing) {
            UpdateRemainingPrintingTime(time);
        }
    }

}