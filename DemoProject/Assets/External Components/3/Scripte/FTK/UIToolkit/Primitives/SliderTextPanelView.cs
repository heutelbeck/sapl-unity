using FTK.UIToolkit.Primitives;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderTextPanelView : TextPanelView
{
    public void OnSliderUpdated(SliderEventData eventData)
    {
        textMesh.text = $"{eventData.NewValue:F0}";
    }
}
