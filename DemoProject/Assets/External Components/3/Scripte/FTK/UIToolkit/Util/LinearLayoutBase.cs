﻿/*
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
    public abstract class LinearLayoutBase : AnimatableBase, I2DResizable
    {

        protected bool rtEditing = false;
        public bool IsRTEditing() => rtEditing;
        public void SetEnableRTEditing(bool enableRTEditing) { rtEditing = enableRTEditing; }
        public abstract Vector2 Size { get; set; }
        public abstract float BorderWidth { get; set; }
        public abstract void ChangeBorderWidthTo(float targetborderWidth, float animationTime);
        public abstract void ChangeSizeTo(Vector2 targetSize, float animationTime);

        protected (float, float) GetExtentAndOffsetOf(int startIndex, int numCells, int proportionalSize, float layoutExtent)
        {
            numCells = Mathf.Max(numCells, 1);
            startIndex = Mathf.Clamp(startIndex, 0, numCells - 1);
            proportionalSize = Mathf.Clamp(proportionalSize, 1, numCells - startIndex);
            if (layoutExtent < 0.001f) return (0f, 0f);

            float midPoint = layoutExtent * ((startIndex + 0.5f * proportionalSize) / numCells - 0.5f);
            float size = layoutExtent * proportionalSize / numCells;
            return (size, midPoint);
        }

        protected (float, float) GetExtentAndOffsetOf(int startIndex, int numCells, int proportionalSize, float layoutExtent, float margin)
        {
            (float size, float midPoint) = GetExtentAndOffsetOf(startIndex, numCells, proportionalSize, layoutExtent);
            float maxMargin = layoutExtent / Mathf.Max(numCells, 1) * 0.5f;
            return (size - maxMargin * margin, midPoint);
        }
    }

}
