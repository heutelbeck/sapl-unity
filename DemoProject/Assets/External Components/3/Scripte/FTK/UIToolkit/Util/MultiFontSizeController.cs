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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FTK.UIToolkit.Util
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(I2DResizable))]
    public class MultiFontSizeController : MonoBehaviour
    {
        [Min(0f)]
        [SerializeField]
        private float fontSizeFactor = 150f;
        [SerializeField]
        private List<ElementDescriptor> textObjects;

        private List<(TextPanelView, float)> textPanels = new List<(TextPanelView, float)>();
        private List<(MultiFontSizeController, float)> multiControllers = new List<(MultiFontSizeController, float)>();

        public float FontSizeFactor
        {
            get => fontSizeFactor;
            set => fontSizeFactor = value;
        }

        public float GetMinSizeOfChildren()
        {
            var rect = GetComponent<I2DResizable>();
            var minLength = Mathf.Min(rect.Size.x, rect.Size.y);
            foreach ((var textPanel, var _) in textPanels)
                minLength = Mathf.Min(minLength, textPanel.Size.x - textPanel.BorderWidth * 3f, textPanel.Size.y - textPanel.BorderWidth * 3f);
            foreach ((var multiController, var _) in multiControllers)
                minLength = Mathf.Min(minLength, multiController.GetMinSizeOfChildren());
            return Mathf.Max(0f, minLength);
        }

        private void LateUpdate()
        {
            var fontSize = GetMinSizeOfChildren() * fontSizeFactor;
            foreach ((var textPanel, var sizeMultiplier) in textPanels)
                {
                    textPanel.OverrideFontSize = true;
                    textPanel.FontSize = fontSize * sizeMultiplier;
                }
            foreach ((var multiController, var sizeMultiplier) in multiControllers)
            {
                var childControllerMinSize = Mathf.Max(multiController.GetMinSizeOfChildren(), 0.000001f);
                multiController.FontSizeFactor = fontSize * sizeMultiplier / childControllerMinSize;
            }
        }

        private void OnValidate()
        {
            Sync();
            foreach (var descriptor in textObjects) if (descriptor.sizeMultiplier <= 0f)
                    descriptor.sizeMultiplier = 1f;
        }

        private void Start() => Sync();

        private void Sync()
        {
            textPanels.Clear();
            multiControllers.Clear();
            for (var i = 0; i < (textObjects?.Count ?? 0); i++) {
                var descriptor = textObjects[i];
                if (descriptor.textObject == null)
                    continue;
                else if (descriptor.TryGetMultiFontSizeController(out var multiFontSizeController))
                    multiControllers.Add((multiFontSizeController, descriptor.sizeMultiplier));
                else if (descriptor.TryGetTextPanelView(out var textPanelView))
                    textPanels.Add((textPanelView, descriptor.sizeMultiplier));
                else
                {
                    textObjects.RemoveAt(i);
                    i--;
                }
            }
        }

        [Serializable]
        private class ElementDescriptor
        {
            [SerializeField]
            internal GameObject textObject;
            [Min(0f)]
            [SerializeField]
            internal float sizeMultiplier = 1f;

            internal bool TryGetTextPanelView(out TextPanelView textPanelView)
            {
                textPanelView = null;
                return textObject != null && textObject.TryGetComponent(out textPanelView);
            }

            internal bool TryGetMultiFontSizeController(out MultiFontSizeController multiFontSizeController)
            {
                multiFontSizeController = null;
                return textObject != null && textObject.TryGetComponent(out multiFontSizeController);
            }
        }
    }
}