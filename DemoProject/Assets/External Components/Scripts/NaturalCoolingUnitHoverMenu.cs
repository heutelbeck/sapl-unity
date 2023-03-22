using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NaturalCoolingUnitHoverMenu : MonoBehaviour { 

    private bool visible;
    private NaturalCoolingUnit coolingUnit;

    public RectTransform window;
    public TextMeshProUGUI textId;
    public TextMeshProUGUI textName;
    public Toggle toggleEmpty;
    public TextMeshProUGUI textPrintJob;
    public TextMeshProUGUI textRemainingCoolingTime;

    public static Action<NaturalCoolingUnit, Vector2> OnMouseHover;
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

    private void ShowHoverMenu(NaturalCoolingUnit coolingUnit, Vector2 mousePos) {
        visible = true;
        this.coolingUnit = coolingUnit;

        textId.text = coolingUnit.id;
        textName.text = coolingUnit.name;
        toggleEmpty.isOn = (coolingUnit.status == Status_CoolingUnit.Ready || 
                            coolingUnit.status == Status_CoolingUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit);
        textPrintJob.text = coolingUnit.GetNameOfPrintJob();
        Timer.UpdateTimerDisplay(textRemainingCoolingTime, coolingUnit.remainingCoolingTime);

        //window.sizeDelta = new Vector2(tipText.preferredWidth > 200 ? 200 : tipText.preferredWidth, tipText.preferredHeight);
        window.gameObject.SetActive(true);
        window.transform.position = new Vector2(mousePos.x, mousePos.y + 30);
    }

    private void HideHoverMenu() {
        visible = false;
        coolingUnit = null;
        window.gameObject.SetActive(false);
    }

    void Update() {
        if (coolingUnit != null && visible == true) {
            textId.text = coolingUnit.id;
            textName.text = coolingUnit.name;
            toggleEmpty.isOn = (coolingUnit.status == Status_CoolingUnit.Ready ||
                                coolingUnit.status == Status_CoolingUnit.InProgress_PushingContentFromBuildUnitToCoolingUnit);
            textPrintJob.text = coolingUnit.GetNameOfPrintJob();
            Timer.UpdateTimerDisplay(textRemainingCoolingTime, coolingUnit.remainingCoolingTime, coolingUnit.status == Status_CoolingUnit.Finished_Cooling);
        }
    }

}