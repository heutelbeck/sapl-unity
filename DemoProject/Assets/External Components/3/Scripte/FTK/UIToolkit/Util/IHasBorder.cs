namespace FTK.UIToolkit.Util
{
    public interface IHasBorder
    {
        float BorderWidth { get; set; }
        void ChangeBorderWidthTo(float targetBorderWidth, float animationTime);
    }
}