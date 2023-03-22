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