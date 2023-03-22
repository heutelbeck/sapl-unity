using System;
using TMPro;
using UnityEngine;

public class PrinterHoverMenu : MonoBehaviour {

    // General
    private bool visible;
    private Printer printer;

    // Text fields
    [SerializeField]
    private RectTransform window;
    [SerializeField]
    private TextMeshProUGUI textId;
    [SerializeField]
    private TextMeshProUGUI textName;
    [SerializeField]
    private TextMeshProUGUI textBuildUnitName;
    [SerializeField]
    private TextMeshProUGUI textRemainingPrintingTime;

    // Actions
    public static Action<Printer, Vector2> OnMouseHover;
    public static Action OnMouseLoseFocus;

    private void OnEnable() {
        OnMouseHover += ShowHoverMenu;
        OnMouseLoseFocus += HideHoverMenu;
    }

    private void OnDisable() {
        OnMouseHover -= ShowHoverMenu;
        OnMouseLoseFocus -= HideHoverMenu;
    }

    void Start() {
        HideHoverMenu();
    }

    private void UpdateInformation() {
        textId.text = printer.Id;
        textName.text = printer.Name;
        textBuildUnitName.text = printer.IsBuildUnitAssigned == true ? printer.BuildUnitAssigned.name : "-";
        Timer.UpdateTimerDisplay(textRemainingPrintingTime, printer.RemainingPrintingTime, (printer.Status == Status_Printer.Finished_Printing));
    }

    void Update() {
        if (printer != null && visible == true) {
            UpdateInformation();
        }
    }

    private void ShowHoverMenu(Printer printer, Vector2 mousePos) {
        visible = true;
        this.printer = printer;

        UpdateInformation();

        window.transform.position = new Vector2(mousePos.x, mousePos.y + 30);
        window.gameObject.SetActive(true);
    }

    private void HideHoverMenu() {
        visible = false;
        printer = null;
        window.gameObject.SetActive(false);
    }

}