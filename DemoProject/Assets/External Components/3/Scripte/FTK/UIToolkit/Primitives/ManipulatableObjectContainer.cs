using FTK.UIToolkit.Util;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace FTK.UIToolkit.Containers
{
    public class ManipulatableObjectContainer : AnimatableBase, ISizeRestrictable, IEnumerable
    {
        private static readonly string boundingBoxGlowProperty = "_InnerGlowColor";
        private static readonly string boundingBoxBorderProperty = "_BorderMinValue";
        private static readonly string boundingBoxHierarchy = "ManipulationTransform/ObjectManipulator/BoundingBox";
        private static readonly string containerHierarchy = "ManipulationTransform/Container";

        [Min(0.01f)]
        [SerializeField]
        private Vector2 minMaxContentSize = new Vector2(0.05f, 0.2f);
        [SerializeField]
        private bool returnToLocalOriginWhenReleased = true;
        [SerializeField]
        private Color boundingBoxColor = Color.black;
        [SerializeField]
        private Color boundingBoxHighlightColor = Color.white;
        [Range(0, 0.5f)]
        [SerializeField]
        private float padding = 0.15f;
        [Space(10)]
        [SerializeField]
        private UnityEvent OnGrab = new UnityEvent();
        [SerializeField]
        private UnityEvent OnRelease = new UnityEvent();

        private Transform boundingBoxTransform;
        private Transform container;
        private Transform anchorTransform;
        private MeshRenderer boundingBoxRenderer;
        private Color _color;
        private float _border;

        private bool cascadeEnableToggle = true;
        public new bool enabled
        {
            get => base.enabled;
            set
            {
                base.enabled = value;
                CheckBoundingBox();
                boundingBoxTransform.gameObject.SetActive(value);
                if (cascadeEnableToggle)
                {
                    CheckContainer();
                    ToggleManipulatableChildrenEnabled();
                }
            }
        }

        public Bounds Bounds
        {
            get
            {
                CheckBoundingBox();
                if (boundingBoxTransform == null)
                    return new Bounds(Vector3.zero, Vector3.zero);
                return new Bounds(Vector3.zero, boundingBoxTransform.localScale);
            }
        }

        public Bounds ScaledBounds
        {
            get
            {
                Bounds b = Bounds;
                Vector3 e = b.extents;
                e.x *= Mathf.Abs(transform.localScale.x);
                e.y *= Mathf.Abs(transform.localScale.y);
                e.z *= Mathf.Abs(transform.localScale.z);
                b.extents = e;
                return b;
            }
        }

        public Transform Container
        {
            get
            {
                CheckContainer();
                return container;
            }
        }

        public bool Pinned
        {
            get => !returnToLocalOriginWhenReleased;
            set { returnToLocalOriginWhenReleased = !value; }
        }
        public void RestrictSize(float minSize, float maxSize)
        {
            if (minSize >= maxSize) maxSize = minSize + Mathf.Epsilon;
            //minMaxContentSize.x = minMaxContentSize.x < minSize ? minMaxContentSize.x : minSize;
            //minMaxContentSize.y = minMaxContentSize.y > maxSize ? minMaxContentSize.y : maxSize;
            minMaxContentSize.x = minSize;
            minMaxContentSize.y = maxSize;
            UpdateBoundingBox();
        }

        public void FixColliderSize(Vector3 superiorScale)
        {
            Vector3 s = boundingBoxTransform.parent.localScale;
            if (superiorScale.x < 0) s.x *= -1;
            if (superiorScale.y < 0) s.y *= -1;
            if (superiorScale.z < 0) s.z *= -1;
            boundingBoxTransform.parent.localScale = s;
        }
        public void SetEnabledNoCascade(bool enabled)
        {
            cascadeEnableToggle = false;
            this.enabled = enabled;
            cascadeEnableToggle = true;
        }

        public void ShowBorder(ManipulationEventData data)
        {
            CheckBoundingBox();
            Animate("Border", _border, 1, 0.25f, value =>
            {
                _border = value;
                UpdateMaterial();
            });
        }
        public void HideBorder(ManipulationEventData data)
        {
            CheckBoundingBox();
            Animate("Border", _border, 0, 0.25f, value =>
            {
                _border = value;
                UpdateMaterial();
            });
        }

        public void ForceReturnHome()
        {
            CheckAnchorTransform();
            Animate("Position", anchorTransform.localPosition, Vector3.zero, 0.5f, value =>
            {
                anchorTransform.localPosition = value;
            });
            Animate("Rotation", anchorTransform.localRotation, Quaternion.identity, 0.5f, value =>
            {
                anchorTransform.localRotation = value;
            });
            Animate("Scale", anchorTransform.localScale, Vector3.one, 0.5f, value =>
            {
                anchorTransform.localScale = value;
            });
        }
        public void OnEndManipulate(ManipulationEventData data) { if (returnToLocalOriginWhenReleased) ForceReturnHome(); }
        public void FocusEnter(ManipulationEventData data)
        {
            CheckBoundingBox();
            Animate("Color", _color, boundingBoxHighlightColor, 0.25f, value =>
            {
                _color = value;
                UpdateMaterial();
            });
        }
        public void FocusExit(ManipulationEventData data)
        {
            CheckBoundingBox();
            Animate("Color", _color, boundingBoxColor, 0.25f, value =>
            {
                _color = value;
                UpdateMaterial();
            });
        }
        public void FitToContents()
        {
            CheckContainer();
            Bounds localBounds = GetLocalBoundsForMeshes();
            Vector3 boundSize = localBounds.size;
            float maxExtents = Mathf.Max(boundSize.x, boundSize.y, boundSize.z);
            maxExtents *= 1 + padding;
            RestrictSize(maxExtents, maxExtents);
        }
        public void UpdateBoundingBox()
        {
            CheckBoundingBox();
            CheckContainer();

            Bounds localBounds = GetLocalBoundsForMeshes();

            Vector3 boundSize = localBounds.size;
            float maxExtends = Mathf.Max(boundSize.x, boundSize.y, boundSize.z);
            if (maxExtends > 0.01f)
            {
                enabled = true;

                float multiplier = 1;
                float minimumSize = minMaxContentSize.x;
                float maximumSize = minMaxContentSize.y;

                if (maxExtends < minimumSize)
                    multiplier = minimumSize / maxExtends;
                else if (maxExtends > maximumSize)
                    multiplier = maximumSize / maxExtends;

                float containerMultiplier = multiplier / (1 + padding);

                boundingBoxTransform.localScale = Constrain(boundSize * multiplier, 0.01f);
                container.localScale = Vector3.one * containerMultiplier;
                container.localPosition = -localBounds.center * containerMultiplier;
            }
            else
            {
                enabled = false;
                float minimumSize = minMaxContentSize.x;
                float containerMultiplier = minimumSize / (1 + padding);

                boundingBoxTransform.localScale = Vector3.one * minimumSize;
                container.localScale = Vector3.one * containerMultiplier;
                container.localPosition = Vector3.zero;
            }
        }

        public void InvokeOnGrab(ManipulationEventData data) { OnGrab.Invoke(); }
        public void InvokeOnRelease(ManipulationEventData data) { OnRelease.Invoke(); }

        public IEnumerator GetEnumerator() => Container.GetEnumerator();
        private void OnValidate()
        {
            if (minMaxContentSize.x > minMaxContentSize.y)
                minMaxContentSize.x = minMaxContentSize.y;
            UpdateBoundingBox();
            UpdateMaterial();
            ToggleManipulatableChildrenEnabled();
        }

        private void CheckBoundingBox()
        {
            if (boundingBoxTransform == null)
            {
                boundingBoxTransform = transform.Find(boundingBoxHierarchy);
                if (boundingBoxTransform == null)
                    Debug.LogError("No '" + boundingBoxHierarchy + "' grandchild attached!");
            }
            if (boundingBoxRenderer == null)
            {
                boundingBoxRenderer = boundingBoxTransform?.gameObject.GetComponent<MeshRenderer>();
                if (boundingBoxRenderer == null)
                    Debug.LogError("No MeshRenderer found at '" + boundingBoxHierarchy + "' grandgrandchild!");
            }
        }

        private void CheckContainer()
        {
            if (container == null)
            {
                container = transform.Find(containerHierarchy);
                if (container == null)
                    Debug.LogError("No '" + containerHierarchy + "' grandchild attached!");
            }
        }

        private void CheckAnchorTransform()
        {

            if (anchorTransform == null)
            {
                anchorTransform = transform.Find("ManipulationTransform");
                if (anchorTransform == null)
                    Debug.LogError("No 'ManipulationTransform' child attached!");
            }
        }
        private void ToggleManipulatableChildrenEnabled()
        {
            RecursiveToggle(container);

            void RecursiveToggle(Transform child)
            {
                if (child.TryGetComponent(out ManipulatableObjectContainer moc))
                    if (enabled)
                    {
                        moc.SetEnabledNoCascade(false);
                    }
                    else
                    {
                        moc.enabled = true;
                        return;
                    }
                foreach (Transform grandChild in child)
                    RecursiveToggle(grandChild);
            }
        }
        private void UpdateMaterial()
        {
            boundingBoxRenderer?.sharedMaterial?.SetFloat(boundingBoxBorderProperty, _border);
            boundingBoxRenderer?.sharedMaterial?.SetColor(boundingBoxGlowProperty, _color);
        }

        private Bounds GetLocalBoundsForMeshes()
        {
            Bounds finalBounds = new Bounds(Vector3.zero, Vector3.zero);
            if (container != null)
                RecurseEncapsulate(container, ref finalBounds);
            return finalBounds;

            void RecurseEncapsulate(Transform child, ref Bounds bounds)
            {
                if (child.TryGetComponent(out MeshFilter filter) && filter?.sharedMesh != null)
                {
                    Bounds lsBounds = filter.sharedMesh.bounds;
                    Vector3 wsMin = child.TransformPoint(lsBounds.min);
                    Vector3 wsMax = child.TransformPoint(lsBounds.max);
                    bounds.Encapsulate(container.InverseTransformPoint(wsMin));
                    bounds.Encapsulate(container.InverseTransformPoint(wsMax));
                }
                foreach (Transform grandChild in child.transform)
                    if (grandChild.gameObject.activeInHierarchy)
                        RecurseEncapsulate(grandChild, ref bounds);
            }
        }

        private Vector3 Constrain(Vector3 vec, float min)
        {
            vec.x = Mathf.Max(vec.x, min);
            vec.y = Mathf.Max(vec.y, min);
            vec.z = Mathf.Max(vec.z, min);
            return vec;
        }

        private bool HasManipulatableAncestor()
        {
            return RecurseAncestors(transform.parent);

            bool RecurseAncestors(Transform t)
            {
                if (t == null) return false;
                if (t.gameObject.TryGetComponent(out ManipulatableObjectContainer _)) return true;
                return RecurseAncestors(t.parent);
            }
        }
    }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ManipulatableObjectContainer))]
    public class ManipulatableObjectContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ManipulatableObjectContainer moc = (ManipulatableObjectContainer)target;
            GUILayout.Space(10);
            if (GUILayout.Button("Update Container"))
                moc.UpdateBoundingBox();
            moc.enabled = GUILayout.Toggle(moc.enabled, "Enable in Hierarchy");
        }

    }
#endif
}