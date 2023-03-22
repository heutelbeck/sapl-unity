using FTK.UIToolkit.Primitives;
using FTK.UIToolkit.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(I2DResizable))]
public class BasicInfoGroupView : MonoBehaviour
{
    [SerializeField]
    private string title = "title";
    private TextPanelView titleTextPanel;

    public string TitelText
    {
        get => title;
        set
        {
            title = value;
            UpdateTitleText();
        }
    }
    private void UpdateTitleText()
    {
        if (titleTextPanel == null)
        {
            titleTextPanel = transform.Find("BasicInfo[HorizontalLayout]/Name[TextPanel]")?.GetComponent<TextPanelView>();
            if (titleTextPanel == null)
                Debug.LogError("No titleText child with TextPanelView attached!");
        }
        titleTextPanel.Text = title;
    }

    public I2DResizable Element => GetComponent<I2DResizable>();
}
