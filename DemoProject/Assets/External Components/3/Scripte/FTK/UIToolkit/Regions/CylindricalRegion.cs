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
    public class CylindricalRegion : MonoBehaviour, IRegion
    {
        private Mesh cylinder = null;

        [Min(0)]
        [SerializeField]
        private float radius = 0.5f;
        [Min(0)]
        [SerializeField]
        private float height = 0.5f;
        [SerializeField]
        private Vector3 center = Vector3.zero;
        [SerializeField]
        private CylinderAxis axis = CylinderAxis.Y;
        [SerializeField]
        private bool inverted = false;
        public float Radius
        {
            get => radius;
            set { radius = Mathf.Max(0, value); }
        }
        public float Height
        {
            get => height;
            set { height = Mathf.Max(0, value); }
        }
        public Vector3 Center
        {
            get => center;
            set { center = value; }
        }

        public CylinderAxis Axis
        {
            get => axis;
            set { axis = value; }
        }
        public bool Inverted
        {
            get => inverted;
            set { inverted = value; }
        }

        public Vector3 ClosestPoint(Vector3 position)
        {
            Vector3 localPosition = MapAxis(transform.InverseTransformPoint(position));
            Vector3 mappedCenter = MapAxis(center);
            localPosition.y -= mappedCenter.y;
            Vector2 center2D = new Vector2(mappedCenter.x, mappedCenter.z);
            Vector2 localPosition2D = new Vector2(localPosition.x, localPosition.z);
            float distance2D = Vector2.Distance(center2D, localPosition2D);
            Vector3 oldLocalPosition = localPosition;
            bool recalc = false;
            if (inverted ^ distance2D > radius)
            {
                localPosition2D -= center2D;
                localPosition2D.Normalize();
                if (localPosition2D == Vector2.zero)
                    localPosition2D = Vector2.up;
                localPosition2D *= radius;
                localPosition2D += center2D;
                localPosition.x = localPosition2D.x;
                localPosition.z = localPosition2D.y;
                recalc = true;
            }
            if (localPosition.y < -0.5f * height || localPosition.y > 0.5f * height)
            {
                if (!inverted)
                {
                    localPosition.y = Mathf.Clamp(localPosition.y, -0.5f * height, 0.5f * height);
                    recalc = true;
                }
            }
            else
            {
                if (inverted)
                {
                    localPosition.y = (Mathf.Round(localPosition.y / height + 0.5f) - 0.5f) * height;
                    recalc = true;
                }
            }
            if (recalc)
            {
                if (inverted)
                {
                    Vector2 oldLocalPosition2D = new Vector2(oldLocalPosition.x, oldLocalPosition.z);
                    if (Vector2.Distance(oldLocalPosition2D, localPosition2D) >= Mathf.Abs(oldLocalPosition.y - localPosition.y))
                    {
                        localPosition.x = oldLocalPosition.x;
                        localPosition.z = oldLocalPosition.z;
                    }
                    else localPosition.y = oldLocalPosition.y;
                }
                localPosition.y += mappedCenter.y;
                position = transform.TransformPoint(MapAxis(localPosition));
            }
            return position;
        }
        public bool IsInsideOfRegion(Vector3 position)
        {
            Vector3 localPosition = MapAxis(transform.InverseTransformPoint(position));
            Vector3 mappedCenter = MapAxis(center);
            Vector2 center2D = new Vector2(mappedCenter.x, mappedCenter.z);
            Vector2 localPosition2D = new Vector2(localPosition.x, localPosition.z);
            float distance2D = Vector2.Distance(center2D, localPosition2D);
            if (inverted) return distance2D > radius || (localPosition.y <= -0.5 * height || localPosition.y >= 0.5 * height);
            else return distance2D < radius && (localPosition.y >= -0.5 * height && localPosition.y <= 0.5 * height);
        }

        private CylinderAxis oldAxis = CylinderAxis.Y;
        private void OnDrawGizmosSelected()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = new Color(1, 0, 0.5f, 0.5f);
                Vector3 drawAtScale = transform.lossyScale;
                drawAtScale.x *= radius;
                drawAtScale.y *= height;
                drawAtScale.z *= radius;
                drawAtScale = MapAxis(drawAtScale);
                Gizmos.DrawWireMesh(GetGizmoMesh(), transform.TransformPoint(center), transform.rotation, drawAtScale);
            }
        }
        private Mesh GetGizmoMesh()
        {
            if (cylinder == null || axis != oldAxis)
            {
                cylinder = new Mesh();
                Vector3[] vertices = new Vector3[48];
                for (int i = 0; i < 48; i+=2)
                {
                    float theta = i * Mathf.PI / 24;
                    float x = Mathf.Cos(theta);
                    float z = Mathf.Sin(theta);
                    vertices[i] = MapAxis(new Vector3(x, -0.5f, z));
                    vertices[i+1] = MapAxis(new Vector3(x, 0.5f, z));
                }
                cylinder.vertices = vertices;
                int[] triangles = new int[144];
                for (int i = 0; i < 144; i += 6)
                {
                    triangles[i] = i / 3;
                    triangles[i+1] = i / 3;
                    triangles[i+2] = (i / 3 + 2) % 48;
                    triangles[i+3] = (i / 3 + 1) % 48;
                    triangles[i+4] = (i / 3 + 1) % 48;
                    triangles[i+5] = (i / 3 + 3) % 48;
                }
                cylinder.triangles = triangles;
                cylinder.RecalculateNormals();
            }
            oldAxis = axis;
            return cylinder;
        }

        private Vector3 MapAxis(Vector3 vec)
        {
            switch (axis)
            {
                case CylinderAxis.X:
                    return new Vector3(vec.y, vec.x, vec.z);
                case CylinderAxis.Z:
                    return new Vector3(vec.x, vec.z, vec.y);
                default:
                    return vec;
            }
        }
        private void OnEnable() { }

        public enum CylinderAxis { X, Y, Z }
    }
}
