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
using FTK.UIToolkit.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FTK.UIToolkit.Primitives
{
    [RequireComponent(typeof(RectTransform))]
    public class StandardPanelView : AnimatableBase, I2DResizable
    {
        [Min(0f)]
        [SerializeField]
        private Vector2 size = Vector2.one;
        [Min(0f)]
        [SerializeField]
        private float borderWidth = 0.001f;
        [SerializeField]
        private Material standardMaterial;

        protected Transform backgroundPanelTransform = null;
        protected bool rtEditing = false;
        public void SetEnableRTEditing(bool rtEditing)
        {
            this.rtEditing = rtEditing;
            if (rtEditing) OnValidate();
        }

        public bool IsRTEditing() => rtEditing;
        public virtual Vector2 Size
        {
            get => size;
            set
            {
                size = value;
                CheckBackground();
                UpdateScale();
            }
        }
        public virtual float BorderWidth
        {
            get => borderWidth;
            set
            {
                borderWidth = Mathf.Max(value, 0f);
                CheckBackground();
                UpdateScale();
            }
        }

        public Material StandardMaterial
        {
            set
            {
                standardMaterial = value;
                CheckMaterial(standardMaterial, "Standard Material");
                SetMaterial(standardMaterial);
            }
        }

        public Vector3 ActualPanelScale
        {
            get
            {
                CheckBackground();
                return backgroundPanelTransform.localScale;
            }
        }

        public void ChangeSizeTo(Vector2 targetSize, float animationTime)
        {
            Animate("Size", size, targetSize, animationTime, value =>
            {
                size = value;
                CheckBackground();
                UpdateScale();
            });
        }

        public void ChangeBorderWidthTo(float targetBorderWidth, float animationTime)
        {
            Animate("BorderWidth", borderWidth, Mathf.Max(targetBorderWidth, 0f), animationTime, value =>
            {
                borderWidth = value;
                CheckBackground();
                UpdateScale();
            });
        }

        protected void OnValidate()
        {
            if (rtEditing)
            {
                CheckBackground();
                UpdateScale();
                CheckMaterial(standardMaterial, "Standard Material");
                SetMaterial(standardMaterial);
            }
        }

        private void CheckBackground()
        {
            if (backgroundPanelTransform == null)
            {
                backgroundPanelTransform = transform.Find("Background_Standard_Panel");
                if (backgroundPanelTransform == null)
                    Debug.LogError("No background-panel child attached!");
            }
        }

        protected static void CheckMaterial(Material material, string materialSlotName)
        {
            Debug.Assert(material != null, "Material '" + materialSlotName + "' is not set!");
        }

        protected void SetMaterial(Material material)
        {
            MeshRenderer renderer = backgroundPanelTransform.gameObject.GetComponent<MeshRenderer>();
            if (renderer == null)
                Debug.LogError("Panel-background has no MeshRenderer attached!");
            else renderer.sharedMaterial = material;
        }

        private void UpdateScale()
        {
            RectTransform transformAsRect = (RectTransform)transform;
            transformAsRect.sizeDelta = this.size;
            backgroundPanelTransform.localScale = new Vector3(size.x, size.y, GetScaleZ());
        }

        protected float GetScaleZ() => Mathf.Max(borderWidth * 8.0f, 0.000001f);

        
    }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StandardPanelView))]
    public class StandardPanelViewEditor : Editor
    {
        private GUIStyle buttonStyle;
        private bool enableToggle = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            StandardPanelView spv = (StandardPanelView)target;
            buttonStyle = new GUIStyle(GUI.skin.button);
            enableToggle = spv.IsRTEditing();
            if (GUILayout.Toggle(enableToggle, "RT editing", buttonStyle) != enableToggle)
            {
                enableToggle = !enableToggle;
                spv.SetEnableRTEditing(enableToggle);
            }
        }

    }
#endif

}
