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