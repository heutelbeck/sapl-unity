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
    public class HorizontalLayout : LinearLayoutBase
    {
        [Min(0)]
        [SerializeField]
        private Vector2 size = new Vector2(0.25f, 0.05f);
        [SerializeField]
        private float borderWidth = 0.001f;
        [Range(0, 1)]
        [SerializeField]
        private float margin = 0;
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
                    if (!element.IsVoid)
                        Destroy(element.ElementTransform.gameObject);
            elements.Clear();
        }

        public void AddSpace(int proportionalSize = 1, bool arrange = true)
        {
            proportionalSize = Math.Max(proportionalSize, 0);
            if (proportionalSize > 0) elements.Add(new ElementDescriptor { element = null, proportionalSize = proportionalSize });
            if (arrange) UpdateLayout();
        }

        public void AddElement(I2DResizable element, int proportionalSize = 1, bool arrange = true)
        {
            proportionalSize = Math.Max(proportionalSize, 0);
            if (proportionalSize > 0)
            {
                elements.Add(new ElementDescriptor { element = element.gameObject, proportionalSize = proportionalSize });
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
            int numCells = GetNumCells();
            int currentIndex = 0;
            foreach (ElementDescriptor elem in elements)
                if (elem.IsValid)
                {
                    if (!elem.IsVoid)
                    {
                        (float extent, float offset) = GetExtentAndOffsetOf(currentIndex, numCells, elem.ProportionalSize, size.x, margin);
                        elem.ElementTransform.SetParent(transform, true);
                        elem.ElementTransform.localPosition = new Vector3(offset, 0, 0);
                        elem.Element.Size = new Vector2(extent, size.y);
                        elem.Element.BorderWidth = borderWidth;
                        children.Add(elem.Element);
                    }
                    currentIndex += elem.ProportionalSize;
                }
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

        private int GetNumCells()
        {
            int numCells = 0;
            foreach (ElementDescriptor elem in elements)
                if (elem.IsValid)
                    numCells += elem.ProportionalSize;
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

        [Serializable]
        private class ElementDescriptor
        {
            [SerializeField]
            internal GameObject element;
            [Min(1)]
            [SerializeField]
            internal int proportionalSize = 1;

            public bool IsValid { get => element == null || (element != null && element.TryGetComponent(out I2DResizable _)); }
            public bool IsVoid { get => element == null; }
            public I2DResizable Element { get => element.GetComponent<I2DResizable>(); }
            public Transform ElementTransform { get => element.transform; }
            public int ProportionalSize { get => proportionalSize; }
            public void EditorCheck() { proportionalSize = Mathf.Max(proportionalSize, 1); }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(HorizontalLayout))]
    public class HorizontalLayoutEditor : Editor
    {
        private GUIStyle buttonStyle;
        private bool enableToggle = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            HorizontalLayout hl = (HorizontalLayout)target;
            buttonStyle = new GUIStyle(GUI.skin.button);
            enableToggle = hl.IsRTEditing();
            if (GUILayout.Toggle(enableToggle, "RT editing", buttonStyle) != enableToggle)
            {
                enableToggle = !enableToggle;
                hl.SetEnableRTEditing(enableToggle);
            }
        }

    }
#endif
}

