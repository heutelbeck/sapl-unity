using TMPro;
using FTK.UIToolkit.Util;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FTK.UIToolkit.Primitives
{
    public class TextPanelView : StandardPanelView
    {
        [SerializeField]
        protected bool showBackgroundPanel = true;
        [Range(0f, 1f)]
        [SerializeField]
        protected float textInset = 0.05f;
        [TextArea]
        [SerializeField]
        protected string text = "Text";
        [SerializeField]
        protected TextAlignmentOptions alignment = TextAlignmentOptions.Center;
        [SerializeField]
        protected FontStyles fontStyle = FontStyles.Normal;
        [SerializeField]
        protected bool overrideFontSize = false;
        [SerializeField]
        [ConditionalField("overrideFontSize")]
        [Min(0)]
        [Tooltip("The font size in mm.")]
        protected float fontSize = 10f;

        protected TextMeshProUGUI textMesh;
        protected void SetSizeToBase(Vector2 size) { base.Size = size; }

        public float FontSize
        {
            get => textMesh.fontSize;
            set
            {
                if (overrideFontSize)
                    textMesh.fontSize = fontSize = value;
            }
        }

        public bool OverrideFontSize
        {
            get => overrideFontSize;
            set => textMesh.enableAutoSizing = !(overrideFontSize = value);
        }

        public string Text
        { 
            get { return text; } 
            set
            {
                text = value;
                CheckText();
                UpdateText();
            }
        }

        public override Vector2 Size
        {
            get => base.Size;
            set
            {
                base.Size = value;
                CheckText();
                UpdateTextTransform();
            }
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
                CheckText();
                UpdateTextTransform();
            });
        }

        protected new void OnValidate()
        {
            CheckText();
            UpdateText();
            if (rtEditing)
            {
                base.OnValidate();
                UpdateTextTransform();
                backgroundPanelTransform?.gameObject.SetActive(showBackgroundPanel);
            }
        }

        protected virtual void CheckText()
        {
            if (textMesh == null)
            {
                textMesh = GetComponentInChildren<TextMeshProUGUI>(true);
                if (textMesh == null)
                    Debug.LogError("No text-mesh child attached!");
            }
        }

        protected virtual void UpdateText()
        {
            textMesh.text = text;
            textMesh.enableAutoSizing = !overrideFontSize;
            textMesh.fontSize = fontSize;
            textMesh.alignment = alignment;
            textMesh.fontStyle = fontStyle;
        }

        protected virtual void UpdateTextTransform()
        {
            RectTransform textTransform = textMesh.GetComponent<RectTransform>();
            if (textTransform != null)
            {
                var inset = 1f - textInset;
                textTransform.anchoredPosition3D = new Vector3(0, 0, Mathf.Min(-BorderWidth, -.0001f));
                textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (inset * Size.x - 3f * BorderWidth) * 1000);
                textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (inset * Size.y - 3f * BorderWidth) * 1000);
            }
        }
    }
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextPanelView))]
    public class TextPanelViewEditor : Editor
    {
        private GUIStyle buttonStyle;
        private bool enableToggle = false;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            TextPanelView tpv = (TextPanelView)target;
            buttonStyle = new GUIStyle(GUI.skin.button);
            enableToggle = tpv.IsRTEditing();
            if (GUILayout.Toggle(enableToggle, "RT editing", buttonStyle) != enableToggle)
            {
                enableToggle = !enableToggle;
                tpv.SetEnableRTEditing(enableToggle);
            }
        }

    }
#endif
}

