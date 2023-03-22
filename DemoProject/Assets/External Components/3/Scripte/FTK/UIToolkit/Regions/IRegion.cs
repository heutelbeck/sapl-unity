using UnityEngine;

namespace FTK.UIToolkit.Regions
{
    public interface IRegion
    {
        bool enabled { get; set; }
        bool isActiveAndEnabled { get; }
        bool Inverted { get; set; }

        Vector3 ClosestPoint(Vector3 position);
        bool IsInsideOfRegion(Vector3 position);
    }

}
