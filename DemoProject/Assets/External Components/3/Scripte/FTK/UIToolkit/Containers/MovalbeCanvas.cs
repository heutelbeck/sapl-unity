using FTK.UIToolkit.Util;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

namespace FTK.UIToolkit.Containers
{
    public class MovalbeCanvas : AnimatableBase
    {
        private static readonly string CANVAS_POSITION = "CanvasPosition";
        private static readonly string CANVAS_ROTATION = "CanvasRotation";
        private static readonly string CANVAS_SIZE = "CanvasSize";

        #region EditorSettings
        [Header("Components")]
        [SerializeField]
        private RectTransform canvasTransform;
        [SerializeField]
        private Image topBar;
        [SerializeField]
        private Image rightArrow;
        [SerializeField]
        private Image leftArrow;
        [SerializeField]
        private GameObject content;
        [SerializeField]
        private BoxCollider topBarCollider;

        [Header("Settings")]
        [SerializeField]
        private bool constrainRoll = true;
        [SerializeField]
        private bool arrowsCanMoveCanvas = true;
        [SerializeField]
        private Color defaultHandleColor;
        [SerializeField]
        private Color focusHandleColor;
        [Min(0f)]
        [SerializeField]
        private float animationTime;
        [Min(0.01f)]
        [SerializeField]
        private Vector2 minCanvasSize;
        #endregion

        #region Properties
        public Vector3 TargetPosition
        {
            get => transform.position;
            set
            {
                var prevCanvasTransform = canvasTransform.position;
                transform.position = value;
                canvasTransform.position = prevCanvasTransform;
            }
        }

        public Vector3 LocalTargetPosition
        {
            get => transform.localPosition;
            set
            {
                var prevCanvasTransform = canvasTransform.position;
                transform.localPosition = value;
                canvasTransform.position = prevCanvasTransform;
            }
        }

        public Quaternion TargetRotation
        {
            get => transform.rotation;
            set
            {
                var prevCanvasPosition = canvasTransform.position;
                var prevCanvasRotation = canvasTransform.rotation;
                transform.rotation = value;
                canvasTransform.position = prevCanvasPosition;
                canvasTransform.rotation = prevCanvasRotation;
            }
        }

        public Quaternion LocalTargetRotation
        {
            get => transform.localRotation;
            set
            {
                var prevCanvasPosition = canvasTransform.position;
                var prevCanvasRotation = canvasTransform.rotation;
                transform.localRotation = value;
                canvasTransform.position = prevCanvasPosition;
                canvasTransform.rotation = prevCanvasRotation;
            }
        }

        public Vector2 LocalTargetSize { get; set; }
        #endregion

        #region Fields
        private bool topBarGrabbed = false;
        private bool rightArrowGrabbed = false;
        private bool leftArrowGrabbed = false;
        private bool start = true;
        private Vector3 rightArrowOffset;
        private Vector3 leftArrowOffset;
        #endregion

        public void ReturnHome()
        {
            if (topBarGrabbed) return;
            Animate(CANVAS_POSITION, canvasTransform.localPosition, Vector3.zero, animationTime, value => canvasTransform.localPosition = value);
            Animate(CANVAS_ROTATION, canvasTransform.localRotation, Quaternion.identity, animationTime, value => canvasTransform.localRotation = value);
        }

        public void Resize()
        {
            if (rightArrowGrabbed || leftArrowGrabbed) return;
            Animate(CANVAS_SIZE, canvasTransform.sizeDelta, LocalTargetSize, animationTime, value =>
            {
                canvasTransform.sizeDelta = value;
                if (content.TryGetComponent(out I2DResizable resizable))
                    resizable.Size = value;
            });
        }

        public void ReturnHomeAndResize()
        {
            ReturnHome();
            Resize();
        }

        #region ManipulationEventHandlers
        public void OnTopBarFocus(ManipulationEventData eventData)
        {
            Animate("topBarColor", topBar.color, focusHandleColor, animationTime, value => topBar.color = value);
            OnRightArrowUnfocus(null);
            OnLeftArrowUnfocus(null);
        }

        public void OnTopBarUnfocus(ManipulationEventData eventData) => Animate("topBarColor", topBar.color, defaultHandleColor, animationTime, value => topBar.color = value);

        public void OnRightArrowFocus(ManipulationEventData eventData)
        {
            Animate("rightArrowColor", rightArrow.color, focusHandleColor, animationTime, value => rightArrow.color = value);
            OnTopBarUnfocus(null);
            OnLeftArrowUnfocus(null);
        }

        public void OnRightArrowUnfocus(ManipulationEventData eventData) => Animate("rightArrowColor", rightArrow.color, defaultHandleColor, animationTime, value => rightArrow.color = value);

        public void OnLeftArrowFocus(ManipulationEventData eventData)
        {
            Animate("leftArrowColor", leftArrow.color, focusHandleColor, animationTime, value => leftArrow.color = value);
            OnTopBarUnfocus(null);
            OnRightArrowUnfocus(null);
        }

        public void OnLeftArrowUnfocus(ManipulationEventData eventData) => Animate("leftArrowColor", leftArrow.color, defaultHandleColor, animationTime, value => leftArrow.color = value);

        public void OnTopBarGrabbed(ManipulationEventData eventData)
        {
            if (IsAnimationRunning(CANVAS_POSITION))
                StopAnimation(CANVAS_POSITION);
            if (IsAnimationRunning(CANVAS_ROTATION))
                StopAnimation(CANVAS_ROTATION);
            topBarGrabbed = true;
        }

        public void OnTopBarReleased(ManipulationEventData eventData)
        { 
            topBarGrabbed = false;
        }

        public void OnRightArrowGrabbed(ManipulationEventData eventData)
        {
            if (IsAnimationRunning(CANVAS_SIZE))
                StopAnimation(CANVAS_SIZE);
            rightArrowGrabbed = true;
        }

        public void OnRightArrowReleased(ManipulationEventData eventData)
        {
            rightArrowGrabbed = false;
            rightArrow.transform.localPosition = rightArrowOffset + new Vector3(canvasTransform.rect.xMax, canvasTransform.rect.yMin, 0f);
        }

        public void OnLeftArrowGrabbed(ManipulationEventData eventData)
        {
            if (IsAnimationRunning(CANVAS_SIZE))
                StopAnimation(CANVAS_SIZE);
            leftArrowGrabbed = true;
        }

        public void OnLeftArrowReleased(ManipulationEventData eventData)
        {
            leftArrowGrabbed = false;
            leftArrow.transform.localPosition = leftArrowOffset + new Vector3(canvasTransform.rect.xMin, canvasTransform.rect.yMin, 0f);
        }
        #endregion

        private void Start()
        {
            if (content.TryGetComponent(out I2DResizable resizable))
                resizable.Size = canvasTransform.sizeDelta = resizable.Size;
            LocalTargetSize = canvasTransform.sizeDelta;
            OnTopBarUnfocus(null);
            OnRightArrowUnfocus(null);
            OnLeftArrowUnfocus(null);
            rightArrowOffset = rightArrow.transform.localPosition - new Vector3(canvasTransform.rect.xMax, canvasTransform.rect.yMin, 0f);
            leftArrowOffset = leftArrow.transform.localPosition - new Vector3(canvasTransform.rect.xMin, canvasTransform.rect.yMin, 0f);
            var colSize = topBarCollider.size;
            colSize.x = canvasTransform.sizeDelta.x - 0.04f;
            topBarCollider.size = colSize;
        }

        private void LateUpdate()
        {
            if (topBarGrabbed && constrainRoll)
                    canvasTransform.LookAt(canvasTransform.position + canvasTransform.forward, Vector3.up);

            var sizeChanged = false;

            if (rightArrowGrabbed)
            {
                var oldSize = new Vector3(canvasTransform.rect.width, canvasTransform.rect.height, 0f);
                var oldRightCorner = new Vector3(canvasTransform.rect.xMax, canvasTransform.rect.yMin, 0f);
                var newRightCorner = rightArrow.transform.localPosition - rightArrowOffset;
                var posDelta = newRightCorner - oldRightCorner;
                posDelta.x *= -1f;
                var newSize = Vector3.zero;
                newSize.x = Mathf.Max(oldSize.x - posDelta.x, minCanvasSize.x);
                newSize.y = Mathf.Max(oldSize.y - posDelta.y, minCanvasSize.y);
                if (!arrowsCanMoveCanvas)
                    posDelta = oldSize - newSize;
                posDelta.x *= -1f;
                newRightCorner = posDelta + oldRightCorner;
                canvasTransform.sizeDelta = newSize;
                canvasTransform.anchoredPosition = canvasTransform.anchoredPosition3D + 0.5f * posDelta;
                rightArrow.transform.localPosition = newRightCorner + rightArrowOffset;
                sizeChanged = true;
            }

            if (leftArrowGrabbed)
            {
                var oldSize = new Vector3(canvasTransform.rect.width, canvasTransform.rect.height, 0f);
                var oldLeftCorner = new Vector3(canvasTransform.rect.xMin, canvasTransform.rect.yMin, 0f);
                var newLeftCorner = leftArrow.transform.localPosition - leftArrowOffset;
                var posDelta = newLeftCorner - oldLeftCorner;
                var newSize = Vector3.zero;
                newSize.x = Mathf.Max(oldSize.x - posDelta.x, minCanvasSize.x);
                newSize.y = Mathf.Max(oldSize.y - posDelta.y, minCanvasSize.y);
                if (!arrowsCanMoveCanvas)
                    posDelta = oldSize - newSize;
                newLeftCorner = posDelta + oldLeftCorner;
                canvasTransform.sizeDelta = newSize;
                canvasTransform.anchoredPosition = canvasTransform.anchoredPosition3D + 0.5f * posDelta;
                leftArrow.transform.localPosition = newLeftCorner + leftArrowOffset;
                sizeChanged = true;
            }

            if (sizeChanged)
            {
                var colSize = topBarCollider.size;
                colSize.x = canvasTransform.sizeDelta.x - 0.04f;
                topBarCollider.size = colSize;
            }

            if ((sizeChanged || start) && content.TryGetComponent(out I2DResizable resizable))
            {
                resizable.Size = canvasTransform.sizeDelta;
                start = false;
            }
        }
    }
}