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
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FTK.UIToolkit.Assemblies
{
    public class RadialObjectMenu : MonoBehaviour
    {
        [Range(0, 0.5f)]
        [SerializeField]
        private float elasticity = 0.25f;
        [Space(10)]
        [SerializeField]
        private List<GameObject> objects = new List<GameObject>();

        private SliderView slider;
        private RadialObjectContainer container;

        public List<GameObject> Objects { get => objects; }
        public RadialObjectContainer Container { get => container; }

        public void Apply()
        {
            CheckSlider();
            CheckLayout();
            SynchronizeObjects();
            SetupSlider();
        }

        public void OnSliderUpdate(float value)
        {
            CheckLayout();
            container.ChangeCenterTo(value, elasticity);
        }

        public void ScrollTo(float value)
        {
            value = Mathf.Clamp(value, 0, 1);
            slider.ChangeSliderValueTo(value, 2 * elasticity);
            container.ChangeCenterTo(value, 2 * elasticity);
        }

        private void OnValidate()
        {
            Apply();
        }

        private void CheckSlider()
        {
            if (slider == null)
            {
                slider = GetComponentInChildren<SliderView>(true);
                if (slider == null)
                    Debug.LogError("No SliderView child attached!");
            }
        }

        private void CheckLayout()
        {
            if (container == null)
            {
                container = GetComponentInChildren<RadialObjectContainer>();
                if (container == null)
                    Debug.LogError("No RadialObjectLayout child attached!");
            }
        }

        private void SynchronizeObjects()
        {
            container.transform.DetachChildren();
            foreach (GameObject obj in objects)
                if (obj != null)
                    obj.transform.parent = container.transform;
            if (objects.Count <= container.VisibleObjects)
            {
                slider.SliderValue = 0.5f;
                container.ChangeCenterTo(0.5f, 0.25f);
                slider.gameObject.SetActive(false);
            }
            else
            {
                slider.gameObject.SetActive(true);
            }
            container.ForceUpdate();
            //if (objects.Count <= container.VisibleObjects) container.ForceVisible();
        }

        private void SetupSlider()
        {
            slider.SelectionWidth = container.VisibleObjects / Mathf.Max(objects.Count, 1.0f);
            Vector2 size = slider.Size;
            size.x = Mathf.Sqrt(20 * container.Radius) * 0.1f;
            slider.Size = size;
        }
    }
#if UNITY_Editor
    [CustomEditor(typeof(RadialObjectMenu))]
    public class RadialObjectMenuEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            RadialObjectMenu rom = (RadialObjectMenu)target;
            GUILayout.Space(5);
            if (GUILayout.Button("Apply"))
                rom.Apply();
        }

    }
#endif
}
