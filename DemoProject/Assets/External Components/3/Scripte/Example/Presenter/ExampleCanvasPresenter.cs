using FTK.UIToolkit.Assemblies;
using FTK.UIToolkit.Primitives;
using FTK.UIToolkit.Util;
using UnityEngine;

namespace Sandbox.Damian.FTK.Example.Presenter
{
    public class ExampleCanvasPresenter : AnimatableBase
    {
        [SerializeField]
        private ProgressBarView progressBar;
        [SerializeField]
        private TextPanelView labeledTextPanel;
        [Min(0f)]
        [Space(20)]
        [SerializeField]
        private float animationTimeSecs = 0.5f;

        public void OnButtonClick()
        {
            labeledTextPanel.Text = "Clicked!";
        }

        public void OnButtonPress()
        {
            labeledTextPanel.Text = "Pressed!";
        }

        public void OnButtonRelease()
        {
            labeledTextPanel.Text = "Released!";
        }

        public void OnSliderValueUpdated(float sliderValue)
        {
            sliderValue = Mathf.Clamp(sliderValue, 0f, 1f);
            Animate("ProgressBarValue", progressBar.Progress, sliderValue, animationTimeSecs, newValue => {
                progressBar.Progress = newValue;
            });
        }
    }
}