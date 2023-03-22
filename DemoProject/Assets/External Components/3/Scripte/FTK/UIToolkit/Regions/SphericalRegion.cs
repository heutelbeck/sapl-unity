using UnityEngine;

namespace FTK.UIToolkit.Regions
{
    public class SphericalRegion : MonoBehaviour, IRegion
    {
        private static Mesh sphere = null;

        [Min(0)]
        [SerializeField]
        private float radius = 0.5f;
        [SerializeField]
        private Vector3 center = Vector3.zero;
        [SerializeField]
        private bool inverted = false;

        public float Radius
        {
            get => radius;
            set { radius = Mathf.Max(0, value); }
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
            if (inverted ^ Vector3.Distance(center, localPosition) > radius)
            {
                localPosition -= center;
                localPosition.Normalize();
                if (localPosition == Vector3.zero)
                    localPosition = Vector3.up;
                localPosition *= radius;
                localPosition += center;
                position = transform.TransformPoint(localPosition);
            }
            return position;
        }
        public bool IsInsideOfRegion(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            float distance = Vector3.Distance(center, localPosition);
            if (inverted) return distance > radius;
            else return distance < radius;
        }

        private void OnDrawGizmosSelected()
        {
            if (isActiveAndEnabled)
            {
                Gizmos.color = new Color(1, 0, 0.5f, 0.5f);
                Gizmos.DrawWireMesh(GetGizmoMesh(), transform.TransformPoint(center), transform.rotation, transform.lossyScale * radius);
            }
        }
        private static Mesh GetGizmoMesh()
        {
            if (sphere == null)
            {
                sphere = new Mesh();
                Vector3[] vertices = new Vector3[72];
                for (int i = 0; i < 72; i += 3)
                {
                    float theta = i * Mathf.PI / 36;
                    float x = Mathf.Cos(theta);
                    float y = Mathf.Sin(theta);
                    vertices[i] = new Vector3(x, y, 0);
                    vertices[i + 1] = new Vector3(0, x, y);
                    vertices[i + 2] = new Vector3(y, 0, x);
                }
                sphere.vertices = vertices;
                int[] triangles = new int[216];
                for (int i = 0; i < 216; i += 9)
                {
                    triangles[i] = i / 3;
                    triangles[i + 1] = i / 3;
                    triangles[i + 2] = (i / 3 + 3) % 72;
                    triangles[i + 3] = (i / 3 + 1) % 72;
                    triangles[i + 4] = (i / 3 + 1) % 72;
                    triangles[i + 5] = (i / 3 + 4) % 72;
                    triangles[i + 6] = (i / 3 + 2) % 72;
                    triangles[i + 7] = (i / 3 + 2) % 72;
                    triangles[i + 8] = (i / 3 + 5) % 72;
                }
                sphere.triangles = triangles;
                sphere.RecalculateNormals();
            }
            return sphere;
        }
        private void OnEnable() { }
    }

}
