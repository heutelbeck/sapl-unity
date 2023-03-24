/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
