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
using UnityEngine;

namespace FTK.UIToolkit.Util
{
    public class FadeOut : AnimatableBase
    {
        [Range(0, 1)]
        [SerializeField]
        private float opacity = 1;

        [SerializeField]
        private Renderer[] renderers;

        private float oldOpacity = 1;
        public float Opacity
        {
            get => opacity;
            set
            {
                StopAnimation("Opacity");
                opacity = Mathf.Clamp(value, 0, 1);
                oldOpacity = opacity;
                SetOpacity();
            }
        }

        public void ChangeToNewOpacity(float targetOpacity, float animationTime)
        {
            Animate("Opacity", opacity, Mathf.Clamp(targetOpacity, 0, 1), animationTime, value =>
            {
                opacity = value;
                SetOpacity();
            });
        }

        public void ChangeToOldOpacity(float animationTime)
        {
            Animate("Opacity", opacity, oldOpacity, animationTime, value =>
            {
                opacity = value;
                SetOpacity();
            });
        }

        public void FadeToMaxAlpha() { ChangeToNewOpacity(1, 0.5f); }

        public void FadeToMinAlpha() { ChangeToNewOpacity(0, 0.5f); }

        public void FadeToOldAlpha() { ChangeToOldOpacity(0.5f); }

        private void OnValidate()
        {
            SetOpacity();
        }

        private void SetOpacity()
        {
            foreach (Renderer renderer in renderers)
                foreach (Material material in renderer.sharedMaterials)
                {
                    material?.SetFloat("_Intensity_", opacity * 2.5f);
                    material?.SetFloat("_Edge_Width_", opacity);
                }
        }
    }
}

