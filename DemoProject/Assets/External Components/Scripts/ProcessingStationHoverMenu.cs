using System;
using TMPro;
using UnityEngine;

public class ProcessingStationHoverMenu : MonoBehaviour {

    // General
    private bool visible;
    private ProcessingStation processingStation;

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
    private TextMeshProUGUI textCoolingUnitName;
    [SerializeField]
    private TextMeshProUGUI textRemainingProcessingTime;

    // Actions
    public static Action<ProcessingStation, Vector2> OnMouseHover;
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
        textId.text = processingStation.Id;
        textName.text = processingStation.Name;
        textBuildUnitName.text = processingStation.IsBuildUnitAssigned == true ? processingStation.BuildUnitAssigned.name : "-";
        textCoolingUnitName.text = processingStation.IsCoolingUnitAssigned == true ? processingStation.CoolingUnitAssigned.name : "-";

        Timer.UpdateTimerDisplay(textRemainingProcessingTime, processingStation.RemainingProcessingTime,
            (processingStation.Status == Status_ProcessingStation.Finished_PushingContentFromBuildUnitToCoolingUnit ||
                processingStation.Status == Status_ProcessingStation.Finished_EmptyingCoolingUnit ||
                processingStation.Status == Status_ProcessingStation.Finished_FillingInMaterial));
    }

    void Update() {
        if (processingStation != null && visible == true) {
            UpdateInformation();
        }
    }

    private void ShowHoverMenu(ProcessingStation processingStation, Vector2 mousePos) {
        visible = true;
        this.processingStation = processingStation;

        UpdateInformation();

        window.gameObject.SetActive(true);
    }

    private void HideHoverMenu() {
        visible = false;
        processingStation = null;
        window.gameObject.SetActive(false);
    }

}