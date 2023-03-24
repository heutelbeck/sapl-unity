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
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace FTK.UIToolkit.Util
{
    public class NearInteractionTouchableMultiVolume : NearInteractionTouchableVolume
    {
        /// <summary>
        /// Is the touchable collider enabled and active in the scene.
        /// </summary>
        public new bool ColliderEnabled => touchableCollider.enabled && touchableCollider.gameObject.activeInHierarchy;

        /// <summary>
        /// The collider used by this touchable.
        /// </summary>
        private MultiCollider touchableCollider;

        public new Collider TouchableCollider => touchableCollider;
        protected override void OnValidate()
        {
            base.OnValidate();
            SetupCollider(true);
        }

        private void Awake()
        {
            SetupCollider(true);
        }

        private void SetupCollider(bool forceNew = false)
        {
            if (forceNew || touchableCollider == null)
                touchableCollider = new MultiCollider().WithChildren(GetComponentsOnlyInChildren<Collider>());
        }

        /// <inheritdoc />
        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            SetupCollider();
            Vector3 closest = touchableCollider.ClosestPoint(samplePoint);

            normal = (samplePoint - closest);
            if (normal == Vector3.zero)
            {
                // inside object, use vector to centre as normal
                normal = samplePoint - touchableCollider.bounds.center;
                normal.Normalize();
                // Return value less than zero so that when poke pointer is inside
                // object, it will not raise a touch up event.
                return -1;
            }
            else
            {
                float dist = normal.magnitude;
                normal.Normalize();
                return dist;
            }
        }

        private List<C> GetComponentsOnlyInChildren<C>()
        {
            var components = new List<C>();
            foreach (var child in transform)
                components.AddRange((child as Transform).GetComponentsInChildren<C>());
            return components;
        }
    }

    internal class MultiCollider : Collider
    {

        private List<Collider> children = new List<Collider>();

        internal MultiCollider WithChildren(List<Collider> children)
        {
            this.children = children;
            return this;
        }
        public new bool enabled
        {
            get => children.All(child => child.enabled);
            set
            {
                foreach (var child in children)
                    child.enabled = value; 
            }
        }

        public new Bounds bounds
        {
            get
            {
                var b = new Bounds();
                foreach (var child in children)
                    b.Encapsulate(child.bounds);
                //Debug.Log("Bounds: " + b);
                return b;
            }
        }

        public new Vector3 ClosestPoint(Vector3 position)
        {
            var minDist = float.PositiveInfinity;
            var closestPoint = position;
            foreach (var child in children)
            {
                var closePoint = child.ClosestPoint(position);
                var dist = Vector3.Distance(position, closePoint);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestPoint = closePoint;
                }
            }
            //Debug.Log("ClosestPoint: " + closestPoint);
            return closestPoint;
        }

        public new Vector3 ClosestPointOnBounds(Vector3 position) => bounds.ClosestPoint(position);

        public new bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
        {
            hitInfo = default;
            var hit = false;
            var minDist = float.PositiveInfinity;
            foreach (var child in children)
                if (child.Raycast(ray, out var childHitInfo, maxDistance))
                {
                    hit = true;
                    if (childHitInfo.distance < minDist)
                    {
                        minDist = childHitInfo.distance;
                        hitInfo = childHitInfo;
                    }
                }
            //Debug.Log("Hit: " + hit);
            //if (hit) Debug.Log("HitPoint: " + hitInfo.point);
            return hit;
        }
    }

    [UnityEditor.CustomEditor(typeof(NearInteractionTouchableMultiVolume), true)]
    public class NearInteractionTouchableMultiVolumeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}