using FTK.UIToolkit.Regions;
using FTK.UIToolkit.Util;
using Microsoft.MixedReality.Toolkit.Rendering;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace FTK.UIToolkit.Primitives {

    [ExecuteInEditMode]
    [RequireComponent(typeof(LineRenderer))]
    public class RadialSelectorView : AnimatableBase, IHasBorder
    {
        private static readonly float TWO_PI = 6.283185307179586476925286766559f;
        private static readonly float PI_OVER_TWO = 1.5707963267948966192313216916398f;
        private static readonly float RESOLUTION_MULTIPLIER = 48f;
        private static readonly int MIN_RESOLUTION = 24;
        private static readonly string BACKGROUND_KEY = "Background";
        private static readonly string FOREGROUND_KEY = "Foreground";
        private static readonly string CONTENT_KEY = "Content";

        private static Vector3 mul(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        private static Vector3 averageX(Vector3 vec, float x) => new Vector3(0.5f * (vec.x + x), vec.y, vec.z);

        [SerializeField]
        private SubUIWrapper defaultSegment;
        [SerializeField]
        private Material standardMaterial;
        [SerializeField]
        private CylindricalRegion outer;
        [SerializeField]
        private CylindricalRegion inner;
        [SerializeField]
        private MultiRegionIntersect linePointerBeginRegion;

        [Min(0.01f)]
        [SerializeField]
        private float uniformScale = 0.1f;
        [Min(0.01f)]
        [SerializeField]
        private float depth = 0.05f;
        [Min(0f)]
        [SerializeField]
        private float borderWidth = 0.001f;
        [Range(0f, 0.999f)]
        [SerializeField]
        private float radius = 0.25f;

        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        [SerializeField]
        private Transform targetTransform;

        private Mesh mesh;
        private float colliderWidth = 0f;

        public Transform TargetTransform
        {
            get => targetTransform;
            set => targetTransform = value;
        }

        public float BorderWidth
        {
            get => borderWidth;
            set
            {
                var angleRange = TWO_PI / entries.Count;
                borderWidth = Mathf.Max(value, 0f);
                ApplyMesh(angleRange);
            }
        }
        public void ChangeBorderWidthTo(float targetBorderWidth, float animationTime)
        {
            Animate("BorderWidth", borderWidth, Mathf.Max(targetBorderWidth, 0f), animationTime, value =>
            {
                var angleRange = TWO_PI / entries.Count;
                borderWidth = value;
                ApplyMesh(angleRange);
            });
        }

        private void InvokeActionsFor(int index)
        {
            if (index < 0 || index >= entries.Count)
                return;
            entries[index].actions.Invoke();
        }

        private void Awake() => Apply();
        private void LateUpdate()
        {
            var lineRenderer = GetComponent<LineRenderer>();
            if (targetTransform == null)
                lineRenderer.enabled = false;
            else
            {
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, linePointerBeginRegion.ClosestPoint(targetTransform.position));
                lineRenderer.SetPosition(1, targetTransform.position);
            }
        }
        internal void Apply()
        {
            while (entries.Count < 2)
                entries.Add(default);
            foreach (var wrapper in GetComponentsInChildren<SubUIWrapper>())
                wrapper.Apply();
            if (defaultSegment != null)
            {
                var angleRange = TWO_PI / entries.Count;
                ApplyMesh(angleRange);
                BuildWheel(angleRange);
                RecomputeRegions(angleRange);
            }
        }

        private void RecomputeRegions(float angleRange)
        {
            inner.Radius = uniformScale + 1.5f * borderWidth / Mathf.Sin(0.5f * angleRange);
            outer.Radius = inner.Radius + borderWidth;
            inner.Height = 4f * (uniformScale + borderWidth);
            outer.Height = borderWidth;
            inner.Center = borderWidth * Vector3.forward;
            outer.Center = inner.Center;
            if (TryGetComponent(out LineRenderer lineRenderer))
            {
                lineRenderer.startWidth = borderWidth;
                lineRenderer.endWidth = 0.5f * borderWidth;
            }
        }

        private void BuildWheel(float angleRange)
        {
            PruneChildren();
            PopulateWheel(angleRange);
        }

        private void PruneChildren()
        {
            if (transform.childCount > 1)
                foreach (var child in transform)
                {
                    var go = (child as Transform).gameObject;
                    go.GetComponentInChildren<SubUIWrapper>()?.GetGameObject(CONTENT_KEY)?.transform.DetachChildren();
                    if (!ReferenceEquals(child, defaultSegment.gameObject.transform.parent))
                    {
#if UNITY_EDITOR
                        EditorApplication.delayCall += () => DestroyImmediate(go, true);
#else
                        Destroy(go);
#endif
                    }
                }
        }

        private void PopulateWheel(float angleRange)
        {
            var angle = 1.5f * angleRange;
            for (var index = 1; index < entries.Count; index++)
            {
                var segment = Instantiate(defaultSegment.transform.parent.gameObject, transform);
                SetupSegment(segment, angle, entries[index], index);

                angle += angleRange;
            }
            if (entries.Count > 0)
            {
                var segment = defaultSegment.transform.parent.gameObject;
                SetupSegment(segment, angle, entries[0]);
            }
        }

        private void SetupSegment(GameObject segment, float angle, Entry entry, int index = 0)
        {
            var localRotation = Quaternion.Euler(0f, 0f, angle / TWO_PI * 360f);
            segment.transform.localRotation = localRotation;

            segment.GetComponentInChildren<RadialSelectorSegmentView>()?.SetActionCallback(() => InvokeActionsFor(index));

            var contentParent = segment.GetComponentInChildren<SubUIWrapper>()?.GetGameObject(CONTENT_KEY)?.transform;
            contentParent.localRotation = Quaternion.Inverse(localRotation);
            if (entry.gameObject != null && contentParent != null)
            {
                entry.gameObject.transform.parent = contentParent;
                entry.gameObject.transform.localPosition = Vector3.zero;
                entry.gameObject.transform.localRotation = Quaternion.identity;
            }
            segment.SetActive(!entry.hidden);
        }

        private void ApplyMesh(float angleRange)
        {
            if (mesh == null) mesh = new Mesh();
            var scaledBorderWidth = borderWidth / uniformScale;
            var offset = scaledBorderWidth * 2f;
            var resolution = Mathf.Max(Mathf.CeilToInt(angleRange * RESOLUTION_MULTIPLIER), MIN_RESOLUTION);
            BuildPanelMesh(offset, angleRange, radius, resolution);

            offset = scaledBorderWidth * 2.5f;
            var origin = Vector3.up * uniformScale;
            origin *= radius + 0.5f * (1f - radius) + offset / Mathf.Sin(0.5f * angleRange);
            var scale = new Vector3(1f, 1f - radius, GetScaleZ());

            var backgroundGO = defaultSegment.GetGameObject(BACKGROUND_KEY);
            backgroundGO.transform.localPosition = origin;
            backgroundGO.transform.localScale = scale;
            var bFilter = backgroundGO.GetComponent<MeshFilter>();
            bFilter.sharedMesh = mesh;

            var foregroundGO = defaultSegment.GetGameObject(FOREGROUND_KEY);
            foregroundGO.transform.localPosition = origin - depth * Vector3.forward;
            foregroundGO.transform.localScale = scale;
            var fFilter = foregroundGO.GetComponent<MeshFilter>();
            fFilter.sharedMesh = mesh;

            var contentGO = defaultSegment.GetGameObject(CONTENT_KEY);
            contentGO.transform.localPosition = origin - 0.5f * depth * Vector3.forward;

            var collider = defaultSegment.gameObject.GetComponent<BoxCollider>();
            collider.center = mul(mesh.bounds.center, scale) + origin - 0.5f * depth * Vector3.forward;
            collider.size = averageX(mul(mesh.bounds.size, scale) + depth * Vector3.forward, colliderWidth);
        }

        private void BuildPanelMesh(float offset, float angleRange, float radius, int resolution)
        {
            mesh.uv = null;
            mesh.triangles = new int[0];
            mesh.vertices = BuildPanelVertices(offset, angleRange, radius, resolution);
            mesh.triangles = BuildPanelTriangles(resolution);
            mesh.uv = BuildPanelUVs(offset, resolution);
            mesh.RecalculateBounds();
        }

        private Vector3[] BuildPanelVertices(float offset, float angleRange, float radius, int resolution)
        {
            var scaleCorrection = 1f / (1f - radius);
            var count = 2 * resolution + 4;
            var verts = new Vector3[count];
            var deltaAngle = angleRange / (resolution - 1);
            var origin = Vector3.up * scaleCorrection;
            origin *= radius + 0.5f * (1f - radius);

            var angle = -0.5f * angleRange;
            Vector3 dirVector = default;
            for (var i = 0; i < resolution; i++, angle += deltaAngle)
            {
                dirVector = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle) * scaleCorrection);
                verts[2 * i + 2] = (dirVector - origin) * uniformScale;
                verts[2 * i + 3] = (dirVector * radius - origin) * uniformScale;
            }

            angle = -0.5f * angleRange - PI_OVER_TWO;
            dirVector = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle) * scaleCorrection);
            verts[0] = verts[2] + offset * uniformScale * dirVector;
            verts[1] = verts[3] + offset * uniformScale * dirVector;

            angle = 0.5f * angleRange + PI_OVER_TWO;
            dirVector = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle) * scaleCorrection);
            verts[count - 2] = verts[count - 4] + offset * uniformScale * dirVector;
            verts[count - 1] = verts[count - 3] + offset * uniformScale * dirVector;

            colliderWidth = Vector3.Distance(verts[1], verts[count - 1]);
            return verts;
        }

        private int[] BuildPanelTriangles(int resolution)
        {
            var tris = new int[6 * resolution + 6];
            for (var i = 0; i <= 2 * resolution; i += 2)
            {
                tris[3 * i    ] = i;
                tris[3 * i + 1] = i + 1;
                tris[3 * i + 2] = i + 3;
                tris[3 * i + 3] = i;
                tris[3 * i + 4] = i + 3;
                tris[3 * i + 5] = i + 2;
            }
            return tris;
        }

        private Vector2[] BuildPanelUVs(float offset, int resolution)
        {
            var count = 2 * resolution + 4;
            var uvs = new Vector2[count];
            var deltaU = (1f - 2f * offset) / (resolution - 1);
            var u = 1f - offset;
            for (var i = 0; i < resolution; i++, u -= deltaU)
            {
                var smoothU = Mathf.Lerp(offset, 1f - offset, GetInterpolationFactor(offset, 1f - offset, u));
                smoothU = Mathf.Lerp(offset, 1f - offset, GetInterpolationFactor(offset, 1f - offset, smoothU));
                uvs[2 * i + 2] = new Vector2(smoothU, 1f);
                uvs[2 * i + 3] = new Vector2(u, 0f);
            }
            uvs[0] = Vector2.one;
            uvs[1] = Vector2.right;
            uvs[count - 2] = Vector2.up;
            uvs[count - 1] = Vector2.zero;
            return uvs;
        }

        private float GetScaleZ() => Mathf.Max(borderWidth * 8.0f / uniformScale, 0.000001f);

        [Serializable]
        private struct Entry
        {
            [SerializeField]
            internal string name;
            [SerializeField]
            internal GameObject gameObject;
            [SerializeField]
            internal bool hidden;
            [SerializeField]
            internal UnityEvent actions;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(RadialSelectorView))]
    public class RadialSelectorViewEditor : UnityEditor.Editor
    {
        private GUIStyle buttonStyle;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            RadialSelectorView rsv = (RadialSelectorView)target;
            buttonStyle = new GUIStyle(GUI.skin.button);
            if (GUILayout.Button("Apply", buttonStyle))
            {
                rsv.Apply();
            }
        }

    }
#endif
}