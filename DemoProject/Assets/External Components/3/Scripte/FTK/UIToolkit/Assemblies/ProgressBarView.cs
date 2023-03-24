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
using FTK.UIToolkit.Containers;
using FTK.UIToolkit.Primitives;
using FTK.UIToolkit.Util;
using Microsoft.MixedReality.Toolkit.Rendering;
using UnityEngine;

namespace FTK.UIToolkit.Assemblies
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(VerticalLayout))]
    public class ProgressBarView : AnimatableBase
    {
        [Range(0, 1)]
        [SerializeField]
        private float progress = 0.25f;
        [SerializeField]
        private Gradient colors;
        [SerializeField]
        private string label = "Progress Bar View";
        [SerializeField]
        private string leftInfo = "";
        [SerializeField]
        private string rightInfo = "";

        private Material barPanelMaterial;
        private Transform barPanel;
        private TextPanelView textLabel;
        private TextPanelView leftInfoPanel;
        private TextPanelView rightInfoPanel;
        public Vector2 Size
        {
            get => GetComponent<VerticalLayout>().Size;
            set { GetComponent<VerticalLayout>().Size = value; }
        }

        public float BorderWidth
        {
            get => GetComponent<VerticalLayout>().BorderWidth;
            set { GetComponent<VerticalLayout>().BorderWidth = value; }
        }
        public float Progress
        {
            get => progress;
            set
            {
                progress = Mathf.Clamp(value, 0, 1);
                SetColorAndBarSize();
            }
        }
        public Gradient Colors
        {
            get => colors;
            set
            {
                colors = value;
                SetColorAndBarSize();
            }
        }

        public string Label
        {
            get => label;
            set
            {
                label = value;
                UpdateTextLabel();
            }
        }

        public string LeftInfo
        {
            get => leftInfo;
            set
            {
                leftInfo = value;
                UpdateLeftInfo();
            }
        }

        public string RightInfo
        {
            get => rightInfo;
            set
            {
                rightInfo = value;
                UpdateRightInfo();
            }
        }

        public void ChangeProgressTo(float targetProgress, float animationTime)
        {
            Animate("Progress", progress, Mathf.Clamp(targetProgress, 0, 1), animationTime, value =>
            {
                progress = value;
                SetColorAndBarSize();
            });
        }

        private void OnValidate()
        {
            UpdateTextLabel();
            UpdateLeftInfo();
            UpdateRightInfo();
        }

        private void Update()
        {
            SetColorAndBarSize();
        }
        private void CheckBarPanel()
        {
            if (barPanel == null)
            {
                barPanel = transform.Find("Bar_Panel_Transform/Bar_Panel");
                if (barPanel == null)
                    Debug.LogError("No bar-panel child attached!");
            }
        }

        private void CheckMaterial()
        {
            if (barPanelMaterial == null)
            {
                CheckBarPanel();
                barPanelMaterial = barPanel.gameObject.GetComponent<MaterialInstance>()?.Material;
                if (barPanelMaterial == null)
                    Debug.LogWarning("No MeshRenderer, MaterialInstance or Material found bar-panel child! " + gameObject.name);
            }
        }
        private void SetColorAndBarSize()
        {
            CheckMaterial();
            if (barPanelMaterial != null)
                barPanelMaterial.SetColor("_Color", colors.Evaluate(progress));

            Vector2 size = Size;
            size.y *= 0.25f;
            float offset = (progress - 1) * 0.5f * size.x;
            size.x *= progress;
            barPanel.localScale = new Vector3(size.x, size.y, size.x);
            barPanel.localPosition = new Vector3(offset, 0, 0);
        }

        private void UpdateTextLabel()
        {
            if (textLabel == null)
            {
                textLabel = transform.Find("Text_Label[TextPanel]")?.GetComponent<TextPanelView>();
                if (textLabel == null)
                    Debug.LogError("No TextLabel child with TextPanelView attached!");
            }
            textLabel.Text = label;
        }

        private void UpdateLeftInfo()
        {
            if (leftInfoPanel == null)
            {
                leftInfoPanel = transform.Find("Text_Info/LeftText[TextPanel]")?.GetComponent<TextPanelView>();
                if (leftInfoPanel == null)
                    Debug.LogError("No LeftInfo child with TextPanelView attached!");
            }
            leftInfoPanel.Text = leftInfo;
        }

        private void UpdateRightInfo()
        {
            if (rightInfoPanel == null)
            {
                rightInfoPanel = transform.Find("Text_Info/RightText[TextPanel]")?.GetComponent<TextPanelView>();
                if (rightInfoPanel == null)
                    Debug.LogError("No RightInfo child with TextPanelView attached!");
            }
            rightInfoPanel.Text = rightInfo;
        }
    }
}
