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
