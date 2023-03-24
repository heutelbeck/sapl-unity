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

namespace FTK.UIToolkit.Regions
{
    public class BoxRegion : MonoBehaviour, IRegion
    {
        private static Mesh box = null;

        [Min(0.000001f)]
        [SerializeField]
        private Vector3 extents = Vector3.one;
        [SerializeField]
        private Vector3 center = Vector3.zero;
        [SerializeField]
        private bool inverted = false;

        public Vector3 Extents
        {
            get => extents;
            set { extents = value; }
        }
        public Vector3 Center
        {
            get => center;
            set { center = value; }
        }
        public bool Inverted
        {
            get => inverted;
            set { inverted = value; }
        }

        public Vector3 ClosestPoint(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            localPosition -= center;
            localPosition.x /= extents.x;
            localPosition.y /= extents.y;
            localPosition.z /= extents.z;
            if (IsInsideOfRegionLocal(localPosition)) return position;
            if (inverted)
            {
                float distX = Mathf.Abs(localPosition.x);
                float distY = Mathf.Abs(localPosition.y);
                float distZ = Mathf.Abs(localPosition.z);
                if (distX > distY && distX > distZ)
                    localPosition.x = localPosition.x >= 0 ? Mathf.Max(localPosition.x, 0.5f) : Mathf.Min(localPosition.x, -0.5f);
                else if (distY > distX && distY > distZ)
                    localPosition.y = localPosition.y >= 0 ? Mathf.Max(localPosition.y, 0.5f) : Mathf.Min(localPosition.y, -0.5f);
                else
                    localPosition.z = localPosition.z >= 0 ? Mathf.Max(localPosition.z, 0.5f) : Mathf.Min(localPosition.z, -0.5f);
            }
            else
            {
                localPosition.x = Mathf.Clamp(localPosition.x, -0.5f, 0.5f);
                localPosition.y = Mathf.Clamp(localPosition.y, -0.5f, 0.5f);
                localPosition.z = Mathf.Clamp(localPosition.z, -0.5f, 0.5f);
            }
            localPosition.x *= extents.x;
            localPosition.y *= extents.y;
            localPosition.z *= extents.z;
            localPosition += center;
            return transform.TransformPoint(localPosition);
        }
        public bool IsInsideOfRegion(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            localPosition -= center;
            localPosition.x /= extents.x;
            localPosition.y /= extents.y;
            localPosition.z /= extents.z;
            return IsInsideOfRegionLocal(localPosition);
        }

        private bool IsInsideOfRegionLocal(Vector3 localPosition)
        {
            if (inverted) return VectorOutOfRange(localPosition);
            else return VectorInRange(localPosition);
        }
        private bool VectorInRange(Vector3 vec) => vec.x <= 0.5f && vec.x >= -0.5f && vec.y <= 0.5f && vec.y >= -0.5f && vec.z <= 0.5f && vec.z >= -0.5f;
        private bool VectorOutOfRange(Vector3 vec) => vec.x >= 0.5f || vec.x <= -0.5f || vec.y >= 0.5f || vec.y <= -0.5f || vec.z >= 0.5f || vec.z <= -0.5f;
        private void OnDrawGizmosSelected()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = new Color(1, 0, 0.5f, 0.5f);
                Gizmos.DrawWireMesh(GetGizmoMesh(), transform.TransformPoint(center), transform.rotation,
                    new Vector3(transform.lossyScale.x * extents.x, transform.lossyScale.y * extents.y, transform.lossyScale.z * extents.z));
            }
        }
        private static Mesh GetGizmoMesh()
        {
            if (box == null)
            {
                box = new Mesh();
                box.vertices = new Vector3[]
                {
                    new Vector3(-0.5f, -0.5f, -0.5f), //0
                    new Vector3(-0.5f, -0.5f,  0.5f), //1
                    new Vector3(-0.5f,  0.5f, -0.5f), //2
                    new Vector3(-0.5f,  0.5f,  0.5f), //3
                    new Vector3( 0.5f, -0.5f, -0.5f), //4
                    new Vector3( 0.5f, -0.5f,  0.5f), //5
                    new Vector3( 0.5f,  0.5f, -0.5f), //6
                    new Vector3( 0.5f,  0.5f,  0.5f)  //7
                };
                box.triangles = new int[]
                {
                    0, 0, 1,
                    0, 0, 2,
                    0, 0, 4,
                    1, 1, 3,
                    1, 1, 5,
                    2, 2, 3,
                    2, 2, 6,
                    3, 3, 7,
                    4, 4, 5,
                    4, 4, 6,
                    5, 5, 7,
                    6, 6, 7
                };
                box.RecalculateNormals();
            }
            return box;
        }
        private void OnEnable() { }
    }
}