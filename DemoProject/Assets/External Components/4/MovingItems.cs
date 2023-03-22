using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingItems : MonoBehaviour
{
    private BuildUnit buildUnit;
    private NaturalCoolingUnit coolingUnit;
    private GameObject gameObjectClicked;
    private ObjectHPJF5200 objectClicked;

    public float forceAmount = 500;
    public float m_Speed = 5f;

    public static Rigidbody selectedRigidbody;
    Camera targetCamera;
    Vector3 originalScreenTargetPosition;
    Vector3 originalRigidbodyPos;
    float selectionDistance;


    // Start is called before the first frame update
    void Start()
    {
        targetCamera = GetComponent<Camera>();
    }

    void Update()
    {
        GameObject gameObject;

        if (!targetCamera)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //Check if we are hovering over Rigidbody, if so, select it
            selectedRigidbody = GetRigidbodyFromMouseClick();

            objectClicked = TestDataManager.getObjectWithId(selectedRigidbody.name);

            if (objectClicked.GetType() == typeof(NaturalCoolingUnit))                
            {
                coolingUnit = (NaturalCoolingUnit)objectClicked;
                buildUnit = null;

            }
            else if (objectClicked.GetType() == typeof(BuildUnit))
            { 
                buildUnit = (BuildUnit)objectClicked;
                coolingUnit = null;
            }


        }

        if (Input.GetMouseButtonUp(0) && selectedRigidbody)
        {
            // ------------------
            // Falls CoolingUnit
            // 
            if (coolingUnit != null)
            {
                Debug.Log("We did it!!! First Try" + coolingUnit.ToString());
                
                // ----------------------------------------
                // CoolingUnit in Processing Station setzen
                // ----------------------------------------
                if (coolingUnit.IsReadyToBeInsertedIntoProcessingStation() == true &&
                coolingUnit.processingStation.IsAssignmentOfCoolingUnitPossible == true)
                {
                    Debug.Log("IsReadyToBeInsertedIntoProcessingStation " + coolingUnit.IsReadyToBeInsertedIntoProcessingStation().ToString());
                    Debug.Log("IsAssignmentOfCoolingUnitPossible " + coolingUnit.processingStation.IsAssignmentOfCoolingUnitPossible.ToString());


                    // CoolingUnit positionieren
                    gameObject = GameObject.Find(coolingUnit.processingStation.Id);
                    ProcessingStationBehaviour other = (ProcessingStationBehaviour)gameObject.GetComponent(typeof(ProcessingStationBehaviour));

                    selectedRigidbody.MovePosition(transform.position + other.positionForCoolingUnit.transform.localPosition * Time.deltaTime * m_Speed); 
                    //other.PositionCoolingUnit(coolingUnit);
                    // CoolingUnit und ProcessingStation einander zuordnen
                    coolingUnit.processingStation.AssignCoolingUnit();
                    coolingUnit.AssignProcessingStation(coolingUnit.processingStation);
                    // Prozess starten: PushingContentFromBuildUnitToCoolingUnit
                    if (coolingUnit.processingStation.IsBuildUnitAssigned == true &&
                    coolingUnit.processingStation.BuildUnitAssigned.status == Status_BuildUnit.Finished_Printing)
                    {
                        coolingUnit.processingStation.StartPushingContentOfBuildUnitToCoolingUnit();
                    }
                    else
                    {
                        // Ggf. Verbindung zu Processing Station lösen
                        if (coolingUnit.processingStation != null)
                        {
                            coolingUnit.processingStation.UnassignCoolingUnit();
                            coolingUnit.UnassignProcessingStation();
                        }
                        // Ggf. Kühlungsvorgang starten
                        if (coolingUnit.status == Status_CoolingUnit.Finished_PushingContentFromBuildUnitToCoolingUnit)
                        {
                            coolingUnit.StartCooling();
                        }
                    }


                }
                // -----------------
                // Falls BuildUnit
                // -----------------
            }
            else if (buildUnit != null)
            {
                // ---------------------------------------
                // BuildUnit in Processing Station setzen
                // ---------------------------------------
                if (buildUnit.IsReadyToBeInsertedIntoProcessingStation() == true &&
                buildUnit.processingStation.IsAssignmentOfBuildUnitPossible == true)
                {
                    // BuildUnit positionieren
                    gameObject = GameObject.Find(buildUnit.processingStation.Id);
                    ProcessingStationBehaviour other = (ProcessingStationBehaviour)gameObject.GetComponent(typeof(ProcessingStationBehaviour));
                    
                    // BuildUnit und ProcessingStation einander zuordnen
                    buildUnit.processingStation.AssignBuildUnit();
                    buildUnit.AssignProcessingStation(buildUnit.processingStation);
                    // Prozess starten: PushingContentFromBuildUnitToCoolingUnit
                    if (buildUnit.status == Status_BuildUnit.Finished_Printing &&
                    buildUnit.processingStation.IsCoolingUnitAssigned == true &&
                    buildUnit.processingStation.CoolingUnitAssigned.status == Status_CoolingUnit.Ready)
                    {
                        buildUnit.processingStation.StartPushingContentOfBuildUnitToCoolingUnit();
                        // Prozess starten: FillingInMaterial
                    }
                    else if (buildUnit.status == Status_BuildUnit.Ready &&
                  buildUnit.processingStation.IsCoolingUnitAssigned == false)
                    {
                        buildUnit.processingStation.StartFillingInMaterial("Neues Material");
                    }
                    // ----------------------------
                    // BuildUnit in Printer setzen    
                    // ----------------------------
                }
                else if (buildUnit.IsReadyToBeInsertedIntoPrinter() == true &&
              buildUnit.printer.IsAssignmentOfBuildUnitPossible == true)
                {
                    // BuildUnit positionieren
                    gameObject = GameObject.Find(buildUnit.printer.Id);
                    PrinterBehaviour other = (PrinterBehaviour)gameObject.GetComponent(typeof(PrinterBehaviour));
                    
                    // BuildUnit und Printer einander zuordnen
                    buildUnit.printer.BuildUnitAssigned = buildUnit;
                    buildUnit.AssignPrinter(buildUnit.printer);
                    // Prozess starten: Printing
                    buildUnit.printer.StartPrinting(new PrintJob("Testjob", 5));
                    // ---------------------------------------------
                    // BuildUnit aus ProcessingStation herausnehmen
                    // ---------------------------------------------
                }
                else if (buildUnit.assignmentToProcessingStationPossible == false &&
              buildUnit.processingStation != null &&
              (buildUnit.status == Status_BuildUnit.Finished_FillingInMaterial ||
              buildUnit.status == Status_BuildUnit.Finished_PushingContentFromBuildUnitToCoolingUnit))
                {
                    buildUnit.processingStation.UnassignBuildUnit();
                    buildUnit.UnassignProcessingStation();
                    if (buildUnit.status == Status_BuildUnit.Finished_PushingContentFromBuildUnitToCoolingUnit)
                    {
                        buildUnit.Reset();
                    }
                    
                    // -----------------------------------
                    // BuildUnit aus Printer herausnehmen
                    // -----------------------------------
                }
                else if (buildUnit.assignmentToPrinterPossible == false &&
              buildUnit.printer != null &&
              buildUnit.status == Status_BuildUnit.Finished_Printing)
                {
                    buildUnit.printer.BuildUnitAssigned = null;
                    buildUnit.UnassignPrinter();
                  
                    // ------------------
                    // BuildUnit bewegen
                    // ------------------
                }
            }

            //Release selected Rigidbody if there any
            selectedRigidbody = null;
        }
    }

    void FixedUpdate()
    {
        if (selectedRigidbody)
        {
            Vector3 mousePositionOffset = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance)) - originalScreenTargetPosition;
            selectedRigidbody.velocity = (originalRigidbodyPos + mousePositionOffset - selectedRigidbody.transform.position) * forceAmount * Time.deltaTime;
        }
    }

    public Rigidbody GetRigidbodyFromMouseClick()
    {
        //Use raycast to see where mouse is pointing
        RaycastHit hitInfo = new RaycastHit();
        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out hitInfo);
        //if raycast hits anything check for pickable component
        if (hit)
        {
            // if (hitInfo.collider.gameObject.GetComponent<Rigidbody>())
            //Check if object is assigned as "Pickupable before moving it
            PickUpable p = hitInfo.collider.GetComponent<PickUpable>();
            if (p != null)
            {
                selectionDistance = Vector3.Distance(ray.origin, hitInfo.point);
                originalScreenTargetPosition = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance));
                originalRigidbodyPos = hitInfo.collider.transform.position;

                //return the rigidbody that was hit
                return hitInfo.collider.gameObject.GetComponent<Rigidbody>();
            }
        }

        return null;
    }
}
