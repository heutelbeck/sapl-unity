using FTK.UIToolkit.Util;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace FTK.UIToolkit.Primitives
{
    [RequireComponent(typeof(RectTransform))]
    public class SliderView : AnimatableBase, I2DResizable
    {
        private static readonly string buttonQuadPath = "PinchSlider/ThumbRoot/Slider_Button";
        private static readonly string trackQuadPath = "PinchSlider/TrackVisuals/SliderTrackSimple";

        [Min(0)]
        [SerializeField]
        private Vector2 size = Vector2.one;
        [Range(0.001f, 1)]
        [SerializeField]
        private float trackAspectRatio = 0.5f;
        [Range(0, 1)]
        [SerializeField]
        private float sliderValue = 0.5f;
        [Range(0, 1)]
        [SerializeField]
        private float selectionWidth = 0.25f;
        [SerializeField]
        private Color color = new Color(0, 0.3764706f, 0.75f);
        [SerializeField]
        private Color hoverColor = new Color(0.25f, 0.6269634f, 1);
        [SerializeField]
        private Color grabColor = new Color(1, 0, 0.6666665f);

        [SerializeField]
        private SliderUpdateEvent onUpdate = new SliderUpdateEvent();

        private PinchSlider pinchSlider;
        private Transform buttonQuadTransform, sliderQuadTransform;
        private SliderAnimationState animationState = SliderAnimationState.Default;
        private Color _color;
        public Vector2 Size
        {
            get => size;
            set
            {
                size = value;
                size.x = Mathf.Max(0, size.x);
                size.y = Mathf.Max(0, size.y);
                CheckQuadTransforms();
                UpdateScale();
                CheckPinchSlider();
                UpdatePinchSlider();
            }
        }

        public float BorderWidth { get => 0; set { } }

        public float SliderValue
        {
            get => sliderValue;
            set
            {
                sliderValue = value;
                CheckPinchSlider();
                UpdatePinchSlider();
            }
        }
        public float SelectionWidth
        {
            get => selectionWidth;
            set
            {
                selectionWidth = Mathf.Clamp(value, 0, 1);
                CheckQuadTransforms();
                UpdateScale();
                CheckPinchSlider();
                UpdatePinchSlider();
            }
        }

        public Color CurrentColor { get => _color; }

        public void ChangeSizeTo(Vector2 targetSize, float animationTime)
        {
            Animate("Size", size, targetSize, animationTime, value =>
            {
                size = value;
                CheckQuadTransforms();
                UpdateScale();
                CheckPinchSlider();
                UpdatePinchSlider();
            });
        }

        public void ChangeBorderWidthTo(float targetBorderWidth, float animationTime) { }

        public void ChangeSliderValueTo(float targetSliderValue, float animationTime)
        {
            Animate("SliderValue", sliderValue, Mathf.Clamp(targetSliderValue, 0, 1), animationTime, value =>
            {
                sliderValue = value;
                CheckPinchSlider();
                UpdatePinchSlider();
            });
        }

        public void ChangeSelectionWidthTo(float targetSelectionWidth, float animationTime)
        {
            Animate("SelectionWidth", selectionWidth, Mathf.Clamp(targetSelectionWidth, 0, 1), animationTime, value =>
            {
                selectionWidth = value;
                CheckQuadTransforms();
                UpdateScale();
                CheckPinchSlider();
                UpdatePinchSlider();
            });
        }

        private void ChangeColorToInternal(Color targetColor, float animationTime)
        {
            Animate("InternalStateColor", _color, targetColor, animationTime, value =>
            {
                _color = value;
                CheckQuadTransforms();
                SetColor();
            });
        }

        public void OnSliderUpdate(SliderEventData data) { onUpdate.Invoke(data.NewValue); }

        public void OnDefaultStateEnter(SliderEventData data)
        {
            if (animationState != SliderAnimationState.Default)
            {
                ChangeColorToInternal(color, 0.25f);
                animationState = SliderAnimationState.Default;
            }
        }
        public void OnHoverStateEnter(SliderEventData data)
        {
            if (animationState != SliderAnimationState.Hover)
            {
                ChangeColorToInternal(hoverColor, 0.25f);
                animationState = SliderAnimationState.Hover;
            }
        }
        public void OnGrabStateEnter(SliderEventData data)
        {
            if (animationState != SliderAnimationState.Grab)
            {
                ChangeColorToInternal(grabColor, 0.25f);
                animationState = SliderAnimationState.Grab;
            }
        }

        private void Awake()
        {
            _color = color;
        }

        private void OnValidate()
        {
            CheckQuadTransforms();
            _color = color;
            SetColor();
            UpdateScale();
            CheckPinchSlider();
            UpdatePinchSlider();
            
        }

        private void CheckQuadTransforms()
        {
            if (buttonQuadTransform == null)
            {
                buttonQuadTransform = transform.Find(buttonQuadPath);
                if (buttonQuadTransform == null)
                    Debug.LogError("No '" + buttonQuadPath + "' quad attached!");
            }
            if (sliderQuadTransform == null)
            {
                sliderQuadTransform = transform.Find(trackQuadPath);
                if (sliderQuadTransform == null)
                    Debug.LogError("No '" + trackQuadPath + "' quad attached!");
            }
        }

        private void UpdateScale()
        {
            RectTransform transformAsRect = (RectTransform)transform;
            transformAsRect.sizeDelta = this.size;

            float trackLength = Mathf.Max(Mathf.Max(size.x, size.y), 0.001f);
            float buttonWidth = Mathf.Max(Mathf.Min(size.x, size.y), 0.001f);
            float absWidth = buttonWidth * trackAspectRatio;
            if (buttonQuadTransform != null)
            {
                float buttonLength = Mathf.Clamp(trackLength * selectionWidth, absWidth, trackLength);
                buttonQuadTransform.localScale = new Vector3(buttonLength, buttonWidth, absWidth);
                Vector3 buttonLocalPos = buttonQuadTransform.localPosition;
                buttonLocalPos.z = absWidth * -0.25f;
                buttonQuadTransform.localPosition = buttonLocalPos;
            }
            if (sliderQuadTransform != null)
                sliderQuadTransform.localScale = new Vector3(trackLength, absWidth, absWidth);
        }

        private void CheckPinchSlider()
        {
            if (pinchSlider == null)
            {
                pinchSlider = GetComponentInChildren<PinchSlider>();
                if (pinchSlider == null)
                    Debug.LogError("No PinchSlider child attached!");
            }
        }

        private void UpdatePinchSlider()
        {
            if (pinchSlider != null)
            {
                pinchSlider.CurrentSliderAxis = size.x > size.y ? SliderAxis.XAxis : SliderAxis.YAxis;
                pinchSlider.SliderValue = sliderValue;
                if (sliderQuadTransform != null && buttonQuadTransform != null)
                {
                    float slidingDistance = (sliderQuadTransform.localScale.x - buttonQuadTransform.localScale.x) * 0.5f;
                    pinchSlider.SliderStartDistance = -slidingDistance;
                    pinchSlider.SliderEndDistance = slidingDistance;
                }
            }
        }

        private void SetColor()
        {
            buttonQuadTransform?.gameObject.GetComponent<MeshRenderer>()?.sharedMaterial?.SetColor("_Color", _color);
        }

        [Serializable]
        private class SliderUpdateEvent : UnityEvent<float> { }

        private enum SliderAnimationState { Default = 0, Hover = 1, Grab = 2 }
    }
}

