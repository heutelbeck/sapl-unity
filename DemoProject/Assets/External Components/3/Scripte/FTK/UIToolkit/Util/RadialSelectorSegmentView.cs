using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using System;

namespace FTK.UIToolkit.Util
{
    public class RadialSelectorSegmentView : AnimatableBase
    {

        [SerializeField]
        private Material selectMaterial;
        [SerializeField]
        private Material moveMaterial;
        [SerializeField]
        private AudioClip playOnMoveStart;
        [SerializeField]
        private AudioClip playOnMoveEnd;
        [SerializeField]
        private AudioClip playOnSelect;
        [SerializeField]
        private AudioClip playOnDeselect;
        [Min(0.01f)]
        [SerializeField]
        private float threshold = 0.05f;
        [Min(0.02f)]
        [SerializeField]
        private float maxMoveDist = 0.075f;
        [SerializeField]
        private MeshRenderer background;
        [SerializeField]
        private LazyAudioSource audioSource;

        private float projectedStartPos = 0f;
        private bool thresholdReached = false;
        private Action callback = () => { };
        private bool isMoving = false;

        public void BeginMove(HandTrackingInputEventData data) => BeginMove(data.InputData);

        public void BeginMove(MixedRealityPointerEventData data) => BeginMove(data.Pointer.Position);

        private void BeginMove(Vector3 startPos)
        {
            if (isMoving) return;
            isMoving = true;
            thresholdReached = false;
            projectedStartPos = Project(startPos);
            audioSource.PlayOneShot(playOnMoveStart);
        }

        public void Move(HandTrackingInputEventData data) => Move(data.InputData);

        public void Move(MixedRealityPointerEventData data) => Move(data.Pointer.Position);

        private void Move(Vector3 currentPos)
        {
            if (!isMoving) return;
            var projectedCurrentPos = Project(currentPos);
            var clampedDelta = Mathf.Clamp(projectedCurrentPos - projectedStartPos, 0f, maxMoveDist);
            var justReachedThreshold = clampedDelta > threshold;
            if (justReachedThreshold && !thresholdReached)
            {
                background.sharedMaterial = selectMaterial;
                audioSource.PlayOneShot(playOnSelect);
            }
            if (!justReachedThreshold && thresholdReached)
            {
                background.sharedMaterial = moveMaterial;
                audioSource.PlayOneShot(playOnDeselect);
            }
            thresholdReached = justReachedThreshold;
            SetYPos(clampedDelta);
        }

        public void EndMove()
        {
            if (!isMoving) return;
            isMoving = false;
            if (thresholdReached)
                callback.Invoke();
            thresholdReached = false;
            SetYPos(0f, true);
            audioSource.PlayOneShot(playOnMoveEnd);
        }

        public void SetActionCallback(Action callback) => this.callback = callback;

        private float Project(Vector3 pos)
        {
            var localPos = transform.parent.InverseTransformPoint(pos);
            return localPos.y;
        }

        private void SetYPos(float y, bool animate = false)
        {
            if (animate)
                Animate("POS", transform.localPosition.y, y, 0.1f, value =>
                {
                    if (float.IsNaN(value))
                        value = 0f;
                    var transformLocalPos = transform.localPosition;
                    transformLocalPos.y = value;
                    transform.localPosition = transformLocalPos;
                });
            else
            {
                if (IsAnimationRunning("POS"))
                    StopAnimation("POS");
                var transformLocalPos = transform.localPosition;
                transformLocalPos.y = y;
                transform.localPosition = transformLocalPos;
            }
        }
    }
}