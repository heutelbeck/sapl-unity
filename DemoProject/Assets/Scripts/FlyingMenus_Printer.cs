using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyingMenus_Printer : MonoBehaviour
{
    public Button back_button,  buttonBackDeviceStatus, buttonBackAlerts, buttonShowAlerts, buttonShowDeviceStatus, buttonSelectPrintJob;
    public Button buttonSelectPrintJobBack, buttoncreatePrintJob1, buttonAddSelectedMaterial, buttonCreatePrintJob2, buttonCreatePrintJob3, buttonCreatePrintJob1Back, buttonCreatePrintJob2Back, buttonCreatePrintJob3Back; 
    [SerializeField] private GameObject Menu2;
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject DeviceStatusMenu;
    [SerializeField] private GameObject AlertsMenu;
    [SerializeField] private GameObject JobSelectionMenu;
    [SerializeField] private GameObject JobCreateMenu1;
    [SerializeField] private GameObject JobCreateMenu2;
    [SerializeField] private GameObject JobCreateMenu3;

    private void Awake()
    {
        //Menu2.SetActive(false);
        DeviceStatusMenu.SetActive(false);
        AlertsMenu.SetActive(false);
        JobSelectionMenu.SetActive(false);
        JobCreateMenu1.SetActive(false);
        JobCreateMenu2.SetActive(false);
        JobCreateMenu3.SetActive(false);

    }


    // Start is called before the first frame update
    void Start()
    {
        //Menu2.SetActive(false);
        
        
        buttonBackDeviceStatus.onClick.AddListener(BackFromStatus);
        buttonShowAlerts.onClick.AddListener(ShowAlerts);
        buttonBackAlerts.onClick.AddListener(BackFromAlerts);
        buttonShowDeviceStatus.onClick.AddListener(GetDeviceStatus);
        buttonSelectPrintJob.onClick.AddListener(SelectPrintJob);
        buttonSelectPrintJobBack.onClick.AddListener(BackFromPrintJobSelect);
        buttoncreatePrintJob1.onClick.AddListener(OpenCreatePrintJob1);
        buttonCreatePrintJob2.onClick.AddListener(OpenCreatePrintJob2);
        buttonCreatePrintJob3.onClick.AddListener(OpenCreatePrintJob3);
        buttonCreatePrintJob1Back.onClick.AddListener(CloseCreateJob1);
        buttonCreatePrintJob2Back.onClick.AddListener(CloseCreateJob2);
        buttonCreatePrintJob3Back.onClick.AddListener(CloseCreateJob3);
        buttonAddSelectedMaterial.onClick.AddListener(AddSelectedMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

   
   

    private void GetDeviceStatus()
    {
        DeviceStatusMenu.SetActive(true);
        Menu2.SetActive(false);
    }

    private void ShowAlerts()
    {
        
        AlertsMenu.SetActive(true);
    }

    private void BackFromStatus()
    {
        DeviceStatusMenu.SetActive(false);
        Menu.SetActive(true);
        AlertsMenu.SetActive(false);
    }

    private void BackFromAlerts()
    {
        AlertsMenu.SetActive(false);
        DeviceStatusMenu.SetActive(true);
    }

    private void SelectPrintJob()
    {
        JobSelectionMenu.SetActive(true);

    }

    private void BackFromPrintJobSelect()
    {
        JobSelectionMenu.SetActive(false);
        Menu.SetActive(true);
    }

    public void OpenCreatePrintJob1()
    {
        JobCreateMenu1.SetActive(true);
    }

    public void OpenCreatePrintJob2()
    {
        JobCreateMenu2.SetActive(true);
    }
    public void OpenCreatePrintJob3()
    {
        JobCreateMenu3.SetActive(true);
    }

    public void CloseCreateJob1()
    {
        JobCreateMenu1.SetActive(false);
        JobCreateMenu2.SetActive(false);
        JobCreateMenu3.SetActive(false);
    }

    public void CloseCreateJob2()
    {
        
        JobCreateMenu2.SetActive(false);
        JobCreateMenu3.SetActive(false);
    }

    public void CloseCreateJob3()
    {
               
        JobCreateMenu3.SetActive(false);
    }

    public void AddSelectedMaterial()
    {
        JobCreateMenu3.SetActive(false);
    }
}
