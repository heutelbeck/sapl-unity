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
    public class AnimatableTransform : AnimatableBase
    {
        public void AnimatePosition(Vector3 targetPosition, float time)
        {
            Animate("Position", transform.position, targetPosition, time, value => { transform.position = value; });
        }
        public void AnimateRotation(Quaternion targetRotation, float time)
        {
            Animate("Rotation", transform.rotation, targetRotation, time, value => { transform.rotation = value; });
        }
        public void AnimateLocalPosition(Vector3 targetPosition, float time)
        {
            if (transform.parent == null) AnimatePosition(targetPosition, time);
            else Animate("Position", transform.position, transform.parent.TransformPoint(targetPosition), time, value => { transform.position = value; });
        }

        public void AnimateLocalRotation(Quaternion targetRotation, float time)
        {
            if (transform.parent == null) AnimateRotation(targetRotation, time);
            else Animate("Rotation", transform.localRotation, targetRotation, time, value => { transform.localRotation = value; });
        }
        public void AnimateLocalScale(Vector3 targetScale, float time)
        {
            Animate("Position", transform.localScale, targetScale, time, value => { transform.localScale = value; });
        }
    }

}