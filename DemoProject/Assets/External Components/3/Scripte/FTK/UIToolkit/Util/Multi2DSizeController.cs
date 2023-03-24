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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTK.UIToolkit.Util
{
    public class Multi2DSizeController : AnimatableBase, I2DResizable
    {
        [Min(0)]
        [SerializeField]
        private Vector2 size = new Vector2(0.1f, 0.1f);
        [SerializeField]
        private float borderWidth = 0.001f;
        [SerializeField]
        private List<ElementDescriptor> elements = new List<ElementDescriptor>();

        private bool rtEditing = false;
        public bool IsRTEditing() => rtEditing;
        public void SetEnableRTEditing(bool enableRTEditing) { rtEditing = enableRTEditing; }
        public Vector2 Size
        {
            get => size;
            set
            {
                size = value;
                size.x = Mathf.Max(size.x, 0);
                size.y = Mathf.Max(size.y, 0);
                Apply();
            }
        }
        public float BorderWidth
        {
            get => borderWidth;
            set
            {
                borderWidth = Mathf.Clamp(value, 0, 1);
                Apply();
            }
        }
        public void ChangeSizeTo(Vector2 targetSize, float animationTime)
        {
            targetSize.x = Mathf.Max(targetSize.x, 0);
            targetSize.y = Mathf.Max(targetSize.y, 0);
            Animate("Size", size, targetSize, animationTime, value =>
            {
                size = value;
                Apply();
            });
        }

        public void ChangeBorderWidthTo(float targetBorderWidth, float animationTime)
        {
            Animate("BorderWidth", borderWidth, Mathf.Clamp(targetBorderWidth, 0, 1), animationTime, value =>
            {
                borderWidth = value;
                Apply();
            });
        }
        public void SetSizeOnUpdate(Vector2 targetSize) { ChangeSizeTo(targetSize, 0); }

        public void SetBorderWidthOnUpdate(float targetborderWidth) { ChangeBorderWidthTo(targetborderWidth, 0); }
        public void Subscribe(I2DResizable subscriber) => elements.Add(new ElementDescriptor(subscriber));

        public void Subscribe(I2DResizable subscriber, Vector2 sizeMutliplier) => elements.Add(new ElementDescriptor(subscriber, sizeMutliplier));

        public void Subscribe(I2DResizable subscriber, Vector2 sizeMutliplier, float borderWidthMultiplier) => elements.Add(new ElementDescriptor(subscriber, sizeMutliplier, borderWidthMultiplier));

        public void Unsubscribe(I2DResizable subscriber) => elements.RemoveAll(elem => elem.Element == subscriber);

        //private void Start() => Apply();

        private void OnValidate()
        {
            EditorCheck();
            if (rtEditing)
            {
                Apply();
            }
        }

        private void Apply()
        {
            if (TryGetComponent(out RectTransform rectTransform))
                rectTransform.sizeDelta = size;
            foreach (var elemDesc in elements)
                elemDesc.Apply(size, borderWidth);
        }

        private void EditorCheck()
        {
            foreach (ElementDescriptor elemDesc in elements)
                elemDesc.EditorCheck();
        }

        [Serializable]
        private class ElementDescriptor
        {
            [SerializeField]
            private GameObject element = null;
            [Min(0)]
            [SerializeField]
            private Vector2 sizeMultiplier = Vector2.one;
            [Min(0)]
            [SerializeField]
            private float borderWidthMultiplier = 1;
            private bool initialized = false;
            private I2DResizable resizableElement = null;

            public ElementDescriptor(I2DResizable element)
            {
                this.resizableElement = element;
                initialized = true;
            }

            public ElementDescriptor(I2DResizable element, Vector2 sizeMultiplier)
            {
                this.resizableElement = element;
                this.sizeMultiplier = sizeMultiplier;
                initialized = true;
            }

            public ElementDescriptor(I2DResizable element, Vector2 sizeMultiplier, float borderWidthMultiplier)
            {
                this.resizableElement = element;
                this.sizeMultiplier = sizeMultiplier;
                this.borderWidthMultiplier = borderWidthMultiplier;
                initialized = true;
            }

            public bool IsValid() => this.resizableElement != null;
            public I2DResizable Element { get => resizableElement; }

            public void Apply(Vector2 size, float borderWidth)
            {
                EditorCheck();
                if (IsValid())
                {
                    resizableElement.Size = new Vector2(size.x * sizeMultiplier.x, size.y * sizeMultiplier.y);
                    resizableElement.BorderWidth = borderWidth * borderWidthMultiplier;
                }
            }
            public void EditorCheck()
            {
                if (!initialized)
                {
                    if (sizeMultiplier == Vector2.zero) sizeMultiplier = Vector2.one;
                    if (borderWidthMultiplier == 0) borderWidthMultiplier = 1;
                    initialized = true;
                }
                if (resizableElement == null && element != null)
                    resizableElement = element.GetComponent<I2DResizable>();
            }
        }
    }
#if UNITY_EDITOR
    [UnityEditor.CanEditMultipleObjects]
    [UnityEditor.CustomEditor(typeof(Multi2DSizeController))]
    public class Multi2DSizeControllerEditor : UnityEditor.Editor
    {
        private GUIStyle buttonStyle;
        private bool enableToggle = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Multi2DSizeController m2dsc = (Multi2DSizeController)target;
            buttonStyle = new GUIStyle(GUI.skin.button);
            enableToggle = m2dsc.IsRTEditing();
            if (GUILayout.Toggle(enableToggle, "RT editing", buttonStyle) != enableToggle)
            {
                enableToggle = !enableToggle;
                m2dsc.SetEnableRTEditing(enableToggle);
            }
        }

    }
#endif
}
