using UnityEngine;

namespace FTK.UIToolkit.Util
{
    public class ViewCamera : AnimatableBase
    {
        [Range(0,1)]
        [SerializeField]
        private float weight = 0.75f;
        [Range(0, 0.5f)]
        [SerializeField]
        private float elasticity = 0.25f;
        [SerializeField]
        private bool backToCamera = true;

        private Vector3 lookAtPoint = Vector3.zero;
        private Transform cameraTransform;

        private void Update()
        {
            if (TryGetCurrentCameraPosition(out Vector3 cameraPosition))
                Animate("LookAtPoint", lookAtPoint, cameraPosition, elasticity, SetViewDirection);
        }

        private bool TryGetCurrentCameraPosition(out Vector3 cameraPosition)
        {
            cameraPosition = Vector3.zero;
            if (cameraTransform == null)
            {
                cameraTransform = GameObject.FindWithTag("MainCamera").transform;
                if (cameraTransform == null)
                    return false;
            }
            cameraPosition = cameraTransform.position;
            return true;
        }

        private void SetViewDirection(Vector3 lookAtPoint)
        {
            this.lookAtPoint = lookAtPoint;
            Vector3 direction = lookAtPoint - transform.position;
            if (direction.sqrMagnitude <= 0.000001f) direction = transform.forward;
            Quaternion rotation = Quaternion.LookRotation(backToCamera ? -direction : direction);
            transform.rotation = Quaternion.Lerp(GetBaseRotation(), rotation, weight);
        }

        private Quaternion GetBaseRotation()
        {
            if (transform.parent == null)
                return transform.rotation;
            return transform.parent.rotation;
        }
    }

}
