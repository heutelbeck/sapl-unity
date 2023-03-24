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
using FTK.UIToolkit.Util;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;

namespace FTK.UIToolkit.Containers
{
    [RequireComponent(typeof(GridObjectCollection))]
    public class RadialObjectContainer : AnimatableBase, IEnumerable
    {
        
        [Range(0.1f, 100)]
        [SerializeField]
        private float radius = 0.25f;
        [Min(1)]
        [SerializeField]
        private int numVisibleObjects = 5;
        [Range(0, 1)]
        [SerializeField]
        private float segmentSize = 0.75f;
        [Range(0, 1)]
        [SerializeField]
        private float fadeOut = 0.25f;
        [Range(0, 1)]
        [SerializeField]
        private float currentCenter = 0.5f;
        [ReadOnly]
        [SerializeField]
        private int currentIndex = 0;
        [SerializeField]
        private bool alignRadially = true;

        private GridObjectCollection visibleObjectCollection;
        private float windowStartPos, windowEndPos;
        private int windowStartIndex, windowEndIndex, oldIndex = 0;
        private bool arrangeObjectsFlag = false;
        private bool forceUpdate = false;

        public GameObject CurrentObject
        {
            get
            {
                if (transform.childCount == 0) return null;
                else return transform.GetChild(currentIndex).gameObject;
            }
        }

        public float Radius
        {
            get => radius;
            set
            {
                StopAnimation("Radius");
                radius = Mathf.Clamp(value, 0, 100);
                CheckGridObjectCollection();
                UpdateGridObjectCollection();
            }
        }

        public float CurrentCenter
        {
            get => currentCenter;
            set
            {
                StopAnimation("Center");
                currentCenter = Mathf.Clamp(value, 0, 1);
                OnValidate();
            }
        }

        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                int count = transform.childCount;
                currentCenter = Mathf.Clamp(value, 0, count) / Mathf.Max(count - 1, 1);
                OnValidate();
            }
        }

        public int VisibleObjects { get => numVisibleObjects; }
        public void ChangeRadiusTo(float targetRadius, float animationTime)
        {
            Animate("Radius", radius, Mathf.Max(targetRadius, 0), animationTime, value =>
            {
                radius = value;
                CheckGridObjectCollection();
                UpdateGridObjectCollection();
            });
        }

        public void ChangeCenterTo(float targetCenter, float animationTime)
        {
            Animate("Center", currentCenter, Mathf.Clamp(targetCenter, 0, 1), animationTime, value =>
            {
                currentCenter = value;
                OnValidate();
            });
        }

        public void MoveToIndex(int targetIndex, float animationTime)
        {
            int count = transform.childCount;
            targetIndex = Mathf.Clamp(targetIndex, 0, count - 1);
            float targetCenter = targetIndex;
            targetCenter /= Mathf.Max(count - 1, 1);
            ChangeCenterTo(targetCenter, animationTime);
        }
        public void PushAt(GameObject obj, int index)
        {
            Transform t = obj.transform;
            t.parent = transform;
            if (index > transform.childCount - 1)
                t.SetAsLastSibling();
            else t.SetSiblingIndex(index);
            OnValidate();
        }

        public GameObject PopAt(int index)
        {
            if (index > transform.childCount - 1)
                return null;
            Transform t = transform.GetChild(index);
            t.parent = null;
            OnValidate();
            return t.gameObject;
        }
        public GameObject ReplaceAt(GameObject obj, int index)
        {
            Transform t = obj.transform;
            if (index > transform.childCount - 1)
            {
                t.parent = transform;
                t.SetAsLastSibling();
                CheckGridObjectCollection();
                ArrangeObjects();
                UpdateGridObjectCollection();
                return null;
            }
            else
            {
                Transform old = transform.GetChild(index);
                old.parent = null;
                t.parent = transform;
                t.SetSiblingIndex(index);
                CheckGridObjectCollection();
                ArrangeObjects();
                UpdateGridObjectCollection();
                return old.gameObject;
            }
        }
        public void PushFront(GameObject obj)
        {
            PushAt(obj, 0);
            Transform t = obj.transform;
            t.parent = transform;
            t.SetAsFirstSibling();
            OnValidate();
        }

        public void PushBack(GameObject obj)
        {
            Transform t = obj.transform;
            t.parent = transform;
            t.SetAsLastSibling();
            OnValidate();
        }

        public GameObject this[int index]
        {
            get
            {
                if (index > transform.childCount - 1)
                    return null;
                return transform.GetChild(index).gameObject;
            }
            set
            {
                Transform t = value.transform;
                if (index > transform.childCount - 1)
                {
                    t.parent = transform;
                    t.SetAsLastSibling();
                }
                else
                {
                    Transform old = transform.GetChild(index);
                    old.parent = null;
                    t.parent = transform;
                    t.SetSiblingIndex(index);
                }
                CheckGridObjectCollection();
                ArrangeObjects();
                UpdateGridObjectCollection();
            }
        }

        public IEnumerator GetEnumerator() => new ObjectEnumerator(transform);

        public GameObject PopFront() => PopAt(0);

        public GameObject PopBack() => PopAt(transform.childCount - 1);

        public void ForceVisible()
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(true);
        }

        public void ForceUpdate()
        {
            forceUpdate = true;
            OnValidate();
        }

        private void OnValidate()
        {
            UpdateIndex();
            CheckGridObjectCollection();
            ArrangeObjects();
            FadeObjects();
            UpdateGridObjectCollection();
            RestrictSizeOfChildren();
        }

        private void UpdateIndex()
        {
            currentIndex = Mathf.RoundToInt(GetFloatIndex());
            windowStartPos = GetFloatIndex() - 0.5f * numVisibleObjects;
            windowEndPos = GetFloatIndex() + 0.5f * numVisibleObjects;
            if (currentIndex != oldIndex || forceUpdate)
            {
                arrangeObjectsFlag = true;
                forceUpdate = false;
                oldIndex = currentIndex;
                windowStartIndex = Mathf.CeilToInt(currentIndex - 0.5f * numVisibleObjects + 0.5f);
                windowEndIndex = Mathf.FloorToInt(currentIndex + 0.5f * numVisibleObjects - 0.5f);
            }
        }

        private void CheckGridObjectCollection()
        {
            if (visibleObjectCollection == null)
            {
                visibleObjectCollection = GetComponent<GridObjectCollection>();
                if (visibleObjectCollection == null)
                    Debug.LogError("No GridObjectCollection component attached!");
            }
        }

        private void ArrangeObjects()
        {
            if (arrangeObjectsFlag)
            {
                arrangeObjectsFlag = false;
                foreach (Transform child in transform)
                    child.gameObject.SetActive(false);
                int endIndex = Mathf.Min(transform.childCount - 1, windowEndIndex);
                int startIndex = Mathf.Max(0, windowStartIndex);
                for (int i = startIndex; i <= endIndex; i++)
                    transform.GetChild(i).gameObject.SetActive(true);
                
            }
        }

        private void FadeObjects()
        {
            float frontFadeOutBegin = Mathf.Lerp(windowStartPos, GetFloatIndex(), fadeOut);
            float backFadeOutBegin = Mathf.Lerp(windowEndPos, GetFloatIndex(), fadeOut);
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out FadeOut f))
                    f.Opacity = GetInterpolationFactor(windowStartPos, frontFadeOutBegin, i)
                        * (1 - GetInterpolationFactor(backFadeOutBegin, windowEndPos, i));
            }
        }
        private void UpdateGridObjectCollection()
        {
            if (visibleObjectCollection != null)
            {
                visibleObjectCollection.transform.localPosition = new Vector3(0, 0, radius);
                float cellWidth = 2 * Mathf.PI * segmentSize / numVisibleObjects;
                float listPos = GetFloatIndex();
                float listOffset = listPos - 0.5f * (transform.childCount - 1);// Mathf.Round(listPos);
                float axisRotation = Mathf.Rad2Deg * cellWidth * listOffset;
                Quaternion objectRotation = Quaternion.Euler(0, 180 - axisRotation, 0);
                visibleObjectCollection.transform.localRotation = Quaternion.Inverse(objectRotation);
                visibleObjectCollection.Radius = radius;
                visibleObjectCollection.CellWidth = cellWidth * radius;
                visibleObjectCollection.OrientType = alignRadially ? OrientationType.FaceCenterAxisReversed : OrientationType.None;
                if (!alignRadially)
                    foreach (Transform child in visibleObjectCollection.transform)
                        child.localRotation = objectRotation;
                visibleObjectCollection.UpdateCollection();
            }
        }
        private void RestrictSizeOfChildren()
        {
            float cellWidth = 1.5f * Mathf.PI * radius * segmentSize / numVisibleObjects;
            foreach (ISizeRestrictable c in GetComponentsInChildren<ISizeRestrictable>(true))
                c.RestrictSize(cellWidth, cellWidth + Mathf.Epsilon);
        }
        private float GetFloatIndex() => currentCenter * Mathf.Max(transform.childCount - 1, 0);

        private class ObjectEnumerator : IEnumerator
        {
            private int currentIndex = -1;
            private Transform transform;

            public ObjectEnumerator(Transform transform) { this.transform = transform; }
            public object Current { get => transform.GetChild(currentIndex).gameObject; }

            public bool MoveNext()
            {
                currentIndex++;
                return currentIndex < transform.childCount;
            }
            public void Reset() { currentIndex = -1; }
        }
    }

}
