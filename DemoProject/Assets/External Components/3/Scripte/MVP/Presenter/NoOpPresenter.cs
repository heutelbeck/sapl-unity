using MVP.Model;

namespace MVP.Presenter
{
    public class NoOpPresenter : PresenterBase
    {
        public override bool Initialized => false;

        public override void ApplyCondition(Condition condition) { }


    }
}