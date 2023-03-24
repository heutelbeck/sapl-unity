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
using Microsoft.MixedReality.Toolkit.Rendering;
using UnityEngine;

namespace FTK.UIToolkit.Util
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(MaterialInstance))]
    public class AnimatableMaterialInstance : AnimatableBase
    {
        private new Renderer renderer;
        private void Start() => renderer = GetComponent<Renderer>();

        public bool AnimateMaterialProperty(string name, float target, float time)
        {
            try
            {
                var startVal = renderer?.sharedMaterial?.GetFloat(name);
                if (!startVal.HasValue)
                    return false;
                Animate("FLOAT:" + name, startVal.Value, target, time, value => renderer?.sharedMaterial?.SetFloat(name, value));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AnimateMaterialProperty(string name, Vector4 target, float time)
        {
            try
            {
                var startVal = renderer?.sharedMaterial?.GetVector(name);
                if (!startVal.HasValue)
                    return false;
                Animate("VECTOR:" + name, startVal.Value, target, time, value => renderer?.sharedMaterial?.SetVector(name, value));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AnimateMaterialProperty(string name, Color target, float time)
        {
            try
            {
                var startVal = renderer?.sharedMaterial?.GetColor(name);
                if (!startVal.HasValue)
                    return false;
                Animate("COLOR:" + name, startVal.Value, target, time, value => renderer?.sharedMaterial?.SetColor(name, value));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}