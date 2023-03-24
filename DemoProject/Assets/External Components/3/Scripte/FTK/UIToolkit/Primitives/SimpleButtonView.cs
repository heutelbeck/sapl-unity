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
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FTK.UIToolkit.Primitives
{
    public class SimpleButtonView : TextPanelView
    {
        [SerializeField]
        private ButtonContentType contentType = ButtonContentType.Text;
        [SerializeField]
        [Range(0.01f, 0.5f)]
        private float buttonAspectRatio = 0.5f;
        [SerializeField]
        [ConditionalField("propertyDrawerIconVisible")]
        private Sprite imageSprite;
        [SerializeField]
        [ConditionalField("propertyDrawerIconVisible")]
        private Color spriteColor = Color.white;
        [Header("Events")]
        [SerializeField]
        [Range(0.25f, 2f)]
        [Tooltip("Maximum duration of short click in seconds")]
        private float maxShortClickDuration = 0.5f;
        [SerializeField]
        private UnityEvent onShortClick = new UnityEvent();
        [SerializeField]
        private UnityEvent onPress = new UnityEvent();
        [SerializeField]
        private UnityEvent onRelease = new UnityEvent();
        private RectTransform frontPanel;
        private Image icon;
        private BoxCollider buttonCollider;
        private NearInteractionTouchable touchable;
        private PressableButton pressableInteractable;
        private float pressingInvoctionTime = -1;
        [SerializeField]
        [HideInInspector]
        private bool propertyDrawerIconVisible = false;

        public float ZSize { get { return buttonAspectRatio * Mathf.Min(Size.x, Size.y); } }

        public Color SpriteColor { get { return spriteColor; } }

        public bool IsPressed { get; private set; } = false;

        public void AddOnShortClickListener(UnityAction action) => onShortClick.AddListener(action);
        public void AddOnPressListener(UnityAction action) => onPress.AddListener(action);
        public void AddOnReleaseListener(UnityAction action) => onRelease.AddListener(action);

        public void RemoveOnShortClickListener(UnityAction action) => onShortClick.RemoveListener(action);
        public void RemoveOnPressListener(UnityAction action) => onPress.RemoveListener(action);
        public void RemoveOnReleaseListener(UnityAction action) => onRelease.RemoveListener(action);

        public void RemoveAllOnShortClickListeners() => onShortClick.RemoveAllListeners();
        public void RemoveAllOnPressListeners() => onPress.RemoveAllListeners();
        public void RemoveAllOnReleaseListeners() => onRelease.RemoveAllListeners();

        public override Vector2 Size
        {
            get => base.Size;
            set
            {
                base.Size = value;
                CheckCompressableFrontPanel();
                UpdateCompressableFrontPanel();
                CheckIcon();
                UpdateIcon();
                CheckCollider();
                UpdateCollider();
                CheckTouchable();
                UpdateTouchable();
                CheckPressableInteractable();
                UpdatePressableButton();
            }
        }

        public override float BorderWidth
        {
            get => base.BorderWidth;
            set
            {
                base.BorderWidth = value;
                CheckCompressableFrontPanel();
                UpdateCompressableFrontPanel();
                CheckIcon();
                UpdateIcon();
            }
        }
        public void ChangeColorTo(Color targetColor, float animationTime)
        {
            Animate("SpriteColor", spriteColor, targetColor, animationTime, value =>
            {
                spriteColor = value;
                UpdateSpriteColor();
            });
        }

        public new void ChangeSizeTo(Vector2 targetSize, float animationTime)
        {
            Animate("Size", Size, targetSize, animationTime, value =>
            {
                Size = value;
            });
        }

        public new void ChangeBorderWidthTo(float targetBorderWidth, float animationTime)
        {
            Animate("BorderWidth", BorderWidth, Mathf.Clamp(targetBorderWidth, 0, 1), animationTime, value =>
            {
                BorderWidth = value;
                CheckCompressableFrontPanel();
                UpdateCompressableFrontPanel();
                CheckText();
                UpdateTextTransform();
                CheckIcon();
                UpdateIcon();
            });
        }

        public new void SetEnableRTEditing(bool rtEditing)
        {
            this.rtEditing = rtEditing;
            if (rtEditing) OnValidate();
        }
        protected new void OnValidate()
        {
            UpdatePropertyDrawerIconVisibleFlag();
            CheckIcon();
            if (rtEditing)
            {
                base.OnValidate();
                CheckCompressableFrontPanel();
                UpdateCompressableFrontPanel();
                UpdateIcon();
                CheckCollider();
                UpdateCollider();
                CheckTouchable();
                UpdateTouchable();
                CheckPressableInteractable();
                UpdatePressableButton();
            }
            ApplyDisplayMode();
            UpdateSpriteColor();
            UpdateSprite();
        }

        private void UpdatePropertyDrawerIconVisibleFlag() { propertyDrawerIconVisible = contentType != ButtonContentType.Text; }
        private void CheckCompressableFrontPanel()
        {
            if (frontPanel == null)
            {
                Transform compressableVisuals = transform.Find("Compressable_Button_Visuals");
                if (compressableVisuals == null)
                    Debug.LogError("No Compressable_Button_Visuals child attached!");
                frontPanel = (RectTransform) compressableVisuals.Find("Front_Highlight_Plate");
                if (frontPanel == null)
                    Debug.LogError("No Front_Highlight_Plate grandchild attached!");
            }
        }

        private void UpdateCompressableFrontPanel()
        {
            frontPanel.localScale = ActualPanelScale;
            frontPanel.localPosition = new Vector3(0, 0, -ZSize);
        }

        protected override void CheckText()
        {
            if (contentType == ButtonContentType.Both || contentType == ButtonContentType.Text)
                base.CheckText();
        }
        protected override void UpdateText()
        {
            if (contentType == ButtonContentType.Both || contentType == ButtonContentType.Text)
                base.UpdateText();
        }
        protected override void UpdateTextTransform()
        {
            RectTransform textTransform = null;
            if (contentType == ButtonContentType.Both || contentType == ButtonContentType.Text) 
                textTransform = textMesh.GetComponent<RectTransform>();
            if (textTransform != null)
            {
                float selectedTextHeight = contentType == ButtonContentType.Both ? Size.y * 0.333333f : Size.y;
                int borderMultiplier = contentType == ButtonContentType.Both ? 1 : 2;
                int borderOffsetMultiplier = 2 - borderMultiplier;
                var inset = 1f - textInset;
                textTransform.anchoredPosition3D = new Vector3(0, 0.5f * (selectedTextHeight + borderOffsetMultiplier * BorderWidth - Size.y), Mathf.Min(-BorderWidth, -.0001f));
                textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (inset * Size.x - 2 * BorderWidth) * 1000);
                textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (selectedTextHeight - borderMultiplier * BorderWidth) * 1000);
            }
        }

        private void CheckIcon()
        {
            if (icon == null && (contentType == ButtonContentType.Both || contentType == ButtonContentType.Icon))
            {
                icon = GetComponentInChildren<Image>(true);
                if (icon == null)
                    Debug.LogError("No image grandchild attached!");
            }
        }

        private void UpdateIcon()
        {
            RectTransform iconTransform = null;
            if (contentType == ButtonContentType.Both || contentType == ButtonContentType.Icon)
                iconTransform = icon.GetComponent<RectTransform>();
            if (iconTransform != null)
            {
                float selectedIconHeight = contentType == ButtonContentType.Both ? Size.y * 0.666667f : Size.y;
                int borderMultiplier = contentType == ButtonContentType.Both ? 1 : 2;
                int borderOffsetMultiplier = 2 - borderMultiplier;
                iconTransform.anchoredPosition3D = new Vector3(0, 0.5f * (Size.y - selectedIconHeight - borderOffsetMultiplier * BorderWidth), Mathf.Min(-BorderWidth, -.0001f));
                iconTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size.x - 2 * BorderWidth);
                iconTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, selectedIconHeight - borderMultiplier * BorderWidth);
            }
        }

        private void ApplyDisplayMode()
        {
            switch (contentType)
            {
                case ButtonContentType.Text:
                    icon?.gameObject.SetActive(false);
                    textMesh?.gameObject.SetActive(true);
                    break;
                case ButtonContentType.Icon:
                    icon?.gameObject.SetActive(true);
                    textMesh?.gameObject.SetActive(false);
                    break;
                default:
                    icon?.gameObject.SetActive(true);
                    textMesh?.gameObject.SetActive(true);
                    break;
            }
        }

        private void CheckCollider()
        {
            if (buttonCollider == null)
            {
                buttonCollider = GetComponentInChildren<BoxCollider>();
                if (buttonCollider == null)
                    Debug.LogError("No BoxCollider child attached!");
            }
        }

        private void UpdateCollider()
        {
            buttonCollider.transform.localPosition = new Vector3(0, 0, -0.5f * ZSize);
            buttonCollider.size = new Vector3(Size.x, Size.y, ZSize);
        }

        private void CheckTouchable()
        {
            if (touchable == null)
            {
                touchable = GetComponentInChildren<NearInteractionTouchable>();
                if (touchable == null)
                    Debug.LogError("No NearInteractionTouchable child attached!");
            }
        }

        private void UpdateTouchable()
        {
            touchable.SetLocalCenter(new Vector3(0, 0, -0.5f * ZSize));
            touchable.SetBounds(Size);
        }

        private void UpdateSpriteColor()
        {
            if (icon != null)
                icon.color = spriteColor;
        }

        private void UpdateSprite()
        {
            if (icon != null)
                icon.sprite = imageSprite;
        }

        private void CheckPressableInteractable()
        {
            if (pressableInteractable == null)
            {
                pressableInteractable = GetComponentInChildren<PressableButton>();
                if (pressableInteractable == null)
                    Debug.LogError("No PressableButton child attached!");
            }
        }

        private void UpdatePressableButton()
        {
            pressableInteractable.StartPushDistance = -ZSize;
            pressableInteractable.MaxPushDistance = -0.25f * ZSize;
            pressableInteractable.PressDistance = -0.5f * ZSize;
            pressableInteractable.ReleaseDistanceDelta = 0.25f * ZSize;
        }

        public void InvokeOnPress()
        {
            IsPressed = true;
            onPress.Invoke();
            pressingInvoctionTime = Time.time;
        }

        public void InvokeOnRelease()
        {
            IsPressed = false;
            onRelease.Invoke();
            if (Time.time - pressingInvoctionTime <= maxShortClickDuration)
            {
                pressingInvoctionTime = -1;
                onShortClick.Invoke();
            }
        }

        public enum ButtonContentType
        {
            Text = 1, Icon = 2, Both = 3
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(SimpleButtonView))]
    public class SimpleButtonViewEditor : Editor
    {
        private GUIStyle buttonStyle;
        private bool enableToggle = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SimpleButtonView sbv = (SimpleButtonView)target;
            buttonStyle = new GUIStyle(GUI.skin.button);
            if (GUILayout.Toggle(enableToggle, "RT editing", buttonStyle) != enableToggle)
            {
                enableToggle = !enableToggle;
                sbv.SetEnableRTEditing(enableToggle);
            }
        }

    }
#endif
}
