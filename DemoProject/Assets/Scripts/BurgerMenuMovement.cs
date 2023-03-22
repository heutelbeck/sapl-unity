using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurgerMenuMovement : MonoBehaviour
{

    public GameObject MenuOrigPosition; 
    public GameObject MenuActivePosition;
    public GameObject MenuPanel;

    public bool Move_Menu_Panel;
    public bool Move_Menu_Panel_back;

    public float moveSpeed;
    public float tempMenuPosition;

    public Button btnShowPrintJobs, btnClosePrintjobs;

    [SerializeField] private GameObject Menu_ActiveJobs;


    private void Awake()
    {
        //Menu2.SetActive(false);
        Menu_ActiveJobs.SetActive(false);
        

    }

    // Start is called before the first frame update
    void Start()
    {
        btnShowPrintJobs.onClick.AddListener(OpenActiveJobsMenu);
        btnClosePrintjobs.onClick.AddListener(CloseActiveJobsMenu);

        MenuPanel.transform.position = MenuOrigPosition.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Move_Menu_Panel)
        {
            MenuPanel.transform.position = Vector3.Lerp(MenuPanel.transform.position, MenuActivePosition.transform.position, moveSpeed * Time.deltaTime);

            if(MenuPanel.transform.localPosition.x == tempMenuPosition)
            {
                Move_Menu_Panel = false;
                MenuPanel.transform.position = MenuActivePosition.transform.position;
                tempMenuPosition = -999999999999.99f;
            }
            if (Move_Menu_Panel)
            {
                tempMenuPosition = MenuPanel.transform.position.x;
            }

        }

        if(Move_Menu_Panel_back)
        {
            MenuPanel.transform.position = Vector3.Lerp(MenuPanel.transform.position, MenuOrigPosition.transform.position, moveSpeed * Time.deltaTime);

            if (MenuPanel.transform.localPosition.x == tempMenuPosition)
            {
                Move_Menu_Panel_back = false;
                MenuPanel.transform.position = MenuOrigPosition.transform.position;
                tempMenuPosition = -999999999999.99f;
            }
            if (Move_Menu_Panel_back)
            {
                tempMenuPosition = MenuPanel.transform.position.x;
            }

        }
    }

    public void MovePanel()
    {
        Move_Menu_Panel = true;
        Move_Menu_Panel_back = false;
    }

    public void MovePanelBack()
    {
        Move_Menu_Panel = false;
        Move_Menu_Panel_back = true;
        Menu_ActiveJobs.SetActive(false);
    }

    public void OpenActiveJobsMenu()
    {
        Menu_ActiveJobs.SetActive(true);
    }

   
    public void CloseActiveJobsMenu()
    {
        
        Menu_ActiveJobs.SetActive(false);
        
    }
}
