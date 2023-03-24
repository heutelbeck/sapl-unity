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
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FTK.UIToolkit.Containers
{
    [RequireComponent(typeof(RectTransform))]
    public class UIGridLayout : LinearLayoutBase
    {
        [Min(0)]
        [SerializeField]
        private Vector2 size = new Vector2(0.25f, 0.25f);
        [SerializeField]
        private float borderWidth = 0.001f;
        [Range(0, 1)]
        [SerializeField]
        private float marginX = 0;
        [Range(0, 1)]
        [SerializeField]
        private float marginY = 0;
        [Space(10)]
        [SerializeField]
        private List<ElementDescriptor> elements = new List<ElementDescriptor>();

        private List<I2DResizable> children = new List<I2DResizable>();

        public int ChildCount => children.Count;

        public override Vector2 Size
        {
            get => size;
            set
            {
                size = value;
                size.x = Mathf.Max(size.x, 0);
                size.y = Mathf.Max(size.y, 0);
                SetTransformSize();
                UpdateLayout();
            }
        }
        public override float BorderWidth
        {
            get => borderWidth;
            set
            {
                borderWidth = value;
                UpdateLayout();
            }
        }

        public I2DResizable GetChild(int index) => children[index];

        public void Clear(bool deleteChildren = true)
        {
            if (deleteChildren)
                foreach (var element in elements)
                    if (!element.IsValid)
                        Destroy(element.ElementTransform.gameObject);
            elements.Clear();
        }

        public void AddElement(I2DResizable element, Vector2Int startCell, bool arrange = true) => AddElement(element, startCell, Vector2Int.one, arrange);

        public void AddElement(I2DResizable element, Vector2Int startCell, Vector2Int proportionalSize, bool arrange = true)
        {
            proportionalSize.x = Math.Max(proportionalSize.x, 1);
            proportionalSize.y = Math.Max(proportionalSize.y, 1);
            startCell.x = Math.Max(startCell.x, 0);
            startCell.y = Math.Max(startCell.y, 0);
            if (element != null)
            {
                elements.Add(new ElementDescriptor { element = element.gameObject, startCell = startCell, proportionalSize = proportionalSize });
                children.Add(element);
            }
            if (arrange) UpdateLayout();
        }

        public override void ChangeSizeTo(Vector2 targetSize, float animationTime)
        {
            targetSize.x = Mathf.Max(targetSize.x, 0);
            targetSize.y = Mathf.Max(targetSize.y, 0);
            Animate("Size", size, targetSize, animationTime, value =>
            {
                size = value;
                SetTransformSize();
                UpdateLayout();
            });
        }

        public override void ChangeBorderWidthTo(float targetBorderWidth, float animationTime)
        {
            Animate("BorderWidth", borderWidth, targetBorderWidth, animationTime, value =>
            {
                borderWidth = value;
                UpdateLayout();
            });
        }

        public IEnumerator<I2DResizable> GetEnumerator()
        {
            PruneElements();
            return children.GetEnumerator();
        }

        public void UpdateLayout()
        {
            children.Clear();
            var numCells = GetNumCells();
            foreach (var elem in elements) if (elem.IsValid)
                {
                    (var extentX, var offsetX) = GetExtentAndOffsetOf(elem.startCell.x, numCells.x, elem.proportionalSize.x, size.x, marginX);
                    (var extentY, var offsetY) = GetExtentAndOffsetOf(elem.startCell.y, numCells.y, elem.proportionalSize.y, size.y, marginY);
                    elem.ElementTransform.SetParent(transform, true);
                    elem.ElementTransform.localPosition = new Vector3(offsetX, -offsetY, 0);
                    elem.Element.Size = new Vector2(extentX, extentY);
                    elem.Element.BorderWidth = borderWidth;
                    children.Add(elem.Element);
                }
        }

        private Vector2Int GetNumCells()
        {
            var numCells = Vector2Int.zero;
            foreach (ElementDescriptor elem in elements)
                if (elem.IsValid)
                {
                    var extent = elem.startCell + elem.proportionalSize;
                    numCells.x = Mathf.Max(numCells.x, extent.x);
                    numCells.y = Mathf.Max(numCells.y, extent.y);
                }
            return numCells;
        }

        private void PruneElements()
        {
            for (int i = 0; i < elements.Count; i++)
                if (!elements[i].IsValid)
                {
                    elements.RemoveAt(i);
                    i--;
                }
        }

        private void FixProportionalSizesEditor()
        {
            foreach (ElementDescriptor elem in elements)
                elem.EditorCheck();
        }

        private void OnValidate()
        {
            if (rtEditing)
            {
                FixProportionalSizesEditor();
                SetTransformSize();
                UpdateLayout();
            }
        }

        private void SetTransformSize()
        {
            RectTransform rect = (RectTransform)transform;
            rect.sizeDelta = size;
        }

        [Serializable]
        private class ElementDescriptor
        {
            [SerializeField]
            internal GameObject element;
            [Min(0)]
            [SerializeField]
            internal Vector2Int startCell = Vector2Int.zero;
            [Min(1)]
            [SerializeField]
            internal Vector2Int proportionalSize = Vector2Int.one;

            public bool IsValid { get => element != null && element.TryGetComponent(out I2DResizable _); }
            public I2DResizable Element { get => element.GetComponent<I2DResizable>(); }
            public Transform ElementTransform { get => element.transform; }
            public Vector2Int ProportionalSize { get => proportionalSize; }
            public void EditorCheck()
            {
                proportionalSize.x = Mathf.Max(proportionalSize.x, 1);
                proportionalSize.y = Mathf.Max(proportionalSize.y, 1);
                startCell.x = Mathf.Max(startCell.x, 0);
                startCell.y = Mathf.Max(startCell.y, 0);
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UIGridLayout))]
    public class GridLayoutEditor : Editor
    {
        private GUIStyle buttonStyle;
        private bool enableToggle = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            UIGridLayout gl = (UIGridLayout)target;
            buttonStyle = new GUIStyle(GUI.skin.button);
            enableToggle = gl.IsRTEditing();
            if (GUILayout.Toggle(enableToggle, "RT editing", buttonStyle) != enableToggle)
            {
                enableToggle = !enableToggle;
                gl.SetEnableRTEditing(enableToggle);
            }
        }

    }
#endif
}