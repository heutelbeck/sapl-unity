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
