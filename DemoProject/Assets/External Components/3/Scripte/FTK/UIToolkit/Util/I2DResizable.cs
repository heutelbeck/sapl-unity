using UnityEngine;

namespace FTK.UIToolkit.Util
{
    public interface I2DResizable : IHasBorder
    {
        Vector2 Size { get; set; }
        GameObject gameObject { get; }
        void ChangeSizeTo(Vector2 targetSize, float animationTime);
    }

}
