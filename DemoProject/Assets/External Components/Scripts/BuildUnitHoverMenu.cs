using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildUnitHoverMenu : MonoBehaviour {

    private bool visible;
    private BuildUnit buildUnit;

    public RectTransform window;
    public TextMeshProUGUI textMaterial;
    public TextMeshProUGUI textJob;
    public Toggle toggleIsJobReady;

    public static Action<BuildUnit, Vector2> OnMouseHover;
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

    private void ShowHoverMenu(BuildUnit buildUnit, Vector2 mousePos) {
        visible = true;
        this.buildUnit = buildUnit;

        textMaterial.text = buildUnit.GetMaterial();
        textJob.text = buildUnit.GetNameOfPrintJob();
        toggleIsJobReady.isOn = (buildUnit.status == Status_BuildUnit.Finished_Printing || 
                                buildUnit.status == Status_BuildUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit);
 
        window.gameObject.SetActive(true);
        window.transform.position = new Vector2(mousePos.x, mousePos.y + 30);
    }

    private void HideHoverMenu() {
        visible = false;
        buildUnit = null;
        window.gameObject.SetActive(false);
    }

    void Update() {
        if (buildUnit != null && visible == true) {
            textMaterial.text = buildUnit.GetMaterial();
            textJob.text = buildUnit.GetNameOfPrintJob();
            toggleIsJobReady.isOn = (buildUnit.status == Status_BuildUnit.Finished_Printing ||
                                    buildUnit.status == Status_BuildUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit);
        }
    }

}