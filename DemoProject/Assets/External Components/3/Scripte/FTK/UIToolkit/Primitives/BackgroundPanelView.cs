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
using UnityEngine;

namespace FTK.UIToolkit.Primitives
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class BackgroundPanelView : AnimatableBase, I2DResizable
    {
        [SerializeField]
        private float borderWidth = 0.001f;
        [Min(0.000001f)]
        [SerializeField]
        private float margin = 0.01f;
        [SerializeField]
        private Material standardMaterial;

        public float BorderWidth
        {
            get => borderWidth;
            set => borderWidth = value;
        }

        public Material StandardMaterial
        {
            get => standardMaterial;
            set
            {
                standardMaterial = value;
                CheckMaterial(standardMaterial, "Standard Material");
                SetMaterial(standardMaterial);
            }
        }

        protected static void CheckMaterial(Material material, string materialSlotName)
        {
            Debug.Assert(material != null, "Material '" + materialSlotName + "' is not set!");
        }

        protected void SetMaterial(Material material)
        {
            MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();
            if (renderer == null)
                Debug.LogError("Panel-background has no MeshRenderer attached!");
            else renderer.sharedMaterial = material;

        }

        public void ChangeBorderWidthTo(float targetBorderWidth, float animationTime)
        {
            Animate("BorderWidth", borderWidth, targetBorderWidth, animationTime, value =>
            {
                borderWidth = value;
            });
        }

        public float Margin
        {
            get => margin;
            set => margin = Mathf.Max(value, 0.000001f);
        }

        public void ChangeMarginTo(float targetMargin, float animationTime)
        {
            Animate("Margin", margin, Mathf.Max(targetMargin, 0.000001f), animationTime, value =>
            {
                margin = value;
            });
        }

        public Vector2 Size { get => (transform as RectTransform).sizeDelta; set { } }

        private void OnValidate()
        {
            SetMaterial(standardMaterial);
        }

        private void LateUpdate()
        {
            RectTransform transformAsRect = transform as RectTransform;
            Vector2 size = transformAsRect.rect.size * 0.5f + Vector2.one * margin;
            transformAsRect.localScale = new Vector3(size.x, size.y, GetScaleZ());
        }

        protected float GetScaleZ()
        {
            return Mathf.Max(borderWidth * 8.0f, 0.000001f);
        }

        public void ChangeSizeTo(Vector2 targetSize, float animationTime) { }
    }
}

