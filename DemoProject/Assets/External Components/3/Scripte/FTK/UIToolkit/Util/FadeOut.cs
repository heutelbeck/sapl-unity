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

