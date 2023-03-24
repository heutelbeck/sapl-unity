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
    /*[RequireComponent(typeof(RectTransform))]
    public class RectTransform2DResizableController : AnimatableBase, I2DResizable
    {
        [SerializeField]
        private Mode mode = Mode.Both;
        public Vector2 Size
        {
            get => ((RectTransform)transform).sizeDelta;
            set { ((RectTransform)transform).sizeDelta = value; }
        }
        public float BorderWidth { get => 0; set { } }
        public void ChangeSizeTo(Vector2 targetSize, float animationTime)
        {
            targetSize.x = Mathf.Max(targetSize.x, 0);
            targetSize.y = Mathf.Max(targetSize.y, 0);
            Animate("Size", ((RectTransform)transform).sizeDelta, targetSize, animationTime, value => { ((RectTransform)transform).sizeDelta = value; });
        }
        public void ChangeBorderWidthTo(float targetborderWidth, float animationTime) { }

        public enum Mode { Both, WidthOnly, HeightOnly }
    }*/

}
