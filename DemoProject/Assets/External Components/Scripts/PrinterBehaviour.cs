using System.Collections;
using UnityEngine;

public class PrinterBehaviour : MonoBehaviour
{
    // General
    private Printer printer;
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

    private void Start() {
        printer = TestDataManager.getPrinterObjectWithId(gameObject.name);
    }

    void Update() {
        if (printer == null) return;

        printer.UpdateStatus(Time.deltaTime);

        if (printer.Status == Status_Printer.Ready) {
            gameObject.GetComponent<Renderer>().material = materialStandard;
            door.GetComponent<Renderer>().material = materialStandard;
        } else if (printer.RemainingPrintingTime > 0) {
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
            if (printer.Status != Status_Printer.InProgress_Printing) {
                OpenDoor();
            } else {
                CloseDoor();
            }
        // Trigger BuildUnit
        } else if (other.tag == "BuildUnit") {
            printer.BuildUnitPossibleToAssign = TestDataManager.getBuildUnitObjectWithId(other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other) {
        // Trigger Mouse
        if (other.tag == "MousePointer") {
            StopAllCoroutines();
            PrinterHoverMenu.OnMouseLoseFocus();
            CloseDoor();
        // Trigger BuildUnit
        } else if (other.tag == "BuildUnit") {
            printer.BuildUnitPossibleToAssign = null;
        }
    }

    private IEnumerator StartTimer() {
        yield return new WaitForSeconds(timeToWait);
        ShowHoverMenu();
    }

    private void ShowHoverMenu() {
        PrinterHoverMenu.OnMouseHover(printer, Input.mousePosition);
    }

    public void positionBuildUnit(ObjectGrabbable objectGrabbable) {
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